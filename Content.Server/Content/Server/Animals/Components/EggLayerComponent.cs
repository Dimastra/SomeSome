using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Storage;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Animals.Components
{
	// Token: 0x020007D1 RID: 2001
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class EggLayerComponent : Component
	{
		// Token: 0x04001AED RID: 6893
		[DataField("eggLayAction", false, 1, false, false, typeof(PrototypeIdSerializer<InstantActionPrototype>))]
		public string EggLayAction = "AnimalLayEgg";

		// Token: 0x04001AEE RID: 6894
		[ViewVariables]
		[DataField("hungerUsage", false, 1, false, false, null)]
		public float HungerUsage = 60f;

		// Token: 0x04001AEF RID: 6895
		[ViewVariables]
		[DataField("eggLayCooldownMin", false, 1, false, false, null)]
		public float EggLayCooldownMin = 60f;

		// Token: 0x04001AF0 RID: 6896
		[ViewVariables]
		[DataField("eggLayCooldownMax", false, 1, false, false, null)]
		public float EggLayCooldownMax = 120f;

		// Token: 0x04001AF1 RID: 6897
		[ViewVariables]
		public float CurrentEggLayCooldown;

		// Token: 0x04001AF2 RID: 6898
		[ViewVariables]
		[DataField("eggSpawn", false, 1, true, false, null)]
		public List<EntitySpawnEntry> EggSpawn;

		// Token: 0x04001AF3 RID: 6899
		[DataField("eggLaySound", false, 1, false, false, null)]
		public SoundSpecifier EggLaySound = new SoundPathSpecifier("/Audio/Effects/pop.ogg", null);

		// Token: 0x04001AF4 RID: 6900
		[DataField("accumulatedFrametime", false, 1, false, false, null)]
		public float AccumulatedFrametime;
	}
}
