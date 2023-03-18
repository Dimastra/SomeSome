using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Administration.Logs;
using Content.Server.Construction.Components;
using Content.Server.Containers;
using Content.Server.DoAfter;
using Content.Server.Examine;
using Content.Server.Power.Components;
using Content.Server.Stack;
using Content.Server.Storage.Components;
using Content.Server.Temperature.Components;
using Content.Server.Temperature.Systems;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Computer;
using Content.Shared.Construction;
using Content.Shared.Construction.Components;
using Content.Shared.Construction.Conditions;
using Content.Shared.Construction.Prototypes;
using Content.Shared.Construction.Steps;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Content.Shared.Verbs;
using Robust.Server.Containers;
using Robust.Shared.Containers;
using Robust.Shared.Exceptions;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Players;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;

namespace Content.Server.Construction
{
	// Token: 0x020005EE RID: 1518
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ConstructionSystem : SharedConstructionSystem
	{
		// Token: 0x0600205C RID: 8284 RVA: 0x000A8EBF File Offset: 0x000A70BF
		private void InitializeComputer()
		{
			base.SubscribeLocalEvent<ComputerComponent, ComponentInit>(new ComponentEventHandler<ComputerComponent, ComponentInit>(this.OnCompInit), null, null);
			base.SubscribeLocalEvent<ComputerComponent, MapInitEvent>(new ComponentEventHandler<ComputerComponent, MapInitEvent>(this.OnCompMapInit), null, null);
			base.SubscribeLocalEvent<ComputerComponent, PowerChangedEvent>(new ComponentEventRefHandler<ComputerComponent, PowerChangedEvent>(this.OnCompPowerChange), null, null);
		}

		// Token: 0x0600205D RID: 8285 RVA: 0x000A8F00 File Offset: 0x000A7100
		private void OnCompInit(EntityUid uid, ComputerComponent component, ComponentInit args)
		{
			this._container.EnsureContainer<Container>(uid, "board", null);
			ApcPowerReceiverComponent powerReceiver;
			if (base.TryComp<ApcPowerReceiverComponent>(uid, ref powerReceiver))
			{
				this._appearance.SetData(uid, ComputerVisuals.Powered, powerReceiver.Powered, null);
			}
		}

		// Token: 0x0600205E RID: 8286 RVA: 0x000A8F49 File Offset: 0x000A7149
		private void OnCompMapInit(EntityUid uid, ComputerComponent component, MapInitEvent args)
		{
			this.CreateComputerBoard(component);
		}

		// Token: 0x0600205F RID: 8287 RVA: 0x000A8F52 File Offset: 0x000A7152
		private void OnCompPowerChange(EntityUid uid, ComputerComponent component, ref PowerChangedEvent args)
		{
			this._appearance.SetData(uid, ComputerVisuals.Powered, args.Powered, null);
		}

		// Token: 0x06002060 RID: 8288 RVA: 0x000A8F74 File Offset: 0x000A7174
		private void CreateComputerBoard(ComputerComponent component)
		{
			ConstructionComponent construction;
			if (base.TryComp<ConstructionComponent>(component.Owner, ref construction))
			{
				this.AddContainer(component.Owner, "board", construction);
			}
			if (string.IsNullOrEmpty(component.BoardPrototype))
			{
				return;
			}
			Container container = this._container.EnsureContainer<Container>(component.Owner, "board", null);
			if (container.ContainedEntities.Count != 0)
			{
				return;
			}
			EntityUid board = this.EntityManager.SpawnEntity(component.BoardPrototype, base.Transform(component.Owner).Coordinates);
			if (!container.Insert(board, null, null, null, null, null))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Couldn't insert board ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(board);
				defaultInterpolatedStringHandler.AppendLiteral(" to computer ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(component.Owner);
				defaultInterpolatedStringHandler.AppendLiteral("!");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}

		// Token: 0x06002061 RID: 8289 RVA: 0x000A9058 File Offset: 0x000A7258
		public override void Initialize()
		{
			base.Initialize();
			this._sawmill = this._logManager.GetSawmill("Construction");
			this.InitializeComputer();
			this.InitializeGraphs();
			this.InitializeGuided();
			this.InitializeInteractions();
			this.InitializeInitial();
			this.InitializeMachines();
			base.SubscribeLocalEvent<ConstructionComponent, ComponentInit>(new ComponentEventHandler<ConstructionComponent, ComponentInit>(this.OnConstructionInit), null, null);
			base.SubscribeLocalEvent<ConstructionComponent, ComponentStartup>(new ComponentEventHandler<ConstructionComponent, ComponentStartup>(this.OnConstructionStartup), null, null);
		}

		// Token: 0x06002062 RID: 8290 RVA: 0x000A90D0 File Offset: 0x000A72D0
		private void OnConstructionInit(EntityUid uid, ConstructionComponent construction, ComponentInit args)
		{
			ConstructionGraphPrototype graph = this.GetCurrentGraph(uid, construction);
			if (graph == null)
			{
				ISawmill sawmill = this._sawmill;
				string str = "Prototype ";
				EntityPrototype entityPrototype = this.EntityManager.GetComponent<MetaDataComponent>(construction.Owner).EntityPrototype;
				sawmill.Warning(str + ((entityPrototype != null) ? entityPrototype.ID : null) + "'s construction component has an invalid graph specified.");
				return;
			}
			ConstructionGraphNode node = this.GetNodeFromGraph(graph, construction.Node);
			if (node == null)
			{
				ISawmill sawmill2 = this._sawmill;
				string str2 = "Prototype ";
				EntityPrototype entityPrototype2 = this.EntityManager.GetComponent<MetaDataComponent>(construction.Owner).EntityPrototype;
				sawmill2.Warning(str2 + ((entityPrototype2 != null) ? entityPrototype2.ID : null) + "'s construction component has an invalid node specified.");
				return;
			}
			ConstructionGraphEdge edge = null;
			int? edgeIndex2 = construction.EdgeIndex;
			if (edgeIndex2 != null)
			{
				int edgeIndex = edgeIndex2.GetValueOrDefault();
				ConstructionGraphEdge currentEdge = this.GetEdgeFromNode(node, edgeIndex);
				if (currentEdge == null)
				{
					ISawmill sawmill3 = this._sawmill;
					string str3 = "Prototype ";
					EntityPrototype entityPrototype3 = this.EntityManager.GetComponent<MetaDataComponent>(construction.Owner).EntityPrototype;
					sawmill3.Warning(str3 + ((entityPrototype3 != null) ? entityPrototype3.ID : null) + "'s construction component has an invalid edge index specified.");
					return;
				}
				edge = currentEdge;
			}
			string targetNodeId = construction.TargetNode;
			if (targetNodeId != null)
			{
				ConstructionGraphNode targetNode = this.GetNodeFromGraph(graph, targetNodeId);
				if (targetNode == null)
				{
					ISawmill sawmill4 = this._sawmill;
					string str4 = "Prototype ";
					EntityPrototype entityPrototype4 = this.EntityManager.GetComponent<MetaDataComponent>(construction.Owner).EntityPrototype;
					sawmill4.Warning(str4 + ((entityPrototype4 != null) ? entityPrototype4.ID : null) + "'s construction component has an invalid target node specified.");
					return;
				}
				this.UpdatePathfinding(uid, graph, node, targetNode, edge, construction);
			}
		}

		// Token: 0x06002063 RID: 8291 RVA: 0x000A9244 File Offset: 0x000A7444
		private void OnConstructionStartup(EntityUid uid, ConstructionComponent construction, ComponentStartup args)
		{
			ConstructionGraphNode node = this.GetCurrentNode(uid, construction);
			if (node == null)
			{
				return;
			}
			this.PerformActions(uid, null, node.Actions);
		}

		// Token: 0x06002064 RID: 8292 RVA: 0x000A9274 File Offset: 0x000A7474
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this.UpdateInteractions();
		}

		// Token: 0x06002065 RID: 8293 RVA: 0x000A9283 File Offset: 0x000A7483
		private void InitializeGraphs()
		{
		}

		// Token: 0x06002066 RID: 8294 RVA: 0x000A9285 File Offset: 0x000A7485
		public bool AddContainer(EntityUid uid, string container, [Nullable(2)] ConstructionComponent construction = null)
		{
			return base.Resolve<ConstructionComponent>(uid, ref construction, true) && construction.Containers.Add(container);
		}

		// Token: 0x06002067 RID: 8295 RVA: 0x000A92A4 File Offset: 0x000A74A4
		[NullableContext(2)]
		public ConstructionGraphPrototype GetCurrentGraph(EntityUid uid, ConstructionComponent construction = null)
		{
			if (!base.Resolve<ConstructionComponent>(uid, ref construction, false))
			{
				return null;
			}
			ConstructionGraphPrototype graph;
			if (!this._prototypeManager.TryIndex<ConstructionGraphPrototype>(construction.Graph, ref graph))
			{
				return null;
			}
			return graph;
		}

		// Token: 0x06002068 RID: 8296 RVA: 0x000A92D8 File Offset: 0x000A74D8
		[NullableContext(2)]
		public ConstructionGraphNode GetCurrentNode(EntityUid uid, ConstructionComponent construction = null)
		{
			if (!base.Resolve<ConstructionComponent>(uid, ref construction, false))
			{
				return null;
			}
			string nodeIdentifier = construction.Node;
			if (nodeIdentifier == null)
			{
				return null;
			}
			ConstructionGraphPrototype graph = this.GetCurrentGraph(uid, construction);
			if (graph != null)
			{
				return this.GetNodeFromGraph(graph, nodeIdentifier);
			}
			return null;
		}

		// Token: 0x06002069 RID: 8297 RVA: 0x000A9318 File Offset: 0x000A7518
		[NullableContext(2)]
		public ConstructionGraphEdge GetCurrentEdge(EntityUid uid, ConstructionComponent construction = null)
		{
			if (!base.Resolve<ConstructionComponent>(uid, ref construction, false))
			{
				return null;
			}
			int? edgeIndex2 = construction.EdgeIndex;
			if (edgeIndex2 == null)
			{
				return null;
			}
			int edgeIndex = edgeIndex2.GetValueOrDefault();
			ConstructionGraphNode node = this.GetCurrentNode(uid, construction);
			if (node != null)
			{
				return this.GetEdgeFromNode(node, edgeIndex);
			}
			return null;
		}

		// Token: 0x0600206A RID: 8298 RVA: 0x000A9368 File Offset: 0x000A7568
		[NullableContext(2)]
		public ConstructionGraphStep GetCurrentStep(EntityUid uid, ConstructionComponent construction = null)
		{
			if (!base.Resolve<ConstructionComponent>(uid, ref construction, false))
			{
				return null;
			}
			ConstructionGraphEdge edge = this.GetCurrentEdge(uid, construction);
			if (edge == null)
			{
				return null;
			}
			return this.GetStepFromEdge(edge, construction.StepIndex);
		}

		// Token: 0x0600206B RID: 8299 RVA: 0x000A93A0 File Offset: 0x000A75A0
		[NullableContext(2)]
		public ConstructionGraphNode GetTargetNode(EntityUid uid, ConstructionComponent construction)
		{
			if (!base.Resolve<ConstructionComponent>(uid, ref construction, true))
			{
				return null;
			}
			string targetNodeId = construction.TargetNode;
			if (targetNodeId == null)
			{
				return null;
			}
			ConstructionGraphPrototype graph = this.GetCurrentGraph(uid, construction);
			if (graph == null)
			{
				return null;
			}
			return this.GetNodeFromGraph(graph, targetNodeId);
		}

		// Token: 0x0600206C RID: 8300 RVA: 0x000A93E0 File Offset: 0x000A75E0
		[NullableContext(2)]
		public ConstructionGraphEdge GetTargetEdge(EntityUid uid, ConstructionComponent construction)
		{
			if (!base.Resolve<ConstructionComponent>(uid, ref construction, true))
			{
				return null;
			}
			int? targetEdgeIndex2 = construction.TargetEdgeIndex;
			if (targetEdgeIndex2 == null)
			{
				return null;
			}
			int targetEdgeIndex = targetEdgeIndex2.GetValueOrDefault();
			ConstructionGraphNode node = this.GetCurrentNode(uid, construction);
			if (node == null)
			{
				return null;
			}
			return this.GetEdgeFromNode(node, targetEdgeIndex);
		}

		// Token: 0x0600206D RID: 8301 RVA: 0x000A9430 File Offset: 0x000A7630
		[NullableContext(2)]
		[return: TupleElementNames(new string[]
		{
			"edge",
			"step"
		})]
		[return: Nullable(new byte[]
		{
			0,
			2,
			2
		})]
		public ValueTuple<ConstructionGraphEdge, ConstructionGraphStep> GetCurrentEdgeAndStep(EntityUid uid, ConstructionComponent construction = null)
		{
			if (!base.Resolve<ConstructionComponent>(uid, ref construction, false))
			{
				return default(ValueTuple<ConstructionGraphEdge, ConstructionGraphStep>);
			}
			ConstructionGraphEdge edge = this.GetCurrentEdge(uid, construction);
			if (edge == null)
			{
				return default(ValueTuple<ConstructionGraphEdge, ConstructionGraphStep>);
			}
			ConstructionGraphStep step = this.GetStepFromEdge(edge, construction.StepIndex);
			return new ValueTuple<ConstructionGraphEdge, ConstructionGraphStep>(edge, step);
		}

		// Token: 0x0600206E RID: 8302 RVA: 0x000A9480 File Offset: 0x000A7680
		[return: Nullable(2)]
		public ConstructionGraphNode GetNodeFromGraph(ConstructionGraphPrototype graph, string id)
		{
			ConstructionGraphNode node;
			if (!graph.Nodes.TryGetValue(id, out node))
			{
				return null;
			}
			return node;
		}

		// Token: 0x0600206F RID: 8303 RVA: 0x000A94A0 File Offset: 0x000A76A0
		[return: Nullable(2)]
		public ConstructionGraphEdge GetEdgeFromNode(ConstructionGraphNode node, int index)
		{
			if (node.Edges.Count <= index)
			{
				return null;
			}
			return node.Edges[index];
		}

		// Token: 0x06002070 RID: 8304 RVA: 0x000A94BE File Offset: 0x000A76BE
		[return: Nullable(2)]
		public ConstructionGraphStep GetStepFromEdge(ConstructionGraphEdge edge, int index)
		{
			if (edge.Steps.Count <= index)
			{
				return null;
			}
			return edge.Steps[index];
		}

		// Token: 0x06002071 RID: 8305 RVA: 0x000A94DC File Offset: 0x000A76DC
		public bool ChangeNode(EntityUid uid, EntityUid? userUid, string id, bool performActions = true, [Nullable(2)] ConstructionComponent construction = null)
		{
			if (!base.Resolve<ConstructionComponent>(uid, ref construction, true))
			{
				return false;
			}
			ConstructionGraphPrototype graph = this.GetCurrentGraph(uid, construction);
			if (graph != null)
			{
				ConstructionGraphNode node = this.GetNodeFromGraph(graph, id);
				if (node != null)
				{
					string oldNode = construction.Node;
					construction.Node = id;
					if (userUid != null)
					{
						ISharedAdminLogManager adminLogger = this._adminLogger;
						LogType type = LogType.Construction;
						LogImpact impact = LogImpact.Low;
						LogStringHandler logStringHandler = new LogStringHandler(30, 4);
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(userUid.Value), "player", "ToPrettyString(userUid.Value)");
						logStringHandler.AppendLiteral(" changed ");
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "entity", "ToPrettyString(uid)");
						logStringHandler.AppendLiteral("'s node from \"");
						logStringHandler.AppendFormatted(oldNode);
						logStringHandler.AppendLiteral("\" to \"");
						logStringHandler.AppendFormatted(id);
						logStringHandler.AppendLiteral("\"");
						adminLogger.Add(type, impact, ref logStringHandler);
					}
					string newEntity = node.Entity;
					if (newEntity != null && this.ChangeEntity(uid, userUid, newEntity, construction, null, null, null) != null)
					{
						return true;
					}
					if (performActions)
					{
						this.PerformActions(uid, userUid, node.Actions);
					}
					if (!base.Exists(uid))
					{
						return false;
					}
					this.UpdatePathfinding(uid, construction);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002072 RID: 8306 RVA: 0x000A960C File Offset: 0x000A780C
		[NullableContext(2)]
		private EntityUid? ChangeEntity(EntityUid uid, EntityUid? userUid, [Nullable(1)] string newEntity, ConstructionComponent construction = null, MetaDataComponent metaData = null, TransformComponent transform = null, ContainerManagerComponent containerManager = null)
		{
			if (!base.Resolve<ConstructionComponent, MetaDataComponent, TransformComponent>(uid, ref construction, ref metaData, ref transform, true))
			{
				throw new Exception("Missing construction components");
			}
			EntityPrototype entityPrototype = metaData.EntityPrototype;
			if (newEntity == ((entityPrototype != null) ? entityPrototype.ID : null) || !this._prototypeManager.HasIndex<EntityPrototype>(newEntity))
			{
				return null;
			}
			base.Resolve<ContainerManagerComponent>(uid, ref containerManager, false);
			EntityUid newUid = this.EntityManager.CreateEntityUninitialized(newEntity, transform.Coordinates);
			ConstructionComponent newConstruction = this.EntityManager.EnsureComponent<ConstructionComponent>(newUid);
			newConstruction.Containers.UnionWith(construction.Containers);
			ContainerFillComponent containerFill;
			if (base.TryComp<ContainerFillComponent>(newUid, ref containerFill) && containerFill.IgnoreConstructionSpawn)
			{
				foreach (string id in newConstruction.Containers)
				{
					containerFill.Containers.Remove(id);
				}
			}
			this.EntityManager.InitializeAndStartEntity(newUid, null);
			this.ChangeGraph(newUid, userUid, construction.Graph, construction.Node, false, newConstruction);
			string targetNode = construction.TargetNode;
			if (targetNode != null)
			{
				this.SetPathfindingTarget(newUid, targetNode, newConstruction);
			}
			object ev;
			while (construction.InteractionQueue.TryDequeue(out ev))
			{
				newConstruction.InteractionQueue.Enqueue(ev);
			}
			if (newConstruction.InteractionQueue.Count > 0 && this._queuedUpdates.Add(newUid))
			{
				this._constructionUpdateQueue.Enqueue(newUid);
			}
			TransformComponent transformComponent = base.Transform(newUid);
			transformComponent.LocalRotation = transform.LocalRotation;
			transformComponent.Anchored = transform.Anchored;
			if (containerManager != null)
			{
				ContainerManagerComponent newContainerManager = this.EntityManager.EnsureComponent<ContainerManagerComponent>(newUid);
				foreach (string container in construction.Containers)
				{
					IContainer ourContainer;
					if (this._container.TryGetContainer(uid, container, ref ourContainer, containerManager))
					{
						Container otherContainer = this._container.EnsureContainer<Container>(newUid, container, newContainerManager);
						for (int i = ourContainer.ContainedEntities.Count - 1; i >= 0; i--)
						{
							EntityUid entity = ourContainer.ContainedEntities[i];
							ourContainer.ForceRemove(entity, null, null);
							otherContainer.Insert(entity, null, null, null, null, null);
						}
					}
				}
			}
			base.QueueDel(uid);
			return new EntityUid?(newUid);
		}

		// Token: 0x06002073 RID: 8307 RVA: 0x000A9880 File Offset: 0x000A7A80
		public bool ChangeGraph(EntityUid uid, EntityUid? userUid, string graphId, string nodeId, bool performActions = true, [Nullable(2)] ConstructionComponent construction = null)
		{
			if (!base.Resolve<ConstructionComponent>(uid, ref construction, true))
			{
				return false;
			}
			ConstructionGraphPrototype graph;
			if (!this._prototypeManager.TryIndex<ConstructionGraphPrototype>(graphId, ref graph))
			{
				return false;
			}
			if (this.GetNodeFromGraph(graph, nodeId) == null)
			{
				return false;
			}
			construction.Graph = graphId;
			return this.ChangeNode(uid, userUid, nodeId, performActions, construction);
		}

		// Token: 0x06002074 RID: 8308 RVA: 0x000A98D0 File Offset: 0x000A7AD0
		private void InitializeGuided()
		{
			base.SubscribeNetworkEvent<SharedConstructionSystem.RequestConstructionGuide>(new EntitySessionEventHandler<SharedConstructionSystem.RequestConstructionGuide>(this.OnGuideRequested), null, null);
			base.SubscribeLocalEvent<ConstructionComponent, GetVerbsEvent<Verb>>(new ComponentEventHandler<ConstructionComponent, GetVerbsEvent<Verb>>(this.AddDeconstructVerb), null, null);
			base.SubscribeLocalEvent<ConstructionComponent, ExaminedEvent>(new ComponentEventHandler<ConstructionComponent, ExaminedEvent>(this.HandleConstructionExamined), null, null);
		}

		// Token: 0x06002075 RID: 8309 RVA: 0x000A9910 File Offset: 0x000A7B10
		private void OnGuideRequested(SharedConstructionSystem.RequestConstructionGuide msg, EntitySessionEventArgs args)
		{
			ConstructionPrototype prototype;
			if (!this._prototypeManager.TryIndex<ConstructionPrototype>(msg.ConstructionId, ref prototype))
			{
				return;
			}
			ConstructionGuide guide = this.GetGuide(prototype);
			if (guide != null)
			{
				base.RaiseNetworkEvent(new SharedConstructionSystem.ResponseConstructionGuide(msg.ConstructionId, guide), args.SenderSession.ConnectedClient);
			}
		}

		// Token: 0x06002076 RID: 8310 RVA: 0x000A995C File Offset: 0x000A7B5C
		private void AddDeconstructVerb(EntityUid uid, ConstructionComponent component, GetVerbsEvent<Verb> args)
		{
			if (!args.CanAccess || !args.CanInteract || args.Hands == null)
			{
				return;
			}
			if (component.TargetNode == component.DeconstructionNode || component.Node == component.DeconstructionNode)
			{
				return;
			}
			Verb verb = new Verb();
			verb.Text = Loc.GetString("deconstructible-verb-begin-deconstruct");
			verb.Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/hammer_scaled.svg.192dpi.png", "/"));
			verb.Act = delegate()
			{
				this.SetPathfindingTarget(uid, component.DeconstructionNode, component);
				if (component.TargetNode == null)
				{
					this._popup.PopupEntity(Loc.GetString("deconstructible-verb-activate-no-target-text"), uid, uid, PopupType.Small);
					return;
				}
				this._popup.PopupEntity(Loc.GetString("deconstructible-verb-activate-text"), args.User, args.User, PopupType.Small);
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x06002077 RID: 8311 RVA: 0x000A9A44 File Offset: 0x000A7C44
		private void HandleConstructionExamined(EntityUid uid, ConstructionComponent component, ExaminedEvent args)
		{
			ConstructionGraphNode target = this.GetTargetNode(uid, component);
			if (target != null)
			{
				args.PushMarkup(Loc.GetString("construction-component-to-create-header", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("targetName", target.Name)
				}) + "\n");
			}
			if (component.EdgeIndex == null)
			{
				ConstructionGraphEdge targetEdge = this.GetTargetEdge(uid, component);
				if (targetEdge != null)
				{
					bool preventStepExamine = false;
					foreach (IGraphCondition condition in targetEdge.Conditions)
					{
						preventStepExamine |= condition.DoExamine(args);
					}
					if (!preventStepExamine)
					{
						targetEdge.Steps[0].DoExamine(args);
					}
					return;
				}
			}
			ConstructionGraphEdge edge = this.GetCurrentEdge(uid, component);
			if (edge != null)
			{
				bool preventStepExamine2 = false;
				foreach (IGraphCondition condition2 in edge.Conditions)
				{
					preventStepExamine2 |= condition2.DoExamine(args);
				}
				if (!preventStepExamine2 && component.StepIndex < edge.Steps.Count)
				{
					edge.Steps[component.StepIndex].DoExamine(args);
				}
			}
		}

		// Token: 0x06002078 RID: 8312 RVA: 0x000A9B9C File Offset: 0x000A7D9C
		[return: Nullable(2)]
		private ConstructionGuide GetGuide(ConstructionPrototype construction)
		{
			ConstructionGuide guide;
			if (this._guideCache.TryGetValue(construction, out guide))
			{
				return guide;
			}
			ConstructionGraphPrototype graph;
			if (!this._prototypeManager.TryIndex<ConstructionGraphPrototype>(construction.Graph, ref graph))
			{
				return null;
			}
			ConstructionGraphNode startNode = this.GetNodeFromGraph(graph, construction.StartNode);
			if (startNode != null)
			{
				ConstructionGraphNode targetNode = this.GetNodeFromGraph(graph, construction.TargetNode);
				if (targetNode != null)
				{
					ConstructionGraphNode[] path = graph.Path(construction.StartNode, construction.TargetNode);
					if (path == null || path.Length == 0)
					{
						return null;
					}
					int step = 1;
					List<ConstructionGuideEntry> entries = new List<ConstructionGuideEntry>
					{
						new ConstructionGuideEntry
						{
							Localization = ((construction.Type == ConstructionType.Structure) ? "construction-presenter-to-build" : "construction-presenter-to-craft"),
							EntryNumber = new int?(step)
						}
					};
					HashSet<string> conditions = new HashSet<string>();
					ConstructionGraphNode node = startNode;
					int index = 0;
					while (node != targetNode)
					{
						ConstructionGraphEdge edge;
						if (!node.TryGetEdge(path[index].Name, out edge))
						{
							return null;
						}
						if (step == 1)
						{
							foreach (ConstructionGraphStep constructionGraphStep in edge.Steps)
							{
								EntityInsertConstructionGraphStep insertStep = constructionGraphStep as EntityInsertConstructionGraphStep;
								if (insertStep == null)
								{
									return null;
								}
								entries.Add(insertStep.GenerateGuideEntry());
							}
							foreach (IConstructionCondition constructionCondition in construction.Conditions)
							{
								ConstructionGuideEntry conditionEntry = constructionCondition.GenerateGuideEntry();
								if (conditionEntry != null)
								{
									conditionEntry.Padding += 4;
									entries.Add(conditionEntry);
								}
							}
							step++;
							node = path[index++];
							if (node != targetNode)
							{
								entries.Add(new ConstructionGuideEntry());
								continue;
							}
							continue;
						}
						HashSet<string> old = conditions;
						conditions = new HashSet<string>();
						foreach (IGraphCondition graphCondition in edge.Conditions)
						{
							foreach (ConstructionGuideEntry conditionEntry2 in graphCondition.GenerateGuideEntry())
							{
								conditions.Add(conditionEntry2.Localization);
								if (conditionEntry2.EntryNumber != null)
								{
									conditionEntry2.EntryNumber = new int?(step++);
								}
								else
								{
									if (old.Contains(conditionEntry2.Localization))
									{
										continue;
									}
									conditionEntry2.Padding += 4;
								}
								entries.Add(conditionEntry2);
							}
						}
						foreach (ConstructionGraphStep constructionGraphStep2 in edge.Steps)
						{
							ConstructionGuideEntry entry = constructionGraphStep2.GenerateGuideEntry();
							entry.EntryNumber = new int?(step++);
							entries.Add(entry);
						}
						node = path[index++];
					}
					guide = new ConstructionGuide(entries.ToArray());
					this._guideCache[construction] = guide;
					return guide;
				}
			}
			return null;
		}

		// Token: 0x06002079 RID: 8313 RVA: 0x000A9EDC File Offset: 0x000A80DC
		private void InitializeInitial()
		{
			base.SubscribeNetworkEvent<SharedConstructionSystem.TryStartStructureConstructionMessage>(new EntitySessionEventHandler<SharedConstructionSystem.TryStartStructureConstructionMessage>(this.HandleStartStructureConstruction), null, null);
			base.SubscribeNetworkEvent<SharedConstructionSystem.TryStartItemConstructionMessage>(new EntitySessionEventHandler<SharedConstructionSystem.TryStartItemConstructionMessage>(this.HandleStartItemConstruction), null, null);
		}

		// Token: 0x0600207A RID: 8314 RVA: 0x000A9F06 File Offset: 0x000A8106
		private IEnumerable<EntityUid> EnumerateNearby(EntityUid user)
		{
			foreach (EntityUid item in this._handsSystem.EnumerateHeld(user, null))
			{
				ServerStorageComponent storage;
				if (base.TryComp<ServerStorageComponent>(item, ref storage))
				{
					foreach (EntityUid storedEntity in storage.StoredEntities)
					{
						yield return storedEntity;
					}
					IEnumerator<EntityUid> enumerator2 = null;
				}
				yield return item;
				item = default(EntityUid);
			}
			IEnumerator<EntityUid> enumerator = null;
			InventorySystem.ContainerSlotEnumerator containerSlotEnumerator;
			if (this._inventorySystem.TryGetContainerSlotEnumerator(user, out containerSlotEnumerator, null))
			{
				ContainerSlot containerSlot;
				while (containerSlotEnumerator.MoveNext(out containerSlot))
				{
					if (containerSlot.ContainedEntity != null)
					{
						ServerStorageComponent storage2;
						if (this.EntityManager.TryGetComponent<ServerStorageComponent>(containerSlot.ContainedEntity.Value, ref storage2))
						{
							foreach (EntityUid storedEntity2 in storage2.StoredEntities)
							{
								yield return storedEntity2;
							}
							enumerator = null;
						}
						yield return containerSlot.ContainedEntity.Value;
					}
				}
			}
			MapCoordinates pos = base.Transform(user).MapPosition;
			foreach (EntityUid near in this._lookupSystem.GetEntitiesInRange(pos, 2f, 43))
			{
				if (!(near == user) && this._interactionSystem.InRangeUnobstructed(pos, near, 2f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null) && this._container.IsInSameOrParentContainer(user, near))
				{
					yield return near;
				}
			}
			HashSet<EntityUid>.Enumerator enumerator3 = default(HashSet<EntityUid>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x0600207B RID: 8315 RVA: 0x000A9F20 File Offset: 0x000A8120
		private Task<EntityUid?> Construct(EntityUid user, string materialContainer, ConstructionGraphPrototype graph, ConstructionGraphEdge edge, ConstructionGraphNode targetNode)
		{
			ConstructionSystem.<Construct>d__50 <Construct>d__;
			<Construct>d__.<>t__builder = AsyncTaskMethodBuilder<EntityUid?>.Create();
			<Construct>d__.<>4__this = this;
			<Construct>d__.user = user;
			<Construct>d__.materialContainer = materialContainer;
			<Construct>d__.graph = graph;
			<Construct>d__.edge = edge;
			<Construct>d__.targetNode = targetNode;
			<Construct>d__.<>1__state = -1;
			<Construct>d__.<>t__builder.Start<ConstructionSystem.<Construct>d__50>(ref <Construct>d__);
			return <Construct>d__.<>t__builder.Task;
		}

		// Token: 0x0600207C RID: 8316 RVA: 0x000A9F90 File Offset: 0x000A8190
		private void HandleStartItemConstruction(SharedConstructionSystem.TryStartItemConstructionMessage ev, EntitySessionEventArgs args)
		{
			ConstructionSystem.<HandleStartItemConstruction>d__51 <HandleStartItemConstruction>d__;
			<HandleStartItemConstruction>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<HandleStartItemConstruction>d__.<>4__this = this;
			<HandleStartItemConstruction>d__.ev = ev;
			<HandleStartItemConstruction>d__.args = args;
			<HandleStartItemConstruction>d__.<>1__state = -1;
			<HandleStartItemConstruction>d__.<>t__builder.Start<ConstructionSystem.<HandleStartItemConstruction>d__51>(ref <HandleStartItemConstruction>d__);
		}

		// Token: 0x0600207D RID: 8317 RVA: 0x000A9FD8 File Offset: 0x000A81D8
		private void HandleStartStructureConstruction(SharedConstructionSystem.TryStartStructureConstructionMessage ev, EntitySessionEventArgs args)
		{
			ConstructionSystem.<HandleStartStructureConstruction>d__52 <HandleStartStructureConstruction>d__;
			<HandleStartStructureConstruction>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<HandleStartStructureConstruction>d__.<>4__this = this;
			<HandleStartStructureConstruction>d__.ev = ev;
			<HandleStartStructureConstruction>d__.args = args;
			<HandleStartStructureConstruction>d__.<>1__state = -1;
			<HandleStartStructureConstruction>d__.<>t__builder.Start<ConstructionSystem.<HandleStartStructureConstruction>d__52>(ref <HandleStartStructureConstruction>d__);
		}

		// Token: 0x0600207E RID: 8318 RVA: 0x000AA020 File Offset: 0x000A8220
		private void InitializeInteractions()
		{
			base.SubscribeLocalEvent<ConstructionSystem.ConstructionDoAfterComplete>(new EntityEventHandler<ConstructionSystem.ConstructionDoAfterComplete>(this.OnDoAfterComplete), null, null);
			base.SubscribeLocalEvent<ConstructionSystem.ConstructionDoAfterCancelled>(new EntityEventHandler<ConstructionSystem.ConstructionDoAfterCancelled>(this.OnDoAfterCancelled), null, null);
			base.SubscribeLocalEvent<ConstructionComponent, ConstructionSystem.ConstructionDoAfterComplete>(new ComponentEventHandler<ConstructionComponent, ConstructionSystem.ConstructionDoAfterComplete>(this.EnqueueEvent), null, null);
			base.SubscribeLocalEvent<ConstructionComponent, ConstructionSystem.ConstructionDoAfterCancelled>(new ComponentEventHandler<ConstructionComponent, ConstructionSystem.ConstructionDoAfterCancelled>(this.EnqueueEvent), null, null);
			base.SubscribeLocalEvent<ConstructionComponent, DoAfterEvent<ConstructionSystem.ConstructionData>>(new ComponentEventHandler<ConstructionComponent, DoAfterEvent<ConstructionSystem.ConstructionData>>(this.OnDoAfter), null, null);
			base.SubscribeLocalEvent<ConstructionComponent, InteractUsingEvent>(new ComponentEventHandler<ConstructionComponent, InteractUsingEvent>(this.EnqueueEvent), new Type[]
			{
				typeof(AnchorableSystem)
			}, null);
			base.SubscribeLocalEvent<ConstructionComponent, OnTemperatureChangeEvent>(new ComponentEventHandler<ConstructionComponent, OnTemperatureChangeEvent>(this.EnqueueEvent), null, null);
		}

		// Token: 0x0600207F RID: 8319 RVA: 0x000AA0CC File Offset: 0x000A82CC
		private ConstructionSystem.HandleResult HandleEvent(EntityUid uid, object ev, bool validation, [Nullable(2)] ConstructionComponent construction = null)
		{
			if (!base.Resolve<ConstructionComponent>(uid, ref construction, true))
			{
				return ConstructionSystem.HandleResult.False;
			}
			ConstructionGraphNode node = this.GetCurrentNode(uid, construction);
			if (node == null)
			{
				return ConstructionSystem.HandleResult.False;
			}
			ConstructionGraphEdge edge = this.GetCurrentEdge(uid, construction);
			if (edge != null)
			{
				ConstructionSystem.HandleResult result = this.HandleEdge(uid, ev, edge, validation, construction);
				if (!validation && result == ConstructionSystem.HandleResult.False && construction.StepIndex == 0)
				{
					construction.EdgeIndex = null;
				}
				return result;
			}
			return this.HandleNode(uid, ev, node, validation, construction);
		}

		// Token: 0x06002080 RID: 8320 RVA: 0x000AA140 File Offset: 0x000A8340
		private ConstructionSystem.HandleResult HandleNode(EntityUid uid, object ev, ConstructionGraphNode node, bool validation, [Nullable(2)] ConstructionComponent construction = null)
		{
			if (!base.Resolve<ConstructionComponent>(uid, ref construction, true))
			{
				return ConstructionSystem.HandleResult.False;
			}
			construction.StepIndex = 0;
			int i = 0;
			while (i < node.Edges.Count)
			{
				ConstructionGraphEdge edge = node.Edges[i];
				ConstructionSystem.HandleResult result = this.HandleEdge(uid, ev, edge, validation, construction);
				if (result != ConstructionSystem.HandleResult.False)
				{
					if (result != ConstructionSystem.HandleResult.True)
					{
						if (result == ConstructionSystem.HandleResult.DoAfter)
						{
							construction.EdgeIndex = new int?(i);
						}
						return result;
					}
					if (construction.Node != node.Name)
					{
						return result;
					}
					construction.EdgeIndex = new int?(i);
					this.UpdatePathfinding(uid, construction);
					return result;
				}
				else
				{
					i++;
				}
			}
			return ConstructionSystem.HandleResult.False;
		}

		// Token: 0x06002081 RID: 8321 RVA: 0x000AA1E0 File Offset: 0x000A83E0
		private ConstructionSystem.HandleResult HandleEdge(EntityUid uid, object ev, ConstructionGraphEdge edge, bool validation, [Nullable(2)] ConstructionComponent construction = null)
		{
			if (!base.Resolve<ConstructionComponent>(uid, ref construction, true))
			{
				return ConstructionSystem.HandleResult.False;
			}
			ConstructionGraphStep step = this.GetStepFromEdge(edge, construction.StepIndex);
			if (step == null)
			{
				ISawmill sawmill = this._sawmill;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(63, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Called ");
				defaultInterpolatedStringHandler.AppendFormatted("HandleEdge");
				defaultInterpolatedStringHandler.AppendLiteral(" on entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
				defaultInterpolatedStringHandler.AppendLiteral(" but the current state is not valid for that!");
				sawmill.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return ConstructionSystem.HandleResult.False;
			}
			if (!this.CheckConditions(uid, edge.Conditions))
			{
				return ConstructionSystem.HandleResult.False;
			}
			EntityUid? user;
			ConstructionSystem.HandleResult handle = this.HandleStep(uid, ev, step, validation, out user, construction);
			if (handle != ConstructionSystem.HandleResult.True)
			{
				return handle;
			}
			ConstructionComponent constructionComponent = construction;
			int stepIndex = constructionComponent.StepIndex;
			constructionComponent.StepIndex = stepIndex + 1;
			if (construction.StepIndex >= edge.Steps.Count)
			{
				this.PerformActions(uid, user, edge.Completed);
				construction.TargetEdgeIndex = null;
				construction.EdgeIndex = null;
				construction.StepIndex = 0;
				this.ChangeNode(uid, user, edge.Target, true, construction);
			}
			return ConstructionSystem.HandleResult.True;
		}

		// Token: 0x06002082 RID: 8322 RVA: 0x000AA2FC File Offset: 0x000A84FC
		private ConstructionSystem.HandleResult HandleStep(EntityUid uid, object ev, ConstructionGraphStep step, bool validation, out EntityUid? user, [Nullable(2)] ConstructionComponent construction = null)
		{
			user = null;
			if (!base.Resolve<ConstructionComponent>(uid, ref construction, true))
			{
				return ConstructionSystem.HandleResult.False;
			}
			ConstructionSystem.HandleResult handle = this.HandleInteraction(uid, ev, step, validation, out user, construction);
			if (handle != ConstructionSystem.HandleResult.True)
			{
				return handle;
			}
			this.PerformActions(uid, user, step.Completed);
			this.UpdatePathfinding(uid, construction);
			return ConstructionSystem.HandleResult.True;
		}

		// Token: 0x06002083 RID: 8323 RVA: 0x000AA354 File Offset: 0x000A8554
		private ConstructionSystem.HandleResult HandleInteraction(EntityUid uid, object ev, ConstructionGraphStep step, bool validation, out EntityUid? user, [Nullable(2)] ConstructionComponent construction = null)
		{
			user = null;
			if (!base.Resolve<ConstructionComponent>(uid, ref construction, true))
			{
				return ConstructionSystem.HandleResult.False;
			}
			ConstructionSystem.DoAfterState doAfterState = validation ? ConstructionSystem.DoAfterState.Validation : ConstructionSystem.DoAfterState.None;
			if (!validation)
			{
				ConstructionSystem.ConstructionDoAfterComplete complete = ev as ConstructionSystem.ConstructionDoAfterComplete;
				if (complete == null)
				{
					ConstructionSystem.ConstructionDoAfterCancelled cancelled = ev as ConstructionSystem.ConstructionDoAfterCancelled;
					if (cancelled != null)
					{
						ev = cancelled.WrappedEvent;
						doAfterState = ConstructionSystem.DoAfterState.Cancelled;
						object customData = cancelled.CustomData;
						construction.WaitingDoAfter = false;
					}
				}
				else
				{
					ev = complete.WrappedEvent;
					doAfterState = ConstructionSystem.DoAfterState.Completed;
					object customData2 = complete.CustomData;
					construction.WaitingDoAfter = false;
				}
			}
			if (construction.WaitingDoAfter)
			{
				return ConstructionSystem.HandleResult.False;
			}
			EntityInsertConstructionGraphStep insertStep = step as EntityInsertConstructionGraphStep;
			if (insertStep == null)
			{
				ToolConstructionGraphStep toolInsertStep = step as ToolConstructionGraphStep;
				if (toolInsertStep == null)
				{
					TemperatureConstructionGraphStep temperatureChangeStep = step as TemperatureConstructionGraphStep;
					if (temperatureChangeStep == null)
					{
						throw new ArgumentOutOfRangeException("step", "You need to code your ConstructionGraphStep behavior by adding a case to the switch.");
					}
					if (ev is OnTemperatureChangeEvent)
					{
						TemperatureComponent tempComp;
						if (base.TryComp<TemperatureComponent>(uid, ref tempComp) && (temperatureChangeStep.MinTemperature == null || tempComp.CurrentTemperature >= temperatureChangeStep.MinTemperature.Value) && (temperatureChangeStep.MaxTemperature == null || tempComp.CurrentTemperature <= temperatureChangeStep.MaxTemperature.Value))
						{
							return ConstructionSystem.HandleResult.True;
						}
						return ConstructionSystem.HandleResult.False;
					}
				}
				else
				{
					InteractUsingEvent interactUsing = ev as InteractUsingEvent;
					if (interactUsing != null)
					{
						user = new EntityUid?(interactUsing.User);
						if (doAfterState == ConstructionSystem.DoAfterState.Validation)
						{
							if (!this._toolSystem.HasQuality(interactUsing.Used, toolInsertStep.Tool, null))
							{
								return ConstructionSystem.HandleResult.False;
							}
							return ConstructionSystem.HandleResult.Validated;
						}
						else if (doAfterState != ConstructionSystem.DoAfterState.None)
						{
							if (doAfterState != ConstructionSystem.DoAfterState.Completed)
							{
								return ConstructionSystem.HandleResult.False;
							}
							return ConstructionSystem.HandleResult.True;
						}
						else
						{
							object ev2 = new ConstructionSystem.ConstructionDoAfterComplete(uid, ev, null);
							float fuel = toolInsertStep.Fuel;
							object cancelledEv = new ConstructionSystem.ConstructionDoAfterCancelled(uid, ev, null);
							EntityUid? entityUid = null;
							ToolEventData toolEvData = new ToolEventData(ev2, fuel, cancelledEv, entityUid);
							if (!this._toolSystem.UseTool(interactUsing.Used, interactUsing.User, new EntityUid?(uid), toolInsertStep.DoAfter, new string[]
							{
								toolInsertStep.Tool
							}, toolEvData, 0f, null, null, null))
							{
								return ConstructionSystem.HandleResult.False;
							}
							if (toolInsertStep.DoAfter <= 0f)
							{
								return ConstructionSystem.HandleResult.True;
							}
							construction.WaitingDoAfter = true;
							return ConstructionSystem.HandleResult.DoAfter;
						}
					}
				}
			}
			else
			{
				InteractUsingEvent interactUsing2 = ev as InteractUsingEvent;
				if (interactUsing2 != null)
				{
					user = new EntityUid?(interactUsing2.User);
					if (doAfterState == ConstructionSystem.DoAfterState.Cancelled)
					{
						return ConstructionSystem.HandleResult.False;
					}
					EntityUid insert = interactUsing2.Used;
					if (!insertStep.EntityValid(insert, this.EntityManager, this._factory))
					{
						return ConstructionSystem.HandleResult.False;
					}
					if (doAfterState == ConstructionSystem.DoAfterState.Validation)
					{
						return ConstructionSystem.HandleResult.Validated;
					}
					if (doAfterState == ConstructionSystem.DoAfterState.None && insertStep.DoAfter > 0f)
					{
						ConstructionSystem.ConstructionData constructionData = new ConstructionSystem.ConstructionData(new ConstructionSystem.ConstructionDoAfterComplete(uid, ev, null), new ConstructionSystem.ConstructionDoAfterCancelled(uid, ev, null));
						EntityUid user2 = interactUsing2.User;
						float doAfter = step.DoAfter;
						EntityUid? entityUid = new EntityUid?(interactUsing2.Target);
						DoAfterEventArgs doAfterEventArgs = new DoAfterEventArgs(user2, doAfter, default(CancellationToken), entityUid, null)
						{
							BreakOnDamage = false,
							BreakOnStun = true,
							BreakOnTargetMove = true,
							BreakOnUserMove = true,
							NeedHand = true
						};
						this._doAfterSystem.DoAfter<ConstructionSystem.ConstructionData>(doAfterEventArgs, constructionData);
						construction.WaitingDoAfter = true;
						return ConstructionSystem.HandleResult.DoAfter;
					}
					MaterialConstructionGraphStep materialInsertStep = insertStep as MaterialConstructionGraphStep;
					if (materialInsertStep != null)
					{
						EntityUid? entityUid = this._stackSystem.Split(insert, materialInsertStep.Amount, base.Transform(interactUsing2.User).Coordinates, null);
						if (entityUid == null)
						{
							return ConstructionSystem.HandleResult.False;
						}
						EntityUid stack = entityUid.GetValueOrDefault();
						insert = stack;
					}
					if (!string.IsNullOrEmpty(insertStep.Store))
					{
						string store = insertStep.Store;
						construction.Containers.Add(store);
						this._container.EnsureContainer<Container>(uid, store, null).Insert(insert, null, null, null, null, null);
					}
					else
					{
						base.Del(insert);
					}
					return ConstructionSystem.HandleResult.True;
				}
			}
			return ConstructionSystem.HandleResult.False;
		}

		// Token: 0x06002084 RID: 8324 RVA: 0x000AA6D8 File Offset: 0x000A88D8
		public bool CheckConditions(EntityUid uid, IEnumerable<IGraphCondition> conditions)
		{
			using (IEnumerator<IGraphCondition> enumerator = conditions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.Condition(uid, this.EntityManager))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06002085 RID: 8325 RVA: 0x000AA730 File Offset: 0x000A8930
		public void PerformActions(EntityUid uid, EntityUid? userUid, IEnumerable<IGraphAction> actions)
		{
			foreach (IGraphAction action in actions)
			{
				if (!base.Exists(uid))
				{
					break;
				}
				action.PerformAction(uid, userUid, this.EntityManager);
			}
		}

		// Token: 0x06002086 RID: 8326 RVA: 0x000AA78C File Offset: 0x000A898C
		[NullableContext(2)]
		public void ResetEdge(EntityUid uid, ConstructionComponent construction = null)
		{
			if (!base.Resolve<ConstructionComponent>(uid, ref construction, true))
			{
				return;
			}
			construction.TargetEdgeIndex = null;
			construction.EdgeIndex = null;
			construction.StepIndex = 0;
			this.UpdatePathfinding(uid, construction);
		}

		// Token: 0x06002087 RID: 8327 RVA: 0x000AA7D4 File Offset: 0x000A89D4
		private void UpdateInteractions()
		{
			EntityUid uid;
			while (this._constructionUpdateQueue.TryDequeue(out uid))
			{
				this._queuedUpdates.Remove(uid);
				ConstructionComponent construction;
				if (base.TryComp<ConstructionComponent>(uid, ref construction))
				{
					try
					{
						object prev = null;
						string queued = string.Join(", ", from x in construction.InteractionQueue
						select x.GetType().Name);
						object interaction;
						while (construction.InteractionQueue.TryDequeue(out interaction))
						{
							if (construction.Deleted)
							{
								ISawmill sawmill = this._sawmill;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(143, 6);
								defaultInterpolatedStringHandler.AppendLiteral("Construction component was deleted while still processing interactions.");
								defaultInterpolatedStringHandler.AppendLiteral("Entity ");
								defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
								defaultInterpolatedStringHandler.AppendLiteral(", graph: ");
								defaultInterpolatedStringHandler.AppendFormatted(construction.Graph);
								defaultInterpolatedStringHandler.AppendLiteral(", ");
								defaultInterpolatedStringHandler.AppendLiteral("Previous: ");
								string text;
								if (prev == null)
								{
									text = null;
								}
								else
								{
									Type type = prev.GetType();
									text = ((type != null) ? type.Name : null);
								}
								defaultInterpolatedStringHandler.AppendFormatted(text ?? "null");
								defaultInterpolatedStringHandler.AppendLiteral(", ");
								defaultInterpolatedStringHandler.AppendLiteral("Next: ");
								defaultInterpolatedStringHandler.AppendFormatted(interaction.GetType().Name);
								defaultInterpolatedStringHandler.AppendLiteral(", ");
								defaultInterpolatedStringHandler.AppendLiteral("Initial Queue: ");
								defaultInterpolatedStringHandler.AppendFormatted(queued);
								defaultInterpolatedStringHandler.AppendLiteral(", ");
								defaultInterpolatedStringHandler.AppendLiteral("Remaining Queue: ");
								defaultInterpolatedStringHandler.AppendFormatted(string.Join(", ", from x in construction.InteractionQueue
								select x.GetType().Name));
								sawmill.Error(defaultInterpolatedStringHandler.ToStringAndClear());
								break;
							}
							prev = interaction;
							this.HandleEvent(uid, interaction, false, construction);
						}
						continue;
					}
					catch (Exception e)
					{
						ISawmill sawmill2 = this._sawmill;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(70, 2);
						defaultInterpolatedStringHandler.AppendLiteral("Caught exception while processing construction queue. Entity ");
						defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
						defaultInterpolatedStringHandler.AppendLiteral(", graph: ");
						defaultInterpolatedStringHandler.AppendFormatted(construction.Graph);
						sawmill2.Error(defaultInterpolatedStringHandler.ToStringAndClear());
						this._runtimeLog.LogException(e, "ConstructionSystem.UpdateInteractions");
						base.Del(uid);
						continue;
					}
					break;
				}
			}
		}

		// Token: 0x06002088 RID: 8328 RVA: 0x000AAA44 File Offset: 0x000A8C44
		private void EnqueueEvent(EntityUid uid, ConstructionComponent construction, object args)
		{
			HandledEntityEventArgs handled = args as HandledEntityEventArgs;
			if (handled != null)
			{
				if (handled.Handled)
				{
					return;
				}
				if (this.HandleEvent(uid, args, true, construction) != ConstructionSystem.HandleResult.Validated)
				{
					return;
				}
				handled.Handled = true;
			}
			construction.InteractionQueue.Enqueue(args);
			if (this._queuedUpdates.Add(uid))
			{
				this._constructionUpdateQueue.Enqueue(uid);
			}
		}

		// Token: 0x06002089 RID: 8329 RVA: 0x000AAAA0 File Offset: 0x000A8CA0
		private void OnDoAfter(EntityUid uid, ConstructionComponent component, DoAfterEvent<ConstructionSystem.ConstructionData> args)
		{
			if (!base.Exists(args.Args.Target) || args.Handled)
			{
				return;
			}
			if (args.Cancelled)
			{
				base.RaiseLocalEvent(args.Args.Target.Value, args.AdditionalData.CancelEvent, false);
				args.Handled = true;
			}
			base.RaiseLocalEvent(args.Args.Target.Value, args.AdditionalData.CompleteEvent, false);
			args.Handled = true;
		}

		// Token: 0x0600208A RID: 8330 RVA: 0x000AAB23 File Offset: 0x000A8D23
		private void OnDoAfterComplete(ConstructionSystem.ConstructionDoAfterComplete ev)
		{
			if (!base.Exists(ev.TargetUid))
			{
				return;
			}
			base.RaiseLocalEvent<ConstructionSystem.ConstructionDoAfterComplete>(ev.TargetUid, ev, false);
		}

		// Token: 0x0600208B RID: 8331 RVA: 0x000AAB42 File Offset: 0x000A8D42
		private void OnDoAfterCancelled(ConstructionSystem.ConstructionDoAfterCancelled ev)
		{
			if (!base.Exists(ev.TargetUid))
			{
				return;
			}
			base.RaiseLocalEvent<ConstructionSystem.ConstructionDoAfterCancelled>(ev.TargetUid, ev, false);
		}

		// Token: 0x0600208C RID: 8332 RVA: 0x000AAB61 File Offset: 0x000A8D61
		private void InitializeMachines()
		{
			base.SubscribeLocalEvent<MachineComponent, ComponentInit>(new ComponentEventHandler<MachineComponent, ComponentInit>(this.OnMachineInit), null, null);
			base.SubscribeLocalEvent<MachineComponent, MapInitEvent>(new ComponentEventHandler<MachineComponent, MapInitEvent>(this.OnMachineMapInit), null, null);
			base.SubscribeLocalEvent<MachineComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<MachineComponent, GetVerbsEvent<ExamineVerb>>(this.OnMachineExaminableVerb), null, null);
		}

		// Token: 0x0600208D RID: 8333 RVA: 0x000AAB9F File Offset: 0x000A8D9F
		private void OnMachineInit(EntityUid uid, MachineComponent component, ComponentInit args)
		{
			component.BoardContainer = this._container.EnsureContainer<Container>(uid, "machine_board", null);
			component.PartContainer = this._container.EnsureContainer<Container>(uid, "machine_parts", null);
		}

		// Token: 0x0600208E RID: 8334 RVA: 0x000AABD1 File Offset: 0x000A8DD1
		private void OnMachineMapInit(EntityUid uid, MachineComponent component, MapInitEvent args)
		{
			this.CreateBoardAndStockParts(component);
			this.RefreshParts(component);
		}

		// Token: 0x0600208F RID: 8335 RVA: 0x000AABE4 File Offset: 0x000A8DE4
		private void OnMachineExaminableVerb(EntityUid uid, MachineComponent component, GetVerbsEvent<ExamineVerb> args)
		{
			if (!args.CanInteract || !args.CanAccess)
			{
				return;
			}
			FormattedMessage markup = new FormattedMessage();
			base.RaiseLocalEvent<UpgradeExamineEvent>(uid, new UpgradeExamineEvent(ref markup), false);
			if (markup.IsEmpty)
			{
				return;
			}
			markup = FormattedMessage.FromMarkup(markup.ToMarkup().TrimEnd('\n'));
			ExamineVerb verb = new ExamineVerb
			{
				Act = delegate()
				{
					this._examineSystem.SendExamineTooltip(args.User, uid, markup, false, false);
				},
				Text = Loc.GetString("machine-upgrade-examinable-verb-text"),
				Message = Loc.GetString("machine-upgrade-examinable-verb-message"),
				Category = VerbCategory.Examine,
				Icon = new SpriteSpecifier.Texture(new ResourcePath("/Textures/Interface/VerbIcons/pickup.svg.192dpi.png", "/"))
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x06002090 RID: 8336 RVA: 0x000AACE4 File Offset: 0x000A8EE4
		public List<MachinePartComponent> GetAllParts(MachineComponent component)
		{
			List<MachinePartComponent> parts = new List<MachinePartComponent>();
			foreach (EntityUid entity in component.PartContainer.ContainedEntities)
			{
				MachinePartComponent machinePart;
				if (base.TryComp<MachinePartComponent>(entity, ref machinePart))
				{
					parts.Add(machinePart);
				}
			}
			return parts;
		}

		// Token: 0x06002091 RID: 8337 RVA: 0x000AAD48 File Offset: 0x000A8F48
		public Dictionary<string, float> GetPartsRatings(List<MachinePartComponent> parts)
		{
			Dictionary<string, float> output = new Dictionary<string, float>();
			using (IEnumerator<MachinePartPrototype> enumerator = this._prototypeManager.EnumeratePrototypes<MachinePartPrototype>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MachinePartPrototype type = enumerator.Current;
					float amount = 0f;
					float sumRating = 0f;
					Func<MachinePartComponent, bool> predicate;
					Func<MachinePartComponent, bool> <>9__0;
					if ((predicate = <>9__0) == null)
					{
						predicate = (<>9__0 = ((MachinePartComponent part) => part.PartType == type.ID));
					}
					foreach (MachinePartComponent part2 in parts.Where(predicate))
					{
						amount += 1f;
						sumRating += (float)part2.Rating;
					}
					float rating = (amount != 0f) ? (sumRating / amount) : 0f;
					output.Add(type.ID, rating);
				}
			}
			return output;
		}

		// Token: 0x06002092 RID: 8338 RVA: 0x000AAE54 File Offset: 0x000A9054
		public void RefreshParts(MachineComponent component)
		{
			List<MachinePartComponent> parts = this.GetAllParts(component);
			this.EntityManager.EventBus.RaiseLocalEvent<RefreshPartsEvent>(component.Owner, new RefreshPartsEvent
			{
				Parts = parts,
				PartRatings = this.GetPartsRatings(parts)
			}, true);
		}

		// Token: 0x06002093 RID: 8339 RVA: 0x000AAE9C File Offset: 0x000A909C
		public void CreateBoardAndStockParts(MachineComponent component)
		{
			Container boardContainer = this._container.EnsureContainer<Container>(component.Owner, "machine_board", null);
			Container partContainer = this._container.EnsureContainer<Container>(component.Owner, "machine_parts", null);
			if (string.IsNullOrEmpty(component.BoardPrototype))
			{
				return;
			}
			if (boardContainer.ContainedEntities.Count > 0)
			{
				return;
			}
			EntityUid board = this.EntityManager.SpawnEntity(component.BoardPrototype, base.Transform(component.Owner).Coordinates);
			if (!component.BoardContainer.Insert(board, null, null, null, null, null))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(65, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Couldn't insert board with prototype ");
				defaultInterpolatedStringHandler.AppendFormatted(component.BoardPrototype);
				defaultInterpolatedStringHandler.AppendLiteral(" to machine with prototype ");
				EntityPrototype entityPrototype = base.MetaData(component.Owner).EntityPrototype;
				defaultInterpolatedStringHandler.AppendFormatted(((entityPrototype != null) ? entityPrototype.ID : null) ?? "N/A");
				defaultInterpolatedStringHandler.AppendLiteral("!");
				throw new Exception(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			MachineBoardComponent machineBoard;
			if (!base.TryComp<MachineBoardComponent>(board, ref machineBoard))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Entity with prototype ");
				defaultInterpolatedStringHandler.AppendFormatted(component.BoardPrototype);
				defaultInterpolatedStringHandler.AppendLiteral(" doesn't have a ");
				defaultInterpolatedStringHandler.AppendFormatted("MachineBoardComponent");
				defaultInterpolatedStringHandler.AppendLiteral("!");
				throw new Exception(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			TransformComponent xform = base.Transform(component.Owner);
			foreach (KeyValuePair<string, int> keyValuePair in machineBoard.Requirements)
			{
				string text;
				int num;
				keyValuePair.Deconstruct(out text, out num);
				string part = text;
				int amount = num;
				MachinePartPrototype partProto = this._prototypeManager.Index<MachinePartPrototype>(part);
				for (int i = 0; i < amount; i++)
				{
					EntityUid p = this.EntityManager.SpawnEntity(partProto.StockPartPrototype, xform.Coordinates);
					if (!partContainer.Insert(p, null, null, null, null, null))
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(65, 2);
						defaultInterpolatedStringHandler.AppendLiteral("Couldn't insert machine part of type ");
						defaultInterpolatedStringHandler.AppendFormatted(part);
						defaultInterpolatedStringHandler.AppendLiteral(" to machine with prototype ");
						defaultInterpolatedStringHandler.AppendFormatted(partProto.StockPartPrototype ?? "N/A");
						defaultInterpolatedStringHandler.AppendLiteral("!");
						throw new Exception(defaultInterpolatedStringHandler.ToStringAndClear());
					}
				}
			}
			foreach (KeyValuePair<StackPrototype, int> keyValuePair2 in machineBoard.MaterialRequirements)
			{
				int num;
				StackPrototype stackPrototype;
				keyValuePair2.Deconstruct(out stackPrototype, out num);
				StackPrototype stackType = stackPrototype;
				int amount2 = num;
				EntityUid stack = this._stackSystem.Spawn(amount2, stackType, base.Transform(component.Owner).Coordinates);
				if (!partContainer.Insert(stack, null, null, null, null, null))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(68, 2);
					defaultInterpolatedStringHandler.AppendLiteral("Couldn't insert machine material of type ");
					defaultInterpolatedStringHandler.AppendFormatted<StackPrototype>(stackType);
					defaultInterpolatedStringHandler.AppendLiteral(" to machine with prototype ");
					EntityPrototype entityPrototype2 = base.MetaData(component.Owner).EntityPrototype;
					defaultInterpolatedStringHandler.AppendFormatted(((entityPrototype2 != null) ? entityPrototype2.ID : null) ?? "N/A");
					throw new Exception(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
			foreach (KeyValuePair<string, GenericPartInfo> keyValuePair3 in machineBoard.ComponentRequirements)
			{
				string text;
				GenericPartInfo genericPartInfo;
				keyValuePair3.Deconstruct(out text, out genericPartInfo);
				string compName = text;
				GenericPartInfo info = genericPartInfo;
				for (int j = 0; j < info.Amount; j++)
				{
					EntityUid c = this.EntityManager.SpawnEntity(info.DefaultPrototype, base.Transform(component.Owner).Coordinates);
					if (!partContainer.Insert(c, null, null, null, null, null))
					{
						string str = "Couldn't insert machine component part with default prototype '";
						string str2 = compName;
						string str3 = "' to machine with prototype ";
						EntityPrototype entityPrototype3 = base.MetaData(component.Owner).EntityPrototype;
						throw new Exception(str + str2 + str3 + (((entityPrototype3 != null) ? entityPrototype3.ID : null) ?? "N/A"));
					}
				}
			}
			foreach (KeyValuePair<string, GenericPartInfo> keyValuePair3 in machineBoard.TagRequirements)
			{
				string text;
				GenericPartInfo genericPartInfo;
				keyValuePair3.Deconstruct(out text, out genericPartInfo);
				string tagName = text;
				GenericPartInfo info2 = genericPartInfo;
				for (int k = 0; k < info2.Amount; k++)
				{
					EntityUid c2 = this.EntityManager.SpawnEntity(info2.DefaultPrototype, base.Transform(component.Owner).Coordinates);
					if (!partContainer.Insert(c2, null, null, null, null, null))
					{
						string str4 = "Couldn't insert machine component part with default prototype '";
						string str5 = tagName;
						string str6 = "' to machine with prototype ";
						EntityPrototype entityPrototype4 = base.MetaData(component.Owner).EntityPrototype;
						throw new Exception(str4 + str5 + str6 + (((entityPrototype4 != null) ? entityPrototype4.ID : null) ?? "N/A"));
					}
				}
			}
		}

		// Token: 0x06002094 RID: 8340 RVA: 0x000AB3C0 File Offset: 0x000A95C0
		[NullableContext(2)]
		public bool SetPathfindingTarget(EntityUid uid, string targetNodeId, ConstructionComponent construction = null)
		{
			if (!base.Resolve<ConstructionComponent>(uid, ref construction, true))
			{
				return false;
			}
			this.ClearPathfinding(uid, construction);
			if (targetNodeId == null)
			{
				return true;
			}
			ConstructionGraphPrototype graph = this.GetCurrentGraph(uid, construction);
			if (graph == null)
			{
				return false;
			}
			ConstructionGraphNode node = this.GetNodeFromGraph(graph, construction.Node);
			if (node == null)
			{
				return false;
			}
			ConstructionGraphNode targetNode = this.GetNodeFromGraph(graph, targetNodeId);
			return targetNode != null && this.UpdatePathfinding(uid, graph, node, targetNode, this.GetCurrentEdge(uid, construction), construction);
		}

		// Token: 0x06002095 RID: 8341 RVA: 0x000AB42C File Offset: 0x000A962C
		[NullableContext(2)]
		public bool UpdatePathfinding(EntityUid uid, ConstructionComponent construction = null)
		{
			if (!base.Resolve<ConstructionComponent>(uid, ref construction, true))
			{
				return false;
			}
			string targetNodeId = construction.TargetNode;
			if (targetNodeId == null)
			{
				return false;
			}
			ConstructionGraphPrototype graph = this.GetCurrentGraph(uid, construction);
			if (graph != null)
			{
				ConstructionGraphNode node = this.GetNodeFromGraph(graph, construction.Node);
				if (node != null)
				{
					ConstructionGraphNode targetNode = this.GetNodeFromGraph(graph, targetNodeId);
					if (targetNode != null)
					{
						return this.UpdatePathfinding(uid, graph, node, targetNode, this.GetCurrentEdge(uid, construction), construction);
					}
				}
			}
			return false;
		}

		// Token: 0x06002096 RID: 8342 RVA: 0x000AB494 File Offset: 0x000A9694
		private bool UpdatePathfinding(EntityUid uid, ConstructionGraphPrototype graph, ConstructionGraphNode currentNode, ConstructionGraphNode targetNode, [Nullable(2)] ConstructionGraphEdge currentEdge, [Nullable(2)] ConstructionComponent construction = null)
		{
			if (!base.Resolve<ConstructionComponent>(uid, ref construction, true))
			{
				return false;
			}
			construction.TargetNode = targetNode.Name;
			if (currentNode == targetNode)
			{
				this.ClearPathfinding(uid, construction);
				return true;
			}
			if (construction.NodePathfinding == null)
			{
				string[] path = graph.PathId(currentNode.Name, targetNode.Name);
				if (path == null || path.Length == 0)
				{
					this.ClearPathfinding(uid, construction);
					return false;
				}
				construction.NodePathfinding = new Queue<string>(path);
			}
			if (construction.NodePathfinding.Peek() == currentNode.Name)
			{
				construction.NodePathfinding.Dequeue();
			}
			if (currentEdge != null)
			{
				int? targetEdgeIndex2 = construction.TargetEdgeIndex;
				if (targetEdgeIndex2 != null)
				{
					int targetEdgeIndex = targetEdgeIndex2.GetValueOrDefault();
					if (currentNode.Edges.Count >= targetEdgeIndex)
					{
						construction.TargetEdgeIndex = null;
					}
					else if (currentNode.Edges[targetEdgeIndex] != currentEdge)
					{
						this.ClearPathfinding(uid, construction);
						return false;
					}
				}
			}
			if (construction.EdgeIndex == null && construction.TargetEdgeIndex == null && construction.NodePathfinding != null)
			{
				construction.TargetEdgeIndex = currentNode.GetEdgeIndex(construction.NodePathfinding.Peek());
			}
			return true;
		}

		// Token: 0x06002097 RID: 8343 RVA: 0x000AB5CC File Offset: 0x000A97CC
		[NullableContext(2)]
		public void ClearPathfinding(EntityUid uid, ConstructionComponent construction = null)
		{
			if (!base.Resolve<ConstructionComponent>(uid, ref construction, true))
			{
				return;
			}
			construction.TargetNode = null;
			construction.TargetEdgeIndex = null;
			construction.NodePathfinding = null;
		}

		// Token: 0x06002099 RID: 8345 RVA: 0x000AB637 File Offset: 0x000A9837
		[CompilerGenerated]
		private void <HandleStartStructureConstruction>g__Cleanup|52_0(ref ConstructionSystem.<>c__DisplayClass52_0 A_1)
		{
			this._beingBuilt[A_1.args.SenderSession].Remove(A_1.ev.Ack);
		}

		// Token: 0x04001408 RID: 5128
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04001409 RID: 5129
		[Dependency]
		private readonly ILogManager _logManager;

		// Token: 0x0400140A RID: 5130
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x0400140B RID: 5131
		[Dependency]
		private readonly IRobustRandom _robustRandom;

		// Token: 0x0400140C RID: 5132
		[Dependency]
		private readonly DoAfterSystem _doAfterSystem;

		// Token: 0x0400140D RID: 5133
		[Dependency]
		private readonly ContainerSystem _container;

		// Token: 0x0400140E RID: 5134
		[Dependency]
		private readonly StackSystem _stackSystem;

		// Token: 0x0400140F RID: 5135
		[Dependency]
		private readonly SharedToolSystem _toolSystem;

		// Token: 0x04001410 RID: 5136
		private const string SawmillName = "Construction";

		// Token: 0x04001411 RID: 5137
		private ISawmill _sawmill;

		// Token: 0x04001412 RID: 5138
		[Dependency]
		private readonly SharedPopupSystem _popup;

		// Token: 0x04001413 RID: 5139
		private readonly Dictionary<ConstructionPrototype, ConstructionGuide> _guideCache = new Dictionary<ConstructionPrototype, ConstructionGuide>();

		// Token: 0x04001414 RID: 5140
		[Dependency]
		private readonly IComponentFactory _factory;

		// Token: 0x04001415 RID: 5141
		[Dependency]
		private readonly InventorySystem _inventorySystem;

		// Token: 0x04001416 RID: 5142
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x04001417 RID: 5143
		[Dependency]
		private readonly ActionBlockerSystem _actionBlocker;

		// Token: 0x04001418 RID: 5144
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;

		// Token: 0x04001419 RID: 5145
		[Dependency]
		private readonly EntityLookupSystem _lookupSystem;

		// Token: 0x0400141A RID: 5146
		private readonly Dictionary<ICommonSession, HashSet<int>> _beingBuilt = new Dictionary<ICommonSession, HashSet<int>>();

		// Token: 0x0400141B RID: 5147
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x0400141C RID: 5148
		[Dependency]
		private readonly IRuntimeLog _runtimeLog;

		// Token: 0x0400141D RID: 5149
		private readonly Queue<EntityUid> _constructionUpdateQueue = new Queue<EntityUid>();

		// Token: 0x0400141E RID: 5150
		private readonly HashSet<EntityUid> _queuedUpdates = new HashSet<EntityUid>();

		// Token: 0x0400141F RID: 5151
		[Dependency]
		private readonly ExamineSystem _examineSystem;

		// Token: 0x02000AC2 RID: 2754
		[Nullable(0)]
		private sealed class ConstructionData
		{
			// Token: 0x060035BB RID: 13755 RVA: 0x0011DA7E File Offset: 0x0011BC7E
			public ConstructionData(object completeEvent, object cancelEvent)
			{
				this.CompleteEvent = completeEvent;
				this.CancelEvent = cancelEvent;
			}

			// Token: 0x040027B8 RID: 10168
			public readonly object CompleteEvent;

			// Token: 0x040027B9 RID: 10169
			public readonly object CancelEvent;
		}

		// Token: 0x02000AC3 RID: 2755
		[Nullable(0)]
		private sealed class ConstructionDoAfterComplete : EntityEventArgs
		{
			// Token: 0x060035BC RID: 13756 RVA: 0x0011DA94 File Offset: 0x0011BC94
			public ConstructionDoAfterComplete(EntityUid targetUid, object wrappedEvent, [Nullable(2)] object customData = null)
			{
				this.TargetUid = targetUid;
				this.WrappedEvent = wrappedEvent;
				this.CustomData = customData;
			}

			// Token: 0x040027BA RID: 10170
			public readonly EntityUid TargetUid;

			// Token: 0x040027BB RID: 10171
			public readonly object WrappedEvent;

			// Token: 0x040027BC RID: 10172
			[Nullable(2)]
			public readonly object CustomData;
		}

		// Token: 0x02000AC4 RID: 2756
		[Nullable(0)]
		private sealed class ConstructionDoAfterCancelled : EntityEventArgs
		{
			// Token: 0x060035BD RID: 13757 RVA: 0x0011DAB1 File Offset: 0x0011BCB1
			public ConstructionDoAfterCancelled(EntityUid targetUid, object wrappedEvent, [Nullable(2)] object customData = null)
			{
				this.TargetUid = targetUid;
				this.WrappedEvent = wrappedEvent;
				this.CustomData = customData;
			}

			// Token: 0x040027BD RID: 10173
			public readonly EntityUid TargetUid;

			// Token: 0x040027BE RID: 10174
			public readonly object WrappedEvent;

			// Token: 0x040027BF RID: 10175
			[Nullable(2)]
			public readonly object CustomData;
		}

		// Token: 0x02000AC5 RID: 2757
		[NullableContext(0)]
		private enum DoAfterState : byte
		{
			// Token: 0x040027C1 RID: 10177
			None,
			// Token: 0x040027C2 RID: 10178
			Validation,
			// Token: 0x040027C3 RID: 10179
			Completed,
			// Token: 0x040027C4 RID: 10180
			Cancelled
		}

		// Token: 0x02000AC6 RID: 2758
		[NullableContext(0)]
		private enum HandleResult : byte
		{
			// Token: 0x040027C6 RID: 10182
			False,
			// Token: 0x040027C7 RID: 10183
			Validated,
			// Token: 0x040027C8 RID: 10184
			True,
			// Token: 0x040027C9 RID: 10185
			DoAfter
		}
	}
}
