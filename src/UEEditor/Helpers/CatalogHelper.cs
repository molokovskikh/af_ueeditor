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

		public static bool IsHiddenProduct(MySqlConnection connection, long productId)
		{
			return Convert.ToBoolean(
				MySql.Data.MySqlClient.MySqlHelper.ExecuteScalar(
					connection,
					String.Format(@"
select
  (products.Hidden or catalog.Hidden) as Hidden
from
  catalogs.catalog,
  catalogs.products
where
    products.Id = {0}
and catalog.Id = products.CatalogId"
					,
					productId)
					)
			);
		}

//        public static bool IsHiddenProducer(MySqlConnection connection, long producerId)
//        {
//            return Convert.ToBoolean(
//                MySql.Data.MySqlClient.MySqlHelper.ExecuteScalar(
//                    connection,
//                    String.Format(@"
//select
//  Hidden
//from
//  farm.catalogfirmcr
//where
//    CodeFirmCr = {0}"
//                    ,
//                    producerId)
//                    )
//            );
//        }

		public static bool IsSynonymExists(MySqlConnection connection, long lockedSynonymPriceCode, string synonymName)
		{
			return
				MySql.Data.MySqlClient.MySqlHelper.ExecuteScalar(
					connection,
					"select ProductId from farm.synonym where synonym = ?SynonymName and PriceCode = ?LockedSynonymPriceCode",
					new MySqlParameter("?LockedSynonymPriceCode", lockedSynonymPriceCode),
				//todo: здесь получается фигня с добавлением пробелов в конце строки
					new MySqlParameter("?SynonymName", String.Format("{0}  ", synonymName))
					)
					!= null;
		}

		public static bool IsProducerSynonymExists(MySqlConnection connection, long lockedSynonymPriceCode, string producerSynonymName)
		{
			return
				MySql.Data.MySqlClient.MySqlHelper.ExecuteScalar(
					connection,
					"select CodeFirmCr from farm.synonymFirmCr where synonym = ?ProducerSynonymName and PriceCode = ?LockedSynonymPriceCode",
					new MySqlParameter("?LockedSynonymPriceCode", lockedSynonymPriceCode),
					new MySqlParameter("?ProducerSynonymName", producerSynonymName)
					)
					!= null;
		}
	}
}
