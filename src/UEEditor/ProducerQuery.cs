using System;
using System.Data;
using Common.MySql;
using MySql.Data.MySqlClient;

namespace UEEditor
{
	public class ProducerQuery
	{
		public Query Producers;
		public Query Equivalents;

		public static ProducerQuery Query(bool pharmacie, uint catalogId, Action<ProducerQuery> action = null)
		{
			var query = new ProducerQuery();
			query.Producers = new Query()
				.Select(@"
p.Id As CCode,
p.Name As CName,
s.Id is not null as HaveOffers")
				.From(@"(catalogs.Producers P, Catalogs.Products pr)
left join OffersStat s on s.ProducerId = p.id and s.ProductId = pr.Id")
				.Where("pr.CatalogId = ?CatalogId", new { catalogId });

			query.Equivalents = new Query()
				.Select(@"
p.Id As CCode,
concat(pe.Name, ' [', p.Name, ']') As CName,
s.Id is not null as HaveOffers")
				.From(@"
(catalogs.Producers P, Catalogs.Products pr)
	join catalogs.ProducerEquivalents PE on pe.ProducerId = p.Id
	left join OffersStat s on s.ProducerId = p.id and s.ProductId = pr.Id")
				.Where("pr.CatalogId = ?CatalogId", new { catalogId });

			if (pharmacie) {
				query.Producers
					.Join("join catalogs.assortment a on a.ProducerId = p.Id and pr.CatalogId = a.CatalogId")
					.Where("a.Checked = 1");
				query.Equivalents
					.Join("join catalogs.assortment a on a.ProducerId = p.Id and pr.CatalogId = a.CatalogId")
					.Where("a.Checked = 1");
			}

			action?.Invoke(query);
			return query;
		}

		public DataTable Load()
		{
			var table = new DataTable("data");
			Load(table);
			return table;
		}

		public void Load(DataTable table)
		{
			table.Clear();

			var sql = Producers.ToSql() + " union " + Equivalents.ToSql() + " order by CName";

			With.Connection(c => {
				var adapter = new MySqlDataAdapter(sql, c);
				Producers.BindParameters(adapter.SelectCommand);
				adapter.Fill(table);
			});

			var drUnknown = table.NewRow();
			drUnknown["CCode"] = 0;
			drUnknown["CName"] = "производитель не известен";
			table.Rows.InsertAt(drUnknown, 0);
		}
	}
}