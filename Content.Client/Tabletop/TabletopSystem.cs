using System;
using System.Runtime.CompilerServices;
using Content.Client.Tabletop.UI;
using Content.Client.Viewport;
using Content.Shared.Tabletop;
using Content.Shared.Tabletop.Components;
using Content.Shared.Tabletop.Events;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Client.Tabletop
{
	// Token: 0x020000FB RID: 251
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TabletopSystem : SharedTabletopSystem
	{
		// Token: 0x0600070B RID: 1803 RVA: 0x00025048 File Offset: 0x00023248
		public override void Initialize()
		{
			base.Initialize();
			base.UpdatesOutsidePrediction = true;
			CommandBinds.Builder.Bind(EngineKeyFunctions.Use, new PointerInputCmdHandler(new PointerInputCmdDelegate2(this.OnUse), false, true)).Register<TabletopSystem>();
			base.SubscribeNetworkEvent<TabletopPlayEvent>(new EntityEventHandler<TabletopPlayEvent>(this.OnTabletopPlay), null, null);
			base.SubscribeLocalEvent<TabletopDraggableComponent, ComponentHandleState>(new ComponentEventRefHandler<TabletopDraggableComponent, ComponentHandleState>(this.HandleComponentState), null, null);
			base.SubscribeLocalEvent<TabletopDraggableComponent, ComponentRemove>(new ComponentEventHandler<TabletopDraggableComponent, ComponentRemove>(this.HandleDraggableRemoved), null, null);
			base.SubscribeLocalEvent<TabletopDraggableComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<TabletopDraggableComponent, AppearanceChangeEvent>(this.OnAppearanceChange), null, null);
		}

		// Token: 0x0600070C RID: 1804 RVA: 0x000250DC File Offset: 0x000232DC
		private void HandleDraggableRemoved(EntityUid uid, TabletopDraggableComponent component, ComponentRemove args)
		{
			EntityUid? draggedEntity = this._draggedEntity;
			if (draggedEntity != null && (draggedEntity == null || draggedEntity.GetValueOrDefault() == uid))
			{
				this.StopDragging(false);
			}
		}

		// Token: 0x0600070D RID: 1805 RVA: 0x00025120 File Offset: 0x00023320
		public override void FrameUpdate(float frameTime)
		{
			if (this._window == null)
			{
				return;
			}
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (localPlayer != null)
			{
				EntityUid? controlledEntity = localPlayer.ControlledEntity;
				if (controlledEntity != null)
				{
					EntityUid valueOrDefault = controlledEntity.GetValueOrDefault();
					if (!base.CanSeeTable(valueOrDefault, this._table))
					{
						this.StopDragging(true);
						DefaultWindow window = this._window;
						if (window == null)
						{
							return;
						}
						window.Close();
						return;
					}
					else
					{
						if (this._draggedEntity == null || this._viewport == null)
						{
							return;
						}
						TabletopDraggableComponent tabletopDraggableComponent;
						if (!base.CanDrag(valueOrDefault, this._draggedEntity.Value, out tabletopDraggableComponent))
						{
							this.StopDragging(true);
							return;
						}
						if (tabletopDraggableComponent.DraggingPlayer != null)
						{
							NetUserId? draggingPlayer = tabletopDraggableComponent.DraggingPlayer;
							LocalPlayer localPlayer2 = this._playerManager.LocalPlayer;
							if (draggingPlayer != ((localPlayer2 != null) ? new NetUserId?(localPlayer2.Session.UserId) : null))
							{
								this.StopDragging(false);
								return;
							}
						}
						MapCoordinates mapCoordinates = TabletopSystem.ClampPositionToViewport(this._viewport.ScreenToMap(this._inputManager.MouseScreenPosition.Position), this._viewport);
						if (mapCoordinates.Equals(MapCoordinates.Nullspace))
						{
							return;
						}
						this.EntityManager.GetComponent<TransformComponent>(this._draggedEntity.Value).WorldPosition = mapCoordinates.Position;
						this._timePassed += frameTime;
						if (this._timePassed >= 0.1f && this._table != null)
						{
							base.RaisePredictiveEvent<TabletopMoveEvent>(new TabletopMoveEvent(this._draggedEntity.Value, mapCoordinates, this._table.Value));
							this._timePassed -= 0.1f;
						}
						return;
					}
				}
			}
		}

		// Token: 0x0600070E RID: 1806 RVA: 0x000252EC File Offset: 0x000234EC
		private void OnTabletopPlay(TabletopPlayEvent msg)
		{
			DefaultWindow window = this._window;
			if (window != null)
			{
				window.Close();
			}
			this._table = new EntityUid?(msg.TableUid);
			EntityUid cameraUid = msg.CameraUid;
			EyeComponent eyeComponent;
			if (!this.EntityManager.TryGetComponent<EyeComponent>(cameraUid, ref eyeComponent))
			{
				Logger.Error("Camera entity does not have eye component!");
				return;
			}
			this._window = new TabletopWindow(eyeComponent.Eye, new ValueTuple<int, int>(msg.Size.X, msg.Size.Y))
			{
				MinWidth = 500f,
				MinHeight = 436f,
				Title = msg.Title
			};
			this._window.OnClose += this.OnWindowClose;
		}

		// Token: 0x0600070F RID: 1807 RVA: 0x000253A8 File Offset: 0x000235A8
		private void HandleComponentState(EntityUid uid, TabletopDraggableComponent component, ref ComponentHandleState args)
		{
			SharedTabletopSystem.TabletopDraggableComponentState tabletopDraggableComponentState = args.Current as SharedTabletopSystem.TabletopDraggableComponentState;
			if (tabletopDraggableComponentState == null)
			{
				return;
			}
			component.DraggingPlayer = tabletopDraggableComponentState.DraggingPlayer;
		}

		// Token: 0x06000710 RID: 1808 RVA: 0x000253D1 File Offset: 0x000235D1
		private void OnWindowClose()
		{
			if (this._table != null)
			{
				base.RaiseNetworkEvent(new TabletopStopPlayingEvent(this._table.Value));
			}
			this.StopDragging(true);
			this._window = null;
		}

		// Token: 0x06000711 RID: 1809 RVA: 0x00025404 File Offset: 0x00023604
		private bool OnUse(in PointerInputCmdHandler.PointerInputCmdArgs args)
		{
			if (!this._gameTiming.IsFirstTimePredicted)
			{
				return false;
			}
			BoundKeyState state = args.State;
			bool result;
			if (state != null)
			{
				result = (state == 1 && this.OnMouseDown(args));
			}
			else
			{
				result = this.OnMouseUp(args);
			}
			return result;
		}

		// Token: 0x06000712 RID: 1810 RVA: 0x00025448 File Offset: 0x00023648
		private bool OnMouseDown(in PointerInputCmdHandler.PointerInputCmdArgs args)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (localPlayer != null)
			{
				EntityUid? controlledEntity = localPlayer.ControlledEntity;
				if (controlledEntity != null)
				{
					EntityUid valueOrDefault = controlledEntity.GetValueOrDefault();
					TabletopDraggableComponent tabletopDraggableComponent;
					if (!base.CanSeeTable(valueOrDefault, this._table) || !base.CanDrag(valueOrDefault, args.EntityUid, out tabletopDraggableComponent))
					{
						return false;
					}
					ScalingViewport scalingViewport = this._uiManger.MouseGetControl(args.ScreenCoordinates) as ScalingViewport;
					if (scalingViewport == null)
					{
						return false;
					}
					this.StartDragging(args.EntityUid, scalingViewport);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000713 RID: 1811 RVA: 0x000254CC File Offset: 0x000236CC
		private bool OnMouseUp(in PointerInputCmdHandler.PointerInputCmdArgs args)
		{
			this.StopDragging(true);
			return false;
		}

		// Token: 0x06000714 RID: 1812 RVA: 0x000254D8 File Offset: 0x000236D8
		private void OnAppearanceChange(EntityUid uid, TabletopDraggableComponent comp, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			Vector2 scale;
			if (this._appearance.TryGetData<Vector2>(uid, TabletopItemVisuals.Scale, ref scale, args.Component))
			{
				args.Sprite.Scale = scale;
			}
			int drawDepth;
			if (this._appearance.TryGetData<int>(uid, TabletopItemVisuals.DrawDepth, ref drawDepth, args.Component))
			{
				args.Sprite.DrawDepth = drawDepth;
			}
		}

		// Token: 0x06000715 RID: 1813 RVA: 0x0002553E File Offset: 0x0002373E
		private void StartDragging(EntityUid draggedEntity, ScalingViewport viewport)
		{
			base.RaisePredictiveEvent<TabletopDraggingPlayerChangedEvent>(new TabletopDraggingPlayerChangedEvent(draggedEntity, true));
			this._draggedEntity = new EntityUid?(draggedEntity);
			this._viewport = viewport;
		}

		// Token: 0x06000716 RID: 1814 RVA: 0x00025560 File Offset: 0x00023760
		private void StopDragging(bool broadcast = true)
		{
			if (broadcast && this._draggedEntity != null && this.EntityManager.HasComponent<TabletopDraggableComponent>(this._draggedEntity.Value))
			{
				base.RaisePredictiveEvent<TabletopMoveEvent>(new TabletopMoveEvent(this._draggedEntity.Value, base.Transform(this._draggedEntity.Value).MapPosition, this._table.Value));
				base.RaisePredictiveEvent<TabletopDraggingPlayerChangedEvent>(new TabletopDraggingPlayerChangedEvent(this._draggedEntity.Value, false));
			}
			this._draggedEntity = null;
			this._viewport = null;
		}

		// Token: 0x06000717 RID: 1815 RVA: 0x000255F8 File Offset: 0x000237F8
		private static MapCoordinates ClampPositionToViewport(MapCoordinates coordinates, ScalingViewport viewport)
		{
			if (coordinates == MapCoordinates.Nullspace)
			{
				return MapCoordinates.Nullspace;
			}
			IEye eye = viewport.Eye;
			if (eye == null)
			{
				return MapCoordinates.Nullspace;
			}
			Vector2 vector = viewport.ViewportSize / 32f;
			Vector2 position = eye.Position.Position;
			Angle rotation = eye.Rotation;
			Vector2 scale = eye.Scale;
			Vector2 vector2 = (position - vector / 2f) / scale;
			Vector2 vector3 = (position + vector / 2f) / scale;
			if (MathHelper.CloseToPercent(rotation.Degrees % 180.0, 90.0, 1E-05) || MathHelper.CloseToPercent(rotation.Degrees % 180.0, -90.0, 1E-05))
			{
				ref float ptr = ref vector2.Y;
				float num = vector2.X;
				float num2 = vector2.Y;
				ptr = num;
				vector2.X = num2;
				ptr = ref vector3.Y;
				num2 = vector3.X;
				num = vector3.Y;
				ptr = num2;
				vector3.X = num;
			}
			return new MapCoordinates(Vector2.Clamp(coordinates.Position, vector2, vector3), eye.Position.MapId);
		}

		// Token: 0x0400033C RID: 828
		[Dependency]
		private readonly IInputManager _inputManager;

		// Token: 0x0400033D RID: 829
		[Dependency]
		private readonly IUserInterfaceManager _uiManger;

		// Token: 0x0400033E RID: 830
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x0400033F RID: 831
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000340 RID: 832
		[Dependency]
		private readonly AppearanceSystem _appearance;

		// Token: 0x04000341 RID: 833
		private const float Delay = 0.1f;

		// Token: 0x04000342 RID: 834
		private float _timePassed;

		// Token: 0x04000343 RID: 835
		private EntityUid? _draggedEntity;

		// Token: 0x04000344 RID: 836
		[Nullable(2)]
		private ScalingViewport _viewport;

		// Token: 0x04000345 RID: 837
		[Nullable(2)]
		private DefaultWindow _window;

		// Token: 0x04000346 RID: 838
		private EntityUid? _table;
	}
}
