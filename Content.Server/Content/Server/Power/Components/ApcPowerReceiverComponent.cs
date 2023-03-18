using System;
using System.Runtime.CompilerServices;
using Content.Server.Power.Pow3r;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Power.Components
{
	// Token: 0x020002A2 RID: 674
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class ApcPowerReceiverComponent : Component
	{
		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x06000DAB RID: 3499 RVA: 0x0004756E File Offset: 0x0004576E
		[ViewVariables]
		public bool Powered
		{
			get
			{
				return (MathHelper.CloseToPercent(this.NetworkLoad.ReceivingPower, this.Load, 1E-05) || !this.NeedsPower) && !this.PowerDisabled;
			}
		}

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x06000DAC RID: 3500 RVA: 0x000475A4 File Offset: 0x000457A4
		// (set) Token: 0x06000DAD RID: 3501 RVA: 0x000475B1 File Offset: 0x000457B1
		[ViewVariables]
		[DataField("powerLoad", false, 1, false, false, null)]
		public float Load
		{
			get
			{
				return this.NetworkLoad.DesiredPower;
			}
			set
			{
				this.NetworkLoad.DesiredPower = value;
			}
		}

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x06000DAE RID: 3502 RVA: 0x000475BF File Offset: 0x000457BF
		// (set) Token: 0x06000DAF RID: 3503 RVA: 0x000475C7 File Offset: 0x000457C7
		[ViewVariables]
		public bool NeedsPower
		{
			get
			{
				return this._needsPower;
			}
			set
			{
				this._needsPower = value;
				this.PoweredLastUpdate = null;
			}
		}

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x06000DB0 RID: 3504 RVA: 0x000475DC File Offset: 0x000457DC
		// (set) Token: 0x06000DB1 RID: 3505 RVA: 0x000475EC File Offset: 0x000457EC
		[ViewVariables]
		[DataField("powerDisabled", false, 1, false, false, null)]
		public bool PowerDisabled
		{
			get
			{
				return !this.NetworkLoad.Enabled;
			}
			set
			{
				this.NetworkLoad.Enabled = !value;
			}
		}

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x06000DB2 RID: 3506 RVA: 0x000475FD File Offset: 0x000457FD
		[ViewVariables]
		public PowerState.Load NetworkLoad { get; } = new PowerState.Load
		{
			DesiredPower = 5f
		};

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x06000DB3 RID: 3507 RVA: 0x00047605 File Offset: 0x00045805
		public float PowerReceived
		{
			get
			{
				return this.NetworkLoad.ReceivingPower;
			}
		}

		// Token: 0x06000DB4 RID: 3508 RVA: 0x00047612 File Offset: 0x00045812
		protected override void OnRemove()
		{
			ApcPowerProviderComponent provider = this.Provider;
			if (provider != null)
			{
				provider.RemoveReceiver(this);
			}
			base.OnRemove();
		}

		// Token: 0x0400081D RID: 2077
		[Nullable(2)]
		public ApcPowerProviderComponent Provider;

		// Token: 0x0400081E RID: 2078
		[DataField("needsPower", false, 1, false, false, null)]
		private bool _needsPower = true;

		// Token: 0x0400081F RID: 2079
		public bool? PoweredLastUpdate;
	}
}
