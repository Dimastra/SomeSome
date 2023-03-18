using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Shared.Construction.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Singularity.Components
{
	// Token: 0x020001A6 RID: 422
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class EmitterComponent : Component
	{
		// Token: 0x040004B4 RID: 1204
		[Nullable(2)]
		public CancellationTokenSource TimerCancel;

		// Token: 0x040004B5 RID: 1205
		[ViewVariables]
		public bool IsOn;

		// Token: 0x040004B6 RID: 1206
		[ViewVariables]
		public bool IsPowered;

		// Token: 0x040004B7 RID: 1207
		[ViewVariables]
		public int FireShotCounter;

		// Token: 0x040004B8 RID: 1208
		[DataField("boltType", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string BoltType = "EmitterBolt";

		// Token: 0x040004B9 RID: 1209
		[DataField("selectableTypes", false, 1, false, false, typeof(PrototypeIdListSerializer<EntityPrototype>))]
		public List<string> SelectableTypes = new List<string>();

		// Token: 0x040004BA RID: 1210
		[DataField("powerUseActive", false, 1, false, false, null)]
		public int PowerUseActive = 600;

		// Token: 0x040004BB RID: 1211
		[DataField("basePowerUseActive", false, 1, false, false, null)]
		[ViewVariables]
		public int BasePowerUseActive = 600;

		// Token: 0x040004BC RID: 1212
		[DataField("powerUseMultiplier", false, 1, false, false, null)]
		public float PowerUseMultiplier = 0.75f;

		// Token: 0x040004BD RID: 1213
		[DataField("machinePartPowerUse", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartPowerUse = "Capacitor";

		// Token: 0x040004BE RID: 1214
		[DataField("fireBurstSize", false, 1, false, false, null)]
		public int FireBurstSize = 3;

		// Token: 0x040004BF RID: 1215
		[DataField("fireInterval", false, 1, false, false, null)]
		public TimeSpan FireInterval = TimeSpan.FromSeconds(2.0);

		// Token: 0x040004C0 RID: 1216
		[DataField("baseFireInterval", false, 1, false, false, null)]
		[ViewVariables]
		public TimeSpan BaseFireInterval = TimeSpan.FromSeconds(2.0);

		// Token: 0x040004C1 RID: 1217
		[DataField("fireBurstDelayMin", false, 1, false, false, null)]
		public TimeSpan FireBurstDelayMin = TimeSpan.FromSeconds(4.0);

		// Token: 0x040004C2 RID: 1218
		[DataField("fireBurstDelayMax", false, 1, false, false, null)]
		public TimeSpan FireBurstDelayMax = TimeSpan.FromSeconds(10.0);

		// Token: 0x040004C3 RID: 1219
		[DataField("baseFireBurstDelayMin", false, 1, false, false, null)]
		public TimeSpan BaseFireBurstDelayMin = TimeSpan.FromSeconds(4.0);

		// Token: 0x040004C4 RID: 1220
		[DataField("baseFireBurstDelayMax", false, 1, false, false, null)]
		public TimeSpan BaseFireBurstDelayMax = TimeSpan.FromSeconds(10.0);

		// Token: 0x040004C5 RID: 1221
		[DataField("fireRateMultiplier", false, 1, false, false, null)]
		[ViewVariables]
		public float FireRateMultiplier = 0.8f;

		// Token: 0x040004C6 RID: 1222
		[DataField("machinePartFireRate", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartFireRate = "Laser";

		// Token: 0x040004C7 RID: 1223
		[Nullable(2)]
		[DataField("onState", false, 1, false, false, null)]
		public string OnState = "beam";

		// Token: 0x040004C8 RID: 1224
		[Nullable(2)]
		[DataField("underpoweredState", false, 1, false, false, null)]
		public string UnderpoweredState = "underpowered";
	}
}
