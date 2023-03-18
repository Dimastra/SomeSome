using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Destructible.Thresholds.Triggers
{
	// Token: 0x0200059E RID: 1438
	[DataDefinition]
	[Serializable]
	public sealed class DamageTrigger : IThresholdTrigger
	{
		// Token: 0x1700046B RID: 1131
		// (get) Token: 0x06001DF5 RID: 7669 RVA: 0x0009EEBF File Offset: 0x0009D0BF
		// (set) Token: 0x06001DF6 RID: 7670 RVA: 0x0009EEC7 File Offset: 0x0009D0C7
		[DataField("damage", false, 1, true, false, null)]
		public int Damage { get; set; }

		// Token: 0x06001DF7 RID: 7671 RVA: 0x0009EED0 File Offset: 0x0009D0D0
		[NullableContext(1)]
		public bool Reached(DamageableComponent damageable, DestructibleSystem system)
		{
			return damageable.TotalDamage >= this.Damage;
		}
	}
}
