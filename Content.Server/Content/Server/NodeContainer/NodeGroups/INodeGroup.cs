using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.NodeContainer.Nodes;
using Robust.Shared.GameObjects;

namespace Content.Server.NodeContainer.NodeGroups
{
	// Token: 0x0200037F RID: 895
	[NullableContext(1)]
	public interface INodeGroup
	{
		// Token: 0x17000295 RID: 661
		// (get) Token: 0x06001251 RID: 4689
		bool Remaking { get; }

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x06001252 RID: 4690
		IReadOnlyList<Node> Nodes { get; }

		// Token: 0x06001253 RID: 4691
		void Create(NodeGroupID groupId);

		// Token: 0x06001254 RID: 4692
		void Initialize(Node sourceNode, IEntityManager entMan);

		// Token: 0x06001255 RID: 4693
		void RemoveNode(Node node);

		// Token: 0x06001256 RID: 4694
		void LoadNodes(List<Node> groupNodes);

		// Token: 0x06001257 RID: 4695
		void AfterRemake([Nullable(new byte[]
		{
			1,
			1,
			2,
			1
		})] IEnumerable<IGrouping<INodeGroup, Node>> newGroups);

		// Token: 0x06001258 RID: 4696
		[NullableContext(2)]
		string GetDebugData();
	}
}
