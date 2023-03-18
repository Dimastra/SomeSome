using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.NodeContainer.Nodes;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.NodeContainer.NodeGroups
{
	// Token: 0x02000380 RID: 896
	[NullableContext(1)]
	[Nullable(0)]
	[NodeGroup(new NodeGroupID[]
	{
		NodeGroupID.Default,
		NodeGroupID.WireNet
	})]
	[Virtual]
	public class BaseNodeGroup : INodeGroup
	{
		// Token: 0x17000297 RID: 663
		// (get) Token: 0x06001259 RID: 4697 RVA: 0x0005F2A1 File Offset: 0x0005D4A1
		// (set) Token: 0x0600125A RID: 4698 RVA: 0x0005F2A9 File Offset: 0x0005D4A9
		public bool Remaking { get; set; }

		// Token: 0x17000298 RID: 664
		// (get) Token: 0x0600125B RID: 4699 RVA: 0x0005F2B2 File Offset: 0x0005D4B2
		IReadOnlyList<Node> INodeGroup.Nodes
		{
			get
			{
				return this.Nodes;
			}
		}

		// Token: 0x17000299 RID: 665
		// (get) Token: 0x0600125C RID: 4700 RVA: 0x0005F2BA File Offset: 0x0005D4BA
		[ViewVariables]
		public int NodeCount
		{
			get
			{
				return this.Nodes.Count;
			}
		}

		// Token: 0x1700029A RID: 666
		// (get) Token: 0x0600125D RID: 4701 RVA: 0x0005F2C7 File Offset: 0x0005D4C7
		// (set) Token: 0x0600125E RID: 4702 RVA: 0x0005F2CF File Offset: 0x0005D4CF
		[ViewVariables]
		public bool Removed { get; set; }

		// Token: 0x1700029B RID: 667
		// (get) Token: 0x0600125F RID: 4703 RVA: 0x0005F2D8 File Offset: 0x0005D4D8
		// (set) Token: 0x06001260 RID: 4704 RVA: 0x0005F2E0 File Offset: 0x0005D4E0
		[ViewVariables]
		public NodeGroupID GroupId { get; private set; }

		// Token: 0x06001261 RID: 4705 RVA: 0x0005F2E9 File Offset: 0x0005D4E9
		public void Create(NodeGroupID groupId)
		{
			this.GroupId = groupId;
		}

		// Token: 0x06001262 RID: 4706 RVA: 0x0005F2F2 File Offset: 0x0005D4F2
		public virtual void Initialize(Node sourceNode, IEntityManager entMan)
		{
		}

		// Token: 0x06001263 RID: 4707 RVA: 0x0005F2F4 File Offset: 0x0005D4F4
		public virtual void RemoveNode(Node node)
		{
		}

		// Token: 0x06001264 RID: 4708 RVA: 0x0005F2F6 File Offset: 0x0005D4F6
		public virtual void LoadNodes(List<Node> groupNodes)
		{
			this.Nodes.AddRange(groupNodes);
		}

		// Token: 0x06001265 RID: 4709 RVA: 0x0005F304 File Offset: 0x0005D504
		public virtual void AfterRemake([Nullable(new byte[]
		{
			1,
			1,
			2,
			1
		})] IEnumerable<IGrouping<INodeGroup, Node>> newGroups)
		{
		}

		// Token: 0x06001266 RID: 4710 RVA: 0x0005F306 File Offset: 0x0005D506
		[NullableContext(2)]
		public virtual string GetDebugData()
		{
			return null;
		}

		// Token: 0x04000B45 RID: 2885
		[ViewVariables]
		public readonly List<Node> Nodes = new List<Node>();

		// Token: 0x04000B47 RID: 2887
		[ViewVariables]
		public int NetId;
	}
}
