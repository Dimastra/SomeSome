using System;
using System.Runtime.CompilerServices;
using Content.Shared.Buckle.Components;
using Content.Shared.DragDrop;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Movement.Events;
using Content.Shared.Physics;
using Content.Shared.Standing;
using Content.Shared.Throwing;
using Content.Shared.Vehicle.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;

namespace Content.Shared.Buckle
{
	// Token: 0x02000640 RID: 1600
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedBuckleSystem : EntitySystem
	{
		// Token: 0x06001347 RID: 4935 RVA: 0x0004036C File Offset: 0x0003E56C
		private void InitializeBuckle()
		{
			base.SubscribeLocalEvent<BuckleComponent, PreventCollideEvent>(new ComponentEventRefHandler<BuckleComponent, PreventCollideEvent>(this.PreventCollision), null, null);
			base.SubscribeLocalEvent<BuckleComponent, DownAttemptEvent>(new ComponentEventHandler<BuckleComponent, DownAttemptEvent>(this.HandleDown), null, null);
			base.SubscribeLocalEvent<BuckleComponent, StandAttemptEvent>(new ComponentEventHandler<BuckleComponent, StandAttemptEvent>(this.HandleStand), null, null);
			base.SubscribeLocalEvent<BuckleComponent, ThrowPushbackAttemptEvent>(new ComponentEventHandler<BuckleComponent, ThrowPushbackAttemptEvent>(this.HandleThrowPushback), null, null);
			base.SubscribeLocalEvent<BuckleComponent, UpdateCanMoveEvent>(new ComponentEventHandler<BuckleComponent, UpdateCanMoveEvent>(this.HandleMove), null, null);
			base.SubscribeLocalEvent<BuckleComponent, ChangeDirectionAttemptEvent>(new ComponentEventHandler<BuckleComponent, ChangeDirectionAttemptEvent>(this.OnBuckleChangeDirectionAttempt), null, null);
		}

		// Token: 0x06001348 RID: 4936 RVA: 0x000403F4 File Offset: 0x0003E5F4
		private void PreventCollision(EntityUid uid, BuckleComponent component, ref PreventCollideEvent args)
		{
			if (args.BodyB.Owner != component.LastEntityBuckledTo)
			{
				return;
			}
			if (component.Buckled || component.DontCollide)
			{
				args.Cancelled = true;
			}
		}

		// Token: 0x06001349 RID: 4937 RVA: 0x00040447 File Offset: 0x0003E647
		private void HandleDown(EntityUid uid, BuckleComponent component, DownAttemptEvent args)
		{
			if (component.Buckled)
			{
				args.Cancel();
			}
		}

		// Token: 0x0600134A RID: 4938 RVA: 0x00040457 File Offset: 0x0003E657
		private void HandleStand(EntityUid uid, BuckleComponent component, StandAttemptEvent args)
		{
			if (component.Buckled)
			{
				args.Cancel();
			}
		}

		// Token: 0x0600134B RID: 4939 RVA: 0x00040467 File Offset: 0x0003E667
		private void HandleThrowPushback(EntityUid uid, BuckleComponent component, ThrowPushbackAttemptEvent args)
		{
			if (component.Buckled)
			{
				args.Cancel();
			}
		}

		// Token: 0x0600134C RID: 4940 RVA: 0x00040477 File Offset: 0x0003E677
		private void HandleMove(EntityUid uid, BuckleComponent component, UpdateCanMoveEvent args)
		{
			if (component.LifeStage > 6)
			{
				return;
			}
			if (component.Buckled && !base.HasComp<VehicleComponent>(base.Transform(uid).ParentUid))
			{
				args.Cancel();
			}
		}

		// Token: 0x0600134D RID: 4941 RVA: 0x000404A5 File Offset: 0x0003E6A5
		private void OnBuckleChangeDirectionAttempt(EntityUid uid, BuckleComponent component, ChangeDirectionAttemptEvent args)
		{
			if (component.Buckled)
			{
				args.Cancel();
			}
		}

		// Token: 0x0600134E RID: 4942 RVA: 0x000404B5 File Offset: 0x0003E6B5
		[NullableContext(2)]
		public bool IsBuckled(EntityUid uid, BuckleComponent component = null)
		{
			return base.Resolve<BuckleComponent>(uid, ref component, false) && component.Buckled;
		}

		// Token: 0x0600134F RID: 4943 RVA: 0x000404CC File Offset: 0x0003E6CC
		public void ReAttach(EntityUid buckleId, StrapComponent strap, [Nullable(2)] BuckleComponent buckle = null)
		{
			if (!base.Resolve<BuckleComponent>(buckleId, ref buckle, false))
			{
				return;
			}
			TransformComponent ownTransform = base.Transform(buckleId);
			TransformComponent strapTransform = base.Transform(strap.Owner);
			ownTransform.Coordinates = new EntityCoordinates(strapTransform.Owner, strap.BuckleOffset);
			if (ownTransform.ParentUid != strapTransform.Owner)
			{
				return;
			}
			ownTransform.LocalRotation = Angle.Zero;
			switch (strap.Position)
			{
			case StrapPosition.None:
				break;
			case StrapPosition.Stand:
				this._standing.Stand(buckleId, null, null, false);
				return;
			case StrapPosition.Down:
				this._standing.Down(buckleId, false, false, null, null, null);
				break;
			default:
				return;
			}
		}

		// Token: 0x06001350 RID: 4944 RVA: 0x0004056E File Offset: 0x0003E76E
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeBuckle();
			this.InitializeStrap();
		}

		// Token: 0x06001351 RID: 4945 RVA: 0x00040582 File Offset: 0x0003E782
		private void InitializeStrap()
		{
			base.SubscribeLocalEvent<StrapComponent, MoveEvent>(new ComponentEventRefHandler<StrapComponent, MoveEvent>(this.OnStrapRotate), null, null);
			base.SubscribeLocalEvent<StrapComponent, ComponentHandleState>(new ComponentEventRefHandler<StrapComponent, ComponentHandleState>(this.OnStrapHandleState), null, null);
			base.SubscribeLocalEvent<StrapComponent, CanDropTargetEvent>(new ComponentEventRefHandler<StrapComponent, CanDropTargetEvent>(this.OnStrapCanDropOn), null, null);
		}

		// Token: 0x06001352 RID: 4946 RVA: 0x000405C0 File Offset: 0x0003E7C0
		private void OnStrapHandleState(EntityUid uid, StrapComponent component, ref ComponentHandleState args)
		{
			StrapComponentState state = args.Current as StrapComponentState;
			if (state == null)
			{
				return;
			}
			component.Position = state.Position;
			component.BuckleOffsetUnclamped = state.BuckleOffsetClamped;
			component.BuckledEntities.Clear();
			component.BuckledEntities.UnionWith(state.BuckledEntities);
			component.MaxBuckleDistance = state.MaxBuckleDistance;
		}

		// Token: 0x06001353 RID: 4947 RVA: 0x00040620 File Offset: 0x0003E820
		private void OnStrapRotate(EntityUid uid, StrapComponent component, ref MoveEvent args)
		{
			if (this.GameTiming.ApplyingState || args.NewRotation == args.OldRotation)
			{
				return;
			}
			foreach (EntityUid buckledEntity in component.BuckledEntities)
			{
				BuckleComponent buckled;
				if (this.EntityManager.TryGetComponent<BuckleComponent>(buckledEntity, ref buckled))
				{
					if (buckled.Buckled)
					{
						EntityUid? lastEntityBuckledTo = buckled.LastEntityBuckledTo;
						if (lastEntityBuckledTo != null && (lastEntityBuckledTo == null || !(lastEntityBuckledTo.GetValueOrDefault() != uid)))
						{
							this.ReAttach(buckledEntity, component, buckled);
							base.Dirty(buckled, null);
							continue;
						}
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(85, 2);
					defaultInterpolatedStringHandler.AppendLiteral("A moving strap entity ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
					defaultInterpolatedStringHandler.AppendLiteral(" attempted to re-parent an entity that does not 'belong' to it ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(buckledEntity));
					Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
		}

		// Token: 0x06001354 RID: 4948 RVA: 0x00040740 File Offset: 0x0003E940
		[NullableContext(2)]
		protected bool StrapCanDragDropOn(EntityUid strapId, EntityUid user, EntityUid target, EntityUid buckleId, StrapComponent strap = null, BuckleComponent buckle = null)
		{
			SharedBuckleSystem.<>c__DisplayClass16_0 CS$<>8__locals1 = new SharedBuckleSystem.<>c__DisplayClass16_0();
			CS$<>8__locals1.user = user;
			CS$<>8__locals1.buckleId = buckleId;
			CS$<>8__locals1.target = target;
			return base.Resolve<StrapComponent>(strapId, ref strap, false) && base.Resolve<BuckleComponent>(CS$<>8__locals1.buckleId, ref buckle, false) && this._interactions.InRangeUnobstructed(CS$<>8__locals1.target, CS$<>8__locals1.buckleId, buckle.Range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, new SharedInteractionSystem.Ignored(CS$<>8__locals1.<StrapCanDragDropOn>g__Ignored|0), false);
		}

		// Token: 0x06001355 RID: 4949 RVA: 0x000407B8 File Offset: 0x0003E9B8
		private void OnStrapCanDropOn(EntityUid uid, StrapComponent strap, ref CanDropTargetEvent args)
		{
			args.CanDrop = this.StrapCanDragDropOn(uid, args.User, uid, args.Dragged, strap, null);
			args.Handled = true;
		}

		// Token: 0x04001342 RID: 4930
		[Dependency]
		protected readonly IGameTiming GameTiming;

		// Token: 0x04001343 RID: 4931
		[Dependency]
		private readonly StandingStateSystem _standing;

		// Token: 0x04001344 RID: 4932
		[Dependency]
		private readonly SharedInteractionSystem _interactions;
	}
}
