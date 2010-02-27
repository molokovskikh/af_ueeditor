using System;
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

		public static void SendMessageToService(Exception exception)
		{

			try
			{
				var messageBody = String.Format("Версия: {0}\nКомпьютер: {1}\nОператор: {2}\nОшибка:{3}\n",
					Application.ProductVersion, Environment.MachineName, Environment.UserName, exception.ToString());
				//Формируем сообщение
				var from = EmailService;
				var to = EmailService;
#if DEBUG
				to = "KvasovTest@analit.net";
#endif
				var m = new MailMessage(from, to, "Ошибка в UEEditor", messageBody);
				var sm = new SmtpClient(SmtpServerName);
				sm.Send(m);
			}
			catch (Exception ex)
			{
				var _logger = LogManager.GetLogger(typeof(Mailer)); 
				_logger.ErrorFormat("Ошибка при отправке уведомления об ошибке в UEEditor:\n{0}", ex);
				MessageBox.Show(
					@"Не удалось отправить разработчику уведомление об ошибке. Свяжитесь с разработчиком.",
					"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
