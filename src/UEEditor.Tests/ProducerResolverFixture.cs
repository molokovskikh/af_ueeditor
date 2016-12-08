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
	public class ProducerResolverFixture : IntegrationFixture
	{
		private TestPrice price;
		private DataTable data;
		private ProducerSynonymResolver resolver;

		[SetUp]
		public void SetUp()
		{
			var supplier = TestSupplier.CreateNaked(session);
			supplier.Prices[0].Costs[0].PriceItem.Format.PriceFormat = PriceFormatType.NativeDbf;
			price = supplier.Prices[0];

			resolver = new ProducerSynonymResolver(price.Id);
		}

		[Test]
		public void After_unresolve_product_clear_exclude()
		{
			var exp = new TestUnrecExp("ПОЛЫНИ ГОРЬКОЙ ТРАВА сырье 75 г N1", "Кентавр ХФК", price);
			exp.Save();
			//исключения работают только для фармацевтики
			var products = Pharmacie().Take(2).ToList();
			var product = products[0];
			var product2 = products[1];
			Load();
			var row = GetRow(exp);
			Resolve(exp, product);
			resolver.ExcludeProducer(row);
			resolver.UnresolveProduct(row);

			Resolve(exp, product2);
			Save();

			var excludes = TestExclude.Queryable.Where(e => e.Price == price);
			Assert.That(excludes.Count(), Is.EqualTo(0), excludes.Implode());
		}

		[Test]
		public void Create_different_synonym_on_same_base_synonym()
		{
			var producerSynonym = new TestProducerSynonym("Test", null, price);
			producerSynonym.Save();
			new TestAutomaticSynonym { ProducerSynonymId = producerSynonym.Id }.Create();

			var productsWithAssortment = Pharmacie().Take(30).ToList().Where(p => p.CatalogProduct.Producers.Count > 0);
			var first = productsWithAssortment.First();
			var products = productsWithAssortment.Where(p => p.CatalogProduct.Producers.All(pr => pr.Id != first.CatalogProduct.Producers[0].Id)).Take(1).ToList();
			products.Add(first);

			var synonym1 = new TestProductSynonym(products[0].CatalogProduct.Name, products[0], price);
			synonym1.Save();
			var synonym2 = new TestProductSynonym(products[1].CatalogProduct.Name, products[1], price);
			synonym2.Save();
			var exp1 = new TestUnrecExp(synonym1, producerSynonym);
			exp1.Save();
			var exp2 = new TestUnrecExp(synonym2, producerSynonym);
			exp2.Save();
			var producer1 = synonym1.Product.CatalogProduct.Producers.First();
			var producer2 = synonym2.Product.CatalogProduct.Producers.First();

			Load();
			Resolve(exp1, producer1);
			Resolve(exp2, producer2);
			Save();

			session.Clear();
			var synonyms = session.Query<TestProducerSynonym>().Where(s => s.Price == price).ToList();
			Assert.That(synonyms.Count, Is.EqualTo(2), "должны были создать два разных синонима, создали только ({0})",
				synonyms.Implode(s => s.Name));
			var producerSynonym1 = synonyms.FirstOrDefault(s => s.Producer.Id == producer1.Id);
			var producerSynonym2 = synonyms.FirstOrDefault(s => s.Producer.Id == producer2.Id);
			Assert.That(producerSynonym1.Name, Is.EqualTo(producerSynonym.Name));
			Assert.That(producerSynonym1.Producer.Id, Is.EqualTo(producer1.Id));
			Assert.That(producerSynonym2.Name, Is.EqualTo(producerSynonym.Name));
			Assert.That(producerSynonym2.Producer.Id, Is.EqualTo(producer2.Id));
			var logs = session.CreateSQLQuery("select OperatorName, OperatorHost" +
				" from Logs.SynonymFirmCrLogs where SynonymFirmCrCode = :id and Operation = 1")
				.SetParameter("id", producerSynonym2.Id)
				.List<object[]>();
			Assert.AreEqual(Environment.UserName.ToLower(), logs[0][0].ToString().ToLower());
			Assert.AreEqual(Environment.MachineName, logs[0][1]);
		}

		[Test]
		public void Do_not_create_empty_producer_synonyms()
		{
			var expression = new TestUnrecExp("test", "", price);
			expression.Save();
			var product = session.Query<TestProduct>().First(p => !p.Hidden);
			Load();
			Resolve(expression, product);
			Save();

			var status = ProducerSynonymResolver.GetStatus(GetRow(expression));
			Assert.That(status & FormMask.FirmForm, Is.EqualTo(FormMask.FirmForm));
		}

		[Test]
		public void CreateForbiddenProducerName()
		{
			var rnd = new Random();
			var producerName = "testTest" + rnd.Next();
			var product = new TestProduct("Тестовый Продукт");
			var catalogProduct = product.CatalogProduct;
			catalogProduct.Pharmacie = true;
			product.Save();
			var synonym = new TestProductSynonym("test", product, price);
			synonym.Save();
			var producerSynonym = new TestProducerSynonym(producerName, null, price);
			producerSynonym.Save();
			var expression = new TestUnrecExp(synonym, producerSynonym);
			expression.Save();
			var notFormExpression = new TestUnrecExp("newTest", producerName, price);
			notFormExpression.Save();
			Load();
			resolver.ForbidProducer(GetRow(expression));
			Save();

			var query = session.CreateSQLQuery(String.Format("SELECT count(*) FROM farm.ForbiddenProducers F where F.Name='{0}'", producerName));
			var count = query.UniqueResult();
			Assert.That(count, Is.GreaterThan(0));
			var exclude = session.Query<TestExclude>().Where(e => e.Price == price);
			Assert.That(exclude.Count(), Is.EqualTo(0));
			var unrec = session.Query<TestUnrecExp>().Where(e => e.FirmCr.ToLower() == producerName && e.Status == 1);
			Assert.That(unrec.Count(), Is.EqualTo(0));
			unrec = session.Query<TestUnrecExp>().Where(e => e.FirmCr.ToLower() == producerName);
			Assert.That(unrec.Count(), Is.GreaterThan(0));
		}

		[Test]
		public void Create_excludes()
		{
			var product = Pharmacie().First();
			var catalogProduct = product.CatalogProduct;
			var synonym = new TestProductSynonym("test", product, price);
			synonym.Save();
			var producerSynonym = new TestProducerSynonym("test", null, price);
			producerSynonym.Save();
			var expression = new TestUnrecExp(synonym, producerSynonym);
			expression.Save();

			Load();
			resolver.ExcludeProducer(GetRow(expression));
			Save();

			var exlcudes = TestExclude.Queryable.Where(e => e.Price == price).ToList();
			Assert.That(exlcudes.Count, Is.EqualTo(1), "не создали исключение");
			var exclude = exlcudes.Single();
			Assert.That(exclude.ProducerSynonym, Is.EqualTo(expression.FirmCr));
			Assert.That(exclude.CatalogProduct.Id, Is.EqualTo(catalogProduct.Id));
			Assert.That(exclude.OriginalSynonym.Id, Is.EqualTo(expression.ProductSynonymId));
		}

		[Test]
		public void Fill_original_synonym_for_second_created_exclude()
		{
			var expression = new TestUnrecExp("Полыни горькой трава" + new Random().Next(), "Тестовый производитель" + new Random().Next(), price);
			expression.Save();
			var product = Pharmacie().First();

			Load();
			Resolve(expression, product);
			resolver.ExcludeProducer(GetRow(expression));
			Save();

			var product2 = Pharmacie().First(p => p.Id != product.Id);

			resolver = new ProducerSynonymResolver(price.Id);
			Load();
			Resolve(expression, product2);
			resolver.ExcludeProducer(GetRow(expression));
			Save();

			var exlcudes = TestExclude.Queryable.Where(e => e.Price == price).ToList();
			Assert.That(exlcudes.Count, Is.EqualTo(2), "не создали исключение");
			var exclude = exlcudes[0];
			Assert.That(exclude.OriginalSynonym, Is.Not.Null);
			exclude = exlcudes[1];
			Assert.That(exclude.OriginalSynonym, Is.Not.Null);
		}

		[Test]
		public void Fill_original_synonym_for_created_exclude()
		{
			var expression = new TestUnrecExp("Полыни горькой трава 2", "Тестовый производитель", price);
			expression.Save();
			var product = Pharmacie().First();

			Load();
			Resolve(expression, product);
			resolver.ExcludeProducer(GetRow(expression));
			Save();

			var exlcudes = TestExclude.Queryable.Where(e => e.Price == price).ToList();
			Assert.That(exlcudes.Count, Is.EqualTo(1), "не создали исключение");
			var exclude = exlcudes.Single();
			Assert.That(exclude.OriginalSynonym, Is.Not.Null);
		}

		[Test]
		public void TestFillSupplierCode()
		{
			var expression = new TestUnrecExp("Тестовый продукт", "Тестовый производитель", price);
			expression.Code = new Random().Next().ToString();
			expression.Save();
			var product = Pharmacie().First();
			var producer = TestProducer.Queryable.FirstOrDefault();
			TestAssortment.CheckAndCreate(session, product, producer);
			Load();
			Resolve(expression, product);
			Resolve(expression, producer);
			Save();
			session.Clear();
			var synonym = session.Query<TestProductSynonym>().FirstOrDefault(s => s.Price == price);
			var synonymFirmCr = session.Query<TestProducerSynonym>().FirstOrDefault(s => s.Price == price);
			Assert.That(synonym, Is.Not.Null);
			Assert.That(synonymFirmCr, Is.Not.Null);
		}

		[Test]
		public void TestFillSupplierCodeIfProducerExclude()
		{
			var expression = new TestUnrecExp("Тестовый продукт", "Тестовый производитель", price);
			expression.Code = new Random().Next().ToString();
			expression.Save();
			var product = Pharmacie().First();
			Load();
			Resolve(expression, product);
			resolver.ExcludeProducer(GetRow(expression));
			Save();
			var synonym = session.Query<TestProductSynonym>().FirstOrDefault(s => s.Price == price);
			var synonymFirmCr = session.Query<TestProducerSynonym>().FirstOrDefault(s => s.Price == price);
			Assert.That(synonym, Is.Not.Null);
			Assert.That(synonymFirmCr, Is.Not.Null);
			var exlcudes = TestExclude.Queryable.Where(e => e.Price == price).ToList();
			Assert.That(exlcudes.Count, Is.EqualTo(1), "не создали исключение");
		}

		[Test]
		public void Unresolve_product_if_product_was_hidden_before_save()
		{
			var expression = new TestUnrecExp("test", "test", price);
			expression.Save();
			var product = Pharmacie().First();

			Load();
			Resolve(expression, product); // сопоставляем по продукту
			// скрываем продукт
			product.CatalogProduct.Hidden = true;
			product.Hidden = true;
			product.Save();
			product.CatalogProduct.Save();
			resolver.ExcludeProducer(GetRow(expression)); // создаем исключение
			Save();

			var exlcudes = TestExclude.Queryable.Where(e => e.Price == price).ToList();
			Assert.That(exlcudes.Count, Is.EqualTo(0));

			product.CatalogProduct.Hidden = false;
			product.Hidden = false;
			product.Save();
			product.CatalogProduct.Save();

			Load();
			Resolve(expression, product);

			resolver.ExcludeProducer(GetRow(expression));
			Save();

			exlcudes = TestExclude.Queryable.Where(e => e.Price == price).ToList();
			Assert.That(exlcudes.Count, Is.EqualTo(1), "не создали исключение");
			var exclude = exlcudes.Single();
			Assert.That(exclude.OriginalSynonym, Is.Not.Null);
		}

		[Test]
		public void Resolve_monobrend_producer_synonym()
		{
			var exp = new TestUnrecExp("test", "test", price);
			session.Save(exp);
			var product = new TestProduct("Тестовый товар монобренд") {
				CatalogProduct = {
					Monobrend = true
				}
			};
			session.Save(product);
			var producer = new TestProducer("Тестовый производитель");
			session.Save(producer);
			session.Save(new TestAssortment(product, producer, true));

			Load();
			Resolve(exp, product);
			Save();

			var synonyms = session.Query<TestProducerSynonym>().Where(s => s.Price == price).ToArray();
			Assert.AreEqual(1, synonyms.Length, synonyms.Implode());
			Assert.AreEqual(synonyms[0].Producer.Id, producer.Id);
			Assert.AreEqual(synonyms[0].Name, exp.FirmCr);
		}

		[Test]
		public void Trim_values()
		{
			var exp = new TestUnrecExp("Цефепим пор. д/приг. р-ра д/в/в и в/м введ. фл. 1г пач.карт. ", "Кентавр ХФК", price);
			session.Save(exp);
			session.Save(new TestUnrecExp("Цефепим пор. д/приг. р-ра д/в/в и в/м введ. фл. 1г пач.карт.", "Кентавр ХФК", price));

			var product = Pharmacie().First();

			Load();
			Resolve(exp, product);
			Save();
		}

		private IQueryable<TestProduct> Pharmacie()
		{
			return session.Query<TestProduct>().Where(p => p.CatalogProduct.Pharmacie
				&& !p.CatalogProduct.Hidden && !p.CatalogProduct.Monobrend);
		}

		private void Load()
		{
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
				WHERE PriceItemId= ?LockedPriceItemId ORDER BY Name1", (MySqlConnection)session.Connection));
			commandHelper.AddParameter("?LockedPriceItemId", price.Costs.First().PriceItem.Id);

			var set = new DataSet();
			commandHelper.Fill(set, "Data");
			data = set.Tables[0];
			if (!data.Columns.Contains("SynonymObject"))
				data.Columns.Add("SynonymObject", typeof(object));
		}

		private void Save()
		{
			if (session.Transaction.IsActive)
				session.Transaction.Commit();
			var updater = new Updater(price.Id, price.Id, price.Costs.First().PriceItem.Id, null, resolver);
			updater.ApplyChanges((MySqlConnection)session.Connection, new FakeNotifier(), data.Rows.Cast<DataRow>().ToList());
		}

		private void Resolve(TestUnrecExp expression, TestProduct product)
		{
			if (session.Transaction.IsActive)
				session.Transaction.Commit();
			var row = GetRow(expression);
			var data = new DataSet();
			var table = data.Tables.Add("products");
			frmUEEMain.ProductsFill(product.CatalogProduct.Id, data, "products");
			Assert.AreEqual(1, table.Rows.Count);
			resolver.ResolveProduct(row, table.Rows[0], false);
		}

		private void Resolve(TestUnrecExp expression, TestProducer producer)
		{
			if (session.Transaction.IsActive)
				session.Transaction.Commit();
			var row = GetRow(expression);
			resolver.ResolveProducer(row, producer.Id);
		}

		private DataRow GetRow(TestUnrecExp expression)
		{
			return data.Rows.Cast<DataRow>().First(r => r.Field<uint>("UERowID") == expression.Id);
		}
	}
}