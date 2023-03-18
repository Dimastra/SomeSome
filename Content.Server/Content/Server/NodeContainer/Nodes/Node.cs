using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.NodeContainer.NodeGroups;
using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.NodeContainer.Nodes
{
	// Token: 0x0200037A RID: 890
	[NullableContext(1)]
	[Nullable(0)]
	[ImplicitDataDefinitionForInheritors]
	public abstract class Node
	{
		// Token: 0x1700028C RID: 652
		// (get) Token: 0x06001229 RID: 4649 RVA: 0x0005EE43 File Offset: 0x0005D043
		// (set) Token: 0x0600122A RID: 4650 RVA: 0x0005EE4B File Offset: 0x0005D04B
		[DataField("nodeGroupID", false, 1, false, false, null)]
		public NodeGroupID NodeGroupID { get; private set; }

		// Token: 0x1700028D RID: 653
		// (get) Token: 0x0600122B RID: 4651 RVA: 0x0005EE54 File Offset: 0x0005D054
		// (set) Token: 0x0600122C RID: 4652 RVA: 0x0005EE5C File Offset: 0x0005D05C
		[ViewVariables]
		public EntityUid Owner { get; private set; }

		// Token: 0x0600122D RID: 4653 RVA: 0x0005EE65 File Offset: 0x0005D065
		public virtual bool Connectable(IEntityManager entMan, [Nullable(2)] TransformComponent xform = null)
		{
			if (this.Deleting)
			{
				return false;
			}
			if (entMan.IsQueuedForDeletion(this.Owner))
			{
				return false;
			}
			if (!this.NeedAnchored)
			{
				return true;
			}
			if (xform == null)
			{
				xform = entMan.GetComponent<TransformComponent>(this.Owner);
			}
			return xform.Anchored;
		}

		// Token: 0x1700028E RID: 654
		// (get) Token: 0x0600122E RID: 4654 RVA: 0x0005EEA2 File Offset: 0x0005D0A2
		[ViewVariables]
		[DataField("needAnchored", false, 1, false, false, null)]
		public bool NeedAnchored { get; } = 1;

		// Token: 0x0600122F RID: 4655 RVA: 0x0005EEAA File Offset: 0x0005D0AA
		public virtual void OnAnchorStateChanged(IEntityManager entityManager, bool anchored)
		{
		}

		// Token: 0x06001230 RID: 4656 RVA: 0x0005EEAC File Offset: 0x0005D0AC
		public virtual void Initialize(EntityUid owner, IEntityManager entMan)
		{
			this.Owner = owner;
		}

		// Token: 0x06001231 RID: 4657
		public abstract IEnumerable<Node> GetReachableNodes(TransformComponent xform, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<NodeContainerComponent> nodeQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery, [Nullable(2)] MapGridComponent grid, IEntityManager entMan);

		// Token: 0x04000B33 RID: 2867
		[Nullable(2)]
		[ViewVariables]
		public INodeGroup NodeGroup;

		// Token: 0x04000B36 RID: 2870
		public bool Deleting;

		// Token: 0x04000B37 RID: 2871
		public readonly HashSet<Node> ReachableNodes = new HashSet<Node>();

		// Token: 0x04000B38 RID: 2872
		internal int FloodGen;

		// Token: 0x04000B39 RID: 2873
		internal int UndirectGen;

		// Token: 0x04000B3A RID: 2874
		internal bool FlaggedForFlood;

		// Token: 0x04000B3B RID: 2875
		internal int NetId;

		// Token: 0x04000B3C RID: 2876
		public string Name;
	}
}
