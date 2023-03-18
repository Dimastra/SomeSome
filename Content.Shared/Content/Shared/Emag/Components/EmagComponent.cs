using System;
using System.Runtime.CompilerServices;
using Content.Shared.Emag.Systems;
using Content.Shared.Tag;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Emag.Components
{
	// Token: 0x020004C8 RID: 1224
	[Access(new Type[]
	{
		typeof(EmagSystem)
	})]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class EmagComponent : Component
	{
		// Token: 0x04000DE5 RID: 3557
		[DataField("maxCharges", false, 1, false, false, null)]
		[ViewVariables]
		public int MaxCharges = 3;

		// Token: 0x04000DE6 RID: 3558
		[DataField("charges", false, 1, false, false, null)]
		[ViewVariables]
		public int Charges = 3;

		// Token: 0x04000DE7 RID: 3559
		[DataField("autoRecharge", false, 1, false, false, null)]
		[ViewVariables]
		public bool AutoRecharge = true;

		// Token: 0x04000DE8 RID: 3560
		[DataField("rechargeDuration", false, 1, false, false, null)]
		[ViewVariables]
		public TimeSpan RechargeDuration = TimeSpan.FromSeconds(90.0);

		// Token: 0x04000DE9 RID: 3561
		[DataField("nextChargeTime", false, 1, false, false, typeof(TimeOffsetSerializer))]
		[ViewVariables]
		public TimeSpan NextChargeTime = TimeSpan.MaxValue;

		// Token: 0x04000DEA RID: 3562
		[Nullable(1)]
		[DataField("emagImmuneTag", false, 1, false, false, typeof(PrototypeIdSerializer<TagPrototype>))]
		[ViewVariables]
		public string EmagImmuneTag = "EmagImmune";
	}
}
