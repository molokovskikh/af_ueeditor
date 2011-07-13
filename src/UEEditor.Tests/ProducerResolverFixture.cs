using System;
using System.Configuration;
using System.Data;
using System.Linq;
using Castle.ActiveRecord;
using Common.MySql;
using Common.Tools;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using Test.Support;
using Test.Support.Catalog;

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

		[SetUp]
		public void SetUp()
		{
			using(new TransactionScope())
				price = TestOldClient.CreateTestSupplierWithPrice();
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

				var productsWithAssortment = TestProduct.Queryable.Take(30).ToList().Where(p => p.CatalogProduct.Producers.Count > 0);
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
				product = TestProduct.Queryable.First();
			}
			Load();
			Resolve(expression, product);
			Save();

			var status = ProducerSynonymResolver.GetStatus(GetRow(expression));
			Assert.That(status & FormMask.FirmForm, Is.EqualTo(FormMask.FirmForm));
		}


		[Test]
		public void Create_excludes()
		{
			TestUnrecExp expression;
			TestCatalogProduct catalogProduct;
			using(new SessionScope())
			{
				var product = TestProduct.Queryable.First();
				catalogProduct = product.CatalogProduct;
				var synonym = new TestProductSynonym("test", product, price);
				synonym.Save();
				var producerSynonym = new TestProducerSynonym("test", null, price);
				producerSynonym.Save();
				expression = new TestUnrecExp(synonym, producerSynonym);
				expression.Save();
			}

			Load();
			ProducerSynonymResolver.CreateExclude(GetRow(expression));
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
		public void Fill_original_synonym_for_created_exclude()
		{
			TestUnrecExp expression;
			TestProduct product;
			using (new SessionScope())
			{
				expression = new TestUnrecExp("test", "test", price);
				expression.Save();
				product = TestProduct.Queryable.First();
			}

			Load();
			Resolve(expression, product);
			ProducerSynonymResolver.CreateExclude(GetRow(expression));
			Save();

			using (new SessionScope(FlushAction.Never))
			{
				var exlcudes = TestExclude.Queryable.Where(e => e.Price == price).ToList();
				Assert.That(exlcudes.Count, Is.EqualTo(1), "не создали исключение");
				var exclude = exlcudes.Single();
				Assert.That(exclude.OriginalSynonym, Is.Not.Null);
			}
		}

		private void Load()
		{
			With.Slave(c => { 
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
			var updater = new Updater(price.Id, price.Id, price.Costs.First().PriceItem.Id, null);
			using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["Main"].ConnectionString))
			{
				connection.Open();
				updater.ApplyChanges(connection, new FakeNotifier(), data.Rows.Cast<DataRow>().ToList());
			}
		}

		private void Resolve(TestUnrecExp expression, TestProduct product)
		{
			var row = GetRow(expression);
			ProducerSynonymResolver.UpdateStatusByProduct(row, product.Id, product.CatalogProduct.Id, true, false);
		}

		private void Resolve(TestUnrecExp expression, TestProducer producer)
		{
			var row = GetRow(expression);
			ProducerSynonymResolver.UpdateStatusByProducer(row, producer.Id);
		}

		private DataRow GetRow(TestUnrecExp expression)
		{
			return data.Rows.Cast<DataRow>().First(r => r.Field<uint>("UERowID") == expression.Id);
		}
	}
}