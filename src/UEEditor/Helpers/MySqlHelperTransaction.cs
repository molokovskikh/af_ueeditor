using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace Inforoom.UEEditor.Helpers
{
	public class MySqlHelperTransaction
	{
		public static readonly string master = "master";
		public static readonly string slave = "slave";

		public static void Transaction(Action<MySqlConnection, MySqlTransaction> action)
		{
			using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings[MySqlHelperTransaction.master].ConnectionString))
			{
				connection.Open();

				var transaction = connection.BeginTransaction();
				try
				{
					action(connection, transaction);
					transaction.Commit();
				}
				catch
				{
					transaction.Rollback();
					throw;
				}
			}
		}

	}
}
