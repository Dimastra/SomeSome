using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts.Events;
using Content.Shared.Mobs.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Random;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Systems
{
	// Token: 0x0200004B RID: 75
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ShuffleArtifactSystem : EntitySystem
	{
		// Token: 0x060000E9 RID: 233 RVA: 0x000065E5 File Offset: 0x000047E5
		public override void Initialize()
		{
			base.SubscribeLocalEvent<ShuffleArtifactComponent, ArtifactActivatedEvent>(new ComponentEventHandler<ShuffleArtifactComponent, ArtifactActivatedEvent>(this.OnActivated), null, null);
		}

		// Token: 0x060000EA RID: 234 RVA: 0x000065FC File Offset: 0x000047FC
		private void OnActivated(EntityUid uid, ShuffleArtifactComponent component, ArtifactActivatedEvent args)
		{
			EntityQuery<MobStateComponent> mobState = base.GetEntityQuery<MobStateComponent>();
			List<EntityCoordinates> allCoords = new List<EntityCoordinates>();
			List<TransformComponent> toShuffle = new List<TransformComponent>();
			foreach (EntityUid ent in this._lookup.GetEntitiesInRange(uid, component.Radius, 10))
			{
				if (mobState.HasComponent(ent))
				{
					TransformComponent xform = base.Transform(ent);
					toShuffle.Add(xform);
					allCoords.Add(xform.Coordinates);
				}
			}
			foreach (TransformComponent xform2 in toShuffle)
			{
				EntityUid xformUid = xform2.Owner;
				this._xform.SetCoordinates(xformUid, xform2, RandomExtensions.PickAndTake<EntityCoordinates>(this._random, allCoords), null, true, null, null);
			}
		}

		// Token: 0x040000A7 RID: 167
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x040000A8 RID: 168
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040000A9 RID: 169
		[Dependency]
		private readonly SharedTransformSystem _xform;
	}
}
