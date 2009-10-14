using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using System.Windows.Forms;
using System.Threading;

namespace Inforoom.UEEditor
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.ThreadException += new ThreadExceptionEventHandler(OnThreadException);

			//Эти две строчки есть в StatViewer'е, возможно, из-за одной из них не работает "корректное" отображение 
			//значений столбца "Сегмент" в фильтрах компонентов DevExpress
			//Application.EnableVisualStyles();
			//Application.SetCompatibleTextRenderingDefault(false);

			Application.Run(new frmUEEMain());
		}

		public static void SendMessageOnException(object sender, Exception exception)
		{
			ILog _logger;
			if (sender == null)
				_logger = LogManager.GetLogger(typeof(Program));
			else
				_logger = LogManager.GetLogger(sender.GetType());

			_logger.Error(exception);
		}

		// Handles the exception event.
		public static void OnThreadException(object sender, System.Threading.ThreadExceptionEventArgs t)
		{
			SendMessageOnException(sender, t.Exception);
			MessageBox.Show("В приложении возникла необработанная ошибка.\r\nИнформация об ошибке была отправлена разработчику.");
		}

	}
}
