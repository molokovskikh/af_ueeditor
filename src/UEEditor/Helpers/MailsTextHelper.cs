using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Common.MySql;
using MySql.Data.MySqlClient;

namespace UEEditor.Helpers
{
	public class MailsText
	{
		public MailsText(string firmBody, string firmSubject, string namesBody, string namesSubject)
		{
			ProcessingAboutFirmBody = firmBody;
			ProcessingAboutFirmSubject = firmSubject;
			ProcessingAboutNamesBody = namesBody;
			ProcessingAboutNamesSubject = namesSubject;
		}

		public string ProcessingAboutFirmBody { get; set; }
		public string ProcessingAboutFirmSubject { get; set; }
		public string ProcessingAboutNamesBody { get; set; }
		public string ProcessingAboutNamesSubject { get; set; }

		public static MailsText GetMailsInfo()
		{
			var ds = new DataSet();
			With.Connection(c => {
				var commandHelper = new CommandHelper(new MySqlCommand(@"
SELECT ProcessingAboutFirmBody, ProcessingAboutFirmSubject, ProcessingAboutNamesSubject, ProcessingAboutNamesBody FROM usersettings.defaults;
", c));
				commandHelper.Fill(ds, "defaults");
			});
			var data = ds.Tables["defaults"].Rows[0];
			return new MailsText(data["ProcessingAboutFirmBody"].ToString(),
				data["ProcessingAboutFirmSubject"].ToString(),
				data["ProcessingAboutNamesBody"].ToString(),
				data["ProcessingAboutNamesSubject"].ToString());
		}
	}
}
