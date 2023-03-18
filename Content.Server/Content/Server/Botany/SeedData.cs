using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Botany.Components;
using Content.Server.Botany.Systems;
using Content.Shared.Atmos;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Analyzers;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Server.Botany
{
	// Token: 0x020006F9 RID: 1785
	[NullableContext(1)]
	[Nullable(0)]
	[Virtual]
	[DataDefinition]
	[Access(new Type[]
	{
		typeof(BotanySystem),
		typeof(PlantHolderSystem),
		typeof(SeedExtractorSystem),
		typeof(PlantHolderComponent),
		typeof(ReagentEffect),
		typeof(MutationSystem)
	})]
	public class SeedData
	{
		// Token: 0x17000582 RID: 1410
		// (get) Token: 0x0600254C RID: 9548 RVA: 0x000C3829 File Offset: 0x000C1A29
		// (set) Token: 0x0600254D RID: 9549 RVA: 0x000C3831 File Offset: 0x000C1A31
		[DataField("name", false, 1, false, false, null)]
		public string Name { get; private set; } = "";

		// Token: 0x17000583 RID: 1411
		// (get) Token: 0x0600254E RID: 9550 RVA: 0x000C383A File Offset: 0x000C1A3A
		// (set) Token: 0x0600254F RID: 9551 RVA: 0x000C3842 File Offset: 0x000C1A42
		[DataField("noun", false, 1, false, false, null)]
		public string Noun { get; private set; } = "";

		// Token: 0x17000584 RID: 1412
		// (get) Token: 0x06002550 RID: 9552 RVA: 0x000C384B File Offset: 0x000C1A4B
		// (set) Token: 0x06002551 RID: 9553 RVA: 0x000C3853 File Offset: 0x000C1A53
		[DataField("displayName", false, 1, false, false, null)]
		public string DisplayName { get; private set; } = "";

		// Token: 0x17000585 RID: 1413
		// (get) Token: 0x06002552 RID: 9554 RVA: 0x000C385C File Offset: 0x000C1A5C
		// (set) Token: 0x06002553 RID: 9555 RVA: 0x000C3864 File Offset: 0x000C1A64
		[DataField("plantRsi", false, 1, true, false, null)]
		public ResourcePath PlantRsi { get; set; }

		// Token: 0x17000586 RID: 1414
		// (get) Token: 0x06002554 RID: 9556 RVA: 0x000C386D File Offset: 0x000C1A6D
		// (set) Token: 0x06002555 RID: 9557 RVA: 0x000C3875 File Offset: 0x000C1A75
		[DataField("plantIconState", false, 1, false, false, null)]
		public string PlantIconState { get; set; } = "produce";

		// Token: 0x17000587 RID: 1415
		// (get) Token: 0x06002556 RID: 9558 RVA: 0x000C387E File Offset: 0x000C1A7E
		// (set) Token: 0x06002557 RID: 9559 RVA: 0x000C3886 File Offset: 0x000C1A86
		[DataField("bioluminescentColor", false, 1, false, false, null)]
		public Color BioluminescentColor { get; set; } = Color.White;

		// Token: 0x17000588 RID: 1416
		// (get) Token: 0x06002558 RID: 9560 RVA: 0x000C388F File Offset: 0x000C1A8F
		// (set) Token: 0x06002559 RID: 9561 RVA: 0x000C3897 File Offset: 0x000C1A97
		[Nullable(2)]
		[DataField("splatPrototype", false, 1, false, false, null)]
		public string SplatPrototype { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x0600255A RID: 9562 RVA: 0x000C38A0 File Offset: 0x000C1AA0
		public SeedData Clone()
		{
			return new SeedData
			{
				Name = this.Name,
				Noun = this.Noun,
				DisplayName = this.DisplayName,
				Mysterious = this.Mysterious,
				PacketPrototype = this.PacketPrototype,
				ProductPrototypes = new List<string>(this.ProductPrototypes),
				Chemicals = new Dictionary<string, SeedChemQuantity>(this.Chemicals),
				ConsumeGasses = new Dictionary<Gas, float>(this.ConsumeGasses),
				ExudeGasses = new Dictionary<Gas, float>(this.ExudeGasses),
				NutrientConsumption = this.NutrientConsumption,
				WaterConsumption = this.WaterConsumption,
				IdealHeat = this.IdealHeat,
				HeatTolerance = this.HeatTolerance,
				IdealLight = this.IdealLight,
				LightTolerance = this.LightTolerance,
				ToxinsTolerance = this.ToxinsTolerance,
				LowPressureTolerance = this.LowPressureTolerance,
				HighPressureTolerance = this.HighPressureTolerance,
				PestTolerance = this.PestTolerance,
				WeedTolerance = this.WeedTolerance,
				Endurance = this.Endurance,
				Yield = this.Yield,
				Lifespan = this.Lifespan,
				Maturation = this.Maturation,
				Production = this.Production,
				GrowthStages = this.GrowthStages,
				HarvestRepeat = this.HarvestRepeat,
				Potency = this.Potency,
				Seedless = this.Seedless,
				Viable = this.Viable,
				Slip = this.Slip,
				Sentient = this.Sentient,
				Ligneous = this.Ligneous,
				PlantRsi = this.PlantRsi,
				PlantIconState = this.PlantIconState,
				Bioluminescent = this.Bioluminescent,
				BioluminescentColor = this.BioluminescentColor,
				SplatPrototype = this.SplatPrototype,
				Unique = true
			};
		}

		// Token: 0x040016EC RID: 5868
		[DataField("mysterious", false, 1, false, false, null)]
		public bool Mysterious;

		// Token: 0x040016ED RID: 5869
		[DataField("immutable", false, 1, false, false, null)]
		public bool Immutable;

		// Token: 0x040016EE RID: 5870
		[ViewVariables]
		public bool Unique;

		// Token: 0x040016EF RID: 5871
		[DataField("packetPrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string PacketPrototype = "SeedBase";

		// Token: 0x040016F0 RID: 5872
		[DataField("productPrototypes", false, 1, false, false, typeof(PrototypeIdListSerializer<EntityPrototype>))]
		public List<string> ProductPrototypes = new List<string>();

		// Token: 0x040016F1 RID: 5873
		[DataField("chemicals", false, 1, false, false, null)]
		public Dictionary<string, SeedChemQuantity> Chemicals = new Dictionary<string, SeedChemQuantity>();

		// Token: 0x040016F2 RID: 5874
		[DataField("consumeGasses", false, 1, false, false, null)]
		public Dictionary<Gas, float> ConsumeGasses = new Dictionary<Gas, float>();

		// Token: 0x040016F3 RID: 5875
		[DataField("exudeGasses", false, 1, false, false, null)]
		public Dictionary<Gas, float> ExudeGasses = new Dictionary<Gas, float>();

		// Token: 0x040016F4 RID: 5876
		[DataField("nutrientConsumption", false, 1, false, false, null)]
		public float NutrientConsumption = 0.25f;

		// Token: 0x040016F5 RID: 5877
		[DataField("waterConsumption", false, 1, false, false, null)]
		public float WaterConsumption = 3f;

		// Token: 0x040016F6 RID: 5878
		[DataField("idealHeat", false, 1, false, false, null)]
		public float IdealHeat = 293f;

		// Token: 0x040016F7 RID: 5879
		[DataField("heatTolerance", false, 1, false, false, null)]
		public float HeatTolerance = 10f;

		// Token: 0x040016F8 RID: 5880
		[DataField("idealLight", false, 1, false, false, null)]
		public float IdealLight = 7f;

		// Token: 0x040016F9 RID: 5881
		[DataField("lightTolerance", false, 1, false, false, null)]
		public float LightTolerance = 3f;

		// Token: 0x040016FA RID: 5882
		[DataField("toxinsTolerance", false, 1, false, false, null)]
		public float ToxinsTolerance = 4f;

		// Token: 0x040016FB RID: 5883
		[DataField("lowPressureTolerance", false, 1, false, false, null)]
		public float LowPressureTolerance = 81f;

		// Token: 0x040016FC RID: 5884
		[DataField("highPressureTolerance", false, 1, false, false, null)]
		public float HighPressureTolerance = 121f;

		// Token: 0x040016FD RID: 5885
		[DataField("pestTolerance", false, 1, false, false, null)]
		public float PestTolerance = 5f;

		// Token: 0x040016FE RID: 5886
		[DataField("weedTolerance", false, 1, false, false, null)]
		public float WeedTolerance = 5f;

		// Token: 0x040016FF RID: 5887
		[DataField("endurance", false, 1, false, false, null)]
		public float Endurance = 100f;

		// Token: 0x04001700 RID: 5888
		[DataField("yield", false, 1, false, false, null)]
		public int Yield;

		// Token: 0x04001701 RID: 5889
		[DataField("lifespan", false, 1, false, false, null)]
		public float Lifespan;

		// Token: 0x04001702 RID: 5890
		[DataField("maturation", false, 1, false, false, null)]
		public float Maturation;

		// Token: 0x04001703 RID: 5891
		[DataField("production", false, 1, false, false, null)]
		public float Production;

		// Token: 0x04001704 RID: 5892
		[DataField("growthStages", false, 1, false, false, null)]
		public int GrowthStages = 6;

		// Token: 0x04001705 RID: 5893
		[DataField("harvestRepeat", false, 1, false, false, null)]
		public HarvestType HarvestRepeat;

		// Token: 0x04001706 RID: 5894
		[DataField("potency", false, 1, false, false, null)]
		public float Potency = 1f;

		// Token: 0x04001707 RID: 5895
		[DataField("seedless", false, 1, false, false, null)]
		public bool Seedless;

		// Token: 0x04001708 RID: 5896
		[DataField("viable", false, 1, false, false, null)]
		public bool Viable = true;

		// Token: 0x04001709 RID: 5897
		[DataField("slip", false, 1, false, false, null)]
		public bool Slip;

		// Token: 0x0400170A RID: 5898
		[DataField("sentient", false, 1, false, false, null)]
		public bool Sentient;

		// Token: 0x0400170B RID: 5899
		[DataField("ligneous", false, 1, false, false, null)]
		public bool Ligneous;

		// Token: 0x0400170E RID: 5902
		[DataField("bioluminescent", false, 1, false, false, null)]
		public bool Bioluminescent;

		// Token: 0x04001710 RID: 5904
		public float BioluminescentRadius = 2f;
	}
}
