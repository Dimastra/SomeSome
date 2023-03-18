using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Dragon
{
	// Token: 0x0200053A RID: 1338
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class DragonComponent : Component
	{
		// Token: 0x1700042F RID: 1071
		// (get) Token: 0x06001BF0 RID: 7152 RVA: 0x00094F33 File Offset: 0x00093133
		public bool Weakened
		{
			get
			{
				return this.WeakenedAccumulator > 0f;
			}
		}

		// Token: 0x040011F6 RID: 4598
		[Nullable(1)]
		[DataField("devourChemical", false, 1, false, false, typeof(PrototypeIdSerializer<ReagentPrototype>))]
		public string DevourChem = "Ichor";

		// Token: 0x040011F7 RID: 4599
		[ViewVariables]
		[DataField("devourHealRate", false, 1, false, false, null)]
		public float DevourHealRate = 15f;

		// Token: 0x040011F8 RID: 4600
		[Nullable(1)]
		[DataField("devourActionId", false, 1, false, false, typeof(PrototypeIdSerializer<EntityTargetActionPrototype>))]
		public string DevourActionId = "DragonDevour";

		// Token: 0x040011F9 RID: 4601
		[DataField("devourAction", false, 1, false, false, null)]
		public EntityTargetAction DevourAction;

		// Token: 0x040011FA RID: 4602
		[Nullable(1)]
		[DataField("rifts", false, 1, false, false, null)]
		public List<EntityUid> Rifts = new List<EntityUid>();

		// Token: 0x040011FB RID: 4603
		[ViewVariables]
		[DataField("weakenedDuration", false, 1, false, false, null)]
		public float WeakenedDuration = 120f;

		// Token: 0x040011FC RID: 4604
		[ViewVariables]
		[DataField("weakenedAccumulator", false, 1, false, false, null)]
		public float WeakenedAccumulator;

		// Token: 0x040011FD RID: 4605
		[ViewVariables]
		[DataField("riftAccumulator", false, 1, false, false, null)]
		public float RiftAccumulator;

		// Token: 0x040011FE RID: 4606
		[ViewVariables]
		[DataField("maxAccumulator", false, 1, false, false, null)]
		public float RiftMaxAccumulator = 300f;

		// Token: 0x040011FF RID: 4607
		[DataField("spawnRiftAction", false, 1, false, false, null)]
		public InstantAction SpawnRiftAction;

		// Token: 0x04001200 RID: 4608
		[Nullable(1)]
		[ViewVariables]
		[DataField("riftPrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string RiftPrototype = "CarpRift";

		// Token: 0x04001201 RID: 4609
		[DataField("structureDevourTime", false, 1, false, false, null)]
		public float StructureDevourTime = 10f;

		// Token: 0x04001202 RID: 4610
		[DataField("devourTime", false, 1, false, false, null)]
		public float DevourTime = 3f;

		// Token: 0x04001203 RID: 4611
		[ViewVariables]
		[DataField("soundDeath", false, 1, false, false, null)]
		public SoundSpecifier SoundDeath = new SoundPathSpecifier("/Audio/Animals/space_dragon_roar.ogg", null);

		// Token: 0x04001204 RID: 4612
		[ViewVariables]
		[DataField("soundDevour", false, 1, false, false, null)]
		public SoundSpecifier SoundDevour = new SoundPathSpecifier("/Audio/Effects/demon_consume.ogg", null)
		{
			Params = AudioParams.Default.WithVolume(-3f)
		};

		// Token: 0x04001205 RID: 4613
		[ViewVariables]
		[DataField("soundStructureDevour", false, 1, false, false, null)]
		public SoundSpecifier SoundStructureDevour = new SoundPathSpecifier("/Audio/Machines/airlock_creaking.ogg", null)
		{
			Params = AudioParams.Default.WithVolume(-3f)
		};

		// Token: 0x04001206 RID: 4614
		[ViewVariables]
		[DataField("soundRoar", false, 1, false, false, null)]
		public SoundSpecifier SoundRoar = new SoundPathSpecifier("/Audio/Animals/space_dragon_roar.ogg", null)
		{
			Params = AudioParams.Default.WithVolume(3f)
		};

		// Token: 0x04001207 RID: 4615
		[ViewVariables]
		[DataField("devourWhitelist", false, 1, false, false, null)]
		public EntityWhitelist DevourWhitelist = new EntityWhitelist
		{
			Components = new string[]
			{
				"Door",
				"MobState"
			},
			Tags = new List<string>
			{
				"Wall"
			}
		};

		// Token: 0x04001208 RID: 4616
		[Nullable(1)]
		public Container DragonStomach;
	}
}
