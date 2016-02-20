using System;
using System.Diagnostics;
using Common.MySql;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using Test.Support;
using UEEditor.Helpers;

namespace UEEditor.Tests
{
	[SetUpFixture]
	public class IntegrationSetup
	{
		[OneTimeSetUp]
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

		[Test]
		public void Email_info_test()
		{
			With.Connection(c => {
				var command = new CommandHelper(new MySqlCommand(string.Format(@"
				delete from usersettings.defaults;
				insert into usersettings.defaults (ProcessingAboutFirmBody, ProcessingAboutFirmSubject, ProcessingAboutNamesSubject, ProcessingAboutNamesBody, senderId, formaterId, EmailFooter)
				value
				('testFirmBody','testFirmSubject','testNameSubject','testNameBody', 1, 12, {0});
", "'С уважением,\r\nАналитическая компания \"Инфорум\" г.Воронеж\r\nМосква +7 499 7097350\r\nС.-Петербург +7 812 3090521\r\nВоронеж +7 473 2606000\r\nЧелябинск +7 351 7501892\r\nОрел +7 4862 632334\r\nСмоленск +7 4812 330364\r\nБрянск +7 4832 300631\r\nКурск +7 4712 745447\r\nКазань +7 843 2495786\r\nE-mail: pharm@analit.net\r\nhttp://www.analit.net'"), c));
				command.Execute();
			});
			var mailParams = MailsText.GetMailsInfo();
			Assert.AreEqual(mailParams.ProcessingAboutFirmBody, "testFirmBody");
			Assert.AreEqual(mailParams.ProcessingAboutFirmSubject, "testFirmSubject");
			Assert.AreEqual(mailParams.ProcessingAboutNamesBody, "testNameBody");
			Assert.AreEqual(mailParams.ProcessingAboutNamesSubject, "testNameSubject");
		}

		[Test, Ignore("Написан, для ручного тестирования")]
		public void SendTest()
		{
			var mailParams = MailsText.GetMailsInfo();
			var subject = string.Format(mailParams.ProcessingAboutFirmSubject, "testFirm", "Vrn");
			var body = mailParams.ProcessingAboutFirmBody;
			body = Uri.UnescapeDataString(String.Format(body, "testFirmName"));
			body = MailHelper.ApplyFooter(body);
			var mailUrl = String.Format("mailto:{0}?cc={1}&Subject={2}&Body={3}",
				"a.zolotarev@analit.net",
				"pharm@analit.net", MailHelper.FakeEscape(subject), MailHelper.FakeEscape(body));
			Process.Start(mailUrl);
		}
	}
}