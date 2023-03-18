using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Construction.Components
{
	// Token: 0x02000604 RID: 1540
	[RegisterComponent]
	[ComponentProtoName("Computer")]
	public sealed class ComputerComponent : Component
	{
		// Token: 0x0400144A RID: 5194
		[Nullable(2)]
		[DataField("board", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string BoardPrototype;
	}
}
