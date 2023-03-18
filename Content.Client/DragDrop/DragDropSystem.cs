using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.CombatMode;
using Content.Client.Gameplay;
using Content.Client.Outline;
using Content.Shared.ActionBlocker;
using Content.Shared.CCVar;
using Content.Shared.DragDrop;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Players;
using Robust.Shared.Prototypes;

namespace Content.Client.DragDrop
{
	// Token: 0x02000341 RID: 833
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DragDropSystem : SharedDragDropSystem
	{
		// Token: 0x060014B5 RID: 5301 RVA: 0x00078DBC File Offset: 0x00076FBC
		public override void Initialize()
		{
			base.Initialize();
			this._sawmill = Logger.GetSawmill("drag_drop");
			base.UpdatesOutsidePrediction = true;
			base.UpdatesAfter.Add(typeof(EyeUpdateSystem));
			this._cfgMan.OnValueChanged<float>(CCVars.DragDropDeadZone, new Action<float>(this.SetDeadZone), true);
			this._dropTargetInRangeShader = this._prototypeManager.Index<ShaderPrototype>("SelectionOutlineInrange").Instance();
			this._dropTargetOutOfRangeShader = this._prototypeManager.Index<ShaderPrototype>("SelectionOutline").Instance();
			CommandBinds.Builder.BindBefore(EngineKeyFunctions.Use, new PointerInputCmdHandler(new PointerInputCmdDelegate2(this.OnUse), false, false), new Type[]
			{
				typeof(SharedInteractionSystem)
			}).Register<DragDropSystem>();
		}

		// Token: 0x060014B6 RID: 5302 RVA: 0x00078E89 File Offset: 0x00077089
		private void SetDeadZone(float deadZone)
		{
			this._deadzone = deadZone;
		}

		// Token: 0x060014B7 RID: 5303 RVA: 0x00078E92 File Offset: 0x00077092
		public override void Shutdown()
		{
			this._cfgMan.UnsubValueChanged<float>(CCVars.DragDropDeadZone, new Action<float>(this.SetDeadZone));
			CommandBinds.Unregister<DragDropSystem>();
			base.Shutdown();
		}

		// Token: 0x060014B8 RID: 5304 RVA: 0x00078EBB File Offset: 0x000770BB
		private bool OnUse(in PointerInputCmdHandler.PointerInputCmdArgs args)
		{
			if (this._inputSystem.Predicted)
			{
				return false;
			}
			if (this._isReplaying)
			{
				return false;
			}
			if (args.State == 1)
			{
				return this.OnUseMouseDown(args);
			}
			return args.State == null && this.OnUseMouseUp(args);
		}

		// Token: 0x060014B9 RID: 5305 RVA: 0x00078EF8 File Offset: 0x000770F8
		private void EndDrag()
		{
			if (this._state == DragState.NotDragging)
			{
				return;
			}
			if (this._dragShadow != null)
			{
				base.Del(this._dragShadow.Value);
				this._dragShadow = null;
			}
			this._draggedEntity = null;
			this._state = DragState.NotDragging;
			this._mouseDownScreenPos = null;
			this.RemoveHighlights();
			this._outline.SetEnabled(true);
			this._mouseDownTime = 0f;
			this._savedMouseDown = null;
		}

		// Token: 0x060014BA RID: 5306 RVA: 0x00078F80 File Offset: 0x00077180
		private bool OnUseMouseDown(in PointerInputCmdHandler.PointerInputCmdArgs args)
		{
			ICommonSession session = args.Session;
			EntityUid? entityUid = (session != null) ? session.AttachedEntity : null;
			if (entityUid != null)
			{
				EntityUid valueOrDefault = entityUid.GetValueOrDefault();
				if (valueOrDefault.Valid && !this._combatMode.IsInCombatMode())
				{
					this.EndDrag();
					if (!base.Exists(args.EntityUid))
					{
						return false;
					}
					if (!this._interactionSystem.InRangeUnobstructed(valueOrDefault, args.EntityUid, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false))
					{
						return false;
					}
					CanDragEvent canDragEvent = default(CanDragEvent);
					base.RaiseLocalEvent<CanDragEvent>(args.EntityUid, ref canDragEvent, false);
					if (!canDragEvent.Handled)
					{
						return false;
					}
					this._draggedEntity = new EntityUid?(args.EntityUid);
					this._state = DragState.MouseDown;
					this._mouseDownScreenPos = new ScreenCoordinates?(this._inputManager.MouseScreenPosition);
					this._mouseDownTime = 0f;
					this._savedMouseDown = new PointerInputCmdHandler.PointerInputCmdArgs?(args);
					return true;
				}
			}
			return false;
		}

		// Token: 0x060014BB RID: 5307 RVA: 0x00079078 File Offset: 0x00077278
		private void StartDrag()
		{
			if (!base.Exists(this._draggedEntity))
			{
				return;
			}
			this._state = DragState.Dragging;
			this._outline.SetEnabled(false);
			this.HighlightTargets();
			SpriteComponent spriteComponent;
			if (base.TryComp<SpriteComponent>(this._draggedEntity, ref spriteComponent))
			{
				MapCoordinates mapCoordinates = this._eyeManager.ScreenToMap(this._inputManager.MouseScreenPosition);
				this._dragShadow = new EntityUid?(this.EntityManager.SpawnEntity("dragshadow", mapCoordinates));
				SpriteComponent spriteComponent2 = base.Comp<SpriteComponent>(this._dragShadow.Value);
				spriteComponent2.CopyFrom(spriteComponent);
				spriteComponent2.RenderOrder = this.EntityManager.CurrentTick.Value;
				spriteComponent2.Color = spriteComponent2.Color.WithAlpha(0.7f);
				spriteComponent2.DrawDepth = 9;
				if (!spriteComponent2.NoRotation)
				{
					base.Transform(this._dragShadow.Value).WorldRotation = base.Transform(this._draggedEntity.Value).WorldRotation;
				}
				return;
			}
			ISawmill sawmill = this._sawmill;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(70, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Unable to display drag shadow for ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(this._draggedEntity.Value));
			defaultInterpolatedStringHandler.AppendLiteral(" because it has no sprite component.");
			sawmill.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x060014BC RID: 5308 RVA: 0x000791C4 File Offset: 0x000773C4
		private bool UpdateDrag(float frameTime)
		{
			if (!base.Exists(this._draggedEntity) || this._combatMode.IsInCombatMode())
			{
				this.EndDrag();
				return false;
			}
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			if (entityUid == null || !this._interactionSystem.InRangeUnobstructed(entityUid.Value, this._draggedEntity.Value, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false))
			{
				return false;
			}
			if (this._dragShadow == null)
			{
				return false;
			}
			this._targetRecheckTime += frameTime;
			if (this._targetRecheckTime > 0.25f)
			{
				this.HighlightTargets();
				this._targetRecheckTime -= 0.25f;
			}
			return true;
		}

		// Token: 0x060014BD RID: 5309 RVA: 0x00079290 File Offset: 0x00077490
		private bool OnUseMouseUp(in PointerInputCmdHandler.PointerInputCmdArgs args)
		{
			if (this._state == DragState.MouseDown)
			{
				try
				{
					if (this._savedMouseDown != null && this._mouseDownTime < 0.85f)
					{
						PointerInputCmdHandler.PointerInputCmdArgs value = this._savedMouseDown.Value;
						this._isReplaying = true;
						FullInputCmdMessage originalMessage = value.OriginalMessage;
						FullInputCmdMessage fullInputCmdMessage = new FullInputCmdMessage(args.OriginalMessage.Tick, args.OriginalMessage.SubTick, originalMessage.InputFunctionId, originalMessage.State, originalMessage.Coordinates, originalMessage.ScreenCoordinates, originalMessage.Uid);
						if (value.Session != null)
						{
							this._inputSystem.HandleInputCommand(value.Session, EngineKeyFunctions.Use, fullInputCmdMessage, true);
						}
						this._isReplaying = false;
					}
				}
				finally
				{
					this.EndDrag();
				}
				return false;
			}
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			if (entityUid == null || !base.Exists(this._draggedEntity))
			{
				this.EndDrag();
				return false;
			}
			GameplayState gameplayState = this._stateManager.CurrentState as GameplayState;
			IEnumerable<EntityUid> enumerable;
			if (gameplayState != null)
			{
				enumerable = gameplayState.GetClickableEntities(args.Coordinates);
			}
			else
			{
				enumerable = Array.Empty<EntityUid>();
			}
			bool flag = false;
			EntityUid value2 = entityUid.Value;
			foreach (EntityUid entityUid2 in enumerable)
			{
				if (!(entityUid2 == this._draggedEntity))
				{
					bool? flag2 = this.ValidDragDrop(value2, this._draggedEntity.Value, entityUid2);
					bool flag3 = true;
					if (flag2.GetValueOrDefault() == flag3 & flag2 != null)
					{
						if (this._interactionSystem.InRangeUnobstructed(value2, entityUid2, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false) && this._interactionSystem.InRangeUnobstructed(value2, this._draggedEntity.Value, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false))
						{
							base.RaiseNetworkEvent(new DragDropRequestEvent(this._draggedEntity.Value, entityUid2));
							this.EndDrag();
							return true;
						}
						flag = true;
					}
				}
			}
			if (flag)
			{
				this._popup.PopupEntity(Loc.GetString("drag-drop-system-out-of-range-text"), this._draggedEntity.Value, Filter.Local(), true, PopupType.Small);
			}
			this.EndDrag();
			return false;
		}

		// Token: 0x060014BE RID: 5310 RVA: 0x00079514 File Offset: 0x00077714
		private void HighlightTargets()
		{
			if (!base.Exists(this._draggedEntity) || !base.Exists(this._dragShadow))
			{
				return;
			}
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			if (entityUid == null)
			{
				return;
			}
			this.RemoveHighlights();
			MapCoordinates mapCoordinates = this._eyeManager.ScreenToMap(this._inputManager.MouseScreenPosition);
			Box2 box;
			box..ctor(mapCoordinates.Position - 1.5f, mapCoordinates.Position + 1.5f);
			HashSet<EntityUid> entitiesIntersecting = this._lookup.GetEntitiesIntersecting(mapCoordinates.MapId, box, 46);
			EntityQuery<SpriteComponent> entityQuery = base.GetEntityQuery<SpriteComponent>();
			foreach (EntityUid entityUid2 in entitiesIntersecting)
			{
				SpriteComponent spriteComponent;
				if (entityQuery.TryGetComponent(entityUid2, ref spriteComponent) && spriteComponent.Visible && !(entityUid2 == this._draggedEntity))
				{
					bool? flag = this.ValidDragDrop(entityUid.Value, this._draggedEntity.Value, entityUid2);
					if (flag != null)
					{
						if (flag.Value)
						{
							flag = new bool?(this._interactionSystem.InRangeUnobstructed(entityUid.Value, this._draggedEntity.Value, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false) && this._interactionSystem.InRangeUnobstructed(entityUid.Value, entityUid2, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false));
						}
						if (spriteComponent.PostShader == null || spriteComponent.PostShader == this._dropTargetInRangeShader || spriteComponent.PostShader == this._dropTargetOutOfRangeShader)
						{
							spriteComponent.PostShader = (flag.Value ? this._dropTargetInRangeShader : this._dropTargetOutOfRangeShader);
							spriteComponent.RenderOrder = this.EntityManager.CurrentTick.Value;
							this._highlightedSprites.Add(spriteComponent);
						}
					}
				}
			}
		}

		// Token: 0x060014BF RID: 5311 RVA: 0x0007974C File Offset: 0x0007794C
		private void RemoveHighlights()
		{
			foreach (SpriteComponent spriteComponent in this._highlightedSprites)
			{
				if (spriteComponent.PostShader == this._dropTargetInRangeShader || spriteComponent.PostShader == this._dropTargetOutOfRangeShader)
				{
					spriteComponent.PostShader = null;
					spriteComponent.RenderOrder = 0U;
				}
			}
			this._highlightedSprites.Clear();
		}

		// Token: 0x060014C0 RID: 5312 RVA: 0x000797D0 File Offset: 0x000779D0
		private bool? ValidDragDrop(EntityUid user, EntityUid dragged, EntityUid target)
		{
			if (!this._actionBlockerSystem.CanInteract(user, new EntityUid?(target)))
			{
				return null;
			}
			GettingInteractedWithAttemptEvent gettingInteractedWithAttemptEvent = new GettingInteractedWithAttemptEvent(user, new EntityUid?(dragged));
			base.RaiseLocalEvent<GettingInteractedWithAttemptEvent>(dragged, gettingInteractedWithAttemptEvent, true);
			if (gettingInteractedWithAttemptEvent.Cancelled)
			{
				return new bool?(false);
			}
			CanDropDraggedEvent canDropDraggedEvent = new CanDropDraggedEvent(user, target);
			base.RaiseLocalEvent<CanDropDraggedEvent>(dragged, ref canDropDraggedEvent, false);
			if (canDropDraggedEvent.Handled && !canDropDraggedEvent.CanDrop)
			{
				return new bool?(false);
			}
			CanDropTargetEvent canDropTargetEvent = new CanDropTargetEvent(user, dragged);
			base.RaiseLocalEvent<CanDropTargetEvent>(target, ref canDropTargetEvent, false);
			if (canDropTargetEvent.Handled)
			{
				return new bool?(canDropTargetEvent.CanDrop);
			}
			return null;
		}

		// Token: 0x060014C1 RID: 5313 RVA: 0x0007987C File Offset: 0x00077A7C
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			DragState state = this._state;
			if (state != DragState.MouseDown)
			{
				if (state != DragState.Dragging)
				{
					return;
				}
				this.UpdateDrag(frameTime);
			}
			else
			{
				ScreenCoordinates mouseScreenPosition = this._inputManager.MouseScreenPosition;
				if ((this._mouseDownScreenPos.Value.Position - mouseScreenPosition.Position).Length > this._deadzone)
				{
					this.StartDrag();
					return;
				}
			}
		}

		// Token: 0x060014C2 RID: 5314 RVA: 0x000798E8 File Offset: 0x00077AE8
		public override void FrameUpdate(float frameTime)
		{
			base.FrameUpdate(frameTime);
			if (base.Exists(this._dragShadow))
			{
				MapCoordinates mapCoordinates = this._eyeManager.ScreenToMap(this._inputManager.MouseScreenPosition);
				base.Transform(this._dragShadow.Value).WorldPosition = mapCoordinates.Position;
			}
		}

		// Token: 0x04000AA0 RID: 2720
		[Dependency]
		private readonly IStateManager _stateManager;

		// Token: 0x04000AA1 RID: 2721
		[Dependency]
		private readonly IInputManager _inputManager;

		// Token: 0x04000AA2 RID: 2722
		[Dependency]
		private readonly IEyeManager _eyeManager;

		// Token: 0x04000AA3 RID: 2723
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000AA4 RID: 2724
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000AA5 RID: 2725
		[Dependency]
		private readonly IConfigurationManager _cfgMan;

		// Token: 0x04000AA6 RID: 2726
		[Dependency]
		private readonly InteractionOutlineSystem _outline;

		// Token: 0x04000AA7 RID: 2727
		[Dependency]
		private readonly SharedInteractionSystem _interactionSystem;

		// Token: 0x04000AA8 RID: 2728
		[Dependency]
		private readonly CombatModeSystem _combatMode;

		// Token: 0x04000AA9 RID: 2729
		[Dependency]
		private readonly InputSystem _inputSystem;

		// Token: 0x04000AAA RID: 2730
		[Dependency]
		private readonly ActionBlockerSystem _actionBlockerSystem;

		// Token: 0x04000AAB RID: 2731
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x04000AAC RID: 2732
		[Dependency]
		private readonly SharedPopupSystem _popup;

		// Token: 0x04000AAD RID: 2733
		private ISawmill _sawmill;

		// Token: 0x04000AAE RID: 2734
		private const float TargetRecheckInterval = 0.25f;

		// Token: 0x04000AAF RID: 2735
		private const float MaxMouseDownTimeForReplayingClick = 0.85f;

		// Token: 0x04000AB0 RID: 2736
		private const string ShaderDropTargetInRange = "SelectionOutlineInrange";

		// Token: 0x04000AB1 RID: 2737
		private const string ShaderDropTargetOutOfRange = "SelectionOutline";

		// Token: 0x04000AB2 RID: 2738
		private EntityUid? _draggedEntity;

		// Token: 0x04000AB3 RID: 2739
		private EntityUid? _dragShadow;

		// Token: 0x04000AB4 RID: 2740
		private float _mouseDownTime;

		// Token: 0x04000AB5 RID: 2741
		private float _targetRecheckTime;

		// Token: 0x04000AB6 RID: 2742
		private PointerInputCmdHandler.PointerInputCmdArgs? _savedMouseDown;

		// Token: 0x04000AB7 RID: 2743
		private bool _isReplaying;

		// Token: 0x04000AB8 RID: 2744
		private float _deadzone;

		// Token: 0x04000AB9 RID: 2745
		private DragState _state;

		// Token: 0x04000ABA RID: 2746
		private ScreenCoordinates? _mouseDownScreenPos;

		// Token: 0x04000ABB RID: 2747
		[Nullable(2)]
		private ShaderInstance _dropTargetInRangeShader;

		// Token: 0x04000ABC RID: 2748
		[Nullable(2)]
		private ShaderInstance _dropTargetOutOfRangeShader;

		// Token: 0x04000ABD RID: 2749
		private readonly List<SpriteComponent> _highlightedSprites = new List<SpriteComponent>();
	}
}
