using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Emag.Components
{
	// Token: 0x020004C9 RID: 1225
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class EmagComponentState : ComponentState
	{
		// Token: 0x06000ED8 RID: 3800 RVA: 0x0002FCFA File Offset: 0x0002DEFA
		public EmagComponentState(int maxCharges, int charges, TimeSpan rechargeTime, TimeSpan nextChargeTime, string emagImmuneTag, bool autoRecharge)
		{
			this.MaxCharges = maxCharges;
			this.Charges = charges;
			this.RechargeTime = rechargeTime;
			this.NextChargeTime = nextChargeTime;
			this.EmagImmuneTag = emagImmuneTag;
			this.AutoRecharge = autoRecharge;
		}

		// Token: 0x04000DEB RID: 3563
		public int MaxCharges;

		// Token: 0x04000DEC RID: 3564
		public int Charges;

		// Token: 0x04000DED RID: 3565
		public bool AutoRecharge;

		// Token: 0x04000DEE RID: 3566
		public TimeSpan RechargeTime;

		// Token: 0x04000DEF RID: 3567
		public TimeSpan NextChargeTime;

		// Token: 0x04000DF0 RID: 3568
		public string EmagImmuneTag;
	}
}
