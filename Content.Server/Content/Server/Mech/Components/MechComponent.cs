using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Content.Shared.Mech.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Server.Mech.Components
{
	// Token: 0x020003CA RID: 970
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	[ComponentReference(typeof(SharedMechComponent))]
	public sealed class MechComponent : SharedMechComponent
	{
		// Token: 0x04000C5F RID: 3167
		[DataField("entryDelay", false, 1, false, false, null)]
		public float EntryDelay = 3f;

		// Token: 0x04000C60 RID: 3168
		[DataField("exitDelay", false, 1, false, false, null)]
		public float ExitDelay = 3f;

		// Token: 0x04000C61 RID: 3169
		[DataField("batteryRemovalDelay", false, 1, false, false, null)]
		public float BatteryRemovalDelay = 2f;

		// Token: 0x04000C62 RID: 3170
		[DataField("airtight", false, 1, false, false, null)]
		[ViewVariables]
		public bool Airtight;

		// Token: 0x04000C63 RID: 3171
		[DataField("startingEquipment", false, 1, false, false, typeof(PrototypeIdListSerializer<EntityPrototype>))]
		public List<string> StartingEquipment = new List<string>();

		// Token: 0x04000C64 RID: 3172
		[Nullable(2)]
		[DataField("startingBattery", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string StartingBattery;

		// Token: 0x04000C65 RID: 3173
		[ViewVariables]
		public GasMixture Air = new GasMixture(70f);

		// Token: 0x04000C66 RID: 3174
		public const float GasMixVolume = 70f;
	}
}
