using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos;
using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NodeContainer.NodeGroups;
using Content.Shared.Atmos;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Server.NodeContainer.Nodes
{
	// Token: 0x0200037C RID: 892
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	[Virtual]
	public class PipeNode : Node, IGasMixtureHolder, IRotatableNode
	{
		// Token: 0x1700028F RID: 655
		// (get) Token: 0x06001236 RID: 4662 RVA: 0x0005EF30 File Offset: 0x0005D130
		// (set) Token: 0x06001237 RID: 4663 RVA: 0x0005EF38 File Offset: 0x0005D138
		public PipeDirection CurrentPipeDirection { get; private set; }

		// Token: 0x06001238 RID: 4664 RVA: 0x0005EF44 File Offset: 0x0005D144
		public void AddAlwaysReachable(PipeNode pipeNode)
		{
			if (pipeNode.NodeGroupID != base.NodeGroupID)
			{
				return;
			}
			if (this._alwaysReachable == null)
			{
				this._alwaysReachable = new HashSet<PipeNode>();
			}
			this._alwaysReachable.Add(pipeNode);
			if (this.NodeGroup != null)
			{
				EntitySystem.Get<NodeGroupSystem>().QueueRemakeGroup((BaseNodeGroup)this.NodeGroup);
			}
		}

		// Token: 0x06001239 RID: 4665 RVA: 0x0005EF9D File Offset: 0x0005D19D
		public void RemoveAlwaysReachable(PipeNode pipeNode)
		{
			if (this._alwaysReachable == null)
			{
				return;
			}
			this._alwaysReachable.Remove(pipeNode);
			if (this.NodeGroup != null)
			{
				EntitySystem.Get<NodeGroupSystem>().QueueRemakeGroup((BaseNodeGroup)this.NodeGroup);
			}
		}

		// Token: 0x17000290 RID: 656
		// (get) Token: 0x0600123A RID: 4666 RVA: 0x0005EFD2 File Offset: 0x0005D1D2
		// (set) Token: 0x0600123B RID: 4667 RVA: 0x0005EFDA File Offset: 0x0005D1DA
		[ViewVariables]
		public bool ConnectionsEnabled
		{
			get
			{
				return this._connectionsEnabled;
			}
			set
			{
				this._connectionsEnabled = value;
				if (this.NodeGroup != null)
				{
					EntitySystem.Get<NodeGroupSystem>().QueueRemakeGroup((BaseNodeGroup)this.NodeGroup);
				}
			}
		}

		// Token: 0x0600123C RID: 4668 RVA: 0x0005F000 File Offset: 0x0005D200
		public override bool Connectable(IEntityManager entMan, [Nullable(2)] TransformComponent xform = null)
		{
			return this._connectionsEnabled && base.Connectable(entMan, xform);
		}

		// Token: 0x17000291 RID: 657
		// (get) Token: 0x0600123D RID: 4669 RVA: 0x0005F014 File Offset: 0x0005D214
		// (set) Token: 0x0600123E RID: 4670 RVA: 0x0005F01C File Offset: 0x0005D21C
		[DataField("rotationsEnabled", false, 1, false, false, null)]
		public bool RotationsEnabled { get; set; } = true;

		// Token: 0x17000292 RID: 658
		// (get) Token: 0x0600123F RID: 4671 RVA: 0x0005F025 File Offset: 0x0005D225
		[Nullable(2)]
		[ViewVariables]
		private IPipeNet PipeNet
		{
			[NullableContext(2)]
			get
			{
				return (IPipeNet)this.NodeGroup;
			}
		}

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x06001240 RID: 4672 RVA: 0x0005F032 File Offset: 0x0005D232
		// (set) Token: 0x06001241 RID: 4673 RVA: 0x0005F04F File Offset: 0x0005D24F
		[ViewVariables]
		public GasMixture Air
		{
			get
			{
				IPipeNet pipeNet = this.PipeNet;
				return ((pipeNet != null) ? pipeNet.Air : null) ?? GasMixture.SpaceGas;
			}
			set
			{
				this.PipeNet.Air = value;
			}
		}

		// Token: 0x17000294 RID: 660
		// (get) Token: 0x06001242 RID: 4674 RVA: 0x0005F05D File Offset: 0x0005D25D
		// (set) Token: 0x06001243 RID: 4675 RVA: 0x0005F065 File Offset: 0x0005D265
		[DataField("volume", false, 1, false, false, null)]
		public float Volume { get; set; } = 200f;

		// Token: 0x06001244 RID: 4676 RVA: 0x0005F070 File Offset: 0x0005D270
		public override void Initialize(EntityUid owner, IEntityManager entMan)
		{
			base.Initialize(owner, entMan);
			if (!this.RotationsEnabled)
			{
				return;
			}
			TransformComponent xform = entMan.GetComponent<TransformComponent>(owner);
			this.CurrentPipeDirection = this._originalPipeDirection.RotatePipeDirection(xform.LocalRotation);
		}

		// Token: 0x06001245 RID: 4677 RVA: 0x0005F0B4 File Offset: 0x0005D2B4
		bool IRotatableNode.RotateNode(in MoveEvent ev)
		{
			if (this._originalPipeDirection == PipeDirection.Fourway)
			{
				return false;
			}
			if (this.RotationsEnabled)
			{
				PipeDirection currentPipeDirection = this.CurrentPipeDirection;
				this.CurrentPipeDirection = this._originalPipeDirection.RotatePipeDirection(ev.NewRotation);
				return currentPipeDirection != this.CurrentPipeDirection;
			}
			if (this.CurrentPipeDirection == this._originalPipeDirection)
			{
				return false;
			}
			this.CurrentPipeDirection = this._originalPipeDirection;
			return true;
		}

		// Token: 0x06001246 RID: 4678 RVA: 0x0005F120 File Offset: 0x0005D320
		public override void OnAnchorStateChanged(IEntityManager entityManager, bool anchored)
		{
			if (!anchored)
			{
				return;
			}
			if (!this.RotationsEnabled)
			{
				this.CurrentPipeDirection = this._originalPipeDirection;
				return;
			}
			TransformComponent xform = entityManager.GetComponent<TransformComponent>(base.Owner);
			this.CurrentPipeDirection = this._originalPipeDirection.RotatePipeDirection(xform.LocalRotation);
		}

		// Token: 0x06001247 RID: 4679 RVA: 0x0005F16F File Offset: 0x0005D36F
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
			if (this._alwaysReachable != null)
			{
				RemQueue<PipeNode> remQ = default(RemQueue<PipeNode>);
				foreach (PipeNode pipe in this._alwaysReachable)
				{
					if (pipe.Deleting)
					{
						remQ.Add(pipe);
					}
					yield return pipe;
				}
				HashSet<PipeNode>.Enumerator enumerator = default(HashSet<PipeNode>.Enumerator);
				foreach (PipeNode pipe2 in remQ)
				{
					this._alwaysReachable.Remove(pipe2);
				}
				remQ = default(RemQueue<PipeNode>);
			}
			if (!xform.Anchored || grid == null)
			{
				yield break;
			}
			Vector2i pos = grid.TileIndicesFor(xform.Coordinates);
			int num;
			for (int i = 0; i < 4; i = num + 1)
			{
				PipeDirection pipeDir = (PipeDirection)(1 << i);
				if (this.CurrentPipeDirection.HasDirection(pipeDir))
				{
					foreach (PipeNode pipe3 in this.LinkableNodesInDirection(pos, pipeDir, grid, nodeQuery))
					{
						yield return pipe3;
					}
					IEnumerator<PipeNode> enumerator3 = null;
				}
				num = i;
			}
			yield break;
			yield break;
		}

		// Token: 0x06001248 RID: 4680 RVA: 0x0005F195 File Offset: 0x0005D395
		private IEnumerable<PipeNode> LinkableNodesInDirection(Vector2i pos, PipeDirection pipeDir, MapGridComponent grid, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<NodeContainerComponent> nodeQuery)
		{
			foreach (PipeNode pipe in this.PipesInDirection(pos, pipeDir, grid, nodeQuery))
			{
				if (pipe.NodeGroupID == base.NodeGroupID && pipe.CurrentPipeDirection.HasDirection(pipeDir.GetOpposite()))
				{
					yield return pipe;
				}
			}
			IEnumerator<PipeNode> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06001249 RID: 4681 RVA: 0x0005F1C2 File Offset: 0x0005D3C2
		protected IEnumerable<PipeNode> PipesInDirection(Vector2i pos, PipeDirection pipeDir, MapGridComponent grid, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<NodeContainerComponent> nodeQuery)
		{
			Vector2i offsetPos = DirectionExtensions.Offset(pos, pipeDir.ToDirection());
			foreach (EntityUid entity in grid.GetAnchoredEntities(offsetPos))
			{
				NodeContainerComponent container;
				if (nodeQuery.TryGetComponent(entity, ref container))
				{
					foreach (Node node in container.Nodes.Values)
					{
						PipeNode pipe = node as PipeNode;
						if (pipe != null)
						{
							yield return pipe;
						}
					}
					Dictionary<string, Node>.ValueCollection.Enumerator enumerator2 = default(Dictionary<string, Node>.ValueCollection.Enumerator);
				}
			}
			IEnumerator<EntityUid> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x04000B3D RID: 2877
		[DataField("pipeDirection", false, 1, false, false, null)]
		private PipeDirection _originalPipeDirection;

		// Token: 0x04000B3F RID: 2879
		[Nullable(new byte[]
		{
			2,
			1
		})]
		private HashSet<PipeNode> _alwaysReachable;

		// Token: 0x04000B40 RID: 2880
		[DataField("connectionsEnabled", false, 1, false, false, null)]
		private bool _connectionsEnabled = true;

		// Token: 0x04000B43 RID: 2883
		private const float DefaultVolume = 200f;
	}
}
