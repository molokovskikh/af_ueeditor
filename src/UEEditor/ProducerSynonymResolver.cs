using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Common.MySql;
using MySql.Data.MySqlClient;
using log4net;

namespace UEEditor
{
	public class ProducerSynonymResolver
	{
		public static ProducerSynonymResolver Resolver;

		private List<ProducerSynonym> synonyms = new List<ProducerSynonym>();
		private uint priceId;

		private static ILog log = LogManager.GetLogger(typeof(ProducerSynonymResolver));

		public ProducerSynonymResolver(uint priceId)
		{
			this.priceId = priceId;
			synonyms = new List<ProducerSynonym>();
		}

		public Query LoadSynonym()
		{
			return new Query()
				.Select("sfc.SynonymFirmCrCode, sfc.CodeFirmCr as ProducerId, sfc.Synonym, null as CatalogId")
				.From("Farm.SynonymFirmCr sfc")
				.Where("sfc.PriceCode = ?PriceId", new { priceId });
		}

		public void ResolveProduct(DataRow item, DataRow product, bool markAsJunk)
		{
			var productId = Convert.ToUInt32(product["Id"]);
			var catalogId = Convert.ToUInt32(product["CatalogId"]);
			var pharmacie = Convert.ToBoolean(product["Pharmacie"]);
			uint? monobrendProducerId = null;
			if (!(product["MonobrendProducerId"] is DBNull))
				monobrendProducerId = Convert.ToUInt32(product["MonobrendProducerId"]);

			var table = item.Table;
			var name = item["UEName1"].ToString().Trim();

			var producer = item["UEFirmCr"].ToString();
			var applicableSynonyms = GetSynonyms(producer)
				.Concat(synonyms.Where(s => String.Equals(s.Name, producer, StringComparison.CurrentCultureIgnoreCase)))
				.ToList();

			for (int i = 0; i < table.Rows.Count; i++) {
				var row = table.Rows[i];
				if (String.Equals(name, GetName(row), StringComparison.CurrentCultureIgnoreCase)) {
					if (((FormMask)Convert.ToByte(row["UEStatus"]) & FormMask.NameForm) != FormMask.NameForm) {
						//TODO: Здесь потребуется завести дополнительный столбец в таблицу нераспознанных выражений
						row["UEStatus"] = (int)((FormMask)Convert.ToByte(row["UEStatus"]) | FormMask.NameForm);
						row["UEJunk"] = Convert.ToByte(markAsJunk);
						row["UEPriorProductId"] = productId;
						row["UEPriorCatalogId"] = catalogId;
						row["Pharmacie"] = pharmacie;

						TryToPickProducerSynonym(row, applicableSynonyms);

						if (monobrendProducerId != null
							&& ((FormMask)Convert.ToByte(row["UEStatus"]) & FormMask.FirmForm) != FormMask.FirmForm) {
							var synonym = ProducerSynonym.CreateSynonym(item, monobrendProducerId.Value);
							applicableSynonyms.Add(synonym);
							synonyms.Add(synonym);
							synonym.Apply(item);
						}
					}
				}
			}
		}

		public void UnresolveProduct(DataRow row)
		{
			try {
				var status = GetStatus(row);
				if ((status & FormMask.NameForm) == FormMask.NameForm) {
					row["UEStatus"] = status & (~FormMask.NameForm);
					row["UEPriorProductId"] = DBNull.Value;
					row["UEPriorCatalogId"] = DBNull.Value;
					row["Pharmacie"] = false;
				}
				UnresolveProducer(row);
			}
			catch (Exception e) {
				log.Error(String.Format("Ошибка при отмене распознования позиции {0}", GetName(row)), e);
			}
		}

		public void UnresolveProducer(DataRow row)
		{
			try {
				var status = GetStatus(row);
				if ((status & FormMask.FirmForm) == FormMask.FirmForm) {
					var synonym = row["SynonymObject"] as ProducerSynonym;

					row["UEStatus"] = status & (~FormMask.FirmForm);
					row["UEProducerSynonymId"] = DBNull.Value;
					row["UEPriorProducerId"] = DBNull.Value;
					row["SynonymObject"] = null;

					if (synonym != null
						&& !row.Table.Rows.Cast<DataRow>().Any(r => r["SynonymObject"] == synonym)) {
						synonyms.Remove(synonym);
					}
				}
			}
			catch (Exception e) {
				log.Error(String.Format("Ошибка при отмене распознования позиции {0}", GetName(row)), e);
			}
		}

		private void TryToPickProducerSynonym(DataRow destination, IEnumerable<ProducerSynonym> synonyms)
		{
			var producer = destination["UEFirmCr"].ToString();
			if (String.IsNullOrEmpty(producer)) {
				destination["UEStatus"] = (int)(GetStatus(destination) | FormMask.FirmForm);
				return;
			}

			synonyms = synonyms.OrderByDescending(s => s.ProducerId);
			var assortment = LoadAssortmentByCatalog(Convert.ToUInt32(destination["UEPriorCatalogId"]));
			var synonym = synonyms.FirstOrDefault(s => s.IsApplicable(destination, assortment));

			if (synonym != null)
				synonym.Apply(destination);
		}

		public void ResolveProducer(DataRow item, uint producerId)
		{
			var table = item.Table;
			var assortment = LoadAssortmentByProducer(producerId);

			var synonym = ProducerSynonym.CreateSynonym(item, producerId);

			var loadedSynonym = LoadSynonym()
				.Where("sfc.Synonym = ?Name && sfc.CodeFirmCr = ?ProducerId", new { synonym.Name, synonym.ProducerId })
				.SingleOrDefault(ProducerSynonym.CreateSynonym);

			if (loadedSynonym != null)
				synonym = loadedSynonym;
			else
				synonyms.Add(synonym);

			for (var i = 0; i < table.Rows.Count; i++) {
				var row = table.Rows[i];
				if (synonym.IsApplicable(row, assortment))
					synonym.Apply(row);
			}
		}

		private IEnumerable<ProducerSynonym> GetSynonyms(string synonym)
		{
			return With.Connection(c => {
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

				using (var reader = command.ExecuteReader())
					synonyms = synonyms.Concat(reader.Cast<DbDataRecord>().Select(ProducerSynonym.CreateSynonym)).ToList();

				return synonyms;
			});
		}

		public DataTable LoadAssortmentByCatalog(uint producerId)
		{
			return With.Connection(c => {
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

		public DataTable LoadAssortmentByProducer(uint producerId)
		{
			return With.Connection(c => {
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
			return (FormMask)Convert.ToByte(row["UEStatus"]);
		}

		public static string GetName(DataRow item)
		{
			return item["UEName1"].ToString().Trim();
		}

		public void ForbidProducer(DataRow source)
		{
			var forbiddenProducer = new ForbiddenProducerSynonym {
				Name = source["UEFirmCr"].ToString()
			};
			synonyms.Add(forbiddenProducer);
			foreach (var destination in source.Table.Rows.Cast<DataRow>())
				if (forbiddenProducer.IsApplicable(destination, null))
					forbiddenProducer.Apply(destination);
		}

		public void ExcludeProducer(DataRow source)
		{
			if (!Convert.ToBoolean(source["Pharmacie"]))
				return;

			var exclude = new Exclude {
				Name = source["UEFirmCr"].ToString(),
				CatalogId = Convert.ToUInt32(source["UEPriorCatalogId"]),
				SupplierCode = source["UECode"].ToString()
			};
			synonyms.Add(exclude);

			foreach (var destination in source.Table.Rows.Cast<DataRow>())
				if (exclude.IsApplicable(destination, null))
					exclude.Apply(destination);
		}
	}
}