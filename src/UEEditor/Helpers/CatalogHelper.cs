using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MySql.Data.MySqlClient;
using Common.MySql;

namespace UEEditor.Helpers
{
	public class CatalogHelper
	{
		public static bool IsAssortmentExists(long productId, long producerId)
		{
			object assortmentExists = null;
			With.Slave((slaveConnection) =>
			{
				assortmentExists = MySql.Data.MySqlClient.MySqlHelper.ExecuteScalar(slaveConnection, @"
select 
  assortment.CatalogId 
from 
  catalogs.products, 
  catalogs.assortment 
where 
    (products.Id = ?ProductId)
and (assortment.CatalogId = products.CatalogId) 
and (assortment.ProducerId = ?ProducerId)",
				new MySqlParameter("?ProductId", productId),
				new MySqlParameter("?ProducerId", producerId));
			});
			return (assortmentExists != null);
		}
	}
}
