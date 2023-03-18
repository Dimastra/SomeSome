using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Construction;
using Content.Shared.Construction.Conditions;
using Content.Shared.Construction.Prototypes;
using Content.Shared.Examine;
using Content.Shared.Input;
using Content.Shared.Interaction;
using Content.Shared.Physics;
using Content.Shared.Wall;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Construction
{
	// Token: 0x0200038F RID: 911
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ConstructionSystem : SharedConstructionSystem
	{
		// Token: 0x17000478 RID: 1144
		// (get) Token: 0x0600165A RID: 5722 RVA: 0x00083799 File Offset: 0x00081999
		// (set) Token: 0x0600165B RID: 5723 RVA: 0x000837A1 File Offset: 0x000819A1
		public bool CraftingEnabled { get; private set; }

		// Token: 0x0600165C RID: 5724 RVA: 0x000837AC File Offset: 0x000819AC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PlayerAttachSysMessage>(new EntityEventHandler<PlayerAttachSysMessage>(this.HandlePlayerAttached), null, null);
			base.SubscribeNetworkEvent<SharedConstructionSystem.AckStructureConstructionMessage>(new EntityEventHandler<SharedConstructionSystem.AckStructureConstructionMessage>(this.HandleAckStructure), null, null);
			base.SubscribeNetworkEvent<SharedConstructionSystem.ResponseConstructionGuide>(new EntityEventHandler<SharedConstructionSystem.ResponseConstructionGuide>(this.OnConstructionGuideReceived), null, null);
			CommandBinds.Builder.Bind(ContentKeyFunctions.OpenCraftingMenu, new PointerInputCmdHandler(new PointerInputCmdDelegate2(this.HandleOpenCraftingMenu), true, false)).Bind(EngineKeyFunctions.Use, new PointerInputCmdHandler(new PointerInputCmdDelegate2(this.HandleUse), true, false)).Register<ConstructionSystem>();
			base.SubscribeLocalEvent<ConstructionGhostComponent, ExaminedEvent>(new ComponentEventHandler<ConstructionGhostComponent, ExaminedEvent>(this.HandleConstructionGhostExamined), null, null);
		}

		// Token: 0x0600165D RID: 5725 RVA: 0x00083854 File Offset: 0x00081A54
		private void OnConstructionGuideReceived(SharedConstructionSystem.ResponseConstructionGuide ev)
		{
			this._guideCache[ev.ConstructionId] = ev.Guide;
			EventHandler<string> constructionGuideAvailable = this.ConstructionGuideAvailable;
			if (constructionGuideAvailable == null)
			{
				return;
			}
			constructionGuideAvailable(this, ev.ConstructionId);
		}

		// Token: 0x0600165E RID: 5726 RVA: 0x00083884 File Offset: 0x00081A84
		public override void Shutdown()
		{
			base.Shutdown();
			CommandBinds.Unregister<ConstructionSystem>();
		}

		// Token: 0x0600165F RID: 5727 RVA: 0x00083894 File Offset: 0x00081A94
		[return: Nullable(2)]
		public ConstructionGuide GetGuide(ConstructionPrototype prototype)
		{
			ConstructionGuide result;
			if (this._guideCache.TryGetValue(prototype.ID, out result))
			{
				return result;
			}
			base.RaiseNetworkEvent(new SharedConstructionSystem.RequestConstructionGuide(prototype.ID));
			return null;
		}

		// Token: 0x06001660 RID: 5728 RVA: 0x000838CC File Offset: 0x00081ACC
		private void HandleConstructionGhostExamined(EntityUid uid, ConstructionGhostComponent component, ExaminedEvent args)
		{
			if (component.Prototype == null)
			{
				return;
			}
			args.PushMarkup(Loc.GetString("construction-ghost-examine-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("name", component.Prototype.Name)
			}));
			ConstructionGraphPrototype constructionGraphPrototype;
			if (!this._prototypeManager.TryIndex<ConstructionGraphPrototype>(component.Prototype.Graph, ref constructionGraphPrototype))
			{
				return;
			}
			ConstructionGraphNode constructionGraphNode = constructionGraphPrototype.Nodes[component.Prototype.StartNode];
			ConstructionGraphNode[] array;
			ConstructionGraphEdge constructionGraphEdge;
			if (!constructionGraphPrototype.TryPath(component.Prototype.StartNode, component.Prototype.TargetNode, out array) || !constructionGraphNode.TryGetEdge(array[0].Name, out constructionGraphEdge))
			{
				return;
			}
			constructionGraphEdge.Steps[0].DoExamine(args);
		}

		// Token: 0x1400007D RID: 125
		// (add) Token: 0x06001661 RID: 5729 RVA: 0x0008398C File Offset: 0x00081B8C
		// (remove) Token: 0x06001662 RID: 5730 RVA: 0x000839C4 File Offset: 0x00081BC4
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event EventHandler<CraftingAvailabilityChangedArgs> CraftingAvailabilityChanged;

		// Token: 0x1400007E RID: 126
		// (add) Token: 0x06001663 RID: 5731 RVA: 0x000839FC File Offset: 0x00081BFC
		// (remove) Token: 0x06001664 RID: 5732 RVA: 0x00083A34 File Offset: 0x00081C34
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event EventHandler<string> ConstructionGuideAvailable;

		// Token: 0x1400007F RID: 127
		// (add) Token: 0x06001665 RID: 5733 RVA: 0x00083A6C File Offset: 0x00081C6C
		// (remove) Token: 0x06001666 RID: 5734 RVA: 0x00083AA4 File Offset: 0x00081CA4
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event EventHandler ToggleCraftingWindow;

		// Token: 0x06001667 RID: 5735 RVA: 0x00083AD9 File Offset: 0x00081CD9
		private void HandleAckStructure(SharedConstructionSystem.AckStructureConstructionMessage msg)
		{
			this.ClearGhost(msg.GhostId);
		}

		// Token: 0x06001668 RID: 5736 RVA: 0x00083AE8 File Offset: 0x00081CE8
		private void HandlePlayerAttached(PlayerAttachSysMessage msg)
		{
			bool available = ConstructionSystem.IsCraftingAvailable(new EntityUid?(msg.AttachedEntity));
			this.UpdateCraftingAvailability(available);
		}

		// Token: 0x06001669 RID: 5737 RVA: 0x00083B0D File Offset: 0x00081D0D
		private bool HandleOpenCraftingMenu(in PointerInputCmdHandler.PointerInputCmdArgs args)
		{
			if (args.State == 1)
			{
				EventHandler toggleCraftingWindow = this.ToggleCraftingWindow;
				if (toggleCraftingWindow != null)
				{
					toggleCraftingWindow(this, EventArgs.Empty);
				}
			}
			return true;
		}

		// Token: 0x0600166A RID: 5738 RVA: 0x00083B30 File Offset: 0x00081D30
		private void UpdateCraftingAvailability(bool available)
		{
			if (this.CraftingEnabled == available)
			{
				return;
			}
			EventHandler<CraftingAvailabilityChangedArgs> craftingAvailabilityChanged = this.CraftingAvailabilityChanged;
			if (craftingAvailabilityChanged != null)
			{
				craftingAvailabilityChanged(this, new CraftingAvailabilityChangedArgs(available));
			}
			this.CraftingEnabled = available;
		}

		// Token: 0x0600166B RID: 5739 RVA: 0x00083B5B File Offset: 0x00081D5B
		private static bool IsCraftingAvailable(EntityUid? entity)
		{
			return entity != null;
		}

		// Token: 0x0600166C RID: 5740 RVA: 0x00083B6C File Offset: 0x00081D6C
		private bool HandleUse(in PointerInputCmdHandler.PointerInputCmdArgs args)
		{
			if (!args.EntityUid.IsValid() || !args.EntityUid.IsClientSide())
			{
				return false;
			}
			ConstructionGhostComponent constructionGhostComponent;
			if (!this.EntityManager.TryGetComponent<ConstructionGhostComponent>(args.EntityUid, ref constructionGhostComponent))
			{
				return false;
			}
			this.TryStartConstruction(constructionGhostComponent.GhostId);
			return true;
		}

		// Token: 0x0600166D RID: 5741 RVA: 0x00083BBC File Offset: 0x00081DBC
		public void SpawnGhost(ConstructionPrototype prototype, EntityCoordinates loc, Direction dir)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			if (entityUid != null)
			{
				EntityUid valueOrDefault = entityUid.GetValueOrDefault();
				if (valueOrDefault.IsValid())
				{
					if (this.GhostPresent(loc))
					{
						return;
					}
					SharedInteractionSystem.Ignored predicate = base.GetPredicate(prototype.CanBuildInImpassable, loc.ToMap(this.EntityManager));
					if (!this._interactionSystem.InRangeUnobstructed(valueOrDefault, loc, 20f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, predicate, false))
					{
						return;
					}
					using (IEnumerator<IConstructionCondition> enumerator = prototype.Conditions.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (!enumerator.Current.Condition(valueOrDefault, loc, dir))
							{
								return;
							}
						}
					}
					EntityUid entityUid2 = this.EntityManager.SpawnEntity("constructionghost", loc);
					ConstructionGhostComponent component = this.EntityManager.GetComponent<ConstructionGhostComponent>(entityUid2);
					component.Prototype = prototype;
					ConstructionGhostComponent constructionGhostComponent = component;
					int nextId = this._nextId;
					this._nextId = nextId + 1;
					constructionGhostComponent.GhostId = nextId;
					this.EntityManager.GetComponent<TransformComponent>(entityUid2).LocalRotation = DirectionExtensions.ToAngle(dir);
					this._ghosts.Add(component.GhostId, component);
					SpriteComponent component2 = this.EntityManager.GetComponent<SpriteComponent>(entityUid2);
					component2.Color = new Color(48, byte.MaxValue, 48, 128);
					component2.AddBlankLayer(new int?(0));
					component2.LayerSetSprite(0, prototype.Icon);
					component2.LayerSetShader(0, "unshaded");
					component2.LayerSetVisible(0, true);
					if (prototype.CanBuildInImpassable)
					{
						base.EnsureComp<WallMountComponent>(entityUid2).Arc = new Angle(6.283185307179586);
					}
					return;
				}
			}
		}

		// Token: 0x0600166E RID: 5742 RVA: 0x00083D70 File Offset: 0x00081F70
		private bool GhostPresent(EntityCoordinates loc)
		{
			foreach (KeyValuePair<int, ConstructionGhostComponent> keyValuePair in this._ghosts)
			{
				if (this.EntityManager.GetComponent<TransformComponent>(keyValuePair.Value.Owner).Coordinates.Equals(loc))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600166F RID: 5743 RVA: 0x00083DEC File Offset: 0x00081FEC
		private void TryStartConstruction(int ghostId)
		{
			ConstructionGhostComponent constructionGhostComponent = this._ghosts[ghostId];
			if (constructionGhostComponent.Prototype == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(66, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Can't start construction for a ghost with no prototype. Ghost id: ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(ghostId);
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			TransformComponent component = this.EntityManager.GetComponent<TransformComponent>(constructionGhostComponent.Owner);
			SharedConstructionSystem.TryStartStructureConstructionMessage tryStartStructureConstructionMessage = new SharedConstructionSystem.TryStartStructureConstructionMessage(component.Coordinates, constructionGhostComponent.Prototype.ID, component.LocalRotation, ghostId);
			base.RaiseNetworkEvent(tryStartStructureConstructionMessage);
		}

		// Token: 0x06001670 RID: 5744 RVA: 0x00083E70 File Offset: 0x00082070
		public void TryStartItemConstruction(string prototypeName)
		{
			base.RaiseNetworkEvent(new SharedConstructionSystem.TryStartItemConstructionMessage(prototypeName));
		}

		// Token: 0x06001671 RID: 5745 RVA: 0x00083E80 File Offset: 0x00082080
		public void ClearGhost(int ghostId)
		{
			ConstructionGhostComponent constructionGhostComponent;
			if (this._ghosts.TryGetValue(ghostId, out constructionGhostComponent))
			{
				this.EntityManager.QueueDeleteEntity(constructionGhostComponent.Owner);
				this._ghosts.Remove(ghostId);
			}
		}

		// Token: 0x06001672 RID: 5746 RVA: 0x00083EBC File Offset: 0x000820BC
		public void ClearAllGhosts()
		{
			foreach (KeyValuePair<int, ConstructionGhostComponent> keyValuePair in this._ghosts)
			{
				int num;
				ConstructionGhostComponent constructionGhostComponent;
				keyValuePair.Deconstruct(out num, out constructionGhostComponent);
				ConstructionGhostComponent constructionGhostComponent2 = constructionGhostComponent;
				this.EntityManager.QueueDeleteEntity(constructionGhostComponent2.Owner);
			}
			this._ghosts.Clear();
		}

		// Token: 0x04000BCB RID: 3019
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000BCC RID: 3020
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000BCD RID: 3021
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x04000BCE RID: 3022
		private readonly Dictionary<int, ConstructionGhostComponent> _ghosts = new Dictionary<int, ConstructionGhostComponent>();

		// Token: 0x04000BCF RID: 3023
		private readonly Dictionary<string, ConstructionGuide> _guideCache = new Dictionary<string, ConstructionGuide>();

		// Token: 0x04000BD0 RID: 3024
		private int _nextId;
	}
}
