using System;
using Content.Shared.Gravity;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Gravity
{
	// Token: 0x02000488 RID: 1160
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(GravityGeneratorSystem)
	})]
	public sealed class GravityGeneratorComponent : SharedGravityGeneratorComponent
	{
		// Token: 0x1700031B RID: 795
		// (get) Token: 0x06001729 RID: 5929 RVA: 0x00079D62 File Offset: 0x00077F62
		// (set) Token: 0x0600172A RID: 5930 RVA: 0x00079D6A File Offset: 0x00077F6A
		[ViewVariables]
		[DataField("chargeRate", false, 1, false, false, null)]
		public float ChargeRate { get; set; } = 0.01f;

		// Token: 0x1700031C RID: 796
		// (get) Token: 0x0600172B RID: 5931 RVA: 0x00079D73 File Offset: 0x00077F73
		// (set) Token: 0x0600172C RID: 5932 RVA: 0x00079D7B File Offset: 0x00077F7B
		[DataField("idlePower", false, 1, false, false, null)]
		public float IdlePowerUse { get; set; }

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x0600172D RID: 5933 RVA: 0x00079D84 File Offset: 0x00077F84
		// (set) Token: 0x0600172E RID: 5934 RVA: 0x00079D8C File Offset: 0x00077F8C
		[DataField("activePower", false, 1, false, false, null)]
		public float ActivePowerUse { get; set; }

		// Token: 0x1700031E RID: 798
		// (get) Token: 0x0600172F RID: 5935 RVA: 0x00079D95 File Offset: 0x00077F95
		// (set) Token: 0x06001730 RID: 5936 RVA: 0x00079D9D File Offset: 0x00077F9D
		[DataField("lightRadiusMin", false, 1, false, false, null)]
		public float LightRadiusMin { get; set; }

		// Token: 0x1700031F RID: 799
		// (get) Token: 0x06001731 RID: 5937 RVA: 0x00079DA6 File Offset: 0x00077FA6
		// (set) Token: 0x06001732 RID: 5938 RVA: 0x00079DAE File Offset: 0x00077FAE
		[DataField("lightRadiusMax", false, 1, false, false, null)]
		public float LightRadiusMax { get; set; }

		// Token: 0x17000320 RID: 800
		// (get) Token: 0x06001733 RID: 5939 RVA: 0x00079DB7 File Offset: 0x00077FB7
		// (set) Token: 0x06001734 RID: 5940 RVA: 0x00079DBF File Offset: 0x00077FBF
		[DataField("switchedOn", false, 1, false, false, null)]
		public bool SwitchedOn { get; set; } = true;

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x06001735 RID: 5941 RVA: 0x00079DC8 File Offset: 0x00077FC8
		// (set) Token: 0x06001736 RID: 5942 RVA: 0x00079DD0 File Offset: 0x00077FD0
		[DataField("intact", false, 1, false, false, null)]
		public bool Intact { get; set; } = true;

		// Token: 0x17000322 RID: 802
		// (get) Token: 0x06001737 RID: 5943 RVA: 0x00079DD9 File Offset: 0x00077FD9
		// (set) Token: 0x06001738 RID: 5944 RVA: 0x00079DE1 File Offset: 0x00077FE1
		[ViewVariables]
		[DataField("charge", false, 1, false, false, null)]
		public float Charge { get; set; } = 1f;

		// Token: 0x17000323 RID: 803
		// (get) Token: 0x06001739 RID: 5945 RVA: 0x00079DEA File Offset: 0x00077FEA
		// (set) Token: 0x0600173A RID: 5946 RVA: 0x00079DF2 File Offset: 0x00077FF2
		[ViewVariables]
		public bool GravityActive { get; set; }

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x0600173B RID: 5947 RVA: 0x00079DFB File Offset: 0x00077FFB
		// (set) Token: 0x0600173C RID: 5948 RVA: 0x00079E03 File Offset: 0x00078003
		[ViewVariables]
		public bool NeedUIUpdate { get; set; }
	}
}
