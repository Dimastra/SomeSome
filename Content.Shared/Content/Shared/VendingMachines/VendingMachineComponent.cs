using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.VendingMachines
{
	// Token: 0x02000094 RID: 148
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class VendingMachineComponent : Component
	{
		// Token: 0x040001F9 RID: 505
		[Nullable(1)]
		[DataField("pack", false, 1, false, false, typeof(PrototypeIdSerializer<VendingMachineInventoryPrototype>))]
		public string PackPrototypeId = string.Empty;

		// Token: 0x040001FA RID: 506
		[DataField("denyDelay", false, 1, false, false, null)]
		public float DenyDelay = 2f;

		// Token: 0x040001FB RID: 507
		[DataField("ejectDelay", false, 1, false, false, null)]
		public float EjectDelay = 1.2f;

		// Token: 0x040001FC RID: 508
		[Nullable(1)]
		[ViewVariables]
		public Dictionary<string, VendingMachineInventoryEntry> Inventory = new Dictionary<string, VendingMachineInventoryEntry>();

		// Token: 0x040001FD RID: 509
		[Nullable(1)]
		[ViewVariables]
		public Dictionary<string, VendingMachineInventoryEntry> EmaggedInventory = new Dictionary<string, VendingMachineInventoryEntry>();

		// Token: 0x040001FE RID: 510
		[Nullable(1)]
		[ViewVariables]
		public Dictionary<string, VendingMachineInventoryEntry> ContrabandInventory = new Dictionary<string, VendingMachineInventoryEntry>();

		// Token: 0x040001FF RID: 511
		public bool Contraband;

		// Token: 0x04000200 RID: 512
		public bool Ejecting;

		// Token: 0x04000201 RID: 513
		public bool Denying;

		// Token: 0x04000202 RID: 514
		public bool DispenseOnHitCoolingDown;

		// Token: 0x04000203 RID: 515
		public string NextItemToEject;

		// Token: 0x04000204 RID: 516
		public bool Broken;

		// Token: 0x04000205 RID: 517
		[DataField("speedLimiter", false, 1, false, false, null)]
		public bool CanShoot;

		// Token: 0x04000206 RID: 518
		public bool ThrowNextItem;

		// Token: 0x04000207 RID: 519
		[DataField("dispenseOnHitChance", false, 1, false, false, null)]
		public float? DispenseOnHitChance;

		// Token: 0x04000208 RID: 520
		[DataField("dispenseOnHitThreshold", false, 1, false, false, null)]
		public float? DispenseOnHitThreshold;

		// Token: 0x04000209 RID: 521
		[DataField("dispenseOnHitCooldown", false, 1, false, false, null)]
		public float? DispenseOnHitCooldown = new float?(1f);

		// Token: 0x0400020A RID: 522
		[Nullable(1)]
		[DataField("soundVend", false, 1, false, false, null)]
		public SoundSpecifier SoundVend = new SoundPathSpecifier("/Audio/Machines/machine_vend.ogg", null);

		// Token: 0x0400020B RID: 523
		[Nullable(1)]
		[DataField("soundDeny", false, 1, false, false, null)]
		public SoundSpecifier SoundDeny = new SoundPathSpecifier("/Audio/Machines/custom_deny.ogg", null);

		// Token: 0x0400020C RID: 524
		[DataField("action", false, 1, false, false, typeof(PrototypeIdSerializer<InstantActionPrototype>))]
		public string Action = "VendingThrow";

		// Token: 0x0400020D RID: 525
		public float NonLimitedEjectForce = 7.5f;

		// Token: 0x0400020E RID: 526
		public float NonLimitedEjectRange = 5f;

		// Token: 0x0400020F RID: 527
		public float EjectAccumulator;

		// Token: 0x04000210 RID: 528
		public float DenyAccumulator;

		// Token: 0x04000211 RID: 529
		public float DispenseOnHitAccumulator;

		// Token: 0x04000212 RID: 530
		[DataField("offState", false, 1, false, false, null)]
		public string OffState;

		// Token: 0x04000213 RID: 531
		[DataField("screenState", false, 1, false, false, null)]
		public string ScreenState;

		// Token: 0x04000214 RID: 532
		[DataField("normalState", false, 1, false, false, null)]
		public string NormalState;

		// Token: 0x04000215 RID: 533
		[DataField("ejectState", false, 1, false, false, null)]
		public string EjectState;

		// Token: 0x04000216 RID: 534
		[DataField("denyState", false, 1, false, false, null)]
		public string DenyState;

		// Token: 0x04000217 RID: 535
		[DataField("brokenState", false, 1, false, false, null)]
		public string BrokenState;

		// Token: 0x04000218 RID: 536
		[DataField("loopDeny", false, 1, false, false, null)]
		public bool LoopDenyAnimation = true;
	}
}
