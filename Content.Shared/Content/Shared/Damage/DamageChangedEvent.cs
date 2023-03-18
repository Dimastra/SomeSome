using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;

namespace Content.Shared.Damage
{
	// Token: 0x02000536 RID: 1334
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DamageChangedEvent : EntityEventArgs
	{
		// Token: 0x0600103E RID: 4158 RVA: 0x00034D80 File Offset: 0x00032F80
		public DamageChangedEvent(DamageableComponent damageable, [Nullable(2)] DamageSpecifier damageDelta, bool interruptsDoAfters, EntityUid? origin)
		{
			this.Damageable = damageable;
			this.DamageDelta = damageDelta;
			this.Origin = origin;
			if (this.DamageDelta == null)
			{
				return;
			}
			using (Dictionary<string, FixedPoint2>.ValueCollection.Enumerator enumerator = this.DamageDelta.DamageDict.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current > 0)
					{
						this.DamageIncreased = true;
						break;
					}
				}
			}
			this.InterruptsDoAfters = (interruptsDoAfters && this.DamageIncreased);
		}

		// Token: 0x04000F4A RID: 3914
		public readonly DamageableComponent Damageable;

		// Token: 0x04000F4B RID: 3915
		[Nullable(2)]
		public readonly DamageSpecifier DamageDelta;

		// Token: 0x04000F4C RID: 3916
		public readonly bool DamageIncreased;

		// Token: 0x04000F4D RID: 3917
		public readonly bool InterruptsDoAfters;

		// Token: 0x04000F4E RID: 3918
		public readonly EntityUid? Origin;
	}
}
