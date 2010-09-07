using System.Data;
using System.Linq;
using Castle.ActiveRecord;
using NUnit.Framework;
using Test.Support;

namespace UEEditor.Tests
{
	[TestFixture]
	public class ProducerQueryFixture
	{
		[Test]
		public void Query_producers()
		{
			uint catalogId;
			using (new SessionScope())
			{
				var product = TestCatalogProduct.Queryable.First(c => c.Producers.Count() > 0);
				catalogId = product.Id;
			}

			var query = ProducerQuery.Query(q => {
				q.Equivalents.Where("a.CatalogId = ?catalogId", new {catalogId});
				q.Producers.Where("a.CatalogId = ?catalogId", new {catalogId});
			});
			var table = new DataTable();
			query.Load(table);
		}
	}
}