using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NodeContainer.Nodes
{
	// Token: 0x02000378 RID: 888
	[DataDefinition]
	public sealed class AdjacentNode : Node
	{
		// Token: 0x06001226 RID: 4646 RVA: 0x0005EE15 File Offset: 0x0005D015
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
			foreach (ValueTuple<Direction, Node> valueTuple in NodeHelpers.GetCardinalNeighborNodes(nodeQuery, grid, gridIndex, true))
			{
				Node node = valueTuple.Item2;
				if (node != this)
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
