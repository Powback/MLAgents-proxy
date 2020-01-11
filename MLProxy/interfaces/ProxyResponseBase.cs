using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLProxy
{
	[Serializable]
	public class ProxyResponseBase
	{
		string type { get; set; }
		int agentId { get; set; }
		dynamic value { get; set; }
	}
}
