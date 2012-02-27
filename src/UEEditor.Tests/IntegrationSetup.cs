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
			{
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
			Assert.That(footer, Is.EqualTo("� ���������,\r\n������������� �������� \"�������\" �.�������\r\n������ +7 499 7097350\r\n�.-��������� +7 812 3090521\r\n������� +7 473 2606000\r\n��������� +7 351 7501892\r\n���� +7 4862 632334\r\n�������� +7 4812 330364\r\n������ +7 4832 300631\r\n����� +7 4712 745447\r\n������ +7 843 2495786\r\nE-mail: pharm@analit.net\r\nhttp://www.analit.net"));
		}
	}
}