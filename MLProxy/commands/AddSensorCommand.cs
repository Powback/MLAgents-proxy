﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLProxy
{
	[Serializable]
	class AddSensorCommand : CommandBase
	{
		public new string type = "addSensor";
		public float[] value;

		public override string Action(ProxyAcademy academy)
		{
			return "ok"; // If this is executed we're already connected m'kay
		}
	}
}