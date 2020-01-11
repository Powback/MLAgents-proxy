using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLProxy
{
	[Serializable]
	class DoActionCommand : CommandBase
	{
		public new string type = "doAction";
		public float[] value;

		public override string Action(ProxyAcademy academy)
		{
			return "ok"; // If this is executed we're already connected m'kay
		}
	}
}