using System;
using System.Runtime.CompilerServices;
using Content.Shared.Construction.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Construction.Components
{
	// Token: 0x0200058F RID: 1423
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class MachinePartComponent : Component
	{
		// Token: 0x17000383 RID: 899
		// (get) Token: 0x06001170 RID: 4464 RVA: 0x000391BC File Offset: 0x000373BC
		// (set) Token: 0x06001171 RID: 4465 RVA: 0x000391C4 File Offset: 0x000373C4
		[DataField("part", false, 1, true, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string PartType { get; private set; }

		// Token: 0x17000384 RID: 900
		// (get) Token: 0x06001172 RID: 4466 RVA: 0x000391CD File Offset: 0x000373CD
		// (set) Token: 0x06001173 RID: 4467 RVA: 0x000391D5 File Offset: 0x000373D5
		[ViewVariables]
		[DataField("rating", false, 1, false, false, null)]
		public int Rating { get; private set; } = 1;

		// Token: 0x0400101A RID: 4122
		public const int MaxRating = 4;
	}
}
