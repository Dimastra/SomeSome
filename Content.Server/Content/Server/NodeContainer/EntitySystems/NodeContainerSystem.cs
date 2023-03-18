using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.NodeContainer.NodeGroups;
using Content.Server.NodeContainer.Nodes;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.NodeContainer.EntitySystems
{
	// Token: 0x02000387 RID: 903
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NodeContainerSystem : EntitySystem
	{
		// Token: 0x0600127C RID: 4732 RVA: 0x0005F720 File Offset: 0x0005D920
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<NodeContainerComponent, ComponentInit>(new ComponentEventHandler<NodeContainerComponent, ComponentInit>(this.OnInitEvent), null, null);
			base.SubscribeLocalEvent<NodeContainerComponent, ComponentStartup>(new ComponentEventHandler<NodeContainerComponent, ComponentStartup>(this.OnStartupEvent), null, null);
			base.SubscribeLocalEvent<NodeContainerComponent, ComponentShutdown>(new ComponentEventHandler<NodeContainerComponent, ComponentShutdown>(this.OnShutdownEvent), null, null);
			base.SubscribeLocalEvent<NodeContainerComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<NodeContainerComponent, AnchorStateChangedEvent>(this.OnAnchorStateChanged), null, null);
			base.SubscribeLocalEvent<NodeContainerComponent, ReAnchorEvent>(new ComponentEventRefHandler<NodeContainerComponent, ReAnchorEvent>(this.OnReAnchor), null, null);
			base.SubscribeLocalEvent<NodeContainerComponent, MoveEvent>(new ComponentEventRefHandler<NodeContainerComponent, MoveEvent>(this.OnMoveEvent), null, null);
			base.SubscribeLocalEvent<NodeContainerComponent, ExaminedEvent>(new ComponentEventHandler<NodeContainerComponent, ExaminedEvent>(this.OnExamine), null, null);
		}

		// Token: 0x0600127D RID: 4733 RVA: 0x0005F7C0 File Offset: 0x0005D9C0
		private void OnInitEvent(EntityUid uid, NodeContainerComponent component, ComponentInit args)
		{
			foreach (KeyValuePair<string, Node> keyValuePair in component.Nodes)
			{
				string text;
				Node node;
				keyValuePair.Deconstruct(out text, out node);
				string key = text;
				Node node2 = node;
				node2.Name = key;
				node2.Initialize(component.Owner, this.EntityManager);
			}
		}

		// Token: 0x0600127E RID: 4734 RVA: 0x0005F834 File Offset: 0x0005DA34
		private void OnStartupEvent(EntityUid uid, NodeContainerComponent component, ComponentStartup args)
		{
			foreach (Node node in component.Nodes.Values)
			{
				this._nodeGroupSystem.QueueReflood(node);
			}
		}

		// Token: 0x0600127F RID: 4735 RVA: 0x0005F894 File Offset: 0x0005DA94
		private void OnShutdownEvent(EntityUid uid, NodeContainerComponent component, ComponentShutdown args)
		{
			foreach (Node node in component.Nodes.Values)
			{
				this._nodeGroupSystem.QueueNodeRemove(node);
				node.Deleting = true;
			}
		}

		// Token: 0x06001280 RID: 4736 RVA: 0x0005F8F8 File Offset: 0x0005DAF8
		private void OnAnchorStateChanged(EntityUid uid, NodeContainerComponent component, ref AnchorStateChangedEvent args)
		{
			foreach (Node node in component.Nodes.Values)
			{
				if (node.NeedAnchored)
				{
					node.OnAnchorStateChanged(this.EntityManager, args.Anchored);
					if (args.Anchored)
					{
						this._nodeGroupSystem.QueueReflood(node);
					}
					else
					{
						this._nodeGroupSystem.QueueNodeRemove(node);
					}
				}
			}
		}

		// Token: 0x06001281 RID: 4737 RVA: 0x0005F988 File Offset: 0x0005DB88
		private void OnReAnchor(EntityUid uid, NodeContainerComponent component, ref ReAnchorEvent args)
		{
			foreach (Node node in component.Nodes.Values)
			{
				this._nodeGroupSystem.QueueNodeRemove(node);
				this._nodeGroupSystem.QueueReflood(node);
			}
		}

		// Token: 0x06001282 RID: 4738 RVA: 0x0005F9F4 File Offset: 0x0005DBF4
		private void OnMoveEvent(EntityUid uid, NodeContainerComponent container, ref MoveEvent ev)
		{
			if (ev.NewRotation == ev.OldRotation)
			{
				return;
			}
			TransformComponent xform = ev.Component;
			foreach (Node node in container.Nodes.Values)
			{
				IRotatableNode rotatableNode = node as IRotatableNode;
				if (rotatableNode != null && node.Connectable(this.EntityManager, xform) && rotatableNode.RotateNode(ev))
				{
					this._nodeGroupSystem.QueueReflood(node);
				}
			}
		}

		// Token: 0x06001283 RID: 4739 RVA: 0x0005FA90 File Offset: 0x0005DC90
		private void OnExamine(EntityUid uid, NodeContainerComponent component, ExaminedEvent args)
		{
			if (!component.Examinable || !args.IsInDetailsRange)
			{
				return;
			}
			foreach (Node node in component.Nodes.Values)
			{
				if (node != null)
				{
					switch (node.NodeGroupID)
					{
					case NodeGroupID.HVPower:
						args.PushMarkup(Loc.GetString("node-container-component-on-examine-details-hvpower"));
						break;
					case NodeGroupID.MVPower:
						args.PushMarkup(Loc.GetString("node-container-component-on-examine-details-mvpower"));
						break;
					case NodeGroupID.Apc:
						args.PushMarkup(Loc.GetString("node-container-component-on-examine-details-apc"));
						break;
					}
				}
			}
		}

		// Token: 0x04000B58 RID: 2904
		[Dependency]
		private readonly NodeGroupSystem _nodeGroupSystem;
	}
}
