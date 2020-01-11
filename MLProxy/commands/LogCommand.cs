using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLProxy
{
	[Serializable]
	class LogCommand : CommandBase
	{
		public new string type = "log";
		public string value;

		public override string Action(ProxyAcademy academy)
		{
			return value; // If this is executed we're already connected m'kay
		}
	}
}
