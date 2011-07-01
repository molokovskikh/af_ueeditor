using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Common.MySql;
using log4net;
using MySql.Data.MySqlClient;
using RemotePriceProcessor;
using UEEditor.Helpers;
using MySqlHelper = MySql.Data.MySqlClient.MySqlHelper;

namespace UEEditor
{
	public interface IProgressNotifier
	{
		string Status { get; set; }
		string Error { get; set; }
		int ApplyProgress { get; set; }
	}

	public class DbExclude
	{
		public uint Id;
		public uint CatalogId;
		public string ProducerSynonym;
		public bool DoNotShow;

		public DataRow OriginalSynonym;
		public uint OriginalSynonymId;

		public uint GetOriginalSynonymId()
		{
			if (OriginalSynonymId == 0)
				return Convert.ToUInt32(OriginalSynonym["SynonymCode"]);
			return OriginalSynonymId;
		}
	}

	public class Unrecexp
	{
		public uint ProductSynonymId;
		public DataRow CreatedProductSynonym;
		public ProducerSynonym ProducerSynonym;
		public DataRow Row;

		public Unrecexp(DataRow dr)
		{
			Row = dr;
			if (!(dr["UEProductSynonymId"] is DBNull))
				ProductSynonymId = Convert.ToUInt32(dr["UEProductSynonymId"]);
		}
	}

	public class Updater
	{
		private uint priceId;
		private uint childPriceId;
		private uint priceItemId;
		private Statistics stat = new Statistics();

		private ILog _logger = LogManager.GetLogger(typeof(Updater));
		private PriceProcessorWcfHelper _remotePriceProcessor;

		public Updater(uint priceId, uint childPriceId, uint priceItemId, PriceProcessorWcfHelper remotePriceProcessor)
		{
			this.priceId = priceId;
			this.childPriceId = childPriceId;
			this.priceItemId = priceItemId;
			_remotePriceProcessor = remotePriceProcessor;
		}

		public void UpdateProducerSynonym(List<Unrecexp> rows, List<DbExclude> excludes, DataTable dtSynonymFirmCr)
		{
			//priceprocessor создает на одно наименование один синоним, но в результате сопоставления мы можем получить два разных синонима
			var synonyms = rows.Select(e => e.Row).Where(r => !(r["SynonymObject"] is DBNull)).Select(r => (ProducerSynonym) r["SynonymObject"]);
			var groups = synonyms.Where(s => !(s is Exclude)).GroupBy(s => new {s.ProducerId, s.Name});
			foreach (var synonymGroup in groups)
			{
				var synonym = synonymGroup.First();
				var synonymRow = dtSynonymFirmCr.Select("SynonymFirmCrCode = " + synonym.Id);
				if (synonymRow.Any(s => Equals(synonym.ProducerId, s["CodeFirmCr"])))
					return;
				if (synonymRow.Length == 0)
					CreateSynonym(dtSynonymFirmCr, synonym);
				else
					UpdateSynonym(synonymRow[0], synonym);

				if (synonymGroup.Count() > 1)
				{
					foreach (var s in synonymGroup.Skip(1))
						CreateSynonym(dtSynonymFirmCr, s);
				}
			}

			foreach (var excludeGroups in synonyms.OfType<Exclude>().GroupBy(e => new {e.CatalogId, e.Name}))
			{
				var exclude = excludeGroups.First();
				var synonymRow = dtSynonymFirmCr.Select(String.Format("Synonym = '{0}' and CodeFirmCr is null", exclude.Name.Replace("'", "''")));
				if (synonymRow.Length == 0)
					CreateSynonym(dtSynonymFirmCr, exclude);
				else
					UpdateSynonym(synonymRow[0], exclude);
				CreateExclude(exclude, excludes, rows.First(r => r.Row["SynonymObject"] == exclude));
			}
		}

		private void CreateExclude(Exclude exclude, List<DbExclude> excludes, Unrecexp expression)
		{
			if (excludes.Any(e => e.CatalogId == exclude.CatalogId && e.ProducerSynonym.Equals(exclude.Name, StringComparison.CurrentCultureIgnoreCase)))
				return;

			excludes.Add(new DbExclude {
				CatalogId = exclude.CatalogId,
				DoNotShow = exclude.State == ProducerSynonymState.Unknown,
				ProducerSynonym = exclude.Name,
				OriginalSynonym = expression.CreatedProductSynonym,
				OriginalSynonymId = expression.ProductSynonymId
			});
		}

		private void UpdateSynonym(DataRow synonym, ProducerSynonym producerSynonym)
		{
			if (!(synonym["Processed"] is DBNull) && Convert.ToBoolean(synonym["Processed"]))
			{
				CreateSynonym(synonym.Table, producerSynonym);
				return;
			}

			if (producerSynonym.ProducerId > 0)
				synonym["CodeFirmCr"] = producerSynonym.ProducerId;
			else
				synonym["CodeFirmCr"] = DBNull.Value;
			synonym["PriceCode"] = priceId;
			synonym["Processed"] = true;
			if (priceId != childPriceId)
				synonym["ChildPriceCode"] = childPriceId;
		}

		public void CreateSynonym(DataTable synonyms, ProducerSynonym synonym)
		{
			if (synonym.Loaded)
				return;
			var synonymRow = synonyms.NewRow();
			UpdateSynonym(synonymRow, synonym);
			synonymRow["Synonym"] = synonym.Name;
			try
			{
				synonyms.Rows.Add(synonymRow);
				stat.SynonymFirmCrCount++;
			}
			catch (ConstraintException)
			{}
		}

		private int UpDateUnrecExp(DataTable dtUnrecExpUpdate, DataRow drUpdated, MySqlConnection masterConnection)
		{
			int DelCount = 0;

			if (!Convert.IsDBNull(drUpdated["UEPriorProductId"]) &&
				CatalogHelper.IsHiddenProduct(masterConnection, Convert.ToInt64(drUpdated["UEPriorProductId"])))
			{
				//Производим проверку того, что синоним может быть сопоставлен со скрытым каталожным наименованием
				//Если в процессе распознования каталожное наименование скрыли, то сбрасываем распознавание
				drUpdated["UEPriorProductId"] = DBNull.Value;
				drUpdated["UEStatus"] = (int)((FormMask)Convert.ToByte(drUpdated["UEStatus"]) & (~FormMask.NameForm));
				stat.HideSynonymCount++;
			}

			if (Convert.IsDBNull(drUpdated["UEProductSynonymId"]) &&
				CatalogHelper.IsSynonymExists(masterConnection, priceId, drUpdated["UEName1"].ToString()))
			{
				//Производим проверку того, что синоним может быть уже вставлен в таблицу синонимов
				//Если в процессе распознования синоним уже кто-то добавил, то сбрасываем распознавание
				drUpdated["UEPriorProductId"] = DBNull.Value;
				drUpdated["UEStatus"] = (int)((FormMask)Convert.ToByte(drUpdated["UEStatus"]) & (~FormMask.NameForm));
				stat.DuplicateSynonymCount++;
			}

/*
			Запрос не работает тк теперь могут быть одинаковые синонимы но разные производители
			if ((((FormMask)Convert.ToByte(drUpdated["UEAlready"]) & FormMask.FirmForm) != FormMask.FirmForm)
				&& (((FormMask)Convert.ToByte(drUpdated["UEStatus"]) & FormMask.FirmForm) == FormMask.FirmForm) 
					&& Convert.IsDBNull(drUpdated["UEProducerSynonymId"])
					&& CatalogHelper.IsProducerSynonymExists(masterConnection, 
						priceId,
						drUpdated["UEFirmCr"].ToString(),
						drUpdated["UEPriorProducerId"]))
			{
				//Производим проверку того, что синоним может быть уже вставлен в таблицу синонимов
				//Если в процессе распознования синоним уже кто-то добавил, то сбрасываем распознавание
				drUpdated["UEPriorProducerId"] = DBNull.Value;
				drUpdated["UEStatus"] = (int)((FormMask)Convert.ToByte(drUpdated["UEStatus"]) & (~FormMask.FirmForm));
				stat.DuplicateProducerSynonymCount++;
			}
*/

			var drNew = dtUnrecExpUpdate.Rows.Find( Convert.ToUInt32( drUpdated["UERowID"] ) );

			if (drNew != null)
			{
				dtUnrecExpUpdate.Rows.Remove(drNew);
				DelCount++;
			}

			return DelCount;
		}

		public void ApplyChanges(MySqlConnection masterConnection, IProgressNotifier formProgress, List<DataRow> rows)
		{
			var operatorName = Environment.UserName.ToLower();

			var LockedSynonym = priceId;
			var LockedPriceCode = childPriceId;
			var LockedPriceItemId = priceItemId;

			formProgress.Status = "Подготовка таблиц...";

			stat.Reset();

			//Кол-во удаленных позиций - если оно равно кол-во нераспознанных позиций, то прайс автоматически проводится
			int DelCount = 0;
			
			formProgress.ApplyProgress = 1;
			//Заполнение таблиц перед вставкой

			//Заполнили таблицу нераспознанных наименований для обновления
			var daUnrecUpdate = new MySqlDataAdapter("select * from farm.UnrecExp where PriceItemId = ?PriceItemId", masterConnection);
			var cbUnrecUpdate = new MySqlCommandBuilder(daUnrecUpdate);
			daUnrecUpdate.SelectCommand.Parameters.AddWithValue("?PriceItemId", LockedPriceItemId);
			var dtUnrecUpdate = new DataTable();
			daUnrecUpdate.Fill(dtUnrecUpdate);
			dtUnrecUpdate.Constraints.Add("UnicNameCode", dtUnrecUpdate.Columns["RowID"], true);

			//Заполнили таблицу синонимов наименований
			var daSynonym = new MySqlDataAdapter("select * from farm.Synonym where PriceCode = ?PriceCode limit 0", masterConnection);
			daSynonym.SelectCommand.Parameters.AddWithValue("?PriceCode", LockedSynonym);
			var dtSynonym = new DataTable();
			daSynonym.Fill(dtSynonym);
			dtSynonym.Constraints.Add("UnicNameCode", dtSynonym.Columns["Synonym"], false);
			dtSynonym.Columns.Add("ChildPriceCode", typeof(long));
			daSynonym.InsertCommand = new MySqlCommand(
				@"
insert into farm.synonym (PriceCode, Synonym, Junk, ProductId) values (?PriceCode, ?Synonym, ?Junk, ?ProductId);
set @LastSynonymID = last_insert_id();
insert into farm.UsedSynonymLogs (SynonymCode) values (@LastSynonymID);
insert into logs.synonymlogs (LogTime, OperatorName, OperatorHost, Operation, SynonymCode, PriceCode, Synonym, Junk, ProductId, ChildPriceCode)
	values (now(), ?OperatorName, ?OperatorHost, 0, @LastSynonymID, ?PriceCode, ?Synonym, ?Junk, ?ProductId, ?ChildPriceCode);
select @LastSynonymID as SynonymCode;", masterConnection);
			daSynonym.InsertCommand.Parameters.AddWithValue("?OperatorName", operatorName);
			daSynonym.InsertCommand.Parameters.AddWithValue("?OperatorHost", Environment.MachineName);
			daSynonym.InsertCommand.Parameters.Add("?PriceCode", MySqlDbType.UInt64, 0, "PriceCode");
			daSynonym.InsertCommand.Parameters.Add("?Synonym", MySqlDbType.VarString, 0, "Synonym");
			daSynonym.InsertCommand.Parameters.Add("?Junk", MySqlDbType.Byte, 0, "Junk");
			daSynonym.InsertCommand.Parameters.Add("?ProductId", MySqlDbType.UInt64, 0, "ProductId");
			daSynonym.InsertCommand.Parameters.Add("?ChildPriceCode", MySqlDbType.Int64, 0, "ChildPriceCode");
			
			formProgress.ApplyProgress += 1;
			//Заполнили таблицу синонимов производителей
			var daSynonymFirmCr = new MySqlDataAdapter("select sfc.* from farm.SynonymFirmCr sfc, farm.AutomaticProducerSynonyms aps where sfc.PriceCode = ?PriceCode and aps.ProducerSynonymId = sfc.SynonymFirmCrCode", masterConnection);
			daSynonymFirmCr.SelectCommand.Parameters.AddWithValue("?PriceCode", LockedSynonym);
			var dtSynonymFirmCr = new DataTable();
			daSynonymFirmCr.Fill(dtSynonymFirmCr);
			dtSynonymFirmCr.Constraints.Add("UnicNameCode", new[] {dtSynonymFirmCr.Columns["Synonym"], dtSynonymFirmCr.Columns["CodeFirmCr"]}, false);
			dtSynonymFirmCr.Columns.Add("ChildPriceCode", typeof(long));
			dtSynonymFirmCr.Columns.Add("Processed", typeof(bool));
			daSynonymFirmCr.InsertCommand = new MySqlCommand(
				@"
insert into farm.synonymFirmCr (PriceCode, CodeFirmCr, Synonym) values (?PriceCode, ?CodeFirmCr, ?Synonym);
set @LastSynonymFirmCrID = last_insert_id();
insert into farm.UsedSynonymFirmCrLogs (SynonymFirmCrCode) values (@LastSynonymFirmCrID); 
", 
				masterConnection);
			var insertSynonymProducerEtalonSQL = daSynonymFirmCr.InsertCommand.CommandText;
			daSynonymFirmCr.InsertCommand.Parameters.AddWithValue("?OperatorName", operatorName);
			daSynonymFirmCr.InsertCommand.Parameters.AddWithValue("?OperatorHost", Environment.MachineName);
			daSynonymFirmCr.InsertCommand.Parameters.Add("?PriceCode", MySqlDbType.UInt64, 0, "PriceCode");
			daSynonymFirmCr.InsertCommand.Parameters.Add("?Synonym", MySqlDbType.VarString, 0, "Synonym");
			daSynonymFirmCr.InsertCommand.Parameters.Add("?CodeFirmCr", MySqlDbType.UInt64, 0, "CodeFirmCr");
			daSynonymFirmCr.InsertCommand.Parameters.Add("?ChildPriceCode", MySqlDbType.Int64, 0, "ChildPriceCode");
			daSynonymFirmCr.UpdateCommand = new MySqlCommand(
				@"
update farm.synonymFirmCr set CodeFirmCr = ?CodeFirmCr where SynonymFirmCrCode = ?SynonymFirmCrCode;
delete from farm.AutomaticProducerSynonyms where ProducerSynonymId = ?SynonymFirmCrCode;
", masterConnection);
			var updateSynonymProducerEtalonSQL = daSynonymFirmCr.UpdateCommand.CommandText;
			daSynonymFirmCr.UpdateCommand.Parameters.AddWithValue("?OperatorName", operatorName);
			daSynonymFirmCr.UpdateCommand.Parameters.AddWithValue("?OperatorHost", Environment.MachineName);
			daSynonymFirmCr.UpdateCommand.Parameters.Add("?PriceCode", MySqlDbType.UInt64, 0, "PriceCode");
			daSynonymFirmCr.UpdateCommand.Parameters.Add("?Synonym", MySqlDbType.VarString, 0, "Synonym");
			daSynonymFirmCr.UpdateCommand.Parameters.Add("?CodeFirmCr", MySqlDbType.UInt64, 0, "CodeFirmCr");
			daSynonymFirmCr.UpdateCommand.Parameters.Add("?ChildPriceCode", MySqlDbType.Int64, 0, "ChildPriceCode");
			daSynonymFirmCr.UpdateCommand.Parameters.Add("?SynonymFirmCrCode", MySqlDbType.Int64, 0, "SynonymFirmCrCode");

			formProgress.ApplyProgress += 1;

			formProgress.ApplyProgress += 1;
			//Заполнили таблицу запрещённых выражений
			var daForbidden = new MySqlDataAdapter("select * from farm.Forbidden limit 0", masterConnection);
			var dtForbidden = new DataTable();
			daForbidden.Fill(dtForbidden);
			dtForbidden.Constraints.Add("UnicNameCode", new[] {dtForbidden.Columns["Forbidden"]}, false);
			daForbidden.InsertCommand = new MySqlCommand(
				@"
insert into farm.Forbidden (PriceCode, Forbidden) values (?PriceCode, ?Forbidden);
insert into logs.ForbiddenLogs (LogTime, OperatorName, OperatorHost, Operation, ForbiddenRowID, PriceCode, Forbidden) 
  values (now(), ?OperatorName, ?OperatorHost, 0, last_insert_id(), ?PriceCode, ?Forbidden);", 
				masterConnection);

			daForbidden.InsertCommand.Parameters.AddWithValue("?OperatorName", operatorName);
			daForbidden.InsertCommand.Parameters.AddWithValue("?OperatorHost", Environment.MachineName);
			daForbidden.InsertCommand.Parameters.Add("?PriceCode", MySqlDbType.UInt64, 0, "PriceCode");
			daForbidden.InsertCommand.Parameters.Add("?Forbidden", MySqlDbType.VarString, 0, "Forbidden");

			formProgress.ApplyProgress = 10;

			var forProducerSynonyms = new List<Unrecexp>();
			foreach (var dr in rows)
			{
				var expression = new Unrecexp(dr);
				DelCount += UpDateUnrecExp(dtUnrecUpdate, dr, masterConnection);
					
				//Вставили новую запись в таблицу запрещённых выражений
				var name = GetFullUnrecName(dr);
				if (!MarkForbidden(dr, "UEAlready") && MarkForbidden(dr, "UEStatus"))
				{
					var newDR = dtForbidden.NewRow();

					newDR["PriceCode"] = LockedPriceCode;
					newDR["Forbidden"] = name;
					try
					{
						dtForbidden.Rows.Add(newDR);
						stat.ForbiddenCount++;
					}
					catch(ConstraintException)
					{}
				}
					//Вставили новую запись в таблицу синонимов наименований
				else if (NotNameForm(dr, "UEAlready") && !NotNameForm(dr, "UEStatus"))
				{
					var newDR = dtSynonym.NewRow();

					newDR["PriceCode"] = LockedSynonym;
					newDR["Synonym"] = name;
					newDR["ProductId"] = dr["UEPriorProductId"];
					newDR["Junk"] = dr["UEJunk"];
					if (LockedSynonym != LockedPriceCode)
						newDR["ChildPriceCode"] = LockedPriceCode;
					var synonym = dtSynonym.Rows
						.Cast<DataRow>()
						.FirstOrDefault(r => r["Synonym"].ToString().Equals(name, StringComparison.CurrentCultureIgnoreCase));
					if (synonym == null)
					{
						synonym = newDR;
						dtSynonym.Rows.Add(newDR);
						stat.SynonymCount++;
					}
					expression.CreatedProductSynonym = synonym;
				}

				//если сопоставили по производителю
				if (NotFirmForm(dr, "UEAlready") && !NotFirmForm(dr, "UEStatus"))
					forProducerSynonyms.Add(expression);
			}

			List<DbExclude> excludes;
			var selectExcludes = new MySqlCommand(@"select Id, CatalogId, ProducerSynonym, DoNotShow
from farm.Excludes
where pricecode = ?PriceCode", masterConnection);
			selectExcludes.Parameters.AddWithValue("?PriceCode", LockedSynonym);
			using (var reader = selectExcludes.ExecuteReader())
			{
				excludes = reader.Cast<DbDataRecord>().Select(r => new DbExclude {
					Id = Convert.ToUInt32(r["Id"]),
					CatalogId = Convert.ToUInt32(r["CatalogId"]),
					ProducerSynonym = r["ProducerSynonym"].ToString(),
					DoNotShow = Convert.ToBoolean(r["DoNotShow"]),
				}).ToList();
			}

			UpdateProducerSynonym(forProducerSynonyms, excludes, dtSynonymFirmCr);

			var changes = dtSynonymFirmCr.GetChanges(DataRowState.Modified);
			if (changes != null)
				stat.SynonymFirmCrCount += changes.Rows.Count;

			formProgress.Status = "Применение изменений в базу данных...";
			DataRow lastUpdateSynonym = null;
			try
			{
				With.DeadlockWraper(c => {

                    _logger.Debug("1-->");
					var humanName = GetHumanName(c, operatorName);

					var helper = new Common.MySql.MySqlHelper(/*masterConnection*/c, null);
					var commandHelper = helper.Command("set @inHost = ?Host; set @inUser = ?UserName;");
					commandHelper.AddParameter("?Host", Environment.MachineName);
					commandHelper.AddParameter("?UserName", operatorName);
                    _logger.Debug("2-->");
					commandHelper.Execute();

					//Заполнили таблицу логов для синонимов наименований
					daSynonym.SelectCommand.Connection = c;
                    _logger.Debug("3-->");
					daSynonym.Update(dtSynonym);

					formProgress.ApplyProgress += 10;

                    _logger.Debug("4-->");
					var insertExclude = new MySqlCommand(@"
insert into Farm.Excludes(CatalogId, PriceCode, ProducerSynonym, DoNotShow, Operator, OriginalSynonymId) 
value (?CatalogId, ?PriceCode, ?ProducerSynonym, ?DoNotShow, ?Operator, ?OriginalSynonymId);", /*masterConnection*/c);
					insertExclude.Parameters.AddWithValue("?PriceCode", LockedSynonym);
					insertExclude.Parameters.AddWithValue("?Operator", humanName);
					insertExclude.Parameters.Add("?ProducerSynonym", MySqlDbType.VarChar);
					insertExclude.Parameters.Add("?DoNotShow", MySqlDbType.Byte);
					insertExclude.Parameters.Add("?CatalogId", MySqlDbType.UInt32);
					insertExclude.Parameters.Add("?OriginalSynonymId", MySqlDbType.UInt32);

                    _logger.Debug("5-->");
					foreach (var exclude in excludes.Where(e => e.Id == 0))
					{
						if (!IsExcludeCorrect(c, exclude))
							continue;

						insertExclude.Parameters["?ProducerSynonym"].Value = exclude.ProducerSynonym;
						insertExclude.Parameters["?DoNotShow"].Value = exclude.DoNotShow;
						insertExclude.Parameters["?CatalogId"].Value = exclude.CatalogId;
						insertExclude.Parameters["?OriginalSynonymId"].Value = exclude.GetOriginalSynonymId();
						insertExclude.ExecuteScalar();
					}

					//Заполнили таблицу логов для синонимов производителей
					daSynonymFirmCr.SelectCommand.Connection = c;
					var dtSynonymFirmCrCopy = dtSynonymFirmCr.Copy();
                    _logger.Debug("6-->");
					foreach (DataRow drInsertProducerSynonym in dtSynonymFirmCrCopy.Rows)
					{
						lastUpdateSynonym = drInsertProducerSynonym;
						daSynonymFirmCr.InsertCommand.CommandText = insertSynonymProducerEtalonSQL;
						daSynonymFirmCr.UpdateCommand.CommandText = updateSynonymProducerEtalonSQL;

						//обновляем по одному синониму производителя, т.к. может быть добавление в исключение
						daSynonymFirmCr.Update(new[] { drInsertProducerSynonym });
					}

                    _logger.Debug("7-->");
					MySqlHelper.ExecuteNonQuery(/*masterConnection*/c,
						@"
update 
usersettings.pricescosts,
usersettings.priceitems
set
priceitems.LastSynonymsCreation = now()
where
pricescosts.PriceCode = ?PriceCode
and priceitems.Id = pricescosts.PriceItemId",
						new MySqlParameter("?PriceCode", LockedSynonym));
					formProgress.ApplyProgress += 10;

					//Заполнили таблицу логов для запрещённых выражений
					daForbidden.SelectCommand.Connection = c;
                    _logger.Debug("8-->");
					var dtForbiddenCopy = dtForbidden.Copy();
                    _logger.Debug("9-->");
					daForbidden.Update(dtForbiddenCopy);

					formProgress.ApplyProgress += 10;
					//Обновление таблицы нераспознанных выражений
                    _logger.Debug("10-->");
					daUnrecUpdate.SelectCommand.Connection = c;
					var dtUnrecUpdateCopy = dtUnrecUpdate.Copy();
                    _logger.Debug("11-->");
					daUnrecUpdate.Update(dtUnrecUpdateCopy);
                    _logger.Debug("11-->");
					formProgress.ApplyProgress += 10;
				});
			}
			catch (Exception e)
			{
				if (e.Message.Contains("Duplicate entry"))
					Mailer.SendDebugLog(dtSynonymFirmCr, e, lastUpdateSynonym);
				throw;
			}

			formProgress.ApplyProgress = 80;

			formProgress.Status = String.Empty;
			formProgress.Error = String.Empty;

			formProgress.Status = "Перепроведение пpайса...";
			formProgress.ApplyProgress = 80;

			try
			{
#if !DEBUG
				_remotePriceProcessor.RetransPriceSmart(priceId);
#endif
			}
			catch (Exception e)
			{
				formProgress.Error = "При перепроведении файлов возникла ошибка, которая отправлена разработчику.";
				_logger.Error(String.Format("Ошибка при перепроведении прайс листа {0}", priceId), e);
			}
				
			_logger.DebugFormat("Перепроведение пpайса завершено.");
			formProgress.ApplyProgress = 100;
		}

		public static string GetHumanName(MySqlConnection c, string operatorName)
		{
			var humanName = "";
			var command = new MySqlCommand(@"select ManagerName from accessright.regionaladmins where username = ?name", c);
			command.Parameters.AddWithValue("name", operatorName);
			using (var reader = command.ExecuteReader())
			{
				var record = reader.Cast<DbDataRecord>().SingleOrDefault();
				if (record != null)
				{
					var name = record["ManagerName"].ToString();
					if (!String.IsNullOrEmpty(name))
						humanName = name;
				}
			}
			return humanName;
		}

		public static bool IsExcludeCorrect(MySqlConnection connection, DbExclude exclude)
		{
			if (exclude.DoNotShow)
				return true;

			var command = new MySqlCommand(@"
select Pharmacie
from Catalogs.Catalog
where Id = ?CatalogId", connection);
			command.Parameters.AddWithValue("?CatalogId", exclude.CatalogId);
			return Convert.ToBoolean(command.ExecuteScalar());
		}

		private bool NotNameForm(DataRow row, string FieldName)
		{
			var m = GetMask(row, FieldName);
			return (m & FormMask.NameForm) != FormMask.NameForm;
		}


		private string GetFullUnrecName(DataRow row)
		{
			return String.Format("{0}  ", row["UEName1"]);
		}

		private bool NotFirmForm(DataRow row, string FieldName)
		{
			var m = GetMask(row, FieldName);
			return (m & FormMask.FirmForm) != FormMask.FirmForm;
		}

		private FormMask GetMask(DataRow row, string FieldName)
		{
			var mask = (FormMask)Convert.ToByte(row[FieldName]);
			return mask;
		}

		private bool MarkForbidden(DataRow row, string FieldName)
		{
			var m = GetMask(row, FieldName);
			return (m & FormMask.MarkForb) == FormMask.MarkForb;
		}

	}
}