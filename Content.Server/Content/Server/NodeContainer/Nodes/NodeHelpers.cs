using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Server.NodeContainer.Nodes
{
	// Token: 0x0200037B RID: 891
	[NullableContext(1)]
	[Nullable(0)]
	public static class NodeHelpers
	{
		// Token: 0x06001233 RID: 4659 RVA: 0x0005EECF File Offset: 0x0005D0CF
		public static IEnumerable<Node> GetNodesInTile([Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<NodeContainerComponent> nodeQuery, MapGridComponent grid, Vector2i coords)
		{
			foreach (EntityUid entityUid in grid.GetAnchoredEntities(coords))
			{
				NodeContainerComponent container;
				if (nodeQuery.TryGetComponent(entityUid, ref container))
				{
					foreach (Node node in container.Nodes.Values)
					{
						yield return node;
					}
					Dictionary<string, Node>.ValueCollection.Enumerator enumerator2 = default(Dictionary<string, Node>.ValueCollection.Enumerator);
				}
			}
			IEnumerator<EntityUid> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06001234 RID: 4660 RVA: 0x0005EEED File Offset: 0x0005D0ED
		[return: TupleElementNames(new string[]
		{
			"dir",
			"node"
		})]
		[return: Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		public static IEnumerable<ValueTuple<Direction, Node>> GetCardinalNeighborNodes([Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<NodeContainerComponent> nodeQuery, MapGridComponent grid, Vector2i coords, bool includeSameTile = true)
		{
			foreach (ValueTuple<Direction, EntityUid> valueTuple in NodeHelpers.GetCardinalNeighborCells(grid, coords, includeSameTile))
			{
				Direction dir = valueTuple.Item1;
				EntityUid entityUid = valueTuple.Item2;
				NodeContainerComponent container;
				if (nodeQuery.TryGetComponent(entityUid, ref container))
				{
					foreach (Node node in container.Nodes.Values)
					{
						yield return new ValueTuple<Direction, Node>(dir, node);
					}
					Dictionary<string, Node>.ValueCollection.Enumerator enumerator2 = default(Dictionary<string, Node>.ValueCollection.Enumerator);
				}
			}
			IEnumerator<ValueTuple<Direction, EntityUid>> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06001235 RID: 4661 RVA: 0x0005EF12 File Offset: 0x0005D112
		[return: TupleElementNames(new string[]
		{
			"dir",
			"entity"
		})]
		[return: Nullable(new byte[]
		{
			1,
			0
		})]
		public static IEnumerable<ValueTuple<Direction, EntityUid>> GetCardinalNeighborCells(MapGridComponent grid, Vector2i coords, bool includeSameTile = true)
		{
			IEnumerator<EntityUid> enumerator;
			if (includeSameTile)
			{
				foreach (EntityUid uid in grid.GetAnchoredEntities(coords))
				{
					yield return new ValueTuple<Direction, EntityUid>(-1, uid);
				}
				enumerator = null;
			}
			foreach (EntityUid uid2 in grid.GetAnchoredEntities(coords + new ValueTuple<int, int>(0, 1)))
			{
				yield return new ValueTuple<Direction, EntityUid>(4, uid2);
			}
			enumerator = null;
			foreach (EntityUid uid3 in grid.GetAnchoredEntities(coords + new ValueTuple<int, int>(0, -1)))
			{
				yield return new ValueTuple<Direction, EntityUid>(0, uid3);
			}
			enumerator = null;
			foreach (EntityUid uid4 in grid.GetAnchoredEntities(coords + new ValueTuple<int, int>(1, 0)))
			{
				yield return new ValueTuple<Direction, EntityUid>(2, uid4);
			}
			enumerator = null;
			foreach (EntityUid uid5 in grid.GetAnchoredEntities(coords + new ValueTuple<int, int>(-1, 0)))
			{
				yield return new ValueTuple<Direction, EntityUid>(6, uid5);
			}
			enumerator = null;
			yield break;
			yield break;
		}
	}
}
