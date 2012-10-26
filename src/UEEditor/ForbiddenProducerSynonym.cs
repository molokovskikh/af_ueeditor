using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace UEEditor
{
	public class ForbiddenProducerSynonym : ProducerSynonym
	{
		public override bool IsApplicable(DataRow destination, DataTable assortment)
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

			return true;
		}
	}
}