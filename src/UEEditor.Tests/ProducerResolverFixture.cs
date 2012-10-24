using System;
using System.Configuration;
using System.Data;
using System.Linq;
using Castle.ActiveRecord;
using Common.MySql;
using Common.Tools;
using MySql.Data.MySqlClient;
using NHibernate.Linq;
using NUnit.Framework;
using Test.Support;
using Test.Support.Catalog;
using Test.Support.Suppliers;

namespace UEEditor.Tests
{
	public class FakeNotifier : IProgressNotifier
	{
		public string Status { get; set; }
		public string Error { get; set; }
		public int ApplyProgress { get; set; }
	}

	[TestFixture]
	public class ProducerResolverFixture
	{
		TestPrice price;
		DataTable data;
		ProducerSynonymResolver resolver;

		[SetUp]
		public void SetUp()
		{
			using(new TransactionScope())
				price = TestSupplier.CreateTestSupplierWithPrice();

			resolver = new ProducerSynonymResolver(price.Id);
		}

		[Test]
		public void After_unresolve_product_clear_exclude()
		{
			TestUnrecExp exp;
			TestProduct product;
			TestProduct product2;
			using(new SessionScope())
			{
				exp = new TestUnrecExp("ПОЛЫНИ ГОРЬКОЙ ТРАВА сырье 75 г N1", "Кентавр ХФК", price);
				exp.Save();
				//исключения работают только для фармацевтики
				var products = Pharmacie().Take(2).ToList();
				product = products[0];
				product2 = products[1];
			}
			Load();
			var row = GetRow(exp);
			Resolve(exp, product);
			resolver.ExcludeProducer(row);
			resolver.UnresolveProduct(row);

			Resolve(exp, product2);
			Save();

			using(new SessionScope())
			{
				var excludes = TestExclude.Queryable.Where(e => e.Price == price);
				Assert.That(excludes.Count(), Is.EqualTo(0), excludes.Implode());
			}
		}

		[Test]
		public void Create_different_synonym_on_same_base_synonym()
		{
			TestProducer producer1;
			TestProducer producer2;
			TestUnrecExp exp1;
			TestUnrecExp exp2;
			TestProducerSynonym producerSynonym;
			using (new TransactionScope())
			{
				producerSynonym = new TestProducerSynonym("Test", null, price);
				producerSynonym.Save();
				new TestAutomaticSynonym{ProducerSynonymId = producerSynonym.Id}.Create();

				var productsWithAssortment = Pharmacie().Take(30).ToList().Where(p => p.CatalogProduct.Producers.Count > 0);
				var first = productsWithAssortment.First();
				var products = productsWithAssortment.Where(p => !p.CatalogProduct.Producers.Any(pr => pr.Id == first.CatalogProduct.Producers[0].Id)).Take(1).ToList();
				products.Add(first);

				var synonym1 = new TestProductSynonym(products[0].CatalogProduct.Name, products[0], price);
				synonym1.Save();
				var synonym2 = new TestProductSynonym(products[1].CatalogProduct.Name, products[1], price);
				synonym2.Save();
				exp1 = new TestUnrecExp(synonym1, producerSynonym);
				exp1.Save();
				exp2 = new TestUnrecExp(synonym2, producerSynonym);
				exp2.Save();
				producer1 = synonym1.Product.CatalogProduct.Producers.First();
				producer2 = synonym2.Product.CatalogProduct.Producers.First();
			}

			Load();
			Resolve(exp1, producer1);
			Resolve(exp2, producer2);
			Save();
			using (new SessionScope(FlushAction.Never))
			{
				var synonyms = TestProducerSynonym.Queryable.Where(s => s.Price == price).ToList();
				Assert.That(synonyms.Count, Is.EqualTo(2), "должны были создать два разных синонима, создали только ({0})",
					synonyms.Implode(s => s.Name));
				Assert.That(synonyms[0].Name, Is.EqualTo(producerSynonym.Name));
				Assert.That(synonyms[0].Producer.Id, Is.EqualTo(producer2.Id));
				Assert.That(synonyms[1].Name, Is.EqualTo(producerSynonym.Name));
				Assert.That(synonyms[1].Producer.Id, Is.EqualTo(producer1.Id));
			}
		}

		[Test]
		public void Do_not_create_empty_producer_synonyms()
		{
			TestUnrecExp expression;
			TestProduct product;
			using (new SessionScope())
			{
				expression = new TestUnrecExp("test", "", price);
				expression.Save();
				product = TestProduct.Queryable.Where(p => !p.Hidden).First();
			}
			Load();
			Resolve(expression, product);
			Save();

			var status = ProducerSynonymResolver.GetStatus(GetRow(expression));
			Assert.That(status & FormMask.FirmForm, Is.EqualTo(FormMask.FirmForm));
		}

		[Test]
		public void CreateForbiddenProducerName()
		{
			TestUnrecExp expression;
			using (new SessionScope()) {
				var product = new TestProduct("Тестовый Продукт");
				var catalogProduct = product.CatalogProduct;
				catalogProduct.Pharmacie = true;
				product.Save();
				var synonym = new TestProductSynonym("test", product, price);
				synonym.Save();
				var producerSynonym = new TestProducerSynonym("testTest", null, price);
				producerSynonym.Save();
				expression = new TestUnrecExp(synonym, producerSynonym);
				expression.Save();
			}
			Load();
			resolver.ForbidProducer(GetRow(expression));
			Save();
			var sessionHolder = ActiveRecordMediator.GetSessionFactoryHolder();
			using(var session = sessionHolder.CreateSession(typeof(ActiveRecordBase))) {
				var query = session.CreateSQLQuery(String.Format("SELECT count(*) FROM farm.ForbiddenProducers F where F.Name='{0}'", "testTest"));
				var count = query.UniqueResult();
				Assert.That(count, Is.GreaterThan(0));
				var exclude = session.Query<TestExclude>().Where(e => e.Price == price);
				Assert.That(exclude.Count(), Is.EqualTo(0));
			}
		}

		[Test]
		public void Create_excludes()
		{
			TestUnrecExp expression;
			TestCatalogProduct catalogProduct;
			using(new SessionScope())
			{
				var product = Pharmacie().First();
				catalogProduct = product.CatalogProduct;
				var synonym = new TestProductSynonym("test", product, price);
				synonym.Save();
				var producerSynonym = new TestProducerSynonym("test", null, price);
				producerSynonym.Save();
				expression = new TestUnrecExp(synonym, producerSynonym);
				expression.Save();
			}

			Load();
			resolver.ExcludeProducer(GetRow(expression));
			Save();

			using (new SessionScope(FlushAction.Never))
			{
				var exlcudes = TestExclude.Queryable.Where(e => e.Price == price).ToList();
				Assert.That(exlcudes.Count, Is.EqualTo(1), "не создали исключение");
				var exclude = exlcudes.Single();
				Assert.That(exclude.ProducerSynonym, Is.EqualTo(expression.FirmCr));
				Assert.That(exclude.CatalogProduct.Id, Is.EqualTo(catalogProduct.Id));
				Assert.That(exclude.OriginalSynonym.Id, Is.EqualTo(expression.ProductSynonymId));
			}
		}

		[Test]
		public void Fill_original_synonym_for_second_created_exclude()
		{
			TestUnrecExp expression;
			TestProduct product;

			using (new SessionScope())
			{
				expression = new TestUnrecExp("Полыни горькой трава" + new Random().Next(), "Тестовый производитель" + new Random().Next(), price);
				expression.Save();
				product = Pharmacie().First();
			}

			Load();
			Resolve(expression, product);
			resolver.ExcludeProducer(GetRow(expression));
			Save();

			TestProduct product2;
			using (new SessionScope())
			{
				product2 = Pharmacie().Where(p => p.Id != product.Id).First();
			}

			resolver = new ProducerSynonymResolver(price.Id);
			Load();
			Resolve(expression, product2);
			resolver.ExcludeProducer(GetRow(expression));
			Save();

			using (new SessionScope(FlushAction.Never))
			{
				var exlcudes = TestExclude.Queryable.Where(e => e.Price == price).ToList();
				Assert.That(exlcudes.Count, Is.EqualTo(2), "не создали исключение");
				var exclude = exlcudes[0];
				Assert.That(exclude.OriginalSynonym, Is.Not.Null);
				exclude = exlcudes[1];
				Assert.That(exclude.OriginalSynonym, Is.Not.Null);
			}
		}

		[Test]
		public void Fill_original_synonym_for_created_exclude()
		{
			TestUnrecExp expression;
			TestProduct product;

            
			using (new SessionScope())
			{
				expression = new TestUnrecExp("Полыни горькой трава 2", "Тестовый производитель", price);
				expression.Save();
				product = Pharmacie().First();
			}

			Load();
			Resolve(expression, product);
			resolver.ExcludeProducer(GetRow(expression));
			Save();

			using (new SessionScope(FlushAction.Never))
			{
				var exlcudes = TestExclude.Queryable.Where(e => e.Price == price).ToList();
				Assert.That(exlcudes.Count, Is.EqualTo(1), "не создали исключение");
				var exclude = exlcudes.Single();
				Assert.That(exclude.OriginalSynonym, Is.Not.Null);
			}
		}

		[Test]
		public void TestFillSupplierCode()
		{
			TestUnrecExp expression;
			TestProduct product;
			TestProducer producer;
			using (new SessionScope())
			{
				expression = new TestUnrecExp("Тестовый продукт", "Тестовый производитель", price);
				expression.Code = new Random().Next().ToString();
				expression.Save();
				product = Pharmacie().First();
				producer = TestProducer.Queryable.FirstOrDefault();
				TestAssortment.CheckAndCreate(product, producer);
			}
			Load();
			Resolve(expression, product);
			Resolve(expression, producer);
			Save();
			using (new SessionScope())
			{
				var synonym = TestProductSynonym.Queryable.Where(s => s.Price == price).FirstOrDefault();
				var synonymFirmCr = TestProducerSynonym.Queryable.Where(s => s.Price == price).FirstOrDefault();
				Assert.That(synonym, Is.Not.Null);
				Assert.That(synonymFirmCr, Is.Not.Null);
				Assert.That(synonym.SupplierCode, Is.EqualTo(expression.Code));
				Assert.That(synonymFirmCr.SupplierCode, Is.EqualTo(expression.Code));
			}
		}

		[Test]
		public void TestFillSupplierCodeIfProducerExclude()
		{
			TestUnrecExp expression;
			TestProduct product;			
			using (new SessionScope())
			{
				expression = new TestUnrecExp("Тестовый продукт", "Тестовый производитель", price);
				expression.Code = new Random().Next().ToString();
				expression.Save();
				product = Pharmacie().First();
			}
			Load();
			Resolve(expression, product);
			resolver.ExcludeProducer(GetRow(expression));
			Save();
			using (new SessionScope())
			{
				var synonym = TestProductSynonym.Queryable.Where(s => s.Price == price).FirstOrDefault();
				var synonymFirmCr = TestProducerSynonym.Queryable.Where(s => s.Price == price).FirstOrDefault();
				Assert.That(synonym, Is.Not.Null);
				Assert.That(synonymFirmCr, Is.Not.Null);
				Assert.That(synonym.SupplierCode, Is.EqualTo(expression.Code));
				Assert.That(synonymFirmCr.SupplierCode, Is.EqualTo(expression.Code));
				var exlcudes = TestExclude.Queryable.Where(e => e.Price == price).ToList();
				Assert.That(exlcudes.Count, Is.EqualTo(1), "не создали исключение");
			}

		}

		[Test]
		public void Unresolve_product_if_product_was_hidden_before_save()
		{
			TestUnrecExp expression;
			TestProduct product;

			using (new SessionScope())
			{
				expression = new TestUnrecExp("test", "test", price);
				expression.Save();
				product = Pharmacie().First();
			}

			Load();
			Resolve(expression, product); // сопоставляем по продукту
			using (new SessionScope()) // скрываем продукт
			{
				product.CatalogProduct.Hidden = true;
				product.Hidden = true;
				product.Save();
				product.CatalogProduct.Save();
			}
			resolver.ExcludeProducer(GetRow(expression)); // создаем исключение
			Save();

			using (new SessionScope(FlushAction.Never))
			{
				var exlcudes = TestExclude.Queryable.Where(e => e.Price == price).ToList();
				Assert.That(exlcudes.Count, Is.EqualTo(0));				
			}


			using (new SessionScope())
			{
				product.CatalogProduct.Hidden = false;
				product.Hidden = false;
				product.Save();
				product.CatalogProduct.Save();
			}

			Load();
			Resolve(expression, product);			

			resolver.ExcludeProducer(GetRow(expression));
			Save();
			using (new SessionScope(FlushAction.Never))
			{
				var exlcudes = TestExclude.Queryable.Where(e => e.Price == price).ToList();
				Assert.That(exlcudes.Count, Is.EqualTo(1), "не создали исключение");
				var exclude = exlcudes.Single();
				Assert.That(exclude.OriginalSynonym, Is.Not.Null);				
			}
		}

		private static IQueryable<TestProduct> Pharmacie()
		{
			return TestProduct.Queryable.Where(p => p.CatalogProduct.Pharmacie && !p.CatalogProduct.Hidden);
		}

		private void Load()
		{
			With.Connection(c => { 
				var commandHelper = new CommandHelper(new MySqlCommand(@"SELECT RowID As UERowID,
				  Name1 As UEName1, 
				  FirmCr As UEFirmCr, 
				  Code As UECode, 
				  CodeCr As UECodeCr, 
				  Unit As UEUnit, 
				  Volume As UEVolume, 
				  Quantity As UEQuantity, 
				  Note, 
				  Period As UEPeriod, 
				  Doc, 
				  PriorProductId As UEPriorProductId,  
				  p.CatalogId As UEPriorCatalogId,
				  PriorProducerId As UEPriorProducerId, 
				  ProductSynonymId As UEProductSynonymId,  
				  ProducerSynonymId As UEProducerSynonymId, 
				  Status As UEStatus,
				  Already As UEAlready, 
				  Junk As UEJunk,
				  HandMade As UEHandMade,
                  c.Pharmacie  
				  FROM farm.UnrecExp 
					left join Catalogs.Products p on p.Id = PriorProductId
                    left join Catalogs.Catalog c on p.CatalogId = c.Id
				  WHERE PriceItemId= ?LockedPriceItemId ORDER BY Name1",c));
				commandHelper.AddParameter("?LockedPriceItemId", price.Costs.First().PriceItem.Id);
				

				var set = new DataSet();
				commandHelper.Fill(set, "Data");
				data = set.Tables[0];
				if (!data.Columns.Contains("SynonymObject"))
					data.Columns.Add("SynonymObject", typeof (object));
			});
		}

		private void Save()
		{
			var updater = new Updater(price.Id, price.Id, price.Costs.First().PriceItem.Id, null, resolver);
			With.Connection(c => {
				updater.ApplyChanges(c, new FakeNotifier(), data.Rows.Cast<DataRow>().ToList());
			});
		}

		private void Resolve(TestUnrecExp expression, TestProduct product)
		{
			var row = GetRow(expression);
			resolver.ResolveProduct(row, product.Id, product.CatalogProduct.Id, true, false);
		}

		private void Resolve(TestUnrecExp expression, TestProducer producer)
		{
			var row = GetRow(expression);
			resolver.ResolveProducer(row, producer.Id);
		}

		private DataRow GetRow(TestUnrecExp expression)
		{
			return data.Rows.Cast<DataRow>().First(r => r.Field<uint>("UERowID") == expression.Id);
		}
	}
}