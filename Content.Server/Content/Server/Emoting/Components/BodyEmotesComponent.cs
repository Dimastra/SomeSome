using System;
using System.Runtime.CompilerServices;
using Content.Server.Emoting.Systems;
using Content.Shared.Chat.Prototypes;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Emoting.Components
{
	// Token: 0x02000530 RID: 1328
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(BodyEmotesSystem)
	})]
	public sealed class BodyEmotesComponent : Component
	{
		// Token: 0x040011B8 RID: 4536
		[DataField("soundsId", false, 1, false, false, typeof(PrototypeIdSerializer<EmoteSoundsPrototype>))]
		public string SoundsId;

		// Token: 0x040011B9 RID: 4537
		public EmoteSoundsPrototype Sounds;
	}
}
