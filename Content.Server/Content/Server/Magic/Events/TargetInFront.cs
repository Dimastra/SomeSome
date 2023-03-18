using System;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Magic.Events
{
	// Token: 0x020003E8 RID: 1000
	public sealed class TargetInFront : MagicSpawnData
	{
		// Token: 0x04000CB4 RID: 3252
		[DataField("width", false, 1, false, false, null)]
		public int Width = 3;
	}
}
