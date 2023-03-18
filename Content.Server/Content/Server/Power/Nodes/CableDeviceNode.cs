using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.Nodes;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Power.Nodes
{
	// Token: 0x0200027D RID: 637
	[DataDefinition]
	[Virtual]
	public class CableDeviceNode : Node
	{
		// Token: 0x06000CBD RID: 3261 RVA: 0x0004312F File Offset: 0x0004132F
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
			foreach (Node node in NodeHelpers.GetNodesInTile(nodeQuery, grid, gridIndex))
			{
				if (node is CableNode)
				{
					yield return node;
				}
			}
			IEnumerator<Node> enumerator = null;
			yield break;
			yield break;
		}
	}
}
