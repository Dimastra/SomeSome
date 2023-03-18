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
	// Token: 0x0200027F RID: 639
	[DataDefinition]
	public sealed class CableTerminalNode : CableDeviceNode
	{
		// Token: 0x06000CC1 RID: 3265 RVA: 0x0004318B File Offset: 0x0004138B
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
			Vector2i vector2i = grid.TileIndicesFor(xform.Coordinates);
			Direction dir = xform.LocalRotation.GetDir();
			Vector2i targetIdx = DirectionExtensions.Offset(vector2i, dir);
			foreach (Node node in NodeHelpers.GetNodesInTile(nodeQuery, grid, targetIdx))
			{
				if (node is CableTerminalPortNode)
				{
					yield return node;
				}
			}
			IEnumerator<Node> enumerator = null;
			foreach (Node node2 in base.GetReachableNodes(xform, nodeQuery, xformQuery, grid, entMan))
			{
				yield return node2;
			}
			enumerator = null;
			yield break;
			yield break;
		}
	}
}
