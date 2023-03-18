using System;
using System.Runtime.CompilerServices;
using Content.Shared.Disease;
using Content.Shared.FixedPoint;
using Content.Shared.Store;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Revenant.Components
{
	// Token: 0x020001F9 RID: 505
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class RevenantComponent : Component
	{
		// Token: 0x040005A1 RID: 1441
		[ViewVariables]
		public FixedPoint2 Essence = 75;

		// Token: 0x040005A2 RID: 1442
		[DataField("stolenEssenceCurrencyPrototype", false, 1, false, false, typeof(PrototypeIdSerializer<CurrencyPrototype>))]
		public string StolenEssenceCurrencyPrototype = "StolenEssence";

		// Token: 0x040005A3 RID: 1443
		[ViewVariables]
		[DataField("maxEssence", false, 1, false, false, null)]
		public FixedPoint2 EssenceRegenCap = 75;

		// Token: 0x040005A4 RID: 1444
		[ViewVariables]
		[DataField("damageToEssenceCoefficient", false, 1, false, false, null)]
		public float DamageToEssenceCoefficient = 0.75f;

		// Token: 0x040005A5 RID: 1445
		[ViewVariables]
		[DataField("essencePerSecond", false, 1, false, false, null)]
		public FixedPoint2 EssencePerSecond = 0.5f;

		// Token: 0x040005A6 RID: 1446
		[ViewVariables]
		public float Accumulator;

		// Token: 0x040005A7 RID: 1447
		[DataField("soulSearchDuration", false, 1, false, false, null)]
		public float SoulSearchDuration = 2.5f;

		// Token: 0x040005A8 RID: 1448
		[DataField("harvestDebuffs", false, 1, false, false, null)]
		public Vector2 HarvestDebuffs = new ValueTuple<float, float>(5f, 5f);

		// Token: 0x040005A9 RID: 1449
		[ViewVariables]
		[DataField("maxEssenceUpgradeAmount", false, 1, false, false, null)]
		public float MaxEssenceUpgradeAmount = 10f;

		// Token: 0x040005AA RID: 1450
		[ViewVariables]
		[DataField("defileCost", false, 1, false, false, null)]
		public FixedPoint2 DefileCost = -30;

		// Token: 0x040005AB RID: 1451
		[DataField("defileDebuffs", false, 1, false, false, null)]
		public Vector2 DefileDebuffs = new ValueTuple<float, float>(1f, 4f);

		// Token: 0x040005AC RID: 1452
		[ViewVariables]
		[DataField("defileRadius", false, 1, false, false, null)]
		public float DefileRadius = 3.5f;

		// Token: 0x040005AD RID: 1453
		[ViewVariables]
		[DataField("defileTilePryAmount", false, 1, false, false, null)]
		public int DefileTilePryAmount = 15;

		// Token: 0x040005AE RID: 1454
		[ViewVariables]
		[DataField("defileEffectChance", false, 1, false, false, null)]
		public float DefileEffectChance = 0.5f;

		// Token: 0x040005AF RID: 1455
		[ViewVariables]
		[DataField("overloadCost", false, 1, false, false, null)]
		public FixedPoint2 OverloadCost = -40;

		// Token: 0x040005B0 RID: 1456
		[DataField("overloadDebuffs", false, 1, false, false, null)]
		public Vector2 OverloadDebuffs = new ValueTuple<float, float>(3f, 8f);

		// Token: 0x040005B1 RID: 1457
		[ViewVariables]
		[DataField("overloadRadius", false, 1, false, false, null)]
		public float OverloadRadius = 5f;

		// Token: 0x040005B2 RID: 1458
		[ViewVariables]
		[DataField("overloadZapRadius", false, 1, false, false, null)]
		public float OverloadZapRadius = 2f;

		// Token: 0x040005B3 RID: 1459
		[ViewVariables]
		[DataField("blightCost", false, 1, false, false, null)]
		public float BlightCost = -50f;

		// Token: 0x040005B4 RID: 1460
		[DataField("blightDebuffs", false, 1, false, false, null)]
		public Vector2 BlightDebuffs = new ValueTuple<float, float>(2f, 5f);

		// Token: 0x040005B5 RID: 1461
		[ViewVariables]
		[DataField("blightRadius", false, 1, false, false, null)]
		public float BlightRadius = 3.5f;

		// Token: 0x040005B6 RID: 1462
		[ViewVariables]
		[DataField("blightDiseasePrototypeId", false, 1, false, false, typeof(PrototypeIdSerializer<DiseasePrototype>))]
		public string BlightDiseasePrototypeId = "SpectralTiredness";

		// Token: 0x040005B7 RID: 1463
		[ViewVariables]
		[DataField("malfunctionCost", false, 1, false, false, null)]
		public FixedPoint2 MalfunctionCost = -60;

		// Token: 0x040005B8 RID: 1464
		[DataField("malfunctionDebuffs", false, 1, false, false, null)]
		public Vector2 MalfunctionDebuffs = new ValueTuple<float, float>(2f, 8f);

		// Token: 0x040005B9 RID: 1465
		[ViewVariables]
		[DataField("malfunctionRadius", false, 1, false, false, null)]
		public float MalfunctionRadius = 3.5f;

		// Token: 0x040005BA RID: 1466
		[DataField("state", false, 1, false, false, null)]
		public string State = "idle";

		// Token: 0x040005BB RID: 1467
		[DataField("corporealState", false, 1, false, false, null)]
		public string CorporealState = "active";

		// Token: 0x040005BC RID: 1468
		[DataField("stunnedState", false, 1, false, false, null)]
		public string StunnedState = "stunned";

		// Token: 0x040005BD RID: 1469
		[DataField("harvestingState", false, 1, false, false, null)]
		public string HarvestingState = "harvesting";
	}
}
