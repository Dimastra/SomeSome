using System;
using System.Runtime.CompilerServices;
using Content.Shared.AirlockPainter.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.AirlockPainter
{
	// Token: 0x0200000E RID: 14
	[RegisterComponent]
	public sealed class PaintableAirlockComponent : Component
	{
		// Token: 0x0400000C RID: 12
		[Nullable(1)]
		[DataField("group", false, 1, false, false, typeof(PrototypeIdSerializer<AirlockGroupPrototype>))]
		public string Group;
	}
}
