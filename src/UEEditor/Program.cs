using System;
using System.Diagnostics;
using System.Reflection;
using log4net;
using System.Windows.Forms;
using Common.MySql;
using Subway.Helpers;
using log4net.Config;

namespace UEEditor
{
	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			XmlConfigurator.Configure();
			ConnectionHelper.DefaultConnectionStringName = "Main";

			if (!Debugger.IsAttached)
				AppDomain.CurrentDomain.UnhandledException += (sender, args) => {
					ILog logger;
					if (sender == null)
						logger = LogManager.GetLogger(typeof(Program));
					else
						logger = LogManager.GetLogger(sender.GetType());
					logger.Error(args.ExceptionObject);

					MessageBox.Show("В приложении возникла необработанная ошибка.\r\nИнформация об ошибке была отправлена разработчику.");
				};

			GlobalContext.Properties["Version"] = Assembly.GetExecutingAssembly().GetName().Version;
			InputLanguageHelper.SetToRussian();
			Application.Run(new frmUEEMain());
		}
	}
}