using System;
using System.Runtime.CompilerServices;
using Content.Shared.CCVar;
using Content.Shared.Gravity;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Pulling.Components;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Controllers;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;

namespace Content.Shared.Friction
{
	// Token: 0x0200046E RID: 1134
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TileFrictionController : VirtualController
	{
		// Token: 0x06000DBF RID: 3519 RVA: 0x0002CB90 File Offset: 0x0002AD90
		public override void Initialize()
		{
			base.Initialize();
			IConfigurationManager configurationManager = IoCManager.Resolve<IConfigurationManager>();
			configurationManager.OnValueChanged<float>(CCVars.TileFrictionModifier, new Action<float>(this.SetFrictionModifier), true);
			configurationManager.OnValueChanged<float>(CCVars.StopSpeed, new Action<float>(this.SetStopSpeed), true);
			base.SubscribeLocalEvent<TileFrictionModifierComponent, ComponentGetState>(new ComponentEventRefHandler<TileFrictionModifierComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<TileFrictionModifierComponent, ComponentHandleState>(new ComponentEventRefHandler<TileFrictionModifierComponent, ComponentHandleState>(this.OnHandleState), null, null);
		}

		// Token: 0x06000DC0 RID: 3520 RVA: 0x0002CC00 File Offset: 0x0002AE00
		private void OnHandleState(EntityUid uid, TileFrictionModifierComponent component, ref ComponentHandleState args)
		{
			TileFrictionController.TileFrictionComponentState tileState = args.Current as TileFrictionController.TileFrictionComponentState;
			if (tileState == null)
			{
				return;
			}
			component.Modifier = tileState.Modifier;
		}

		// Token: 0x06000DC1 RID: 3521 RVA: 0x0002CC29 File Offset: 0x0002AE29
		private void OnGetState(EntityUid uid, TileFrictionModifierComponent component, ref ComponentGetState args)
		{
			args.State = new TileFrictionController.TileFrictionComponentState(component.Modifier);
		}

		// Token: 0x06000DC2 RID: 3522 RVA: 0x0002CC3C File Offset: 0x0002AE3C
		private void SetStopSpeed(float value)
		{
			this._stopSpeed = value;
		}

		// Token: 0x06000DC3 RID: 3523 RVA: 0x0002CC45 File Offset: 0x0002AE45
		private void SetFrictionModifier(float value)
		{
			this._frictionModifier = value;
		}

		// Token: 0x06000DC4 RID: 3524 RVA: 0x0002CC4E File Offset: 0x0002AE4E
		public override void Shutdown()
		{
			base.Shutdown();
			IConfigurationManager configurationManager = IoCManager.Resolve<IConfigurationManager>();
			configurationManager.UnsubValueChanged<float>(CCVars.TileFrictionModifier, new Action<float>(this.SetFrictionModifier));
			configurationManager.UnsubValueChanged<float>(CCVars.StopSpeed, new Action<float>(this.SetStopSpeed));
		}

		// Token: 0x06000DC5 RID: 3525 RVA: 0x0002CC88 File Offset: 0x0002AE88
		public override void UpdateBeforeMapSolve(bool prediction, PhysicsMapComponent mapComponent, float frameTime)
		{
			base.UpdateBeforeMapSolve(prediction, mapComponent, frameTime);
			EntityQuery<TileFrictionModifierComponent> frictionQuery = base.GetEntityQuery<TileFrictionModifierComponent>();
			EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
			EntityQuery<SharedPullerComponent> pullerQuery = base.GetEntityQuery<SharedPullerComponent>();
			EntityQuery<SharedPullableComponent> pullableQuery = base.GetEntityQuery<SharedPullableComponent>();
			foreach (PhysicsComponent body in mapComponent.AwakeBodies)
			{
				EntityUid uid = body.Owner;
				if ((!prediction || body.Predict) && body.BodyStatus != 1 && !this._mover.UseMobMovement(body.Owner) && (!body.LinearVelocity.Equals(Vector2.Zero) || !body.AngularVelocity.Equals(0f)))
				{
					TransformComponent xform;
					if (!xformQuery.TryGetComponent(uid, ref xform))
					{
						string text = "physics";
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(54, 1);
						defaultInterpolatedStringHandler.AppendLiteral("Unable to get transform for ");
						defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(body.Owner));
						defaultInterpolatedStringHandler.AppendLiteral(" in tilefrictioncontroller");
						Logger.ErrorS(text, defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						float surfaceFriction = this.GetTileFriction(uid, body, xform);
						float bodyModifier = 1f;
						TileFrictionModifierComponent frictionComp;
						if (frictionQuery.TryGetComponent(uid, ref frictionComp))
						{
							bodyModifier = frictionComp.Modifier;
						}
						TileFrictionEvent ev = new TileFrictionEvent(bodyModifier);
						base.RaiseLocalEvent<TileFrictionEvent>(uid, ref ev, false);
						bodyModifier = ev.Modifier;
						SharedPullerComponent puller;
						SharedPullableComponent pullable;
						if (pullerQuery.TryGetComponent(uid, ref puller) && puller.Pulling != null && pullableQuery.TryGetComponent(uid, ref pullable) && pullable.BeingPulled)
						{
							bodyModifier *= 0.2f;
						}
						float friction = this._frictionModifier * surfaceFriction * bodyModifier;
						this.ReduceLinearVelocity(uid, prediction, body, friction, frameTime);
						this.ReduceAngularVelocity(uid, prediction, body, friction, frameTime);
					}
				}
			}
		}

		// Token: 0x06000DC6 RID: 3526 RVA: 0x0002CE7C File Offset: 0x0002B07C
		private void ReduceLinearVelocity(EntityUid uid, bool prediction, PhysicsComponent body, float friction, float frameTime)
		{
			float speed = body.LinearVelocity.Length;
			if (speed <= 0f)
			{
				return;
			}
			float drop = 0f;
			if (friction > 0f)
			{
				float control;
				if (!prediction)
				{
					control = ((speed < this._stopSpeed) ? this._stopSpeed : speed);
				}
				else
				{
					control = speed;
				}
				drop += control * friction * frameTime;
			}
			float newSpeed = MathF.Max(0f, speed - drop);
			newSpeed /= speed;
			this._physics.SetLinearVelocity(uid, body.LinearVelocity * newSpeed, true, true, null, body);
		}

		// Token: 0x06000DC7 RID: 3527 RVA: 0x0002CF00 File Offset: 0x0002B100
		private void ReduceAngularVelocity(EntityUid uid, bool prediction, PhysicsComponent body, float friction, float frameTime)
		{
			float speed = MathF.Abs(body.AngularVelocity);
			if (speed <= 0f)
			{
				return;
			}
			float drop = 0f;
			if (friction > 0f)
			{
				float control;
				if (!prediction)
				{
					control = ((speed < this._stopSpeed) ? this._stopSpeed : speed);
				}
				else
				{
					control = speed;
				}
				drop += control * friction * frameTime;
			}
			float newSpeed = MathF.Max(0f, speed - drop);
			newSpeed /= speed;
			this._physics.SetAngularVelocity(uid, body.AngularVelocity * newSpeed, true, null, body);
		}

		// Token: 0x06000DC8 RID: 3528 RVA: 0x0002CF80 File Offset: 0x0002B180
		private float GetTileFriction(EntityUid uid, PhysicsComponent body, TransformComponent xform)
		{
			if (this._gravity.IsWeightless(uid, body, xform))
			{
				return 0f;
			}
			if (!xform.Coordinates.IsValid(this.EntityManager))
			{
				return 0f;
			}
			MapGridComponent grid;
			if (this._mapManager.TryGetGrid(xform.GridUid, ref grid))
			{
				TileRef tile = grid.GetTileRef(xform.Coordinates);
				GravityComponent gravity;
				if (tile.Tile.IsEmpty && base.HasComp<MapComponent>(xform.GridUid) && (!base.TryComp<GravityComponent>(xform.GridUid, ref gravity) || gravity.Enabled))
				{
					return 0.3f;
				}
				return this._tileDefinitionManager[(int)tile.Tile.TypeId].Friction;
			}
			else
			{
				TileFrictionModifierComponent friction;
				if (!base.TryComp<TileFrictionModifierComponent>(xform.MapUid, ref friction))
				{
					return 0.3f;
				}
				return friction.Modifier;
			}
		}

		// Token: 0x04000D16 RID: 3350
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000D17 RID: 3351
		[Dependency]
		private readonly ITileDefinitionManager _tileDefinitionManager;

		// Token: 0x04000D18 RID: 3352
		[Dependency]
		private readonly SharedGravitySystem _gravity;

		// Token: 0x04000D19 RID: 3353
		[Dependency]
		private readonly SharedMoverController _mover;

		// Token: 0x04000D1A RID: 3354
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x04000D1B RID: 3355
		private float _stopSpeed;

		// Token: 0x04000D1C RID: 3356
		private float _frictionModifier;

		// Token: 0x04000D1D RID: 3357
		private const float DefaultFriction = 0.3f;

		// Token: 0x02000809 RID: 2057
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		private sealed class TileFrictionComponentState : ComponentState
		{
			// Token: 0x060018D0 RID: 6352 RVA: 0x0004EE54 File Offset: 0x0004D054
			public TileFrictionComponentState(float modifier)
			{
				this.Modifier = modifier;
			}

			// Token: 0x040018B3 RID: 6323
			public float Modifier;
		}
	}
}
