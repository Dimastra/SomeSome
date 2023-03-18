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
	// Token: 0x02000607 RID: 1543
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedTypingIndicatorSystem)
	})]
	public sealed class TypingIndicatorComponent : Component
	{
		// Token: 0x0400119F RID: 4511
		[Nullable(1)]
		[ViewVariables]
		[DataField("proto", false, 1, false, false, typeof(PrototypeIdSerializer<TypingIndicatorPrototype>))]
		public string Prototype = "default";
	}
}
