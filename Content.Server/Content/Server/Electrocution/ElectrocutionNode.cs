using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.Nodes;
using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Electrocution
{
	// Token: 0x02000534 RID: 1332
	[DataDefinition]
	public sealed class ElectrocutionNode : Node
	{
		// Token: 0x06001BCE RID: 7118 RVA: 0x00093EE3 File Offset: 0x000920E3
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
			NodeContainerComponent nodeContainer;
			if (!nodeQuery.TryGetComponent(this.CableEntity, ref nodeContainer))
			{
				yield break;
			}
			Node node;
			if (nodeContainer.TryGetNode<Node>(this.NodeName, out node))
			{
				yield return node;
			}
			yield break;
		}

		// Token: 0x040011D4 RID: 4564
		[DataField("cable", false, 1, false, false, null)]
		public EntityUid CableEntity;

		// Token: 0x040011D5 RID: 4565
		[Nullable(2)]
		[DataField("node", false, 1, false, false, null)]
		public string NodeName;
	}
}
