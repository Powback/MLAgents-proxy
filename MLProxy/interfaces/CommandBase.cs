using System;

namespace MLProxy
{
	[Serializable]
	public class CommandBase : ResponseBase
	{
		public new string responseType = "command";
		public virtual string Action(ProxyAcademy academy) {
			throw new NotImplementedException();
		}
	}
}