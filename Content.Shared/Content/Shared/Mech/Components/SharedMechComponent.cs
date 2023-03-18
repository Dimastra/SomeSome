using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.FixedPoint;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Mech.Components
{
	// Token: 0x0200032A RID: 810
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedMechComponent : Component
	{
		// Token: 0x04000927 RID: 2343
		[ViewVariables]
		public FixedPoint2 Integrity;

		// Token: 0x04000928 RID: 2344
		[DataField("maxIntegrity", false, 1, false, false, null)]
		public FixedPoint2 MaxIntegrity = 250;

		// Token: 0x04000929 RID: 2345
		[ViewVariables]
		public FixedPoint2 Energy = 0;

		// Token: 0x0400092A RID: 2346
		[DataField("maxEnergy", false, 1, false, false, null)]
		public FixedPoint2 MaxEnergy = 0;

		// Token: 0x0400092B RID: 2347
		[ViewVariables]
		public ContainerSlot BatterySlot;

		// Token: 0x0400092C RID: 2348
		[ViewVariables]
		public readonly string BatterySlotId = "mech-battery-slot";

		// Token: 0x0400092D RID: 2349
		[DataField("mechToPilotDamageMultiplier", false, 1, false, false, null)]
		public float MechToPilotDamageMultiplier;

		// Token: 0x0400092E RID: 2350
		[ViewVariables]
		public bool Broken;

		// Token: 0x0400092F RID: 2351
		[ViewVariables]
		public ContainerSlot PilotSlot;

		// Token: 0x04000930 RID: 2352
		[ViewVariables]
		public readonly string PilotSlotId = "mech-pilot-slot";

		// Token: 0x04000931 RID: 2353
		[ViewVariables]
		public EntityUid? CurrentSelectedEquipment;

		// Token: 0x04000932 RID: 2354
		[DataField("maxEquipmentAmount", false, 1, false, false, null)]
		public int MaxEquipmentAmount = 3;

		// Token: 0x04000933 RID: 2355
		[Nullable(2)]
		[DataField("equipmentWhitelist", false, 1, false, false, null)]
		public EntityWhitelist EquipmentWhitelist;

		// Token: 0x04000934 RID: 2356
		[ViewVariables]
		public Container EquipmentContainer;

		// Token: 0x04000935 RID: 2357
		[ViewVariables]
		public readonly string EquipmentContainerId = "mech-equipment-container";

		// Token: 0x04000936 RID: 2358
		[DataField("mechCycleAction", false, 1, false, false, typeof(PrototypeIdSerializer<InstantActionPrototype>))]
		public string MechCycleAction = "MechCycleEquipment";

		// Token: 0x04000937 RID: 2359
		[DataField("mechUiAction", false, 1, false, false, typeof(PrototypeIdSerializer<InstantActionPrototype>))]
		public string MechUiAction = "MechOpenUI";

		// Token: 0x04000938 RID: 2360
		[DataField("mechEjectAction", false, 1, false, false, typeof(PrototypeIdSerializer<InstantActionPrototype>))]
		public string MechEjectAction = "MechEject";

		// Token: 0x04000939 RID: 2361
		[Nullable(2)]
		[DataField("baseState", false, 1, false, false, null)]
		public string BaseState;

		// Token: 0x0400093A RID: 2362
		[Nullable(2)]
		[DataField("openState", false, 1, false, false, null)]
		public string OpenState;

		// Token: 0x0400093B RID: 2363
		[Nullable(2)]
		[DataField("brokenState", false, 1, false, false, null)]
		public string BrokenState;
	}
}
