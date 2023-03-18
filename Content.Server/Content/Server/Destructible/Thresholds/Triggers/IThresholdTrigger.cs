using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;

namespace Content.Server.Destructible.Thresholds.Triggers
{
	// Token: 0x020005A0 RID: 1440
	[NullableContext(1)]
	public interface IThresholdTrigger
	{
		// Token: 0x06001DFF RID: 7679
		bool Reached(DamageableComponent damageable, DestructibleSystem system);
	}
}
