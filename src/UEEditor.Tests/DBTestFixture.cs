using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace UEEditor.Tests
{
	[TestFixture]
	public class DBTestFixture
	{
		[Test, Ignore("пока не работает")]
		public void get_synonym_last_insert_id()
		{
			using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["master"].ConnectionString))
			{
				connection.Open();
				var transaction = connection.BeginTransaction();
				try
				{					
					var dataAdapter = new MySqlDataAdapter("select * from farm.synonym where PriceCode = 5 limit 10", connection);
					var commandBuilder = new MySqlCommandBuilder(dataAdapter);
					var dataTable = new DataTable();
					var dataSet = new DataSet();
					dataAdapter.FillSchema(dataSet, SchemaType.Source);
					dataAdapter.Fill(dataSet);
					dataTable = dataSet.Tables[0];
					//commandBuilder.DataAdapter.SelectCommand.CommandText = "select * from farm.synonym";
					//commandBuilder.RefreshSchema();
					var newRow = dataTable.NewRow();
					newRow["SynonymCode"] = 0;
					newRow["Synonym"] = "тест 123";
					newRow["ProductId"] = 5;
					newRow["Junk"] = false;
					dataTable.Rows.Add(newRow);
					var updateCount = dataAdapter.Update(dataTable);
					Console.WriteLine("updateCount : {0}", updateCount);
				}
				catch
				{
					transaction.Rollback();
					throw;
				}
			}
		}

		[Test, Ignore("пока не работает")]
		public void get_synonymFirmCr_last_insert_id()
		{
			using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["master"].ConnectionString))
			{
				connection.Open();
				var transaction = connection.BeginTransaction();
				try
				{
					var dataAdapter = new MySqlDataAdapter("select * from farm.synonymFirmCr where PriceCode = 5 limit 10", connection);
					dataAdapter.InsertCommand = new MySqlCommand(@"
insert into farm.synonymFirmCr (PriceCode, CodeFirmCr, Synonym) values (?PriceCode, ?CodeFirmCr, ?Synonym);
set @LastSynonymFirmCrID = last_insert_id();
insert into logs.synonymFirmCrLogs (LogTime, OperatorName, OperatorHost, Operation, SynonymFirmCrCode, PriceCode, CodeFirmCr, Synonym, ChildPriceCode) 
  values (now(), ?OperatorName, ?OperatorHost, 0, @LastSynonymFirmCrID, ?PriceCode, ?CodeFirmCr, ?Synonym, ?ChildPriceCode)", connection);
					dataAdapter.InsertCommand.Parameters.AddWithValue("?OperatorName", Environment.UserName.ToLower());
					dataAdapter.InsertCommand.Parameters.AddWithValue("?OperatorHost", Environment.MachineName);
					dataAdapter.InsertCommand.Parameters.Add("?PriceCode", MySqlDbType.UInt64, 0, "PriceCode");
					dataAdapter.InsertCommand.Parameters.Add("?Synonym", MySqlDbType.VarString, 0, "Synonym");
					dataAdapter.InsertCommand.Parameters.Add("?CodeFirmCr", MySqlDbType.UInt64, 0, "CodeFirmCr");
					dataAdapter.InsertCommand.Parameters.Add("?ChildPriceCode", MySqlDbType.Int64, 0, "ChildPriceCode");
					//var commandBuilder = new MySqlCommandBuilder(dataAdapter);
					var dataTable = new DataTable();
					var dataSet = new DataSet();
					//dataAdapter.FillSchema(dataSet, SchemaType.Source);
					dataAdapter.Fill(dataSet);
					dataTable = dataSet.Tables[0];
					//commandBuilder.DataAdapter.SelectCommand.CommandText = "select * from farm.synonym";
					//commandBuilder.RefreshSchema();
					var newRow = dataTable.NewRow();
					//newRow["SynonymCode"] = 0;
					newRow["PriceCode"] = 5;
					newRow["Synonym"] = "тест 123 _45";
					newRow["CodeFirmCr"] = 5;
					//newRow["Junk"] = false;
					dataTable.Rows.Add(newRow);
					var updateCount = dataAdapter.Update(dataTable);
					var lastInsertId = dataAdapter.InsertCommand.LastInsertedId;
					Console.WriteLine("updateCount : {0}", updateCount);
					Console.WriteLine("lastInsertId : {0}", lastInsertId);
					transaction.Commit();
				}
				catch
				{
					transaction.Rollback();
					throw;
				}
			}
		}

		[Test, Ignore("пока не работает")]
		public void get_synonym_last_insert_id_fromMySql()
		{
			using (var connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["master"].ConnectionString))
			{
				connection.Open();
				var transaction = connection.BeginTransaction();
				try
				{
	//                MySqlHelper.ExecuteNonQuery(connection, "CREATE TABLE Test (id INT NOT NULL AUTO_INCREMENT, " +
	//"id2 INT NOT NULL, name VARCHAR(100), dt DATETIME, tm TIME, " +
	//"ts TIMESTAMP, OriginalId INT, PRIMARY KEY(id, id2))");

					MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM Test", connection);
					MySqlCommandBuilder cb = new MySqlCommandBuilder(da);
					DataTable dt = new DataTable();
					da.Fill(dt);
					da.InsertCommand = cb.GetInsertCommand();
					//da.InsertCommand.CommandText += "; select last_insert_id() as id";
					da.InsertCommand.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;

					dt.Columns[0].AutoIncrement = true;
					Assert.IsTrue(dt.Columns[0].AutoIncrement);
					dt.Columns[0].AutoIncrementSeed = -1;
					dt.Columns[0].AutoIncrementStep = -1;

					DataRow dr = dt.NewRow();
					dr["id2"] = 2;
					dr["name"] = "TestName1";
					dt.Rows.Add(dr);
					int count = da.Update(dt);

					// make sure our refresh of auto increment values worked
					Assert.AreEqual(1, count, "checking insert count");
					Assert.IsNotNull(dt.Rows[dt.Rows.Count - 1]["id"],
						"Checking auto increment column");

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
