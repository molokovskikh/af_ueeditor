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

		public static ProducerQuery Query(bool pharmacie, uint catalogId, Action<ProducerQuery> action)
		{
			var query = new ProducerQuery();
			query.Producers = new Query()
				.Select(@"
p.Id As CCode,
p.Name As CName")
				.From("catalogs.Producers P");

			query.Equivalents = new Query()
				.Select(@"
p.Id As CCode,
concat(pe.Name, ' [', p.Name, ']') As CName")
				.From(@"
catalogs.Producers P
  join catalogs.ProducerEquivalents PE on pe.ProducerId = p.Id");

			if (pharmacie)
			{
				query.Producers
					.Join("join catalogs.assortment a on a.ProducerId = p.Id")
					.Where("a.CatalogId = ?CatalogId", new {catalogId})
					.Where("a.Checked = 1");
				query.Equivalents
					.Join("join catalogs.assortment a on a.ProducerId = p.Id")
					.Where("a.CatalogId = ?CatalogId", new {catalogId})
					.Where("a.Checked = 1");
			}

			action(query);
			return query;
		}

		public void Load(DataTable table)
		{
			table.Clear();

			var sql = Producers.ToSql() + " union " + Equivalents.ToSql() + " order by CName";

			With.Slave(c => {
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