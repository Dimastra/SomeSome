using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.Nodes;
using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Power.Nodes
{
	// Token: 0x02000280 RID: 640
	[DataDefinition]
	public sealed class CableTerminalPortNode : Node
	{
		// Token: 0x06000CC4 RID: 3268 RVA: 0x000431D7 File Offset: 0x000413D7
		[NullableContext(1)]
		public override IEnumerable<Node> GetReachableNodes(TransformComponent xform, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<NodeContainerComponent> nodeQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery, [Nullable(2)] MapGridComponent grid, IEntityManager entMan)
		{
			if (!xform.Anchored || grid == null)
			{
				yield break;
			}
			Vector2i gridIndex = grid.TileIndicesFor(xform.Coordinates);
			IEnumerable<ValueTuple<Direction, Node>> nodes = NodeHelpers.GetCardinalNeighborNodes(nodeQuery, grid, gridIndex, false);
			foreach (ValueTuple<Direction, Node> valueTuple in nodes)
			{
				Node node = valueTuple.Item2;
				if (node is CableTerminalNode)
				{
					yield return node;
				}
			}
			IEnumerator<ValueTuple<Direction, Node>> enumerator = null;
			yield break;
			yield break;
		}
	}
}
