using System;
using System.Runtime.CompilerServices;
using Content.Shared.Radio;
using Content.Shared.Salvage;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Salvage
{
	// Token: 0x02000219 RID: 537
	[NetworkedComponent]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SalvageSystem)
	})]
	public sealed class SalvageMagnetComponent : SharedSalvageMagnetComponent
	{
		// Token: 0x04000683 RID: 1667
		[ViewVariables]
		[DataField("offset", false, 1, false, false, null)]
		public Vector2 Offset = Vector2.Zero;

		// Token: 0x04000684 RID: 1668
		[ViewVariables]
		[DataField("offsetRadiusMin", false, 1, false, false, null)]
		public float OffsetRadiusMin;

		// Token: 0x04000685 RID: 1669
		[ViewVariables]
		[DataField("offsetRadiusMax", false, 1, false, false, null)]
		public float OffsetRadiusMax;

		// Token: 0x04000686 RID: 1670
		[ViewVariables]
		[DataField("attachedEntity", false, 1, false, false, null)]
		public EntityUid? AttachedEntity;

		// Token: 0x04000687 RID: 1671
		[ViewVariables]
		[DataField("magnetState", false, 1, false, false, null)]
		public MagnetState MagnetState = MagnetState.Inactive;

		// Token: 0x04000688 RID: 1672
		[ViewVariables]
		[DataField("attachingTime", false, 1, false, false, null)]
		public TimeSpan AttachingTime = TimeSpan.FromSeconds(10.0);

		// Token: 0x04000689 RID: 1673
		[ViewVariables]
		[DataField("holdTime", false, 1, false, false, null)]
		public TimeSpan HoldTime = TimeSpan.FromSeconds(10.0);

		// Token: 0x0400068A RID: 1674
		[ViewVariables]
		[DataField("detachingTime", false, 1, false, false, null)]
		public TimeSpan DetachingTime = TimeSpan.FromSeconds(10.0);

		// Token: 0x0400068B RID: 1675
		[ViewVariables]
		[DataField("cooldownTime", false, 1, false, false, null)]
		public TimeSpan CooldownTime = TimeSpan.FromSeconds(10.0);

		// Token: 0x0400068C RID: 1676
		[Nullable(1)]
		[DataField("salvageChannel", false, 1, false, false, typeof(PrototypeIdSerializer<RadioChannelPrototype>))]
		public string SalvageChannel = "Supply";

		// Token: 0x0400068D RID: 1677
		public int ChargeRemaining = 5;

		// Token: 0x0400068E RID: 1678
		public int ChargeCapacity = 5;

		// Token: 0x0400068F RID: 1679
		public int PreviousCharge = 5;
	}
}
