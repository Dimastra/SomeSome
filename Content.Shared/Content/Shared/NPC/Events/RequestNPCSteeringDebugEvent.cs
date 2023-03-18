using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC.Events
{
	// Token: 0x020002D4 RID: 724
	[NetSerializable]
	[Serializable]
	public sealed class RequestNPCSteeringDebugEvent : EntityEventArgs
	{
		// Token: 0x04000827 RID: 2087
		public bool Enabled;
	}
}
