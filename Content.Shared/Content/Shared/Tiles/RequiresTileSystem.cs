using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;

namespace Content.Shared.Tiles
{
	// Token: 0x020000CC RID: 204
	public sealed class RequiresTileSystem : EntitySystem
	{
		// Token: 0x06000230 RID: 560 RVA: 0x0000AFF2 File Offset: 0x000091F2
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<TileChangedEvent>(new EntityEventRefHandler<TileChangedEvent>(this.OnTileChange), null, null);
		}

		// Token: 0x06000231 RID: 561 RVA: 0x0000B010 File Offset: 0x00009210
		private void OnTileChange(ref TileChangedEvent ev)
		{
			MapGridComponent grid;
			if (!base.TryComp<MapGridComponent>(ev.Entity, ref grid))
			{
				return;
			}
			AnchoredEntitiesEnumerator anchored = grid.GetAnchoredEntitiesEnumerator(ev.NewTile.GridIndices);
			if (anchored.Equals(AnchoredEntitiesEnumerator.Empty))
			{
				return;
			}
			EntityQuery<RequiresTileComponent> query = base.GetEntityQuery<RequiresTileComponent>();
			EntityUid? ent;
			while (anchored.MoveNext(ref ent))
			{
				if (query.HasComponent(ent.Value))
				{
					base.QueueDel(ent.Value);
				}
			}
		}
	}
}
