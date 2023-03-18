using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Destructible.Thresholds.Triggers
{
	// Token: 0x020005A1 RID: 1441
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	[Serializable]
	public sealed class OrTrigger : IThresholdTrigger
	{
		// Token: 0x1700046E RID: 1134
		// (get) Token: 0x06001E00 RID: 7680 RVA: 0x0009EF4D File Offset: 0x0009D14D
		[DataField("triggers", false, 1, false, false, null)]
		public List<IThresholdTrigger> Triggers { get; } = new List<IThresholdTrigger>();

		// Token: 0x06001E01 RID: 7681 RVA: 0x0009EF58 File Offset: 0x0009D158
		public bool Reached(DamageableComponent damageable, DestructibleSystem system)
		{
			using (List<IThresholdTrigger>.Enumerator enumerator = this.Triggers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Reached(damageable, system))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
