using System;
using Content.Server.Body.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Body.Components
{
	// Token: 0x02000719 RID: 1817
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(ThermalRegulatorSystem)
	})]
	public sealed class ThermalRegulatorComponent : Component
	{
		// Token: 0x170005A4 RID: 1444
		// (get) Token: 0x06002630 RID: 9776 RVA: 0x000C9828 File Offset: 0x000C7A28
		// (set) Token: 0x06002631 RID: 9777 RVA: 0x000C9830 File Offset: 0x000C7A30
		[DataField("metabolismHeat", false, 1, false, false, null)]
		public float MetabolismHeat { get; private set; }

		// Token: 0x170005A5 RID: 1445
		// (get) Token: 0x06002632 RID: 9778 RVA: 0x000C9839 File Offset: 0x000C7A39
		// (set) Token: 0x06002633 RID: 9779 RVA: 0x000C9841 File Offset: 0x000C7A41
		[DataField("radiatedHeat", false, 1, false, false, null)]
		public float RadiatedHeat { get; private set; }

		// Token: 0x170005A6 RID: 1446
		// (get) Token: 0x06002634 RID: 9780 RVA: 0x000C984A File Offset: 0x000C7A4A
		// (set) Token: 0x06002635 RID: 9781 RVA: 0x000C9852 File Offset: 0x000C7A52
		[DataField("sweatHeatRegulation", false, 1, false, false, null)]
		public float SweatHeatRegulation { get; private set; }

		// Token: 0x170005A7 RID: 1447
		// (get) Token: 0x06002636 RID: 9782 RVA: 0x000C985B File Offset: 0x000C7A5B
		// (set) Token: 0x06002637 RID: 9783 RVA: 0x000C9863 File Offset: 0x000C7A63
		[DataField("shiveringHeatRegulation", false, 1, false, false, null)]
		public float ShiveringHeatRegulation { get; private set; }

		// Token: 0x170005A8 RID: 1448
		// (get) Token: 0x06002638 RID: 9784 RVA: 0x000C986C File Offset: 0x000C7A6C
		// (set) Token: 0x06002639 RID: 9785 RVA: 0x000C9874 File Offset: 0x000C7A74
		[DataField("implicitHeatRegulation", false, 1, false, false, null)]
		public float ImplicitHeatRegulation { get; private set; }

		// Token: 0x170005A9 RID: 1449
		// (get) Token: 0x0600263A RID: 9786 RVA: 0x000C987D File Offset: 0x000C7A7D
		// (set) Token: 0x0600263B RID: 9787 RVA: 0x000C9885 File Offset: 0x000C7A85
		[DataField("normalBodyTemperature", false, 1, false, false, null)]
		public float NormalBodyTemperature { get; private set; }

		// Token: 0x170005AA RID: 1450
		// (get) Token: 0x0600263C RID: 9788 RVA: 0x000C988E File Offset: 0x000C7A8E
		// (set) Token: 0x0600263D RID: 9789 RVA: 0x000C9896 File Offset: 0x000C7A96
		[DataField("thermalRegulationTemperatureThreshold", false, 1, false, false, null)]
		public float ThermalRegulationTemperatureThreshold { get; private set; }

		// Token: 0x040017C7 RID: 6087
		public float AccumulatedFrametime;
	}
}
