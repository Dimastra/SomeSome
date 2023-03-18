using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Server.Bible.Components
{
	// Token: 0x02000723 RID: 1827
	[RegisterComponent]
	public sealed class SummonableComponent : Component
	{
		// Token: 0x040017DB RID: 6107
		[Nullable(2)]
		[DataField("specialItem", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string SpecialItemPrototype;

		// Token: 0x040017DC RID: 6108
		public bool AlreadySummoned;

		// Token: 0x040017DD RID: 6109
		[DataField("requriesBibleUser", false, 1, false, false, null)]
		public bool RequiresBibleUser = true;

		// Token: 0x040017DE RID: 6110
		[ViewVariables]
		public EntityUid? Summon;

		// Token: 0x040017DF RID: 6111
		[Nullable(1)]
		[DataField("summonAction", false, 1, false, false, null)]
		public InstantAction SummonAction = new InstantAction
		{
			Icon = new SpriteSpecifier.Texture(new ResourcePath("Clothing/Head/Hats/witch.rsi/icon.png", "/")),
			DisplayName = "bible-summon-verb",
			Description = "bible-summon-verb-desc",
			Event = new SummonActionEvent()
		};

		// Token: 0x040017E0 RID: 6112
		[DataField("accumulator", false, 1, false, false, null)]
		public float Accumulator;

		// Token: 0x040017E1 RID: 6113
		[DataField("respawnTime", false, 1, false, false, null)]
		public float RespawnTime = 180f;
	}
}
