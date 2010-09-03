﻿using System;
using System.Data;
using Common.MySql;
using MySql.Data.MySqlClient;

namespace UEEditor
{
	public class ProducerQuery
	{
		public Query Producers;
		public Query Equivalents;

		public static ProducerQuery Query(Action<ProducerQuery> action)
		{
			var query = new ProducerQuery();
			query.Producers = new Query()
				.Select(@"
p.Id As CCode,
p.Name As CName")
				.From(@"
catalogs.Producers P
  join catalogs.assortment a on a.CatalogId = ?CatalogId and a.ProducerId = p.Id");

			query.Equivalents = new Query()
				.Select(@"
p.Id As CCode,
concat(pe.Name, ' (', p.Name, ')') As CName")
				.From(@"
catalogs.Producers P
  join catalogs.ProducerEquivalents PE on pe.ProducerId = p.Id
  join catalogs.assortment a on a.CatalogId = ?CatalogId and a.ProducerId = p.Id");
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