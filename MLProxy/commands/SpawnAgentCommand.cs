using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace MLProxy
{
	[Serializable]
	class SpawnAgentCommand : CommandBase
	{
		public new string type = "spawnAgent";
		public Vector3 rotation;
		public Vector3 position;
		public IPEndPoint endPoint;
		public string Action(ProxyAcademy academy, IPEndPoint endPoint)
		{
			this.endPoint = endPoint;
			GameObject agent = academy.SpawnAgent(this.id, position, Quaternion.Euler(rotation.x, rotation.y, rotation.z), endPoint);
			return "ok"; // If this is executed we're already connected m'kay
		}
	}
}