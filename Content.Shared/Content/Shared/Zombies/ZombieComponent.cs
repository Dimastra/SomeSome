using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Humanoid;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Zombies
{
	// Token: 0x02000011 RID: 17
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class ZombieComponent : Component
	{
		// Token: 0x0400000D RID: 13
		[ViewVariables]
		public float OtherZombieDamageCoefficient = 0.25f;

		// Token: 0x0400000E RID: 14
		[ViewVariables]
		public float MaxZombieInfectionChance = 0.5f;

		// Token: 0x0400000F RID: 15
		[ViewVariables]
		public float MinZombieInfectionChance = 0.05f;

		// Token: 0x04000010 RID: 16
		[ViewVariables]
		public float ZombieMovementSpeedDebuff = 0.75f;

		// Token: 0x04000011 RID: 17
		[DataField("skinColor", false, 1, false, false, null)]
		public Color SkinColor = new Color(0.45f, 0.51f, 0.29f, 1f);

		// Token: 0x04000012 RID: 18
		[DataField("eyeColor", false, 1, false, false, null)]
		public Color EyeColor = new Color(0.96f, 0.13f, 0.24f, 1f);

		// Token: 0x04000013 RID: 19
		[DataField("baseLayerExternal", false, 1, false, false, null)]
		public string BaseLayerExternal = "MobHumanoidMarkingMatchSkin";

		// Token: 0x04000014 RID: 20
		[DataField("attackArc", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string AttackAnimation = "WeaponArcBite";

		// Token: 0x04000015 RID: 21
		[DataField("zombieRoleId", false, 1, false, false, typeof(PrototypeIdSerializer<AntagPrototype>))]
		public readonly string ZombieRoleId = "Zombie";

		// Token: 0x04000016 RID: 22
		[DataField("beforeZombifiedEntityName", false, 1, false, false, null)]
		[ViewVariables]
		public string BeforeZombifiedEntityName = string.Empty;

		// Token: 0x04000017 RID: 23
		[DataField("beforeZombifiedCustomBaseLayers", false, 1, false, false, null)]
		public Dictionary<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo> BeforeZombifiedCustomBaseLayers = new Dictionary<HumanoidVisualLayers, HumanoidAppearanceState.CustomBaseLayerInfo>();

		// Token: 0x04000018 RID: 24
		[DataField("beforeZombifiedSkinColor", false, 1, false, false, null)]
		public Color BeforeZombifiedSkinColor;

		// Token: 0x04000019 RID: 25
		[Nullable(2)]
		[DataField("emoteId", false, 1, false, false, typeof(PrototypeIdSerializer<EmoteSoundsPrototype>))]
		public string EmoteSoundsId = "Zombie";

		// Token: 0x0400001A RID: 26
		[Nullable(2)]
		public EmoteSoundsPrototype EmoteSounds;
	}
}
