using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts.Events;
using Content.Shared.Damage;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Systems
{
	// Token: 0x02000041 RID: 65
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BreakWindowArtifactSystem : EntitySystem
	{
		// Token: 0x060000C8 RID: 200 RVA: 0x00005E10 File Offset: 0x00004010
		public override void Initialize()
		{
			base.SubscribeLocalEvent<DamageNearbyArtifactComponent, ArtifactActivatedEvent>(new ComponentEventHandler<DamageNearbyArtifactComponent, ArtifactActivatedEvent>(this.OnActivated), null, null);
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00005E28 File Offset: 0x00004028
		private void OnActivated(EntityUid uid, DamageNearbyArtifactComponent component, ArtifactActivatedEvent args)
		{
			HashSet<EntityUid> ents = this._lookup.GetEntitiesInRange(uid, component.Radius, 46);
			if (args.Activator != null)
			{
				ents.Add(args.Activator.Value);
			}
			foreach (EntityUid ent in ents)
			{
				if (component.Whitelist == null || component.Whitelist.IsValid(ent, null))
				{
					if (!RandomExtensions.Prob(this._random, component.DamageChance))
					{
						break;
					}
					this._damageable.TryChangeDamage(new EntityUid?(ent), component.Damage, component.IgnoreResistances, true, null, null);
				}
			}
		}

		// Token: 0x04000092 RID: 146
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000093 RID: 147
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x04000094 RID: 148
		[Dependency]
		private readonly DamageableSystem _damageable;
	}
}
