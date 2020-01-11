using System;

namespace MLProxy
{
	[Serializable]
	public class ResponseBase
	{
		public string responseType;
		public string type;
		public int id;
	}
}