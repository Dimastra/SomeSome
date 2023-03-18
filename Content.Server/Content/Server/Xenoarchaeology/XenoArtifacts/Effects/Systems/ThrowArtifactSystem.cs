using System;
using System.Runtime.CompilerServices;
using Content.Server.Maps;
using Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Components;
using Content.Server.Xenoarchaeology.XenoArtifacts.Events;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Random;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Effects.Systems
{
	// Token: 0x0200004F RID: 79
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ThrowArtifactSystem : EntitySystem
	{
		// Token: 0x060000F6 RID: 246 RVA: 0x00006A2B File Offset: 0x00004C2B
		public override void Initialize()
		{
			base.SubscribeLocalEvent<ThrowArtifactComponent, ArtifactActivatedEvent>(new ComponentEventHandler<ThrowArtifactComponent, ArtifactActivatedEvent>(this.OnActivated), null, null);
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00006A44 File Offset: 0x00004C44
		private void OnActivated(EntityUid uid, ThrowArtifactComponent component, ArtifactActivatedEvent args)
		{
			TransformComponent xform = base.Transform(uid);
			MapGridComponent grid;
			if (this._map.TryGetGrid(xform.GridUid, ref grid))
			{
				foreach (TileRef tile in grid.GetTilesIntersecting(Box2.CenteredAround(xform.WorldPosition, new ValueTuple<float, float>(component.Range * 2f, component.Range)), true, null))
				{
					if (RandomExtensions.Prob(this._random, component.TilePryChance))
					{
						this._tile.PryTile(tile);
					}
				}
			}
			foreach (EntityUid ent in this._lookup.GetEntitiesInRange(uid, component.Range, 10))
			{
				Vector2 foo = base.Transform(ent).MapPosition.Position - xform.MapPosition.Position;
				this._throwing.TryThrow(ent, foo * 2f, component.ThrowStrength, new EntityUid?(uid), 0f, null, null, null, null);
			}
		}

		// Token: 0x040000B2 RID: 178
		[Dependency]
		private readonly IMapManager _map;

		// Token: 0x040000B3 RID: 179
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x040000B4 RID: 180
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x040000B5 RID: 181
		[Dependency]
		private readonly ThrowingSystem _throwing;

		// Token: 0x040000B6 RID: 182
		[Dependency]
		private readonly TileSystem _tile;
	}
}
