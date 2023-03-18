using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Systems
{
	// Token: 0x02000045 RID: 69
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class IgniteArtifactSystem : EntitySystem
	{
		// Token: 0x060000D7 RID: 215 RVA: 0x00006297 File Offset: 0x00004497
		public override void Initialize()
		{
			base.SubscribeLocalEvent<IgniteArtifactComponent, ArtifactActivatedEvent>(new ComponentEventHandler<IgniteArtifactComponent, ArtifactActivatedEvent>(this.OnActivate), null, null);
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x000062B0 File Offset: 0x000044B0
		private void OnActivate(EntityUid uid, IgniteArtifactComponent component, ArtifactActivatedEvent args)
		{
			EntityQuery<FlammableComponent> flammable = base.GetEntityQuery<FlammableComponent>();
			HashSet<EntityUid> targets = new HashSet<EntityUid>();
			if (args.Activator != null)
			{
				targets.Add(args.Activator.Value);
			}
			else
			{
				targets = this._lookup.GetEntitiesInRange(uid, component.Range, 46);
			}
			foreach (EntityUid target in targets)
			{
				FlammableComponent fl;
				if (flammable.TryGetComponent(target, ref fl))
				{
					fl.FireStacks += (float)this._random.Next(component.MinFireStack, component.MaxFireStack);
					this._flammable.Ignite(target, fl);
				}
			}
		}

		// Token: 0x0400009B RID: 155
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400009C RID: 156
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x0400009D RID: 157
		[Dependency]
		private readonly FlammableSystem _flammable;
	}
}
