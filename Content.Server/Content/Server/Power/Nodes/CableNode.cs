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
	// Token: 0x0200027E RID: 638
	[DataDefinition]
	public sealed class CableNode : Node
	{
		// Token: 0x06000CBF RID: 3263 RVA: 0x00043156 File Offset: 0x00041356
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
			int terminalDirs = 0;
			List<ValueTuple<Direction, Node>> nodeDirs = new List<ValueTuple<Direction, Node>>();
			foreach (ValueTuple<Direction, Node> valueTuple in NodeHelpers.GetCardinalNeighborNodes(nodeQuery, grid, gridIndex, true))
			{
				Direction dir = valueTuple.Item1;
				Node node = valueTuple.Item2;
				if (node is CableNode && node != this)
				{
					nodeDirs.Add(new ValueTuple<Direction, Node>(dir, node));
				}
				if (node is CableDeviceNode && dir == -1)
				{
					nodeDirs.Add(new ValueTuple<Direction, Node>(-1, node));
				}
				if (node is CableTerminalNode)
				{
					if (dir == -1)
					{
						terminalDirs |= 1 << xformQuery.GetComponent(node.Owner).LocalRotation.GetCardinalDir();
					}
					else if (DirectionExtensions.GetOpposite(xformQuery.GetComponent(node.Owner).LocalRotation.GetCardinalDir()) == dir)
					{
						terminalDirs |= 1 << dir;
						break;
					}
				}
			}
			foreach (ValueTuple<Direction, Node> valueTuple2 in nodeDirs)
			{
				Direction dir2 = valueTuple2.Item1;
				Node node2 = valueTuple2.Item2;
				if (dir2 == -1 || (terminalDirs & 1 << dir2) == 0)
				{
					yield return node2;
				}
			}
			List<ValueTuple<Direction, Node>>.Enumerator enumerator2 = default(List<ValueTuple<Direction, Node>>.Enumerator);
			yield break;
			yield break;
		}
	}
}
