using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Construction.Components
{
	// Token: 0x02000603 RID: 1539
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class ComputerBoardComponent : Component
	{
		// Token: 0x170004EF RID: 1263
		// (get) Token: 0x0600210E RID: 8462 RVA: 0x000AD869 File Offset: 0x000ABA69
		// (set) Token: 0x0600210F RID: 8463 RVA: 0x000AD871 File Offset: 0x000ABA71
		[DataField("prototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Prototype { get; private set; }
	}
}
