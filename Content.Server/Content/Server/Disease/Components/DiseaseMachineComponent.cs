using System;
using System.Runtime.CompilerServices;
using Content.Shared.Disease;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Disease.Components
{
	// Token: 0x02000575 RID: 1397
	[RegisterComponent]
	public sealed class DiseaseMachineComponent : Component
	{
		// Token: 0x040012DB RID: 4827
		[DataField("delay", false, 1, false, false, null)]
		public float Delay = 5f;

		// Token: 0x040012DC RID: 4828
		[DataField("accumulator", false, 1, false, false, null)]
		public float Accumulator;

		// Token: 0x040012DD RID: 4829
		[Nullable(2)]
		[ViewVariables]
		public DiseasePrototype Disease;

		// Token: 0x040012DE RID: 4830
		[Nullable(1)]
		[DataField("machineOutput", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string MachineOutput = string.Empty;
	}
}
