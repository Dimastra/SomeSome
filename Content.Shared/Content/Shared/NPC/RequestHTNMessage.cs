using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC
{
	// Token: 0x020002CE RID: 718
	[NetSerializable]
	[Serializable]
	public sealed class RequestHTNMessage : EntityEventArgs
	{
		// Token: 0x0400081A RID: 2074
		public bool Enabled;
	}
}
