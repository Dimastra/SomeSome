using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Construction.Components
{
	// Token: 0x02000607 RID: 1543
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentProtoName("Machine")]
	public sealed class MachineComponent : Component
	{
		// Token: 0x170004FA RID: 1274
		// (get) Token: 0x06002128 RID: 8488 RVA: 0x000AD968 File Offset: 0x000ABB68
		// (set) Token: 0x06002129 RID: 8489 RVA: 0x000AD970 File Offset: 0x000ABB70
		[DataField("board", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string BoardPrototype { get; private set; }

		// Token: 0x04001457 RID: 5207
		[Nullable(1)]
		[ViewVariables]
		public Container BoardContainer;

		// Token: 0x04001458 RID: 5208
		[Nullable(1)]
		[ViewVariables]
		public Container PartContainer;
	}
}
