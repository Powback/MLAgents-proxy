﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLProxy
{
	[Serializable]
	class GetObservationsCommand : CommandBase
	{
		public new string type = "getObservations";
		public float[] value;

		public override string Action(ProxyAcademy academy)
		{
			return "ok"; // If this is executed we're already connected m'kay
		}
	}
}