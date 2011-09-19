using System;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace UEEditor
{
	public enum ProducerSynonymState
	{
		Normal = 0,
		NotProcessed = 1,
		Unknown = 2,
		Exclude = 3,
	}

	public class ProducerSynonym
	{
		public uint Id;
		public uint ProducerId;
		public string Name;
		public bool Loaded;
		public string SupplierCode;

		public ProducerSynonymState State;

		public ProducerSynonym()
		{
			State = ProducerSynonymState.Normal;
		}

		public ProducerSynonym(DataRow item, uint producerId) : this()
		{
			ProducerId = producerId;
			Name = item["UEFirmCr"].ToString();
			if (!(item["UEProducerSynonymId"] is DBNull))
				Id = Convert.ToUInt32(item["UEProducerSynonymId"]);
			SupplierCode = item["UECode"].ToString();
		}

		public void Apply(DataRow row)
		{
			if (row["UEProducerSynonymId"] is DBNull)
				row["UEProducerSynonymId"] = Id;

			row["UEStatus"] = (int)(ProducerSynonymResolver.GetStatus(row) | FormMask.FirmForm);
			if (ProducerId == 0)
				row["UEPriorProducerId"] = DBNull.Value;
			else
				row["UEPriorProducerId"] = ProducerId;
			row["SynonymObject"] = this;
		}

		public bool IsApplicable(DataRow destination, DataTable assortment)
		{
			var status = ProducerSynonymResolver.GetStatus(destination);
			if ((status & FormMask.NameForm) != FormMask.NameForm)
				return false;

			if ((status & FormMask.FirmForm) == FormMask.FirmForm)
				return false;

			if (destination["UEPriorProductId"] is DBNull)
				return false;

			if (!Name.Equals(destination["UEFirmCr"].ToString(), StringComparison.CurrentCultureIgnoreCase))
				return false;

			var catalogId = Convert.ToUInt32(destination["UEPriorCatalogId"]);
			if (this is Exclude)
				return ((Exclude) this).CatalogId == catalogId;

			//если это фармацевтика то не нужно делать проверки по ассортименту
			if (!Convert.ToBoolean(destination["pharmacie"]))
				return true;

			if (assortment == null)
				return false;

			return assortment.Rows.Cast<DataRow>().Any(r => Convert.ToUInt32(r["CatalogId"]) == catalogId 
				&& Convert.ToUInt32(r["ProducerId"]) == ProducerId);
		}

		public static ProducerSynonym CreateSynonym(DbDataRecord record)
		{
			if (record == null)
				return null;
			if (record["CatalogId"] is DBNull && !(record["ProducerId"] is DBNull))
			{
				return new ProducerSynonym {
					Loaded = true,
					Id = Convert.ToUInt32(record["SynonymFirmCrCode"]),
					ProducerId = Convert.ToUInt32(record["ProducerId"]),
					Name = record["Synonym"].ToString()
				};
			}
			else
			{
				return new Exclude(record);
			}
		}

		public static ProducerSynonym CreateSynonym(DataRow row, uint producerId)
		{
			if (producerId != 0)
			{
				return new ProducerSynonym(row, producerId);
			}
			else
				return new Exclude
				       	{
				       		Name = row["UEFirmCr"].ToString(),
				       		CatalogId = Convert.ToUInt32(row["UEPriorCatalogId"]),
				       		State = ProducerSynonymState.Unknown,
				       		SupplierCode = row["UECode"].ToString()
						};
		}
	}
}