using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Destructible.Thresholds.Triggers
{
	// Token: 0x0200059C RID: 1436
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	[Serializable]
	public sealed class AndTrigger : IThresholdTrigger
	{
		// Token: 0x17000468 RID: 1128
		// (get) Token: 0x06001DEB RID: 7659 RVA: 0x0009EDF7 File Offset: 0x0009CFF7
		// (set) Token: 0x06001DEC RID: 7660 RVA: 0x0009EDFF File Offset: 0x0009CFFF
		[DataField("triggers", false, 1, false, false, null)]
		public List<IThresholdTrigger> Triggers { get; set; } = new List<IThresholdTrigger>();

		// Token: 0x06001DED RID: 7661 RVA: 0x0009EE08 File Offset: 0x0009D008
		public bool Reached(DamageableComponent damageable, DestructibleSystem system)
		{
			using (List<IThresholdTrigger>.Enumerator enumerator = this.Triggers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.Reached(damageable, system))
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
