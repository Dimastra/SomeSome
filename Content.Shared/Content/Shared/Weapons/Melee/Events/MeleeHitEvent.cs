using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;

namespace Content.Shared.Weapons.Melee.Events
{
	// Token: 0x02000076 RID: 118
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MeleeHitEvent : HandledEntityEventArgs
	{
		// Token: 0x06000175 RID: 373 RVA: 0x00008A1A File Offset: 0x00006C1A
		public MeleeHitEvent(List<EntityUid> hitEntities, EntityUid user, DamageSpecifier baseDamage)
		{
			this.HitEntities = hitEntities;
			this.User = user;
			this.BaseDamage = baseDamage;
		}

		// Token: 0x0400018E RID: 398
		public readonly DamageSpecifier BaseDamage;

		// Token: 0x0400018F RID: 399
		public List<DamageModifierSet> ModifiersList = new List<DamageModifierSet>();

		// Token: 0x04000190 RID: 400
		public DamageSpecifier BonusDamage = new DamageSpecifier();

		// Token: 0x04000191 RID: 401
		public IReadOnlyList<EntityUid> HitEntities;

		// Token: 0x04000192 RID: 402
		[Nullable(2)]
		public SoundSpecifier HitSoundOverride;

		// Token: 0x04000193 RID: 403
		public readonly EntityUid User;

		// Token: 0x04000194 RID: 404
		public bool IsHit = true;
	}
}
