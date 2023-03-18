using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC
{
	// Token: 0x020002C0 RID: 704
	[NetSerializable]
	[Serializable]
	public sealed class HTNMessage : EntityEventArgs
	{
		// Token: 0x040007EC RID: 2028
		public EntityUid Uid;

		// Token: 0x040007ED RID: 2029
		[Nullable(1)]
		public string Text = string.Empty;
	}
}
