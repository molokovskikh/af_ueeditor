using System;
using System.Data;
using System.Linq;
using Castle.ActiveRecord;
using Common.MySql;
using Common.Tools;
using NHibernate.Linq;
using NUnit.Framework;
using Test.Support;
using Test.Support.Suppliers;

namespace UEEditor.Tests
{
	[TestFixture]
	public class ProducerQueryFixture : IntegrationFixture
	{
		[Test]
		public void Query_producers()
		{
			var product = session.Query<TestCatalogProduct>().First(c => c.Producers.Any());
			var catalogId = product.Id;

			var query = ProducerQuery.Query(true, catalogId, q => { });
			Assert.IsNotNull(query.Load());
		}

		[Test]
		public void If_exclude_not_for_pharmacie_ignore_it()
		{
			var product = session.Query<TestCatalogProduct>().First(c => !c.Pharmacie);
			var exclude = new DbExclude {
				CatalogId = product.Id
			};
			With.Connection(c => { Assert.That(Updater.IsExcludeCorrect(c, exclude), Is.False); });
		}

		[Test]
		public void Calculate_have_offers()
		{
			session.CreateSQLQuery("delete from Farm.Core0").ExecuteUpdate();
			var supplier = TestSupplier.CreateNaked(session);
			var product1 = new TestProduct("Тестовая фармацевтика", "табл.") {
				CatalogProduct = {
					Pharmacie = true
				}
			};
			var product2 = new TestProduct("Тестовая нефармацевтика", "табл.");
			var producer1 = new TestProducer("Тестовый производитель1");
			var producer2 = new TestProducer("Тестовый производитель2");
			session.Save(product1);
			session.Save(product2);
			session.Save(producer1);
			session.Save(producer2);
			session.Save(new TestAssortment(product1, producer1));
			session.Save(new TestAssortment(product1, producer2));
			supplier.CreateSampleCore(session, new [] { product1, product2 }, new [] { producer1, producer1 });
			session.CreateSQLQuery("call Farm.UpdateOffersStat();").ExecuteUpdate();

			FlushAndCommit();
			var query = ProducerQuery.Query(true, product1.CatalogProduct.Id);
			var table = query.Load();
			Assert.IsTrue(Convert.ToBoolean(table.AsEnumerable()
				.First(x => Convert.ToUInt32(x["CCode"]) == producer1.Id)["HaveOffers"]));
			Assert.IsFalse(Convert.ToBoolean(table.AsEnumerable()
				.First(x => Convert.ToUInt32(x["CCode"]) == producer2.Id)["HaveOffers"]));

			query = ProducerQuery.Query(false, product2.CatalogProduct.Id);
			table = query.Load();

			Assert.IsTrue(Convert.ToBoolean(table.AsEnumerable()
				.First(x => Convert.ToUInt32(x["CCode"]) == producer1.Id)["HaveOffers"]));
			Assert.IsFalse(Convert.ToBoolean(table.AsEnumerable()
				.First(x => Convert.ToUInt32(x["CCode"]) == producer2.Id)["HaveOffers"]));
		}
	}
}