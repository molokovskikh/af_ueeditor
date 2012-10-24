using Common.MySql;
using NUnit.Framework;
using Test.Support;

namespace UEEditor.Tests
{
	[SetUpFixture]
	public class IntegrationSetup
	{
		[SetUp]
		public void SetUp()
		{
			Setup.Initialize();
			if (Setup.IsIntegration()) {
				With.DefaultConnectionStringName = "integration";
			}
		}
	}

	[TestFixture]
	public class TestFixture
	{
		[Test]
		public void TestFooter()
		{
			var footer = MailHelper.GetFooter();
			Assert.That(footer, Is.EqualTo("С уважением,\r\nАналитическая компания \"Инфорум\" г.Воронеж\r\nМосква +7 499 7097350\r\nС.-Петербург +7 812 3090521\r\nВоронеж +7 473 2606000\r\nЧелябинск +7 351 7501892\r\nОрел +7 4862 632334\r\nСмоленск +7 4812 330364\r\nБрянск +7 4832 300631\r\nКурск +7 4712 745447\r\nКазань +7 843 2495786\r\nE-mail: pharm@analit.net\r\nhttp://www.analit.net"));
		}
	}
}