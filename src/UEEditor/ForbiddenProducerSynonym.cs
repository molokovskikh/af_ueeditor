using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UEEditor
{
	public class ForbiddenProducerSynonym : ProducerSynonym
	{
		public ForbiddenProducerSynonym()
		{
			State = ProducerSynonymState.Exclude;
		}
	}
}
