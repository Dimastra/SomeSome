using System;
using System.Runtime.CompilerServices;
using Content.Shared.Dataset;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Roles;
using Robust.Shared.Audio;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Utility;

namespace Content.Server.GameTicking.Rules.Configurations
{
	// Token: 0x020004C9 RID: 1225
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NukeopsRuleConfiguration : GameRuleConfiguration
	{
		// Token: 0x170003A4 RID: 932
		// (get) Token: 0x0600195F RID: 6495 RVA: 0x00085F89 File Offset: 0x00084189
		public override string Id
		{
			get
			{
				return "Nukeops";
			}
		}

		// Token: 0x04000FED RID: 4077
		[DataField("playersPerOperative", false, 1, false, false, null)]
		public int PlayersPerOperative = 15;

		// Token: 0x04000FEE RID: 4078
		[DataField("maxOps", false, 1, false, false, null)]
		public int MaxOperatives = 5;

		// Token: 0x04000FEF RID: 4079
		[DataField("randomHumanoidSettings", false, 1, false, false, typeof(PrototypeIdSerializer<RandomHumanoidSettingsPrototype>))]
		public string RandomHumanoidSettingsPrototype = "NukeOp";

		// Token: 0x04000FF0 RID: 4080
		[DataField("spawnPointProto", false, 1, false, false, typeof(PrototypeIdSerializer<StartingGearPrototype>))]
		public string SpawnPointPrototype = "SpawnPointNukies";

		// Token: 0x04000FF1 RID: 4081
		[DataField("ghostSpawnPointProto", false, 1, false, false, typeof(PrototypeIdSerializer<StartingGearPrototype>))]
		public string GhostSpawnPointProto = "SpawnPointGhostNukeOperative";

		// Token: 0x04000FF2 RID: 4082
		[DataField("commanderRoleProto", false, 1, false, false, typeof(PrototypeIdSerializer<StartingGearPrototype>))]
		public string CommanderRolePrototype = "NukeopsCommander";

		// Token: 0x04000FF3 RID: 4083
		[DataField("operativeRoleProto", false, 1, false, false, typeof(PrototypeIdSerializer<StartingGearPrototype>))]
		public string OperativeRoleProto = "Nukeops";

		// Token: 0x04000FF4 RID: 4084
		[DataField("commanderStartingGearProto", false, 1, false, false, typeof(PrototypeIdSerializer<StartingGearPrototype>))]
		public string CommanderStartGearPrototype = "SyndicateCommanderGearFull";

		// Token: 0x04000FF5 RID: 4085
		[DataField("medicStartGearProto", false, 1, false, false, typeof(PrototypeIdSerializer<StartingGearPrototype>))]
		public string MedicStartGearPrototype = "SyndicateOperativeMedicFull";

		// Token: 0x04000FF6 RID: 4086
		[DataField("operativeStartGearProto", false, 1, false, false, typeof(PrototypeIdSerializer<StartingGearPrototype>))]
		public string OperativeStartGearPrototype = "SyndicateOperativeGearFull";

		// Token: 0x04000FF7 RID: 4087
		[DataField("eliteNames", false, 1, false, false, typeof(PrototypeIdSerializer<DatasetPrototype>))]
		public string EliteNames = "SyndicateNamesElite";

		// Token: 0x04000FF8 RID: 4088
		[DataField("normalNames", false, 1, false, false, typeof(PrototypeIdSerializer<DatasetPrototype>))]
		public string NormalNames = "SyndicateNamesNormal";

		// Token: 0x04000FF9 RID: 4089
		[Nullable(2)]
		[DataField("outpostMap", false, 1, false, false, typeof(ResourcePathSerializer))]
		public ResourcePath NukieOutpostMap = new ResourcePath("/Maps/nukieplanet.yml", "/");

		// Token: 0x04000FFA RID: 4090
		[Nullable(2)]
		[DataField("shuttleMap", false, 1, false, false, typeof(ResourcePathSerializer))]
		public ResourcePath NukieShuttleMap = new ResourcePath("/Maps/infiltrator.yml", "/");

		// Token: 0x04000FFB RID: 4091
		[Nullable(2)]
		[DataField("greetingSound", false, 1, false, false, typeof(SoundSpecifierTypeSerializer))]
		public SoundSpecifier GreetSound = new SoundPathSpecifier("/Audio/Misc/nukeops.ogg", null);
	}
}
