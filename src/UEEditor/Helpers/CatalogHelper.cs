using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace UEEditor.Helpers
{
	public class CatalogHelper
	{
		static string ConnectionString()
		{
			return ConfigurationManager.ConnectionStrings[MySqlHelperTransaction.slave].ConnectionString;
		}

		public static bool IsAssortmentExists(long productId, long producerId)
		{
			object assortmentExists = MySqlHelper.ExecuteScalar(ConnectionString(),
				"select ProductId from catalogs.assortment where ProductId = ?ProductId and ProducerId = ?ProducerId",
				new MySqlParameter("?ProductId", productId),
				new MySqlParameter("?ProducerId", producerId));
			return (assortmentExists != null);
		}
	}
}
