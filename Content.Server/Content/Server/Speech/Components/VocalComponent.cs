using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Speech.EntitySystems;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Humanoid;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.ViewVariables;

namespace Content.Server.Speech.Components
{
	// Token: 0x020001D0 RID: 464
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(VocalSystem)
	})]
	public sealed class VocalComponent : Component
	{
		// Token: 0x04000558 RID: 1368
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("sounds", false, 1, false, false, typeof(PrototypeIdValueDictionarySerializer<Sex, EmoteSoundsPrototype>))]
		public Dictionary<Sex, string> Sounds;

		// Token: 0x04000559 RID: 1369
		[DataField("screamId", false, 1, false, false, typeof(PrototypeIdSerializer<EmotePrototype>))]
		public string ScreamId = "Scream";

		// Token: 0x0400055A RID: 1370
		[DataField("wilhelm", false, 1, false, false, null)]
		public SoundSpecifier Wilhelm = new SoundPathSpecifier("/Audio/Voice/Human/wilhelm_scream.ogg", null);

		// Token: 0x0400055B RID: 1371
		[DataField("wilhelmProbability", false, 1, false, false, null)]
		public float WilhelmProbability = 0.0002f;

		// Token: 0x0400055C RID: 1372
		[DataField("screamActionId", false, 1, false, false, typeof(PrototypeIdSerializer<InstantActionPrototype>))]
		public string ScreamActionId = "Scream";

		// Token: 0x0400055D RID: 1373
		[Nullable(2)]
		[DataField("screamAction", false, 1, false, false, null)]
		public InstantAction ScreamAction;

		// Token: 0x0400055E RID: 1374
		[Nullable(2)]
		[ViewVariables]
		public EmoteSoundsPrototype EmoteSounds;
	}
}
