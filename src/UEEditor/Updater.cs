using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
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
		private ProducerSynonymResolver resolver;

		public Updater(uint priceId, uint childPriceId, uint priceItemId, PriceProcessorWcfHelper remotePriceProcessor, ProducerSynonymResolver _resolver)
		{
			this.priceId = priceId;
			this.childPriceId = childPriceId;
			this.priceItemId = priceItemId;
			_remotePriceProcessor = remotePriceProcessor;
			resolver = _resolver;
		}

		public void UpdateProducerSynonym(List<Unrecexp> rows, List<DbExclude> excludes, DataTable dtSynonymFirmCr, List<ForbiddenProducerSynonym> forbiddenProducers)
		{
			//priceprocessor создает на одно наименование один синоним, но в результате сопоставления мы можем получить два разных синонима
			var synonyms = rows.Select(e => e.Row).Where(r => !(r["SynonymObject"] is DBNull)).Select(r => (ProducerSynonym)r["SynonymObject"]);
			var groups = synonyms.Where(s => !(s is Exclude) && !(s is ForbiddenProducerSynonym)).GroupBy(s => new { s.ProducerId, s.Name });
			foreach (var synonymGroup in groups) {
				var synonym = synonymGroup.First();
				var synonymRow = dtSynonymFirmCr.Select("SynonymFirmCrCode = " + synonym.Id);
				if (synonymRow.Any(s => Equals(synonym.ProducerId, s["CodeFirmCr"])))
					return;
				if (synonymRow.Length == 0)
					CreateSynonym(dtSynonymFirmCr, synonym);
				else
					UpdateSynonym(synonymRow[0], synonym);

				if (synonymGroup.Count() > 1) {
					foreach (var s in synonymGroup.Skip(1))
						CreateSynonym(dtSynonymFirmCr, s);
				}
			}

			foreach (var excludeGroups in synonyms.OfType<Exclude>().GroupBy(e => new { e.CatalogId, e.Name })) {
				var exclude = excludeGroups.First();
				var synonymRow = dtSynonymFirmCr.Select(String.Format("Synonym = '{0}' and CodeFirmCr is null", exclude.Name.Replace("'", "''")));
				if (synonymRow.Length == 0)
					CreateSynonym(dtSynonymFirmCr, exclude);
				else
					UpdateSynonym(synonymRow[0], exclude);

				CreateExclude(exclude, excludes, rows.First(r => r.Row["SynonymObject"] == exclude));
			}
			foreach (var forbiddenGroups in synonyms.OfType<ForbiddenProducerSynonym>().GroupBy(e => new { e.Name })) {
				var forbiddenProducer = forbiddenGroups.First();
				var synonymRow = dtSynonymFirmCr.Select(String.Format("Synonym = '{0}' and CodeFirmCr is null", forbiddenProducer.Name.Replace("'", "''")));
				if (synonymRow.Length == 0)
					CreateSynonym(dtSynonymFirmCr, forbiddenProducer);
				else
					UpdateSynonym(synonymRow[0], forbiddenProducer);
				CreateForbiddenProducer(forbiddenProducer, forbiddenProducers);
			}
		}

		private void CreateForbiddenProducer(ForbiddenProducerSynonym exclude, List<ForbiddenProducerSynonym> forbiddenProducers)
		{
			if (forbiddenProducers.Any(e => e.Name.Equals(exclude.Name, StringComparison.CurrentCultureIgnoreCase)))
				return;

			forbiddenProducers.Add(new ForbiddenProducerSynonym {
				Name = exclude.Name
			});
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
			if (!(synonym["Processed"] is DBNull) && Convert.ToBoolean(synonym["Processed"])) {
				CreateSynonym(synonym.Table, producerSynonym);
				return;
			}

			if (producerSynonym.ProducerId > 0)
				synonym["CodeFirmCr"] = producerSynonym.ProducerId;
			else
				synonym["CodeFirmCr"] = DBNull.Value;
			synonym["PriceCode"] = priceId;
			synonym["SupplierCode"] = producerSynonym.SupplierCode;
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
			try {
				synonyms.Rows.Add(synonymRow);
				stat.SynonymFirmCrCount++;
			}
			catch (ConstraintException) {
			}
		}

		private int UpDateUnrecExp(DataTable dtUnrecExpUpdate, DataRow drUpdated, MySqlConnection masterConnection, Unrecexp expression)
		{
			int DelCount = 0;

			if (!Convert.IsDBNull(drUpdated["UEPriorProductId"]) &&
				CatalogHelper.IsHiddenProduct(masterConnection, Convert.ToInt64(drUpdated["UEPriorProductId"]))) {
				//Производим проверку того, что синоним может быть сопоставлен со скрытым каталожным наименованием
				//Если в процессе распознования каталожное наименование скрыли, то сбрасываем распознавание
				resolver.UnresolveProduct(drUpdated);
				stat.HideSynonymCount++;
			}

			if (Convert.IsDBNull(drUpdated["UEProductSynonymId"]) &&
				CatalogHelper.IsSynonymExists(masterConnection, priceId, drUpdated["UEName1"].ToString())) {
				//Производим проверку того, что синоним может быть уже вставлен в таблицу синонимов
				//Если в процессе распознования синоним уже кто-то добавил, то сбрасываем распознавание
				drUpdated["UEPriorProductId"] = DBNull.Value;
				drUpdated["UEStatus"] = (int)((FormMask)Convert.ToByte(drUpdated["UEStatus"]) & (~FormMask.NameForm));
				var synonym = dtSynonym.NewRow();
				synonym["SynonymCode"] = MySqlHelper.ExecuteScalar(
					masterConnection,
					"select SynonymCode from farm.synonym where synonym = ?SynonymName and PriceCode = ?LockedSynonymPriceCode",
					new MySqlParameter("?LockedSynonymPriceCode", priceId),
					new MySqlParameter("?SynonymName", String.Format("{0}  ", drUpdated["UEName1"].ToString())));
				expression.CreatedProductSynonym = synonym;
				stat.DuplicateSynonymCount++;
			}

			var drNew = dtUnrecExpUpdate.Rows.Find(Convert.ToUInt32(drUpdated["UERowID"]));

			if (drNew != null) {
				dtUnrecExpUpdate.Rows.Remove(drNew);
				DelCount++;
			}

			return DelCount;
		}

		private DataTable dtForbidden;
		public List<DbExclude> excludes;
		private MySqlDataAdapter daUnrecUpdate;
		private DataTable dtUnrecUpdate;
		private MySqlDataAdapter daForbidden;
		private DataTable dtSynonymFirmCr;
		private MySqlDataAdapter daSynonym;
		private DataTable dtSynonym;
		private MySqlDataAdapter daSynonymFirmCr;
		public List<ForbiddenProducerSynonym> ForbiddenProducers;

		private string operatorName;

		public void ApplyChanges(MySqlConnection masterConnection, IProgressNotifier formProgress, List<DataRow> rows)
		{
			operatorName = Environment.UserName.ToLower();
			CalculateChanges(masterConnection, formProgress, rows);

			var updateSynonymProducerEtalonSQL = daSynonymFirmCr.UpdateCommand.CommandText;
			var insertSynonymProducerEtalonSQL = daSynonymFirmCr.InsertCommand.CommandText;

			var changes = dtSynonymFirmCr.GetChanges(DataRowState.Modified);
			if (changes != null)
				stat.SynonymFirmCrCount += changes.Rows.Count;

			formProgress.Status = "Применение изменений в базу данных...";
			DataRow lastUpdateSynonym = null;
			var debugString = new StringBuilder();

			try {
				With.DeadlockWraper(c => {
					var humanName = GetHumanName(c, operatorName);

					var helper = new Common.MySql.MySqlHelper(c, null);
					var commandHelper = helper.Command("set @inHost = ?Host; set @inUser = ?UserName;");
					commandHelper.AddParameter("?Host", Environment.MachineName);
					commandHelper.AddParameter("?UserName", operatorName);
					commandHelper.Execute();

					//Заполнили таблицу логов для синонимов наименований
					daSynonym.SelectCommand.Connection = c;
					daSynonym.Update(dtSynonym);

					formProgress.ApplyProgress += 10;

					var insertExclude = new MySqlCommand(@"
insert into Farm.Excludes(CatalogId, PriceCode, ProducerSynonym, DoNotShow, Operator, OriginalSynonymId)
value (?CatalogId, ?PriceCode, ?ProducerSynonym, ?DoNotShow, ?Operator, ?OriginalSynonymId);", c);
					insertExclude.Parameters.AddWithValue("?PriceCode", priceId);
					insertExclude.Parameters.AddWithValue("?Operator", humanName);
					insertExclude.Parameters.Add("?ProducerSynonym", MySqlDbType.VarChar);
					insertExclude.Parameters.Add("?DoNotShow", MySqlDbType.Byte);
					insertExclude.Parameters.Add("?CatalogId", MySqlDbType.UInt32);
					insertExclude.Parameters.Add("?OriginalSynonymId", MySqlDbType.UInt32);

					foreach (var exclude in excludes.Where(e => e.Id == 0)) {
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
					daSynonymFirmCr.UpdateCommand.Connection = c;
					daSynonymFirmCr.InsertCommand.Connection = c;
					var dtSynonymFirmCrCopy = dtSynonymFirmCr.Copy();
					foreach (DataRow drInsertProducerSynonym in dtSynonymFirmCrCopy.Rows) {
						lastUpdateSynonym = drInsertProducerSynonym;
						daSynonymFirmCr.InsertCommand.CommandText = insertSynonymProducerEtalonSQL;
						daSynonymFirmCr.UpdateCommand.CommandText = updateSynonymProducerEtalonSQL;

						//обновляем по одному синониму производителя, т.к. может быть добавление в исключение
						daSynonymFirmCr.Update(new[] { drInsertProducerSynonym });
					}

					MySqlHelper.ExecuteNonQuery(c,
						@"
update
usersettings.pricescosts,
usersettings.priceitems
set
priceitems.LastSynonymsCreation = now()
where
pricescosts.PriceCode = ?PriceCode
and priceitems.Id = pricescosts.PriceItemId",
						new MySqlParameter("?PriceCode", priceId));
					formProgress.ApplyProgress += 10;

					//Заполнили таблицу логов для запрещённых выражений
					daForbidden.SelectCommand.Connection = c;
					var dtForbiddenCopy = dtForbidden.Copy();
					daForbidden.Update(dtForbiddenCopy);

					formProgress.ApplyProgress += 10;
					//Обновление таблицы нераспознанных выражений
					daUnrecUpdate.SelectCommand.Connection = c;
					var dtUnrecUpdateCopy = dtUnrecUpdate.Copy();
					daUnrecUpdate.Update(dtUnrecUpdateCopy);
					formProgress.ApplyProgress += 10;

					// Сохраняем запрещенные имена производителей
					var deleteUnrec = new MySqlCommand("delete from farm.UnrecExp where LOWER(FirmCr) = ?FirmName and Status = 1", c);

					var insertForbiddenProducer = new MySqlCommand(@"
insert into Farm.Forbiddenproducers(Name)
value (?Name);", c);
					insertForbiddenProducer.Parameters.Add("?Name", MySqlDbType.VarChar);
					foreach (var producer in ForbiddenProducers.Where(e => e.Id == 0)) {
						insertForbiddenProducer.Parameters["?Name"].Value = producer.Name;
						insertForbiddenProducer.ExecuteScalar();

						// удаляем нераспознанные выражения с таким же наименованием производителя
						deleteUnrec.Parameters.Clear();
						deleteUnrec.Parameters.AddWithValue("?FirmName", producer.Name.ToLower());
						deleteUnrec.ExecuteNonQuery();
					}
				});
			}
			catch (Exception e) {
				if (e.Message.Contains("Duplicate entry"))
					Mailer.SendDebugLog(dtSynonymFirmCr, e, lastUpdateSynonym);
				if (!String.IsNullOrEmpty(debugString.ToString()))
					Mailer.SendMessageToService(e, debugString.ToString(), "a.tyutin@analit.net");
				throw;
			}

			formProgress.ApplyProgress = 80;

			formProgress.Status = String.Empty;
			formProgress.Error = String.Empty;

			formProgress.Status = "Перепроведение пpайса...";
			formProgress.ApplyProgress = 80;

			try {
#if !DEBUG
				_remotePriceProcessor.RetransPriceSmartMsMq(priceId);
#endif
			}
			catch (Exception e) {
				formProgress.Error = "При перепроведении файлов возникла ошибка, которая отправлена разработчику.";
				_logger.Error(String.Format("Ошибка при перепроведении прайс листа {0}", priceId), e);
			}

			_logger.DebugFormat("Перепроведение пpайса завершено.");
			formProgress.ApplyProgress = 100;
		}

		public void CalculateChanges(MySqlConnection masterConnection, IProgressNotifier formProgress, List<DataRow> rows)
		{
			formProgress.Status = "Подготовка таблиц...";

			stat.Reset();

			//Кол-во удаленных позиций - если оно равно кол-во нераспознанных позиций, то прайс автоматически проводится
			int DelCount = 0;

			formProgress.ApplyProgress = 1;
			//Заполнение таблиц перед вставкой

			//Заполнили таблицу нераспознанных наименований для обновления
			daUnrecUpdate = new MySqlDataAdapter("select * from farm.UnrecExp where PriceItemId = ?PriceItemId", masterConnection);
			var cbUnrecUpdate = new MySqlCommandBuilder(daUnrecUpdate);
			daUnrecUpdate.SelectCommand.Parameters.AddWithValue("?PriceItemId", priceItemId);
			dtUnrecUpdate = new DataTable();
			daUnrecUpdate.Fill(dtUnrecUpdate);
			dtUnrecUpdate.Constraints.Add("UnicNameCode", dtUnrecUpdate.Columns["RowID"], true);

			//Заполнили таблицу синонимов наименований
			daSynonym = new MySqlDataAdapter("select * from farm.Synonym where PriceCode = ?PriceCode limit 0", masterConnection);
			daSynonym.SelectCommand.Parameters.AddWithValue("?PriceCode", priceId);
			dtSynonym = new DataTable();
			daSynonym.Fill(dtSynonym);
			dtSynonym.Constraints.Add("UnicNameCode", dtSynonym.Columns["Synonym"], false);
			dtSynonym.Columns.Add("ChildPriceCode", typeof(long));
			daSynonym.InsertCommand = new MySqlCommand(
				@"
replace into farm.synonym (PriceCode, Synonym, Junk, ProductId, SupplierCode) values (?PriceCode, ?Synonym, ?Junk, ?ProductId, ?SupplierCode);
set @LastSynonymID = last_insert_id();
insert into farm.UsedSynonymLogs (SynonymCode) values (@LastSynonymID);
insert into logs.synonymlogs (LogTime, OperatorName, OperatorHost, Operation, SynonymCode, PriceCode, Synonym, Junk, ProductId, ChildPriceCode)
	values (now(), ?OperatorName, ?OperatorHost, 0, @LastSynonymID, ?PriceCode, ?Synonym, ?Junk, ?ProductId, ?ChildPriceCode);
select @LastSynonymID as SynonymCode;",
				masterConnection);
			daSynonym.InsertCommand.Parameters.AddWithValue("?OperatorName", operatorName);
			daSynonym.InsertCommand.Parameters.AddWithValue("?OperatorHost", Environment.MachineName);
			daSynonym.InsertCommand.Parameters.Add("?PriceCode", MySqlDbType.UInt64, 0, "PriceCode");
			daSynonym.InsertCommand.Parameters.Add("?Synonym", MySqlDbType.VarString, 0, "Synonym");
			daSynonym.InsertCommand.Parameters.Add("?Junk", MySqlDbType.Byte, 0, "Junk");
			daSynonym.InsertCommand.Parameters.Add("?ProductId", MySqlDbType.UInt64, 0, "ProductId");
			daSynonym.InsertCommand.Parameters.Add("?SupplierCode", MySqlDbType.VarString, 0, "SupplierCode");
			daSynonym.InsertCommand.Parameters.Add("?ChildPriceCode", MySqlDbType.Int64, 0, "ChildPriceCode");

			formProgress.ApplyProgress += 1;
			//Заполнили таблицу синонимов производителей
			daSynonymFirmCr =
				new MySqlDataAdapter(
					"select sfc.* from farm.SynonymFirmCr sfc, farm.AutomaticProducerSynonyms aps where sfc.PriceCode = ?PriceCode and aps.ProducerSynonymId = sfc.SynonymFirmCrCode",
					masterConnection);
			daSynonymFirmCr.SelectCommand.Parameters.AddWithValue("?PriceCode", priceId);
			dtSynonymFirmCr = new DataTable();
			daSynonymFirmCr.Fill(dtSynonymFirmCr);
			dtSynonymFirmCr.Constraints.Add("UnicNameCode",
				new[] { dtSynonymFirmCr.Columns["Synonym"], dtSynonymFirmCr.Columns["CodeFirmCr"] }, false);
			dtSynonymFirmCr.Columns.Add("ChildPriceCode", typeof(long));
			dtSynonymFirmCr.Columns.Add("Processed", typeof(bool));
			daSynonymFirmCr.InsertCommand = new MySqlCommand(
				@"
select farm.CreateProducerSynonym(?PriceCode, ?CodeFirmCr, ?Synonym, false);
",
				masterConnection);
			daSynonymFirmCr.InsertCommand.Parameters.AddWithValue("?OperatorName", operatorName);
			daSynonymFirmCr.InsertCommand.Parameters.AddWithValue("?OperatorHost", Environment.MachineName);
			daSynonymFirmCr.InsertCommand.Parameters.Add("?PriceCode", MySqlDbType.UInt64, 0, "PriceCode");
			daSynonymFirmCr.InsertCommand.Parameters.Add("?Synonym", MySqlDbType.VarString, 0, "Synonym");
			daSynonymFirmCr.InsertCommand.Parameters.Add("?CodeFirmCr", MySqlDbType.UInt64, 0, "CodeFirmCr");
			daSynonymFirmCr.InsertCommand.Parameters.Add("?SupplierCode", MySqlDbType.VarString, 0, "SupplierCode");
			daSynonymFirmCr.InsertCommand.Parameters.Add("?ChildPriceCode", MySqlDbType.Int64, 0, "ChildPriceCode");
			daSynonymFirmCr.UpdateCommand = new MySqlCommand(
				@"
update farm.synonymFirmCr set CodeFirmCr = ?CodeFirmCr where SynonymFirmCrCode = ?SynonymFirmCrCode;
delete from farm.AutomaticProducerSynonyms where ProducerSynonymId = ?SynonymFirmCrCode;
",
				masterConnection);
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
			daForbidden = new MySqlDataAdapter("select * from farm.Forbidden limit 0", masterConnection);
			dtForbidden = new DataTable();
			daForbidden.Fill(dtForbidden);
			dtForbidden.Constraints.Add("UnicNameCode", new[] { dtForbidden.Columns["Forbidden"] }, false);
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
			foreach (var dr in rows) {
				var expression = new Unrecexp(dr);
				DelCount += UpDateUnrecExp(dtUnrecUpdate, dr, masterConnection, expression);

				//Вставили новую запись в таблицу запрещённых выражений
				var name = GetFullUnrecName(dr);
				if (!MarkForbidden(dr, "UEAlready") && MarkForbidden(dr, "UEStatus")) {
					var newDR = dtForbidden.NewRow();

					newDR["PriceCode"] = childPriceId;
					newDR["Forbidden"] = name;
					try {
						dtForbidden.Rows.Add(newDR);
						stat.ForbiddenCount++;
					}
					catch (ConstraintException) {
					}
				}
				else if (NotNameForm(dr, "UEAlready") && !NotNameForm(dr, "UEStatus")) {
					//Вставили новую запись в таблицу синонимов наименований
					var newDR = dtSynonym.NewRow();

					newDR["PriceCode"] = priceId;
					newDR["Synonym"] = name;
					newDR["ProductId"] = dr["UEPriorProductId"];
					newDR["Junk"] = dr["UEJunk"];
					newDR["SupplierCode"] = dr["UECode"];
					if (priceId != childPriceId)
						newDR["ChildPriceCode"] = childPriceId;
					var synonym = dtSynonym.Rows
						.Cast<DataRow>()
						.FirstOrDefault(r => r["Synonym"].ToString().Equals(name, StringComparison.CurrentCultureIgnoreCase));
					if (synonym == null) {
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

			var selectExcludes =
				new MySqlCommand(
					@"select Id, CatalogId, ProducerSynonym, DoNotShow
from farm.Excludes
where pricecode = ?PriceCode",
					masterConnection);
			selectExcludes.Parameters.AddWithValue("?PriceCode", priceId);
			using (var reader = selectExcludes.ExecuteReader()) {
				excludes = reader.Cast<DbDataRecord>().Select(r => new DbExclude {
					Id = Convert.ToUInt32(r["Id"]),
					CatalogId = Convert.ToUInt32(r["CatalogId"]),
					ProducerSynonym = r["ProducerSynonym"].ToString(),
					DoNotShow = Convert.ToBoolean(r["DoNotShow"]),
				}).ToList();
			}

			var selectForbiddenProducers = new MySqlCommand(
				"select * from farm.forbiddenproducers",
				masterConnection);
			using (var reader = selectForbiddenProducers.ExecuteReader()) {
				ForbiddenProducers = reader.Cast<DbDataRecord>().Select(r => new ForbiddenProducerSynonym {
					Id = Convert.ToUInt32(r["Id"]),
					Name = r["Name"].ToString()
				}).ToList();
			}

			UpdateProducerSynonym(forProducerSynonyms, excludes, dtSynonymFirmCr, ForbiddenProducers);
		}

		public static string GetHumanName(MySqlConnection c, string operatorName)
		{
			var humanName = "";
			var command = new MySqlCommand(@"select ManagerName from accessright.regionaladmins where username = ?name", c);
			command.Parameters.AddWithValue("name", operatorName);
			using (var reader = command.ExecuteReader()) {
				var record = reader.Cast<DbDataRecord>().SingleOrDefault();
				if (record != null) {
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
			return String.Format("{0}", row["UEName1"]);
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