using System;
using System.Runtime.CompilerServices;
using Content.Client.Gameplay;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.State;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;

namespace Content.Client.Weapons.Ranged.Systems
{
	// Token: 0x02000035 RID: 53
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TetherGunSystem : SharedTetherGunSystem
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000F1 RID: 241 RVA: 0x00008BBC File Offset: 0x00006DBC
		// (set) Token: 0x060000F2 RID: 242 RVA: 0x00008BC4 File Offset: 0x00006DC4
		public bool Enabled { get; set; }

		// Token: 0x060000F3 RID: 243 RVA: 0x00008BCD File Offset: 0x00006DCD
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<PredictTetherEvent>(new EntityEventHandler<PredictTetherEvent>(this.OnPredictTether), null, null);
			base.SubscribeNetworkEvent<TetherGunToggleMessage>(new EntityEventHandler<TetherGunToggleMessage>(this.OnTetherGun), null, null);
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00008BFD File Offset: 0x00006DFD
		private void OnTetherGun(TetherGunToggleMessage ev)
		{
			this.Enabled = ev.Enabled;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00008C0C File Offset: 0x00006E0C
		private void OnPredictTether(PredictTetherEvent ev)
		{
			EntityUid? dragging = this._dragging;
			EntityUid entity = ev.Entity;
			if (dragging == null || (dragging != null && dragging.GetValueOrDefault() != entity))
			{
				return;
			}
			this._tether = new EntityUid?(ev.Entity);
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00008C60 File Offset: 0x00006E60
		public override void FrameUpdate(float frameTime)
		{
			base.FrameUpdate(frameTime);
			PhysicsComponent physicsComponent;
			if (!base.TryComp<PhysicsComponent>(this._dragging, ref physicsComponent))
			{
				return;
			}
			physicsComponent.Predict = true;
			PhysicsComponent physicsComponent2;
			if (base.TryComp<PhysicsComponent>(this._tether, ref physicsComponent2))
			{
				physicsComponent2.Predict = true;
			}
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00008CA4 File Offset: 0x00006EA4
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (!this.Enabled || !this._gameTiming.IsFirstTimePredicted)
			{
				return;
			}
			if (this._inputSystem.CmdStates.GetState(EngineKeyFunctions.Use) != 1)
			{
				this.StopDragging();
				return;
			}
			ScreenCoordinates mouseScreenPosition = this._inputManager.MouseScreenPosition;
			MapCoordinates mapCoordinates = this._eyeManager.ScreenToMap(mouseScreenPosition);
			if (this._dragging == null)
			{
				GameplayState gameplayState = IoCManager.Resolve<IStateManager>().CurrentState as GameplayState;
				if (gameplayState != null)
				{
					EntityUid? clickedEntity = gameplayState.GetClickedEntity(mapCoordinates);
					if (clickedEntity != null)
					{
						this.StartDragging(clickedEntity.Value, mapCoordinates);
					}
				}
				if (this._dragging == null)
				{
					return;
				}
			}
			TransformComponent transformComponent;
			PhysicsComponent physicsComponent;
			if (!base.TryComp<TransformComponent>(this._dragging.Value, ref transformComponent) || this._lastMousePosition.Value.MapId != transformComponent.MapID || !base.TryComp<PhysicsComponent>(this._dragging, ref physicsComponent))
			{
				this.StopDragging();
				return;
			}
			physicsComponent.Predict = true;
			PhysicsComponent physicsComponent2;
			if (base.TryComp<PhysicsComponent>(this._tether, ref physicsComponent2))
			{
				physicsComponent2.Predict = true;
			}
			if (this._lastMousePosition.Value.Position.EqualsApprox(mapCoordinates.Position))
			{
				return;
			}
			this._lastMousePosition = new MapCoordinates?(mapCoordinates);
			base.RaiseNetworkEvent(new TetherMoveEvent
			{
				Coordinates = this._lastMousePosition.Value
			});
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00008E09 File Offset: 0x00007009
		private void StopDragging()
		{
			if (this._dragging == null)
			{
				return;
			}
			base.RaiseNetworkEvent(new StopTetherEvent());
			this._dragging = null;
			this._lastMousePosition = null;
			this._tether = null;
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00008E48 File Offset: 0x00007048
		private void StartDragging(EntityUid uid, MapCoordinates coordinates)
		{
			this._dragging = new EntityUid?(uid);
			this._lastMousePosition = new MapCoordinates?(coordinates);
			base.RaiseNetworkEvent(new StartTetherEvent
			{
				Entity = this._dragging.Value,
				Coordinates = coordinates
			});
		}

		// Token: 0x04000093 RID: 147
		[Dependency]
		private readonly IEyeManager _eyeManager;

		// Token: 0x04000094 RID: 148
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000095 RID: 149
		[Dependency]
		private readonly IInputManager _inputManager;

		// Token: 0x04000096 RID: 150
		[Dependency]
		private readonly InputSystem _inputSystem;

		// Token: 0x04000098 RID: 152
		private EntityUid? _dragging;

		// Token: 0x04000099 RID: 153
		private EntityUid? _tether;

		// Token: 0x0400009A RID: 154
		private MapCoordinates? _lastMousePosition;
	}
}
