using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.NodeContainer.NodeGroups;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Power.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Power.NodeGroups
{
	// Token: 0x02000283 RID: 643
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class BaseNetConnectorNodeGroup<[Nullable(2)] TNetType> : BaseNodeGroup
	{
		// Token: 0x06000CDA RID: 3290 RVA: 0x0004358C File Offset: 0x0004178C
		public override void LoadNodes(List<Node> groupNodes)
		{
			base.LoadNodes(groupNodes);
			IEntityManager entManager = IoCManager.Resolve<IEntityManager>();
			foreach (Node node in groupNodes)
			{
				List<IBaseNetConnectorComponent<TNetType>> newNetConnectorComponents = new List<IBaseNetConnectorComponent<TNetType>>();
				foreach (IBaseNetConnectorComponent<TNetType> comp in entManager.GetComponents<IBaseNetConnectorComponent<TNetType>>(node.Owner))
				{
					if ((comp.NodeId == null || comp.NodeId == node.Name) && (NodeGroupID)comp.Voltage == node.NodeGroupID)
					{
						newNetConnectorComponents.Add(comp);
					}
				}
				foreach (IBaseNetConnectorComponent<TNetType> netConnector in newNetConnectorComponents)
				{
					this.SetNetConnectorNet(netConnector);
				}
			}
		}

		// Token: 0x06000CDB RID: 3291
		protected abstract void SetNetConnectorNet(IBaseNetConnectorComponent<TNetType> netConnectorComponent);
	}
}
