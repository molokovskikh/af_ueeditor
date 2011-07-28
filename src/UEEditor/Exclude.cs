using System;
using System.Data;

namespace UEEditor
{
	public class Exclude : ProducerSynonym
	{
		public uint CatalogId;

		public Exclude()
		{
			State = ProducerSynonymState.Exclude;
		}

		public Exclude(IDataRecord record) : this()
		{
			Loaded = true;
			Id = Convert.ToUInt32(record["SynonymFirmCrCode"]);
			Name = record["Synonym"].ToString();
			CatalogId = Convert.ToUInt32(record["CatalogId"]);
		}
	}
}