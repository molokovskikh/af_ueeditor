using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace UEEditor.Tests
{
	[TestFixture]
	public class ForbiddenProducerSynonymFixture
	{
		[Test]
		public void IsApplicableTest()
		{
			var forbidden = new ForbiddenProducerSynonym() {
				Name = "Производитель"
			};
			var table = new DataTable();
			table.Columns.Add(new DataColumn("UEStatus"));
			table.Columns.Add(new DataColumn("UEPriorProductId"));
			table.Columns.Add(new DataColumn("UEFirmCr"));
			table.Columns.Add(new DataColumn("UEPriorCatalogId"));
			var row = table.NewRow();
			row["UEstatus"] = 1;
			row["UEFirmCr"] = "Производитель";
			row["UEPriorCatalogId"] = 1;
			row["UEPriorProductId"] = 1;
			table.Rows.Add(row);
			row = table.NewRow();
			row["UEstatus"] = 1;
			row["UEFirmCr"] = "Производитель";
			row["UEPriorCatalogId"] = 5;
			row["UEPriorProductId"] = 1;
			table.Rows.Add(row);
			row = table.NewRow();
			row["UEstatus"] = 2;
			row["UEFirmCr"] = "Производитель";
			row["UEPriorCatalogId"] = 1;
			row["UEPriorProductId"] = 1;
			table.Rows.Add(row);
			row = table.NewRow();
			row["UEstatus"] = 1;
			row["UEFirmCr"] = "Тест";
			row["UEPriorCatalogId"] = 5;
			row["UEPriorProductId"] = 1;
			table.Rows.Add(row);
			Assert.That(forbidden.IsApplicable(table.Rows[0], null), Is.True);
			Assert.That(forbidden.IsApplicable(table.Rows[1], null), Is.True);
			Assert.That(forbidden.IsApplicable(table.Rows[2], null), Is.False);
			Assert.That(forbidden.IsApplicable(table.Rows[3], null), Is.False);
		}
	}
}
