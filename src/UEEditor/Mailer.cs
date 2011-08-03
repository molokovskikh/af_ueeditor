using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Windows.Forms;
using log4net;
using UEEditor.Properties;

namespace UEEditor
{
	public static class Mailer
	{
		private static string SmtpServerName = "mail.adc.analit.net";

		private static string EmailService = Settings.Default.EmailService;

		public static void SendMessageToService(Exception exception, string msg = null, string _to = null)
		{
			try
			{
				var messageBody = String.Format("Версия: {0}\nКомпьютер: {1}\nОператор: {2}\nОшибка:{3}\n",
					Application.ProductVersion, Environment.MachineName, Environment.UserName, exception.ToString());

                if (!String.IsNullOrEmpty(msg))
                    messageBody = String.Format("Версия: {0}\nКомпьютер: {1}\nОператор: {2}\nОшибка:{3}\nОтладка:{4}\n",
                      Application.ProductVersion, Environment.MachineName, Environment.UserName, exception.ToString(), msg);
				//Формируем сообщение
				var from = EmailService;
                var to = EmailService;
                if (!String.IsNullOrEmpty(_to))
                    to = _to;
#if DEBUG
				to = "KvasovTest@analit.net";
#endif
				var m = new MailMessage(from, to, "Ошибка в UEEditor", messageBody);
				var sm = new SmtpClient(SmtpServerName);
				sm.Send(m);
			}
			catch (Exception ex)
			{
				var logger = LogManager.GetLogger(typeof(Mailer)); 
				logger.ErrorFormat("Ошибка при отправке уведомления об ошибке в UEEditor:\n{0}", ex);
				MessageBox.Show(
					@"Не удалось отправить разработчику уведомление об ошибке. Свяжитесь с разработчиком.",
					"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public static void SendDebugLog(DataTable synonyms, Exception exception, DataRow lastSynonym)
		{
			try
			{
				var messageBody = String.Format("Версия: {0}\nКомпьютер: {1}\nОператор: {2}\nОшибка:{3}\n",
					Application.ProductVersion, Environment.MachineName, Environment.UserName, exception);
				//Формируем сообщение
				var from = EmailService;
				var to = EmailService;
#if DEBUG
				to = "KvasovTest@analit.net";
#endif
				using(var message = new MailMessage(from, to, "Ошибка в UEEditor", messageBody))
				using(var memory = new MemoryStream())
				using(var writer = new StreamWriter(memory))
				{
					writer.Write("последний обработанный синонима = ");
					if (lastSynonym != null)
						Writesynonym(synonyms, writer, lastSynonym);
					else
						writer.Write("null");
					writer.WriteLine();

					foreach (var row in synonyms.Rows.Cast<DataRow>())
					{
						Writesynonym(synonyms, writer, row);
						writer.WriteLine();
					}
					writer.WriteLine();
					memory.Position = 0;
					message.Attachments.Add(new Attachment(memory, "Debug.txt"));
					var sm = new SmtpClient(SmtpServerName);
					sm.Send(message);
				}
			}
			catch (Exception ex)
			{
				var logger = LogManager.GetLogger(typeof(Mailer)); 
				logger.ErrorFormat("Ошибка при отправке уведомления об ошибке в UEEditor:\n{0}", ex);
				MessageBox.Show(
					@"Не удалось отправить разработчику уведомление об ошибке. Свяжитесь с разработчиком.",
					"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private static void Writesynonym(DataTable synonyms, StreamWriter writer, DataRow row)
		{
			writer.Write("row state = {0}", row.RowState);
			foreach (var column in synonyms.Columns.Cast<DataColumn>())
				writer.Write("{0} = '{1}' ", column.ColumnName, row[column.ColumnName]);
		}
	}
}
