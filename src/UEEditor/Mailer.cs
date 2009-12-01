using System;
using System.Net.Mail;
using System.Windows.Forms;
using log4net;

namespace UEEditor
{
	public static class Mailer
	{
		private static string SmtpServerName = "mail.adc.analit.net";

		private static string EmailService = "service@analit.net";

		public static void SendMessageToService(Exception exception)
		{
			try
			{
				var messageBody = String.Format("Компьютер: {0}\nОператор: {1}\nОшибка:{2}\n",
					Environment.MachineName, Environment.UserName, exception);
				//Формируем сообщение
				var m = new MailMessage(EmailService, EmailService, "Ошибка в UEEditor", messageBody);
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
