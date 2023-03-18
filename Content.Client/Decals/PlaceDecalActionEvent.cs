using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Content.Shared.Decals;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Client.Decals
{
	// Token: 0x0200035A RID: 858
	public sealed class PlaceDecalActionEvent : WorldTargetActionEvent
	{
		// Token: 0x04000B09 RID: 2825
		[Nullable(1)]
		[DataField("decalId", false, 1, false, false, typeof(PrototypeIdSerializer<DecalPrototype>))]
		public string DecalId = string.Empty;

		// Token: 0x04000B0A RID: 2826
		[DataField("color", false, 1, false, false, null)]
		public Color Color;

		// Token: 0x04000B0B RID: 2827
		[DataField("rotation", false, 1, false, false, null)]
		public double Rotation;

		// Token: 0x04000B0C RID: 2828
		[DataField("snap", false, 1, false, false, null)]
		public bool Snap;

		// Token: 0x04000B0D RID: 2829
		[DataField("zIndex", false, 1, false, false, null)]
		public int ZIndex;

		// Token: 0x04000B0E RID: 2830
		[DataField("cleanable", false, 1, false, false, null)]
		public bool Cleanable;
	}
}
