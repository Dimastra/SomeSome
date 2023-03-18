using System;
using System.Runtime.CompilerServices;
using Content.Shared.Maps;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;

namespace Content.Client.Maps
{
	// Token: 0x0200024A RID: 586
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GridDraggingSystem : SharedGridDraggingSystem
	{
		// Token: 0x17000324 RID: 804
		// (get) Token: 0x06000ECB RID: 3787 RVA: 0x000592A0 File Offset: 0x000574A0
		// (set) Token: 0x06000ECC RID: 3788 RVA: 0x000592A8 File Offset: 0x000574A8
		public bool Enabled { get; set; }

		// Token: 0x06000ECD RID: 3789 RVA: 0x000592B1 File Offset: 0x000574B1
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<GridDragToggleMessage>(new EntityEventHandler<GridDragToggleMessage>(this.OnToggleMessage), null, null);
		}

		// Token: 0x06000ECE RID: 3790 RVA: 0x000592CD File Offset: 0x000574CD
		private void OnToggleMessage(GridDragToggleMessage ev)
		{
			if (this.Enabled == ev.Enabled)
			{
				return;
			}
			this.Enabled = ev.Enabled;
			if (!this.Enabled)
			{
				this.StopDragging();
			}
		}

		// Token: 0x06000ECF RID: 3791 RVA: 0x000592F8 File Offset: 0x000574F8
		private void StartDragging(EntityUid grid, Vector2 localPosition)
		{
			this._dragging = new EntityUid?(grid);
			this._localPosition = localPosition;
			PhysicsComponent physicsComponent;
			if (base.TryComp<PhysicsComponent>(grid, ref physicsComponent))
			{
				base.RaiseNetworkEvent(new GridDragVelocityRequest
				{
					Grid = grid,
					LinearVelocity = Vector2.Zero
				});
			}
		}

		// Token: 0x06000ED0 RID: 3792 RVA: 0x00059340 File Offset: 0x00057540
		private void StopDragging()
		{
			if (this._dragging == null)
			{
				return;
			}
			TransformComponent transformComponent;
			PhysicsComponent physicsComponent;
			if (this._lastMousePosition != null && base.TryComp<TransformComponent>(this._dragging.Value, ref transformComponent) && base.TryComp<PhysicsComponent>(this._dragging.Value, ref physicsComponent) && transformComponent.MapID == this._lastMousePosition.Value.MapId)
			{
				TimeSpan tickPeriod = this._gameTiming.TickPeriod;
				Vector2 vector = this._lastMousePosition.Value.Position - transformComponent.WorldPosition;
				base.RaiseNetworkEvent(new GridDragVelocityRequest
				{
					Grid = this._dragging.Value,
					LinearVelocity = ((vector.LengthSquared > 0f) ? (vector / (float)tickPeriod.TotalSeconds * 0.25f) : Vector2.Zero)
				});
			}
			this._dragging = null;
			this._localPosition = Vector2.Zero;
			this._lastMousePosition = null;
		}

		// Token: 0x06000ED1 RID: 3793 RVA: 0x00059454 File Offset: 0x00057654
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
				MapGridComponent mapGridComponent;
				if (!this._mapManager.TryFindGridAt(mapCoordinates, ref mapGridComponent))
				{
					return;
				}
				this.StartDragging(mapGridComponent.Owner, base.Transform(mapGridComponent.Owner).InvWorldMatrix.Transform(mapCoordinates.Position));
			}
			TransformComponent transformComponent;
			if (!base.TryComp<TransformComponent>(this._dragging, ref transformComponent))
			{
				this.StopDragging();
				return;
			}
			if (transformComponent.MapID != mapCoordinates.MapId)
			{
				this.StopDragging();
				return;
			}
			if (transformComponent.WorldMatrix.Transform(this._localPosition).EqualsApprox(mapCoordinates.Position, 0.009999999776482582))
			{
				return;
			}
			Vector2 vector = mapCoordinates.Position - transformComponent.WorldRotation.RotateVec(ref this._localPosition);
			this._lastMousePosition = new MapCoordinates?(new MapCoordinates(vector, mapCoordinates.MapId));
			base.RaiseNetworkEvent(new GridDragRequestPosition
			{
				Grid = this._dragging.Value,
				WorldPosition = vector
			});
		}

		// Token: 0x04000752 RID: 1874
		[Dependency]
		private readonly IEyeManager _eyeManager;

		// Token: 0x04000753 RID: 1875
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000754 RID: 1876
		[Dependency]
		private readonly IInputManager _inputManager;

		// Token: 0x04000755 RID: 1877
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000756 RID: 1878
		[Dependency]
		private readonly InputSystem _inputSystem;

		// Token: 0x04000758 RID: 1880
		private EntityUid? _dragging;

		// Token: 0x04000759 RID: 1881
		private Vector2 _localPosition;

		// Token: 0x0400075A RID: 1882
		private MapCoordinates? _lastMousePosition;
	}
}
