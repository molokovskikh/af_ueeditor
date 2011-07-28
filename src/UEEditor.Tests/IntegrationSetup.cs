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
			if (Setup.IsIntegration())
				With.DefaultConnectionStringName = "integration";
		}
	}

	[TestFixture]
	public class TestFixture
	{
		[Test]
		public void TestFooter()
		{
			string footer = frmUEEMain.GetFooter();
			Assert.That(footer, Is.EqualTo("С уважением,%0D%0AАналитическая компания \"Инфорум\" г.Воронеж%0D%0AМосква +7 499 7097350%0D%0AС.-Петербург +7 812 3090521%0D%0AВоронеж +7 473 2606000%0D%0AЧелябинск +7 351 7501892%0D%0AОрел +7 4862 632334%0D%0AСмоленск +7 4812 330364%0D%0AБрянск +7 4832 338897%0D%0AКурск +7 4712 745447%0D%0AКазань +7 843 2495786%0D%0AE-mail: pharm@analit.net%0D%0Ahttp://www.analit.net"));
		}
	}
}