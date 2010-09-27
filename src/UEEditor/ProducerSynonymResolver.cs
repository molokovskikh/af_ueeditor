using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Common.MySql;
using MySql.Data.MySqlClient;

namespace UEEditor
{
	public class Exclude : ProducerSynonym
	{
		public uint CatalogId;

		public Exclude()
		{
			State = ProducerSynonymState.Exclude;
		}

		public Exclude(IDataRecord record) : this()
		{
			Loaded = true;
			Id = Convert.ToUInt32(record["SynonymFirmCrCode"]);
			Name = record["Synonym"].ToString();
			CatalogId = Convert.ToUInt32(record["CatalogId"]);
		}
	}

	public class ProducerSynonym
	{
		public uint Id;
		public uint ProducerId;
		public string Name;
		public bool Loaded;

		public ProducerSynonymState State;

		public ProducerSynonym()
		{
			State = ProducerSynonymState.Normal;
		}

		public ProducerSynonym(DataRow item, uint producerId) : this()
		{
			ProducerId = producerId;
			Name = item["UEFirmCr"].ToString();
			if (!(item["UEProducerSynonymId"] is DBNull))
				Id = Convert.ToUInt32(item["UEProducerSynonymId"]);
		}

		public void Apply(DataRow row)
		{
			if (row["UEProducerSynonymId"] is DBNull)
				row["UEProducerSynonymId"] = Id;

			row["UEStatus"] = (int)(ProducerSynonymResolver.GetStatus(row) | FormMask.FirmForm);
			if (ProducerId == 0)
				row["UEPriorProducerId"] = DBNull.Value;
			else
				row["UEPriorProducerId"] = ProducerId;
			row["SynonymObject"] = this;
		}

		public bool IsApplicable(DataRow destination, DataTable assortment)
		{
			var status = ProducerSynonymResolver.GetStatus(destination);
			if ((status & FormMask.NameForm) != FormMask.NameForm)
				return false;

			if ((status & FormMask.FirmForm) == FormMask.FirmForm)
				return false;

			if (destination["UEPriorProductId"] is DBNull)
				return false;

			if (!Name.Equals(destination["UEFirmCr"].ToString(), StringComparison.CurrentCultureIgnoreCase))
				return false;

			var catalogId = Convert.ToUInt32(destination["UEPriorCatalogId"]);
			if (this is Exclude)
				return ((Exclude) this).CatalogId == catalogId;

			//если это фармацевтика то не нужно делать проверки по ассортименту
			if (!Convert.ToBoolean(destination["pharmacie"]))
				return true;

			if (assortment == null)
				return false;

			return assortment.Rows.Cast<DataRow>().Any(r => Convert.ToUInt32(r["CatalogId"]) == catalogId 
				&& Convert.ToUInt32(r["ProducerId"]) == ProducerId);
		}

		public static ProducerSynonym CreateSynonym(DbDataRecord record)
		{
			if (record == null)
				return null;
			if (record["CatalogId"] is DBNull && !(record["ProducerId"] is DBNull))
			{
				return new ProducerSynonym {
					Loaded = true,
					Id = Convert.ToUInt32(record["SynonymFirmCrCode"]),
					ProducerId = Convert.ToUInt32(record["ProducerId"]),
					Name = record["Synonym"].ToString()
				};
			}
			else
			{
				return new Exclude(record);
			}
		}

		public static ProducerSynonym CreateSynonym(DataRow row, uint producerId)
		{
			if (producerId != 0)
			{
				return new ProducerSynonym(row, producerId);
			}
			else
				return new Exclude {
					Name = row["UEFirmCr"].ToString(),
					CatalogId = Convert.ToUInt32(row["UEPriorCatalogId"]),
					State = ProducerSynonymState.Unknown
				};
		}
	}

	public enum ProducerSynonymState
	{
		Normal = 0,
		NotProcessed = 1,
		Unknown = 2,
		Exclude = 3,
	}

	public class ProducerSynonymResolver
	{
		private static List<ProducerSynonym> synonyms = new List<ProducerSynonym>();
		private static uint priceId;

		public static void Init(uint priceId)
		{
			ProducerSynonymResolver.priceId = priceId;
			synonyms = new List<ProducerSynonym>();
		}

		
		public static Query LoadSynonym()
		{
			return new Query()
				.Select("sfc.SynonymFirmCrCode, sfc.CodeFirmCr as ProducerId, sfc.Synonym, null as CatalogId")
				.From("Farm.SynonymFirmCr sfc")
				.Where("sfc.PriceCode = ?PriceId", new {priceId});
		}

		public static void UpdateStatusByProduct(DataRow item, uint productId, uint catalogId, bool pharmacie, bool markAsJunk)
		{
			var table = item.Table;
			var name = String.Format("{0}  ", item["UEName1"]);

			var producer = item["UEFirmCr"].ToString();
			var synonyms = GetSynonyms(producer);
			synonyms = synonyms.Concat(synonyms.Where(s => s.Name == producer)).ToList();

			for(int i = 0; i < table.Rows.Count; i++)
			{
				var row = table.Rows[i];
				if (name == GetName(row))
				{
					if (((FormMask)Convert.ToByte(row["UEStatus"]) & FormMask.NameForm) != FormMask.NameForm)
					{
						//TODO: Здесь потребуется завести дополнительный столбец в таблицу нераспознанных выражений
						row["UEStatus"] = (int)((FormMask)Convert.ToByte(row["UEStatus"]) | FormMask.NameForm);
						row["UEJunk"] = Convert.ToByte(markAsJunk);
						row["UEPriorProductId"] = productId;
						row["UEPriorCatalogId"] = catalogId;
						row["Pharmacie"] = pharmacie;

						TryToPickProducerSynonym(row, synonyms);
					}
				}
			}
		}

		private static void TryToPickProducerSynonym(DataRow destination, IEnumerable<ProducerSynonym> synonyms)
		{
			var producer = destination["UEFirmCr"].ToString();
			if (String.IsNullOrEmpty(producer))
			{
				destination["UEStatus"] = (int) (GetStatus(destination) | FormMask.FirmForm);
				return;
			}

			synonyms = synonyms.OrderByDescending(s => s.ProducerId);
			var assortment = LoadAssortmentByCatalog(Convert.ToUInt32(destination["UEPriorCatalogId"]));
			var synonym = synonyms.FirstOrDefault(s => s.IsApplicable(destination, assortment));

			if (synonym != null)
				synonym.Apply(destination);
		}

		public static void UpdateStatusByProducer(DataRow item, uint producerId)
		{
			var table = item.Table;
			var assortment = LoadAssortmentByProducer(producerId);

			var synonym = ProducerSynonym.CreateSynonym(item, producerId);
			
			var loadedSynonym = LoadSynonym()
				.Where("sfc.Synonym = ?Name && sfc.CodeFirmCr = ?ProducerId", new {synonym.Name, synonym.ProducerId})
				.SingleOrDefault(ProducerSynonym.CreateSynonym);

			if (loadedSynonym != null)
				synonym = loadedSynonym;
			else
				synonyms.Add(synonym);

			for(var i = 0; i < table.Rows.Count; i++)
			{
				var row = table.Rows[i];
				if (synonym.IsApplicable(row, assortment))
					synonym.Apply(row);
			}
		}

		private static IEnumerable<ProducerSynonym> GetSynonyms(string synonym)
		{
			return With.Slave(c => {
				var command = new MySqlCommand(@"
select sfc.SynonymFirmCrCode, sfc.CodeFirmCr as ProducerId, sfc.Synonym, null as CatalogId
from Farm.SynonymFirmCr sfc
where sfc.Synonym = ?Synonym and sfc.PriceCode = ?PriceCode and sfc.CodeFirmCr is not null", c);
				command.Parameters.AddWithValue("?PriceCode", priceId);
				command.Parameters.AddWithValue("?Synonym", synonym);

				List<ProducerSynonym> synonyms;

				using (var reader = command.ExecuteReader())
					synonyms = reader.Cast<DbDataRecord>().Select(ProducerSynonym.CreateSynonym).ToList();

				command = new MySqlCommand(@"
select sfc.SynonymFirmCrCode, sfc.CodeFirmCr as ProducerId, sfc.Synonym, e.CatalogId
from Farm.Excludes e
join Farm.SynonymFirmCr sfc on sfc.Synonym = e.ProducerSynonym and e.PriceCode = sfc.PriceCode
where sfc.Synonym = ?Synonym and sfc.PriceCode = ?PriceCode and sfc.CodeFirmCr is null
group by e.CatalogId, sfc.Synonym", c);
				command.Parameters.AddWithValue("?PriceCode", priceId);
				command.Parameters.AddWithValue("?Synonym", synonym);

				using(var reader = command.ExecuteReader())
					synonyms = synonyms.Concat(reader.Cast<DbDataRecord>().Select(ProducerSynonym.CreateSynonym)).ToList();

				return synonyms;
			});
		}

		public static DataTable LoadAssortmentByCatalog(uint producerId)
		{
			return With.Slave(c => {
				var command = new MySqlCommand(@"
select a.CatalogId, a.ProducerId
from Catalogs.Assortment a
where a.CatalogId = ?CatalogId", c);
				command.Parameters.AddWithValue("?CatalogId", producerId);

				var adapter = new MySqlDataAdapter(command);
				var table = new DataTable();
				adapter.Fill(table);
				return table;
			});
		}

		public static DataTable LoadAssortmentByProducer(uint producerId)
		{
			return With.Slave(c => {
				var command = new MySqlCommand(@"
select a.CatalogId, a.ProducerId
from Catalogs.Assortment a
where a.ProducerId = ?ProducerId", c);
				command.Parameters.AddWithValue("?producerId", producerId);

				var adapter = new MySqlDataAdapter(command);
				var table = new DataTable();
				adapter.Fill(table);
				return table;
			});
		}


		public static FormMask GetStatus(DataRow row)
		{
			return (FormMask) Convert.ToByte(row["UEStatus"]);
		}

		public static string GetName(DataRow item)
		{
			return String.Format("{0}  ", item["UEName1"]);
		}

		public static void CreateExclude(DataRow source)
		{
			if (!Convert.ToBoolean(source["Pharmacie"]))
				return;

			var exclude = new Exclude {
				Name = source["UEFirmCr"].ToString(),
				CatalogId = Convert.ToUInt32(source["UEPriorCatalogId"]),
			};
			synonyms.Add(exclude);

			foreach (var destination in source.Table.Rows.Cast<DataRow>())
				if (exclude.IsApplicable(destination, null))
					exclude.Apply(destination);
		}
	}
}