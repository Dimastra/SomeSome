using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Trigger
{
	// Token: 0x020000AC RID: 172
	[NetSerializable]
	[Serializable]
	public enum TriggerVisualState : byte
	{
		// Token: 0x0400025F RID: 607
		Primed,
		// Token: 0x04000260 RID: 608
		Unprimed
	}
}
