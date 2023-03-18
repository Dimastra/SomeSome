using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Managers;
using Content.Server.NodeContainer.NodeGroups;
using Content.Server.NodeContainer.Nodes;
using Content.Shared.Administration;
using Content.Shared.NodeContainer;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Server.NodeContainer.EntitySystems
{
	// Token: 0x02000388 RID: 904
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NodeGroupSystem : EntitySystem
	{
		// Token: 0x1700029F RID: 671
		// (get) Token: 0x06001285 RID: 4741 RVA: 0x0005FB50 File Offset: 0x0005DD50
		public bool VisEnabled
		{
			get
			{
				return this._visPlayers.Count != 0;
			}
		}

		// Token: 0x06001286 RID: 4742 RVA: 0x0005FB60 File Offset: 0x0005DD60
		public override void Initialize()
		{
			base.Initialize();
			this._sawmill = this._logManager.GetSawmill("nodegroup");
			this._playerManager.PlayerStatusChanged += this.OnPlayerStatusChanged;
			base.SubscribeNetworkEvent<NodeVis.MsgEnable>(new EntitySessionEventHandler<NodeVis.MsgEnable>(this.HandleEnableMsg), null, null);
		}

		// Token: 0x06001287 RID: 4743 RVA: 0x0005FBB4 File Offset: 0x0005DDB4
		public override void Shutdown()
		{
			base.Shutdown();
			this._playerManager.PlayerStatusChanged -= this.OnPlayerStatusChanged;
		}

		// Token: 0x06001288 RID: 4744 RVA: 0x0005FBD4 File Offset: 0x0005DDD4
		private void HandleEnableMsg(NodeVis.MsgEnable msg, EntitySessionEventArgs args)
		{
			IPlayerSession session = (IPlayerSession)args.SenderSession;
			if (!this._adminManager.HasAdminFlag(session, AdminFlags.Debug))
			{
				return;
			}
			if (msg.Enabled)
			{
				this._visPlayers.Add(session);
				this.VisSendFullStateImmediate(session);
				return;
			}
			this._visPlayers.Remove(session);
		}

		// Token: 0x06001289 RID: 4745 RVA: 0x0005FC28 File Offset: 0x0005DE28
		private void OnPlayerStatusChanged([Nullable(2)] object sender, SessionStatusEventArgs e)
		{
			if (e.NewStatus == 4)
			{
				this._visPlayers.Remove(e.Session);
			}
		}

		// Token: 0x0600128A RID: 4746 RVA: 0x0005FC48 File Offset: 0x0005DE48
		public void QueueRemakeGroup(BaseNodeGroup group)
		{
			if (group.Remaking)
			{
				return;
			}
			this._toRemake.Add(group);
			group.Remaking = true;
			foreach (Node node in group.Nodes)
			{
				this.QueueReflood(node);
			}
			if (group.NodeCount == 0)
			{
				this._nodeGroups.Remove(group);
			}
		}

		// Token: 0x0600128B RID: 4747 RVA: 0x0005FCD0 File Offset: 0x0005DED0
		public void QueueReflood(Node node)
		{
			if (node.FlaggedForFlood)
			{
				return;
			}
			this._toReflood.Add(node);
			node.FlaggedForFlood = true;
		}

		// Token: 0x0600128C RID: 4748 RVA: 0x0005FCEE File Offset: 0x0005DEEE
		public void QueueNodeRemove(Node node)
		{
			this._toRemove.Add(node);
		}

		// Token: 0x0600128D RID: 4749 RVA: 0x0005FCFD File Offset: 0x0005DEFD
		public void CreateSingleNetImmediate(Node node)
		{
			if (node.NodeGroup != null)
			{
				return;
			}
			this.QueueReflood(node);
			this.InitGroup(node, new List<Node>
			{
				node
			});
		}

		// Token: 0x0600128E RID: 4750 RVA: 0x0005FD23 File Offset: 0x0005DF23
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (!this.PauseUpdating)
			{
				this.DoGroupUpdates();
				this.VisDoUpdate(frameTime);
			}
		}

		// Token: 0x0600128F RID: 4751 RVA: 0x0005FD44 File Offset: 0x0005DF44
		private void DoGroupUpdates()
		{
			if (this._toRemake.Count == 0 && this._toReflood.Count == 0 && this._toRemove.Count == 0)
			{
				return;
			}
			Stopwatch sw = Stopwatch.StartNew();
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			EntityQuery<NodeContainerComponent> nodeQuery = base.GetEntityQuery<NodeContainerComponent>();
			foreach (Node toRemove in this._toRemove)
			{
				if (toRemove.NodeGroup != null)
				{
					BaseNodeGroup group = (BaseNodeGroup)toRemove.NodeGroup;
					group.RemoveNode(toRemove);
					toRemove.NodeGroup = null;
					this.QueueRemakeGroup(group);
				}
			}
			foreach (BaseNodeGroup toRemake in this._toRemake)
			{
				this.QueueRemakeGroup(toRemake);
			}
			this._gen++;
			for (int i = 0; i < this._toReflood.Count; i++)
			{
				Node node = this._toReflood[i];
				if (!node.Deleting)
				{
					this.ClearReachableIfNecessary(node);
					INodeGroup nodeGroup = node.NodeGroup;
					if (nodeGroup != null && !nodeGroup.Remaking)
					{
						this.QueueRemakeGroup((BaseNodeGroup)node.NodeGroup);
					}
					foreach (Node compatible in this.GetCompatibleNodes(node, xformQuery, nodeQuery))
					{
						this.ClearReachableIfNecessary(compatible);
						INodeGroup nodeGroup2 = compatible.NodeGroup;
						if (nodeGroup2 != null && !nodeGroup2.Remaking)
						{
							BaseNodeGroup group2 = (BaseNodeGroup)compatible.NodeGroup;
							this.QueueRemakeGroup(group2);
						}
						node.ReachableNodes.Add(compatible);
						compatible.ReachableNodes.Add(node);
					}
				}
			}
			List<BaseNodeGroup> newGroups = new List<BaseNodeGroup>();
			foreach (Node node2 in this._toReflood)
			{
				node2.FlaggedForFlood = false;
				if (node2.FloodGen != this._gen && !node2.Deleting)
				{
					List<Node> groupNodes = this.FloodFillNode(node2);
					BaseNodeGroup newGroup = this.InitGroup(node2, groupNodes);
					newGroups.Add(newGroup);
				}
			}
			foreach (BaseNodeGroup oldGroup in this._toRemake)
			{
				IEnumerable<IGrouping<INodeGroup, Node>> newGrouped = from n in oldGroup.Nodes
				group n by n.NodeGroup;
				oldGroup.Removed = true;
				oldGroup.AfterRemake(newGrouped);
				this._nodeGroups.Remove(oldGroup);
				if (this.VisEnabled)
				{
					this._visDeletes.Add(oldGroup.NetId);
				}
			}
			int refloodCount = this._toReflood.Count;
			this._toReflood.Clear();
			this._toRemake.Clear();
			this._toRemove.Clear();
			HashSet<EntityUid> entities = new HashSet<EntityUid>();
			foreach (BaseNodeGroup baseNodeGroup in newGroups)
			{
				foreach (Node node3 in baseNodeGroup.Nodes)
				{
					entities.Add(node3.Owner);
				}
			}
			foreach (EntityUid uid in entities)
			{
				NodeGroupsRebuilt ev = new NodeGroupsRebuilt(uid);
				base.RaiseLocalEvent<NodeGroupsRebuilt>(uid, ref ev, true);
			}
			ISawmill sawmill = this._sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(57, 3);
			defaultInterpolatedStringHandler.AppendLiteral("Updated node groups in ");
			defaultInterpolatedStringHandler.AppendFormatted<double>(sw.Elapsed.TotalMilliseconds);
			defaultInterpolatedStringHandler.AppendLiteral("ms. ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(newGroups.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" new groups, ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(refloodCount);
			defaultInterpolatedStringHandler.AppendLiteral(" nodes processed.");
			sawmill.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x06001290 RID: 4752 RVA: 0x000601F0 File Offset: 0x0005E3F0
		private void ClearReachableIfNecessary(Node node)
		{
			if (node.UndirectGen != this._gen)
			{
				node.ReachableNodes.Clear();
				node.UndirectGen = this._gen;
			}
		}

		// Token: 0x06001291 RID: 4753 RVA: 0x00060218 File Offset: 0x0005E418
		private BaseNodeGroup InitGroup(Node node, List<Node> groupNodes)
		{
			BaseNodeGroup newGroup = (BaseNodeGroup)this._nodeGroupFactory.MakeNodeGroup(node.NodeGroupID);
			newGroup.Initialize(node, this.EntityManager);
			BaseNodeGroup baseNodeGroup = newGroup;
			int groupNetIdCounter = this._groupNetIdCounter;
			this._groupNetIdCounter = groupNetIdCounter + 1;
			baseNodeGroup.NetId = groupNetIdCounter;
			int netIdCounter = 0;
			foreach (Node node2 in groupNodes)
			{
				node2.NodeGroup = newGroup;
				netIdCounter = (node2.NetId = netIdCounter + 1);
			}
			newGroup.LoadNodes(groupNodes);
			this._nodeGroups.Add(newGroup);
			if (this.VisEnabled)
			{
				this._visSends.Add(newGroup);
			}
			return newGroup;
		}

		// Token: 0x06001292 RID: 4754 RVA: 0x000602D8 File Offset: 0x0005E4D8
		private List<Node> FloodFillNode(Node rootNode)
		{
			List<Node> allNodes = new List<Node>();
			Stack<Node> stack = new Stack<Node>();
			stack.Push(rootNode);
			rootNode.FloodGen = this._gen;
			Node node;
			while (stack.TryPop(out node))
			{
				allNodes.Add(node);
				using (HashSet<Node>.Enumerator enumerator = node.ReachableNodes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Node reachable = enumerator.Current;
						if (reachable.FloodGen != this._gen)
						{
							reachable.FloodGen = this._gen;
							stack.Push(reachable);
						}
					}
					continue;
				}
				break;
			}
			return allNodes;
		}

		// Token: 0x06001293 RID: 4755 RVA: 0x0006037C File Offset: 0x0005E57C
		private IEnumerable<Node> GetCompatibleNodes(Node node, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<TransformComponent> xformQuery, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<NodeContainerComponent> nodeQuery)
		{
			TransformComponent xform = xformQuery.GetComponent(node.Owner);
			MapGridComponent grid;
			this._mapManager.TryGetGrid(xform.GridUid, ref grid);
			if (!node.Connectable(this.EntityManager, xform))
			{
				yield break;
			}
			foreach (Node reachable in node.GetReachableNodes(xform, nodeQuery, xformQuery, grid, this.EntityManager))
			{
				if (reachable.NodeGroupID == node.NodeGroupID && reachable.Connectable(this.EntityManager, xformQuery.GetComponent(reachable.Owner)))
				{
					yield return reachable;
				}
			}
			IEnumerator<Node> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06001294 RID: 4756 RVA: 0x000603A4 File Offset: 0x0005E5A4
		private void VisDoUpdate(float frametime)
		{
			if (this._visPlayers.Count == 0)
			{
				return;
			}
			this._accumulatedFrameTime += frametime;
			if (this._accumulatedFrameTime < 1f && this._visSends.Count == 0 && this._visDeletes.Count == 0)
			{
				return;
			}
			NodeVis.MsgData msg = new NodeVis.MsgData();
			msg.GroupDeletions.AddRange(this._visDeletes);
			List<NodeVis.GroupData> groups = msg.Groups;
			IEnumerable<BaseNodeGroup> visSends = this._visSends;
			Func<BaseNodeGroup, NodeVis.GroupData> selector;
			if ((selector = NodeGroupSystem.<>O.<0>__VisMakeGroupState) == null)
			{
				selector = (NodeGroupSystem.<>O.<0>__VisMakeGroupState = new Func<BaseNodeGroup, NodeVis.GroupData>(NodeGroupSystem.VisMakeGroupState));
			}
			groups.AddRange(visSends.Select(selector));
			if (this._accumulatedFrameTime > 1f)
			{
				this._accumulatedFrameTime -= 1f;
				foreach (BaseNodeGroup group in this._nodeGroups)
				{
					if (!this._visSends.Contains(group))
					{
						msg.GroupDataUpdates.Add(group.NetId, group.GetDebugData());
					}
				}
			}
			this._visSends.Clear();
			this._visDeletes.Clear();
			foreach (IPlayerSession player in this._visPlayers)
			{
				base.RaiseNetworkEvent(msg, player.ConnectedClient);
			}
		}

		// Token: 0x06001295 RID: 4757 RVA: 0x00060520 File Offset: 0x0005E720
		private void VisSendFullStateImmediate(IPlayerSession player)
		{
			NodeVis.MsgData msg = new NodeVis.MsgData();
			foreach (BaseNodeGroup network in this._nodeGroups)
			{
				msg.Groups.Add(NodeGroupSystem.VisMakeGroupState(network));
			}
			base.RaiseNetworkEvent(msg, player.ConnectedClient);
		}

		// Token: 0x06001296 RID: 4758 RVA: 0x00060590 File Offset: 0x0005E790
		private static NodeVis.GroupData VisMakeGroupState(BaseNodeGroup group)
		{
			NodeVis.GroupData groupData = new NodeVis.GroupData();
			groupData.NetId = group.NetId;
			groupData.GroupId = group.GroupId.ToString();
			groupData.Color = NodeGroupSystem.CalcNodeGroupColor(group);
			groupData.Nodes = group.Nodes.Select(delegate(Node n)
			{
				NodeVis.NodeDatum nodeDatum = new NodeVis.NodeDatum();
				nodeDatum.Name = n.Name;
				nodeDatum.NetId = n.NetId;
				nodeDatum.Reachable = (from r in n.ReachableNodes
				select r.NetId).ToArray<int>();
				nodeDatum.Entity = n.Owner;
				nodeDatum.Type = n.GetType().Name;
				return nodeDatum;
			}).ToArray<NodeVis.NodeDatum>();
			groupData.DebugData = group.GetDebugData();
			return groupData;
		}

		// Token: 0x06001297 RID: 4759 RVA: 0x00060618 File Offset: 0x0005E818
		private static Color CalcNodeGroupColor(BaseNodeGroup group)
		{
			Color result;
			switch (group.GroupId)
			{
			case NodeGroupID.HVPower:
				result = Color.Orange;
				break;
			case NodeGroupID.MVPower:
				result = Color.Yellow;
				break;
			case NodeGroupID.Apc:
				result = Color.LimeGreen;
				break;
			case NodeGroupID.AMEngine:
				result = Color.Purple;
				break;
			case NodeGroupID.Pipe:
				result = Color.Blue;
				break;
			case NodeGroupID.WireNet:
				result = Color.DarkMagenta;
				break;
			default:
				result = Color.White;
				break;
			}
			return result;
		}

		// Token: 0x04000B59 RID: 2905
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000B5A RID: 2906
		[Dependency]
		private readonly IAdminManager _adminManager;

		// Token: 0x04000B5B RID: 2907
		[Dependency]
		private readonly INodeGroupFactory _nodeGroupFactory;

		// Token: 0x04000B5C RID: 2908
		[Dependency]
		private readonly ILogManager _logManager;

		// Token: 0x04000B5D RID: 2909
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000B5E RID: 2910
		private readonly List<int> _visDeletes = new List<int>();

		// Token: 0x04000B5F RID: 2911
		private readonly List<BaseNodeGroup> _visSends = new List<BaseNodeGroup>();

		// Token: 0x04000B60 RID: 2912
		private readonly HashSet<IPlayerSession> _visPlayers = new HashSet<IPlayerSession>();

		// Token: 0x04000B61 RID: 2913
		private readonly HashSet<BaseNodeGroup> _toRemake = new HashSet<BaseNodeGroup>();

		// Token: 0x04000B62 RID: 2914
		private readonly HashSet<BaseNodeGroup> _nodeGroups = new HashSet<BaseNodeGroup>();

		// Token: 0x04000B63 RID: 2915
		private readonly HashSet<Node> _toRemove = new HashSet<Node>();

		// Token: 0x04000B64 RID: 2916
		private readonly List<Node> _toReflood = new List<Node>();

		// Token: 0x04000B65 RID: 2917
		private ISawmill _sawmill;

		// Token: 0x04000B66 RID: 2918
		private const float VisDataUpdateInterval = 1f;

		// Token: 0x04000B67 RID: 2919
		private float _accumulatedFrameTime;

		// Token: 0x04000B68 RID: 2920
		private int _gen = 1;

		// Token: 0x04000B69 RID: 2921
		private int _groupNetIdCounter = 1;

		// Token: 0x04000B6A RID: 2922
		public bool PauseUpdating;

		// Token: 0x02000993 RID: 2451
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x0400216D RID: 8557
			[Nullable(0)]
			public static Func<BaseNodeGroup, NodeVis.GroupData> <0>__VisMakeGroupState;
		}
	}
}
