using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Singularity.Components
{
	// Token: 0x020001A4 RID: 420
	[NetSerializable]
	[Serializable]
	public enum FieldLevelVisuals : byte
	{
		// Token: 0x040004A8 RID: 1192
		NoLevel,
		// Token: 0x040004A9 RID: 1193
		On,
		// Token: 0x040004AA RID: 1194
		OneField,
		// Token: 0x040004AB RID: 1195
		MultipleFields
	}
}
