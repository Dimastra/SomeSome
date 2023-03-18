using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.ViewVariables;

namespace Content.Server.Magic
{
	// Token: 0x020003E2 RID: 994
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class SpellbookComponent : Component
	{
		// Token: 0x04000C9B RID: 3227
		[ViewVariables]
		public readonly List<ActionType> Spells = new List<ActionType>();

		// Token: 0x04000C9C RID: 3228
		[DataField("worldSpells", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<int, WorldTargetActionPrototype>))]
		public readonly Dictionary<string, int> WorldSpells = new Dictionary<string, int>();

		// Token: 0x04000C9D RID: 3229
		[DataField("entitySpells", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<int, EntityTargetActionPrototype>))]
		public readonly Dictionary<string, int> EntitySpells = new Dictionary<string, int>();

		// Token: 0x04000C9E RID: 3230
		[DataField("instantSpells", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<int, InstantActionPrototype>))]
		public readonly Dictionary<string, int> InstantSpells = new Dictionary<string, int>();

		// Token: 0x04000C9F RID: 3231
		[DataField("learnTime", false, 1, false, false, null)]
		public float LearnTime = 0.75f;
	}
}
