using System;
using System.Runtime.CompilerServices;
using Content.Shared.Alert;
using Content.Shared.Clothing;
using Content.Shared.Inventory;
using Content.Shared.Movement.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared.Gravity
{
	// Token: 0x0200044D RID: 1101
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedGravitySystem : EntitySystem
	{
		// Token: 0x06000D58 RID: 3416 RVA: 0x0002C1C8 File Offset: 0x0002A3C8
		[NullableContext(2)]
		public bool IsWeightless(EntityUid uid, PhysicsComponent body = null, TransformComponent xform = null)
		{
			base.Resolve<PhysicsComponent>(uid, ref body, false);
			if (body == null || (body.BodyType & 4) > 0)
			{
				return false;
			}
			MovementIgnoreGravityComponent ignoreGravityComponent;
			if (base.TryComp<MovementIgnoreGravityComponent>(uid, ref ignoreGravityComponent))
			{
				return ignoreGravityComponent.Weightless;
			}
			GravityComponent gravity;
			GravityComponent mapGravity;
			EntityUid? ent;
			MagbootsComponent boots;
			return !base.Resolve<TransformComponent>(uid, ref xform, true) || ((!base.TryComp<GravityComponent>(xform.GridUid, ref gravity) || !gravity.Enabled) && (!base.TryComp<GravityComponent>(xform.MapUid, ref mapGravity) || !mapGravity.Enabled) && ((gravity == null && mapGravity == null) || !this._inventory.TryGetSlotEntity(uid, "shoes", out ent, null, null) || !base.TryComp<MagbootsComponent>(ent, ref boots) || !boots.On));
		}

		// Token: 0x06000D59 RID: 3417 RVA: 0x0002C284 File Offset: 0x0002A484
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GridInitializeEvent>(new EntityEventHandler<GridInitializeEvent>(this.OnGridInit), null, null);
			base.SubscribeLocalEvent<AlertSyncEvent>(new EntityEventHandler<AlertSyncEvent>(this.OnAlertsSync), null, null);
			base.SubscribeLocalEvent<AlertsComponent, EntParentChangedMessage>(new ComponentEventRefHandler<AlertsComponent, EntParentChangedMessage>(this.OnAlertsParentChange), null, null);
			base.SubscribeLocalEvent<GravityChangedEvent>(new EntityEventRefHandler<GravityChangedEvent>(this.OnGravityChange), null, null);
			base.SubscribeLocalEvent<GravityComponent, ComponentGetState>(new ComponentEventRefHandler<GravityComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<GravityComponent, ComponentHandleState>(new ComponentEventRefHandler<GravityComponent, ComponentHandleState>(this.OnHandleState), null, null);
			this.InitializeShake();
		}

		// Token: 0x06000D5A RID: 3418 RVA: 0x0002C315 File Offset: 0x0002A515
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this.UpdateShake();
		}

		// Token: 0x06000D5B RID: 3419 RVA: 0x0002C324 File Offset: 0x0002A524
		private void OnHandleState(EntityUid uid, GravityComponent component, ref ComponentHandleState args)
		{
			SharedGravitySystem.GravityComponentState state = args.Current as SharedGravitySystem.GravityComponentState;
			if (state == null)
			{
				return;
			}
			if (component.EnabledVV == state.Enabled)
			{
				return;
			}
			component.EnabledVV = state.Enabled;
			GravityChangedEvent ev = new GravityChangedEvent(uid, component.EnabledVV);
			base.RaiseLocalEvent<GravityChangedEvent>(uid, ref ev, true);
		}

		// Token: 0x06000D5C RID: 3420 RVA: 0x0002C374 File Offset: 0x0002A574
		private void OnGetState(EntityUid uid, GravityComponent component, ref ComponentGetState args)
		{
			args.State = new SharedGravitySystem.GravityComponentState(component.EnabledVV);
		}

		// Token: 0x06000D5D RID: 3421 RVA: 0x0002C388 File Offset: 0x0002A588
		private void OnGravityChange(ref GravityChangedEvent ev)
		{
			foreach (ValueTuple<AlertsComponent, TransformComponent> valueTuple in base.EntityQuery<AlertsComponent, TransformComponent>(true))
			{
				AlertsComponent comp = valueTuple.Item1;
				EntityUid? gridUid = valueTuple.Item2.GridUid;
				EntityUid changedGridIndex = ev.ChangedGridIndex;
				if (gridUid != null && (gridUid == null || !(gridUid.GetValueOrDefault() != changedGridIndex)))
				{
					if (!ev.HasGravity)
					{
						this._alerts.ShowAlert(comp.Owner, AlertType.Weightless, null, null);
					}
					else
					{
						this._alerts.ClearAlert(comp.Owner, AlertType.Weightless);
					}
				}
			}
		}

		// Token: 0x06000D5E RID: 3422 RVA: 0x0002C458 File Offset: 0x0002A658
		private void OnAlertsSync(AlertSyncEvent ev)
		{
			if (this.IsWeightless(ev.Euid, null, null))
			{
				this._alerts.ShowAlert(ev.Euid, AlertType.Weightless, null, null);
				return;
			}
			this._alerts.ClearAlert(ev.Euid, AlertType.Weightless);
		}

		// Token: 0x06000D5F RID: 3423 RVA: 0x0002C4AC File Offset: 0x0002A6AC
		private void OnAlertsParentChange(EntityUid uid, AlertsComponent component, ref EntParentChangedMessage args)
		{
			if (this.IsWeightless(component.Owner, null, null))
			{
				this._alerts.ShowAlert(uid, AlertType.Weightless, null, null);
				return;
			}
			this._alerts.ClearAlert(uid, AlertType.Weightless);
		}

		// Token: 0x06000D60 RID: 3424 RVA: 0x0002C4F6 File Offset: 0x0002A6F6
		private void OnGridInit(GridInitializeEvent ev)
		{
			this.EntityManager.EnsureComponent<GravityComponent>(ev.EntityUid);
		}

		// Token: 0x06000D61 RID: 3425 RVA: 0x0002C50A File Offset: 0x0002A70A
		private void InitializeShake()
		{
			base.SubscribeLocalEvent<GravityShakeComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<GravityShakeComponent, EntityUnpausedEvent>(this.OnShakeUnpaused), null, null);
			base.SubscribeLocalEvent<GravityShakeComponent, ComponentGetState>(new ComponentEventRefHandler<GravityShakeComponent, ComponentGetState>(this.OnShakeGetState), null, null);
			base.SubscribeLocalEvent<GravityShakeComponent, ComponentHandleState>(new ComponentEventRefHandler<GravityShakeComponent, ComponentHandleState>(this.OnShakeHandleState), null, null);
		}

		// Token: 0x06000D62 RID: 3426 RVA: 0x0002C548 File Offset: 0x0002A748
		private void OnShakeUnpaused(EntityUid uid, GravityShakeComponent component, ref EntityUnpausedEvent args)
		{
			component.NextShake += args.PausedTime;
		}

		// Token: 0x06000D63 RID: 3427 RVA: 0x0002C564 File Offset: 0x0002A764
		private void UpdateShake()
		{
			TimeSpan curTime = this.Timing.CurTime;
			EntityQuery<GravityComponent> gravityQuery = base.GetEntityQuery<GravityComponent>();
			foreach (GravityShakeComponent comp in base.EntityQuery<GravityShakeComponent>(false))
			{
				if (comp.NextShake <= curTime)
				{
					GravityComponent gravity;
					if (comp.ShakeTimes == 0 || !gravityQuery.TryGetComponent(comp.Owner, ref gravity))
					{
						base.RemCompDeferred<GravityShakeComponent>(comp.Owner);
					}
					else
					{
						this.ShakeGrid(comp.Owner, gravity);
						comp.ShakeTimes--;
						comp.NextShake += TimeSpan.FromSeconds(0.20000000298023224);
						base.Dirty(comp, null);
					}
				}
			}
		}

		// Token: 0x06000D64 RID: 3428 RVA: 0x0002C640 File Offset: 0x0002A840
		[NullableContext(2)]
		public void StartGridShake(EntityUid uid, GravityComponent gravity = null)
		{
			if (base.Terminating(uid, null))
			{
				return;
			}
			if (!base.Resolve<GravityComponent>(uid, ref gravity, false))
			{
				return;
			}
			GravityShakeComponent shake;
			if (!base.TryComp<GravityShakeComponent>(uid, ref shake))
			{
				shake = base.AddComp<GravityShakeComponent>(uid);
				shake.NextShake = this.Timing.CurTime;
			}
			shake.ShakeTimes = 10;
			base.Dirty(shake, null);
		}

		// Token: 0x06000D65 RID: 3429 RVA: 0x0002C699 File Offset: 0x0002A899
		[NullableContext(2)]
		protected virtual void ShakeGrid(EntityUid uid, GravityComponent comp = null)
		{
		}

		// Token: 0x06000D66 RID: 3430 RVA: 0x0002C69C File Offset: 0x0002A89C
		private void OnShakeHandleState(EntityUid uid, GravityShakeComponent component, ref ComponentHandleState args)
		{
			SharedGravitySystem.GravityShakeComponentState state = args.Current as SharedGravitySystem.GravityShakeComponentState;
			if (state == null)
			{
				return;
			}
			component.ShakeTimes = state.ShakeTimes;
			component.NextShake = state.NextShake;
		}

		// Token: 0x06000D67 RID: 3431 RVA: 0x0002C6D1 File Offset: 0x0002A8D1
		private void OnShakeGetState(EntityUid uid, GravityShakeComponent component, ref ComponentGetState args)
		{
			args.State = new SharedGravitySystem.GravityShakeComponentState
			{
				ShakeTimes = component.ShakeTimes,
				NextShake = component.NextShake
			};
		}

		// Token: 0x04000CE0 RID: 3296
		[Dependency]
		protected readonly IGameTiming Timing;

		// Token: 0x04000CE1 RID: 3297
		[Dependency]
		private readonly AlertsSystem _alerts;

		// Token: 0x04000CE2 RID: 3298
		[Dependency]
		private readonly InventorySystem _inventory;

		// Token: 0x04000CE3 RID: 3299
		protected const float GravityKick = 100f;

		// Token: 0x04000CE4 RID: 3300
		protected const float ShakeCooldown = 0.2f;

		// Token: 0x02000806 RID: 2054
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		private sealed class GravityComponentState : ComponentState
		{
			// Token: 0x17000510 RID: 1296
			// (get) Token: 0x060018CD RID: 6349 RVA: 0x0004EE35 File Offset: 0x0004D035
			public bool Enabled { get; }

			// Token: 0x060018CE RID: 6350 RVA: 0x0004EE3D File Offset: 0x0004D03D
			public GravityComponentState(bool enabled)
			{
				this.Enabled = enabled;
			}
		}

		// Token: 0x02000807 RID: 2055
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		protected sealed class GravityShakeComponentState : ComponentState
		{
			// Token: 0x040018AA RID: 6314
			public int ShakeTimes;

			// Token: 0x040018AB RID: 6315
			public TimeSpan NextShake;
		}
	}
}
