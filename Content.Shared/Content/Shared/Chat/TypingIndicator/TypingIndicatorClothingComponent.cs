using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Chat.TypingIndicator
{
	// Token: 0x02000606 RID: 1542
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedTypingIndicatorSystem)
	})]
	public sealed class TypingIndicatorClothingComponent : Component
	{
		// Token: 0x0400119E RID: 4510
		[Nullable(1)]
		[ViewVariables]
		[DataField("proto", false, 1, true, false, typeof(PrototypeIdSerializer<TypingIndicatorPrototype>))]
		public string Prototype;
	}
}
