using System;
using System.Net.Mail;
using System.Windows.Forms;
using log4net;

namespace UEEditor
{
	public static class Mailer
	{
		private static string SmtpServerName = "mail.adc.analit.net";

		public static void SendLetterWithException(Exception exception)
		{
			try
			{
				string messageBody = String.Format("Оператор: {0}\nОшибка:{1}\n",
					Environment.UserName, exception);
				//Формируем сообщение
				MailMessage m = new MailMessage(
					"service@analit.net", "d.dorofeev@analit.net", "Ошибка в UEditor", messageBody);
				SmtpClient sm = new SmtpClient(SmtpServerName);
				sm.Send(m);
			}
			catch (Exception ex)
			{
				ILog _logger = LogManager.GetLogger(typeof(Mailer)); 
				_logger.ErrorFormat("Ошибка при отправке уведомления об ошибке в UEEditor:\n{0}", ex);
				MessageBox.Show(
					@"Не удалось отправить уведомление об изменениях. Сообщение было отправлено разработчику.",
					"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
