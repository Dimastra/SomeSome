using System;
using Robust.Shared.GameObjects;

namespace Content.Server.White.Mindshield
{
	// Token: 0x02000095 RID: 149
	public class MindShieldImplanted
	{
		// Token: 0x06000251 RID: 593 RVA: 0x0000CA67 File Offset: 0x0000AC67
		public MindShieldImplanted(EntityUid target, EntityUid mindShield)
		{
			this.Target = target;
			this.MindShield = mindShield;
		}

		// Token: 0x040001AA RID: 426
		public EntityUid Target;

		// Token: 0x040001AB RID: 427
		public EntityUid MindShield;
	}
}
