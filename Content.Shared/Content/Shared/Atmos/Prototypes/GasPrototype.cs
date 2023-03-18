using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Atmos.Prototypes
{
	// Token: 0x02000699 RID: 1689
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("gas", 1)]
	public sealed class GasPrototype : IPrototype
	{
		// Token: 0x17000417 RID: 1047
		// (get) Token: 0x060014A1 RID: 5281 RVA: 0x00044C15 File Offset: 0x00042E15
		// (set) Token: 0x060014A2 RID: 5282 RVA: 0x00044C1D File Offset: 0x00042E1D
		[DataField("name", false, 1, false, false, null)]
		public string Name { get; set; } = "";

		// Token: 0x17000418 RID: 1048
		// (get) Token: 0x060014A3 RID: 5283 RVA: 0x00044C26 File Offset: 0x00042E26
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000419 RID: 1049
		// (get) Token: 0x060014A4 RID: 5284 RVA: 0x00044C2E File Offset: 0x00042E2E
		// (set) Token: 0x060014A5 RID: 5285 RVA: 0x00044C36 File Offset: 0x00042E36
		[DataField("specificHeat", false, 1, false, false, null)]
		public float SpecificHeat { get; private set; }

		// Token: 0x1700041A RID: 1050
		// (get) Token: 0x060014A6 RID: 5286 RVA: 0x00044C3F File Offset: 0x00042E3F
		// (set) Token: 0x060014A7 RID: 5287 RVA: 0x00044C47 File Offset: 0x00042E47
		[DataField("heatCapacityRatio", false, 1, false, false, null)]
		public float HeatCapacityRatio { get; private set; } = 1.4f;

		// Token: 0x1700041B RID: 1051
		// (get) Token: 0x060014A8 RID: 5288 RVA: 0x00044C50 File Offset: 0x00042E50
		// (set) Token: 0x060014A9 RID: 5289 RVA: 0x00044C58 File Offset: 0x00042E58
		[DataField("molarMass", false, 1, false, false, null)]
		public float MolarMass { get; set; } = 1f;

		// Token: 0x1700041C RID: 1052
		// (get) Token: 0x060014AA RID: 5290 RVA: 0x00044C61 File Offset: 0x00042E61
		[DataField("gasMolesVisible", false, 1, false, false, null)]
		public float GasMolesVisible { get; } = 0.25f;

		// Token: 0x1700041D RID: 1053
		// (get) Token: 0x060014AB RID: 5291 RVA: 0x00044C69 File Offset: 0x00042E69
		public float GasMolesVisibleMax
		{
			get
			{
				return this.GasMolesVisible * this.GasVisibilityFactor;
			}
		}

		// Token: 0x1700041E RID: 1054
		// (get) Token: 0x060014AC RID: 5292 RVA: 0x00044C78 File Offset: 0x00042E78
		[DataField("gasOverlayTexture", false, 1, false, false, null)]
		public string GasOverlayTexture { get; } = string.Empty;

		// Token: 0x1700041F RID: 1055
		// (get) Token: 0x060014AD RID: 5293 RVA: 0x00044C80 File Offset: 0x00042E80
		// (set) Token: 0x060014AE RID: 5294 RVA: 0x00044C88 File Offset: 0x00042E88
		[DataField("gasOverlayState", false, 1, false, false, null)]
		public string GasOverlayState { get; set; } = string.Empty;

		// Token: 0x17000420 RID: 1056
		// (get) Token: 0x060014AF RID: 5295 RVA: 0x00044C91 File Offset: 0x00042E91
		// (set) Token: 0x060014B0 RID: 5296 RVA: 0x00044C99 File Offset: 0x00042E99
		[DataField("gasOverlaySprite", false, 1, false, false, null)]
		public string GasOverlaySprite { get; set; } = string.Empty;

		// Token: 0x17000421 RID: 1057
		// (get) Token: 0x060014B1 RID: 5297 RVA: 0x00044CA2 File Offset: 0x00042EA2
		[DataField("overlayPath", false, 1, false, false, null)]
		public string OverlayPath { get; } = string.Empty;

		// Token: 0x17000422 RID: 1058
		// (get) Token: 0x060014B2 RID: 5298 RVA: 0x00044CAA File Offset: 0x00042EAA
		[Nullable(2)]
		[DataField("reagent", false, 1, false, false, typeof(PrototypeIdSerializer<ReagentPrototype>))]
		public string Reagent { [NullableContext(2)] get; }

		// Token: 0x17000423 RID: 1059
		// (get) Token: 0x060014B3 RID: 5299 RVA: 0x00044CB2 File Offset: 0x00042EB2
		[DataField("color", false, 1, false, false, null)]
		public string Color { get; } = string.Empty;

		// Token: 0x17000424 RID: 1060
		// (get) Token: 0x060014B4 RID: 5300 RVA: 0x00044CBA File Offset: 0x00042EBA
		// (set) Token: 0x060014B5 RID: 5301 RVA: 0x00044CC2 File Offset: 0x00042EC2
		[DataField("pricePerMole", false, 1, false, false, null)]
		public float PricePerMole { get; set; }

		// Token: 0x0400149F RID: 5279
		[DataField("gasVisbilityFactor", false, 1, false, false, null)]
		public float GasVisibilityFactor = 20f;
	}
}
