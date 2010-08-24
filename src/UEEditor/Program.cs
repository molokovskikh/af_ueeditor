using System;
using System.Diagnostics;
using log4net;
using System.Windows.Forms;
using Subway.Helpers;
using log4net.Config;

namespace UEEditor
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			XmlConfigurator.Configure();

			if (!Debugger.IsAttached)
				AppDomain.CurrentDomain.UnhandledException += (sender, args) => {
					SendMessageOnException(sender, (Exception)args.ExceptionObject);
					MessageBox.Show("В приложении возникла необработанная ошибка.\r\nИнформация об ошибке была отправлена разработчику.");
				};

			//Эти две строчки есть в StatViewer'е, возможно, из-за одной из них не работает "корректное" отображение 
			//значений столбца "Сегмент" в фильтрах компонентов DevExpress
			//Application.EnableVisualStyles();
			//Application.SetCompatibleTextRenderingDefault(false);

			InputLanguageHelper.SetToRussian();
			Application.Run(new frmUEEMain());
		}

		public static void SendMessageOnException(object sender, Exception exception)
		{
			ILog logger;
			if (sender == null)
				logger = LogManager.GetLogger(typeof(Program));
			else
				logger = LogManager.GetLogger(sender.GetType());
			logger.Error(exception);
			Mailer.SendMessageToService(exception);
		}
	}
}
