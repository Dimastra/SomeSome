using System;
using System.Runtime.CompilerServices;
using Content.Server.DeviceNetwork.Components;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Power.EntitySystems;
using Content.Server.Power.Nodes;
using Robust.Shared.GameObjects;

namespace Content.Server.DeviceNetwork.Systems
{
	// Token: 0x02000580 RID: 1408
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ApcNetworkSystem : EntitySystem
	{
		// Token: 0x06001D7D RID: 7549 RVA: 0x0009D258 File Offset: 0x0009B458
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ApcNetworkComponent, BeforePacketSentEvent>(new ComponentEventHandler<ApcNetworkComponent, BeforePacketSentEvent>(this.OnBeforePacketSent), null, null);
			base.SubscribeLocalEvent<ApcNetworkComponent, ExtensionCableSystem.ProviderConnectedEvent>(new ComponentEventHandler<ApcNetworkComponent, ExtensionCableSystem.ProviderConnectedEvent>(this.OnProviderConnected), null, null);
			base.SubscribeLocalEvent<ApcNetworkComponent, ExtensionCableSystem.ProviderDisconnectedEvent>(new ComponentEventHandler<ApcNetworkComponent, ExtensionCableSystem.ProviderDisconnectedEvent>(this.OnProviderDisconnected), null, null);
		}

		// Token: 0x06001D7E RID: 7550 RVA: 0x0009D2A8 File Offset: 0x0009B4A8
		private void OnBeforePacketSent(EntityUid uid, ApcNetworkComponent receiver, BeforePacketSentEvent args)
		{
			ApcNetworkComponent sender;
			if (!this.EntityManager.TryGetComponent<ApcNetworkComponent>(args.Sender, ref sender))
			{
				return;
			}
			Node connectedNode = sender.ConnectedNode;
			if (((connectedNode != null) ? connectedNode.NodeGroup : null) != null)
			{
				object nodeGroup = sender.ConnectedNode.NodeGroup;
				Node connectedNode2 = receiver.ConnectedNode;
				if (nodeGroup.Equals((connectedNode2 != null) ? connectedNode2.NodeGroup : null))
				{
					return;
				}
			}
			args.Cancel();
		}

		// Token: 0x06001D7F RID: 7551 RVA: 0x0009D30C File Offset: 0x0009B50C
		private void OnProviderConnected(EntityUid uid, ApcNetworkComponent component, ExtensionCableSystem.ProviderConnectedEvent args)
		{
			NodeContainerComponent nodeContainer;
			if (!this.EntityManager.TryGetComponent<NodeContainerComponent>(args.Provider.Owner, ref nodeContainer))
			{
				return;
			}
			CableNode node;
			if (nodeContainer.TryGetNode<CableNode>("power", out node))
			{
				component.ConnectedNode = node;
				return;
			}
			CableDeviceNode deviceNode;
			if (nodeContainer.TryGetNode<CableDeviceNode>("output", out deviceNode))
			{
				component.ConnectedNode = deviceNode;
			}
		}

		// Token: 0x06001D80 RID: 7552 RVA: 0x0009D361 File Offset: 0x0009B561
		private void OnProviderDisconnected(EntityUid uid, ApcNetworkComponent component, ExtensionCableSystem.ProviderDisconnectedEvent args)
		{
			component.ConnectedNode = null;
		}
	}
}
