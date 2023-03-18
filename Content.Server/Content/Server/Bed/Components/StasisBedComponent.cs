using System;
using System.Runtime.CompilerServices;
using Content.Shared.Construction.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Bed.Components
{
	// Token: 0x02000729 RID: 1833
	[RegisterComponent]
	public sealed class StasisBedComponent : Component
	{
		// Token: 0x040017F3 RID: 6131
		[DataField("baseMultiplier", false, 1, true, false, null)]
		[ViewVariables]
		public float BaseMultiplier = 10f;

		// Token: 0x040017F4 RID: 6132
		[ViewVariables]
		public float Multiplier = 10f;

		// Token: 0x040017F5 RID: 6133
		[Nullable(1)]
		[DataField("machinePartMetabolismModifier", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartMetabolismModifier = "Manipulator";
	}
}
