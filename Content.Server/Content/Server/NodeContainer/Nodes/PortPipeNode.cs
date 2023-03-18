using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.NodeContainer.Nodes
{
	// Token: 0x0200037E RID: 894
	[DataDefinition]
	public sealed class PortPipeNode : PipeNode
	{
		// Token: 0x0600124E RID: 4686 RVA: 0x0005F255 File Offset: 0x0005D455
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
				if (node is PortablePipeNode)
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
