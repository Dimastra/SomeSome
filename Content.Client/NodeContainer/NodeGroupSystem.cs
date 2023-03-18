using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.NodeContainer;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

namespace Content.Client.NodeContainer
{
	// Token: 0x02000216 RID: 534
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NodeGroupSystem : EntitySystem
	{
		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x06000DEC RID: 3564 RVA: 0x000548D6 File Offset: 0x00052AD6
		// (set) Token: 0x06000DED RID: 3565 RVA: 0x000548DE File Offset: 0x00052ADE
		public bool VisEnabled { get; private set; }

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x06000DEE RID: 3566 RVA: 0x000548E7 File Offset: 0x00052AE7
		public Dictionary<int, NodeVis.GroupData> Groups { get; } = new Dictionary<int, NodeVis.GroupData>();

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x06000DEF RID: 3567 RVA: 0x000548EF File Offset: 0x00052AEF
		public HashSet<string> Filtered { get; } = new HashSet<string>();

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x06000DF0 RID: 3568 RVA: 0x000548F7 File Offset: 0x00052AF7
		// (set) Token: 0x06000DF1 RID: 3569 RVA: 0x000548FF File Offset: 0x00052AFF
		[TupleElementNames(new string[]
		{
			"group",
			"node"
		})]
		[Nullable(new byte[]
		{
			1,
			1,
			0,
			1,
			1
		})]
		public Dictionary<EntityUid, ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum>[]> Entities { [return: TupleElementNames(new string[]
		{
			"group",
			"node"
		})] [return: Nullable(new byte[]
		{
			1,
			1,
			0,
			1,
			1
		})] get; [param: TupleElementNames(new string[]
		{
			"group",
			"node"
		})] [param: Nullable(new byte[]
		{
			1,
			1,
			0,
			1,
			1
		})] private set; } = new Dictionary<EntityUid, ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum>[]>();

		// Token: 0x170002FC RID: 764
		// (get) Token: 0x06000DF2 RID: 3570 RVA: 0x00054908 File Offset: 0x00052B08
		// (set) Token: 0x06000DF3 RID: 3571 RVA: 0x00054910 File Offset: 0x00052B10
		[TupleElementNames(new string[]
		{
			"group",
			"node"
		})]
		[Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		public Dictionary<ValueTuple<int, int>, NodeVis.NodeDatum> NodeLookup { [return: TupleElementNames(new string[]
		{
			"group",
			"node"
		})] [return: Nullable(new byte[]
		{
			1,
			0,
			1
		})] get; [param: TupleElementNames(new string[]
		{
			"group",
			"node"
		})] [param: Nullable(new byte[]
		{
			1,
			0,
			1
		})] private set; } = new Dictionary<ValueTuple<int, int>, NodeVis.NodeDatum>();

		// Token: 0x06000DF4 RID: 3572 RVA: 0x00054919 File Offset: 0x00052B19
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<NodeVis.MsgData>(new EntityEventHandler<NodeVis.MsgData>(this.DataMsgHandler), null, null);
		}

		// Token: 0x06000DF5 RID: 3573 RVA: 0x00054935 File Offset: 0x00052B35
		public override void Shutdown()
		{
			base.Shutdown();
			this._overlayManager.RemoveOverlay<NodeVisualizationOverlay>();
		}

		// Token: 0x06000DF6 RID: 3574 RVA: 0x0005494C File Offset: 0x00052B4C
		private void DataMsgHandler(NodeVis.MsgData ev)
		{
			if (!this.VisEnabled)
			{
				return;
			}
			foreach (int key in ev.GroupDeletions)
			{
				this.Groups.Remove(key);
			}
			foreach (NodeVis.GroupData groupData in ev.Groups)
			{
				this.Groups.Add(groupData.NetId, groupData);
			}
			foreach (KeyValuePair<int, string> keyValuePair in ev.GroupDataUpdates)
			{
				int num;
				string text;
				keyValuePair.Deconstruct(out num, out text);
				int key2 = num;
				string debugData = text;
				NodeVis.GroupData groupData2;
				if (this.Groups.TryGetValue(key2, out groupData2))
				{
					groupData2.DebugData = debugData;
				}
			}
			this.Entities = (from n in this.Groups.Values.SelectMany((NodeVis.GroupData g) => g.Nodes, (NodeVis.GroupData data, NodeVis.NodeDatum nodeData) => new ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum>(data, nodeData))
			group n by n.Item2.Entity).ToDictionary(([TupleElementNames(new string[]
			{
				"data",
				"nodeData"
			})] IGrouping<EntityUid, ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum>> g) => g.Key, ([TupleElementNames(new string[]
			{
				"data",
				"nodeData"
			})] IGrouping<EntityUid, ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum>> g) => g.ToArray<ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum>>());
			this.NodeLookup = this.Groups.Values.SelectMany((NodeVis.GroupData g) => g.Nodes, (NodeVis.GroupData data, NodeVis.NodeDatum nodeData) => new ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum>(data, nodeData)).ToDictionary(([TupleElementNames(new string[]
			{
				"data",
				"nodeData"
			})] ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum> n) => new ValueTuple<int, int>(n.Item1.NetId, n.Item2.NetId), ([TupleElementNames(new string[]
			{
				"data",
				"nodeData"
			})] ValueTuple<NodeVis.GroupData, NodeVis.NodeDatum> n) => n.Item2);
		}

		// Token: 0x06000DF7 RID: 3575 RVA: 0x00054BB8 File Offset: 0x00052DB8
		public void SetVisEnabled(bool enabled)
		{
			this.VisEnabled = enabled;
			base.RaiseNetworkEvent(new NodeVis.MsgEnable(enabled));
			if (enabled)
			{
				NodeVisualizationOverlay nodeVisualizationOverlay = new NodeVisualizationOverlay(this, this._entityLookup, this._mapManager, this._inputManager, this._resourceCache, this.EntityManager);
				this._overlayManager.AddOverlay(nodeVisualizationOverlay);
				return;
			}
			this.Groups.Clear();
			this.Entities.Clear();
		}

		// Token: 0x040006DC RID: 1756
		[Dependency]
		private readonly IOverlayManager _overlayManager;

		// Token: 0x040006DD RID: 1757
		[Dependency]
		private readonly EntityLookupSystem _entityLookup;

		// Token: 0x040006DE RID: 1758
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x040006DF RID: 1759
		[Dependency]
		private readonly IInputManager _inputManager;

		// Token: 0x040006E0 RID: 1760
		[Dependency]
		private readonly IResourceCache _resourceCache;
	}
}
