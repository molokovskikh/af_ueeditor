using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace UEEditor
{
	public class DbExclude
	{
		public uint Id;
		public uint CatalogId;
		public string ProducerSynonym;
		public bool DoNotShow;
	}

	public class Updater
	{
		public static void UpdateProducerSynonym(List<DataRow> rows, List<DbExclude> excludes, DataTable dtSynonymFirmCr, uint priceId, uint childPriceId, Statistics stat)
		{
			//priceprocessor создает на одно наименование один синоним, но в результате сопоставления мы можем получить два разных синонима
			var synonyms = rows.Select(r => (ProducerSynonym) r["SynonymObject"]);
			var groups = synonyms.Where(s => !(s is Exclude)).GroupBy(s => new {s.ProducerId, s.Name});
			foreach (var synonymGroup in groups)
			{
				var synonym = synonymGroup.First();
				var synonymRow = dtSynonymFirmCr.Select("SynonymFirmCrCode = " + synonym.Id);
				if (synonymRow.Any(s => Equals(synonym.ProducerId, s["CodeFirmCr"])))
					return;
				if (synonymRow.Length == 0)
					CreateSynonym(dtSynonymFirmCr, synonym.Name, synonym.ProducerId, stat, priceId, childPriceId);
				else
					UpdateSynonym(synonymRow[0], synonym.ProducerId, priceId, childPriceId);

				if (synonymGroup.Count() > 1)
				{
					foreach (var s in synonymGroup.Skip(1))
						CreateSynonym(dtSynonymFirmCr, s.Name, s.ProducerId, stat, priceId, childPriceId);
				}
			}

			foreach (var excludeGroups in synonyms.OfType<Exclude>().GroupBy(e => new {e.CatalogId, e.Name}))
			{
				var exclude = excludeGroups.First();
				var synonymRow = dtSynonymFirmCr.Select(String.Format("Synonym = '{0}' and CodeFirmCr is null", exclude.Name.Replace("'", "''")));
				if (synonymRow.Length == 0)
					CreateSynonym(dtSynonymFirmCr, exclude.Name, 0, stat, priceId, childPriceId);
				else
					UpdateSynonym(synonymRow[0], 0, priceId, childPriceId);
				CreateExclude(exclude, excludes);
			}
		}

		private static void CreateExclude(Exclude exclude, List<DbExclude> excludes)
		{
			if (excludes.Any(e => e.CatalogId == exclude.CatalogId && e.ProducerSynonym.Equals(exclude.Name, StringComparison.CurrentCultureIgnoreCase)))
				return;

			excludes.Add(new DbExclude {
				CatalogId = exclude.CatalogId,
				DoNotShow = exclude.State == ProducerSynonymState.Unknown,
				ProducerSynonym = exclude.Name,
			});
		}

		private static void UpdateSynonym(DataRow synonym, uint producerId, uint priceId, uint childPriceId)
		{
			if (producerId > 0)
				synonym["CodeFirmCr"] = producerId;
			else
				synonym["CodeFirmCr"] = DBNull.Value;
			synonym["PriceCode"] = priceId;
			if (priceId != childPriceId)
				synonym["ChildPriceCode"] = childPriceId;
		}

		public static void CreateSynonym(DataTable synonyms, string name, uint producerId, Statistics stat, uint priceId, uint childPriceId)
		{
			var synonym = synonyms.NewRow();
			UpdateSynonym(synonym, producerId, priceId, childPriceId);
			synonym["Synonym"] = name;
			try
			{
				synonyms.Rows.Add(synonym);
				stat.SynonymFirmCrCount++;
			}
			catch (ConstraintException)
			{}
		}
	}
}