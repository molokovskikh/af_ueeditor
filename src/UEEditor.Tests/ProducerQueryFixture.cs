using System.Data;
using System.Linq;
using Castle.ActiveRecord;
using Common.MySql;
using NUnit.Framework;
using Test.Support;

namespace UEEditor.Tests
{
	[TestFixture]
	public class ProducerQueryFixture
	{
		private SessionScope session;

		[SetUp]
		public void Setup()
		{
			session = new SessionScope();
		}

		[TearDown]
		public void TearDown()
		{
			if (session != null)
				session.Dispose();
		}

		[Test]
		public void Query_producers()
		{
			var product = TestCatalogProduct.Queryable.First(c => c.Producers.Count() > 0);
			var catalogId = product.Id;

			var query = ProducerQuery.Query(true, catalogId, q => { });
			var table = new DataTable();
			query.Load(table);
		}

		[Test]
		public void If_exclude_not_for_pharmacie_ignore_it()
		{
			var product = TestCatalogProduct.Queryable.First(c => !c.Pharmacie);
			var exclude = new DbExclude {
				CatalogId = product.Id
			};
			With.Connection(c => { Assert.That(Updater.IsExcludeCorrect(c, exclude), Is.False); });
		}
	}
}