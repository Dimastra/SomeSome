using System;
using System.Runtime.CompilerServices;
using Content.Shared.StepTrigger.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;

namespace Content.Shared.StepTrigger.Systems
{
	// Token: 0x0200014A RID: 330
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StepTriggerSystem : EntitySystem
	{
		// Token: 0x060003F1 RID: 1009 RVA: 0x0000FC74 File Offset: 0x0000DE74
		public override void Initialize()
		{
			base.UpdatesOutsidePrediction = true;
			ComponentEventRefHandler<StepTriggerComponent, ComponentGetState> componentEventRefHandler;
			if ((componentEventRefHandler = StepTriggerSystem.<>O.<0>__TriggerGetState) == null)
			{
				componentEventRefHandler = (StepTriggerSystem.<>O.<0>__TriggerGetState = new ComponentEventRefHandler<StepTriggerComponent, ComponentGetState>(StepTriggerSystem.TriggerGetState));
			}
			base.SubscribeLocalEvent<StepTriggerComponent, ComponentGetState>(componentEventRefHandler, null, null);
			base.SubscribeLocalEvent<StepTriggerComponent, ComponentHandleState>(new ComponentEventRefHandler<StepTriggerComponent, ComponentHandleState>(this.TriggerHandleState), null, null);
			base.SubscribeLocalEvent<StepTriggerComponent, StartCollideEvent>(new ComponentEventRefHandler<StepTriggerComponent, StartCollideEvent>(this.OnStartCollide), null, null);
			base.SubscribeLocalEvent<StepTriggerComponent, EndCollideEvent>(new ComponentEventRefHandler<StepTriggerComponent, EndCollideEvent>(this.OnEndCollide), null, null);
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x0000FCE8 File Offset: 0x0000DEE8
		public override void Update(float frameTime)
		{
			EntityQuery<PhysicsComponent> query = base.GetEntityQuery<PhysicsComponent>();
			StepTriggerActiveComponent active;
			StepTriggerComponent trigger;
			TransformComponent transform;
			while (base.EntityQueryEnumerator<StepTriggerActiveComponent, StepTriggerComponent, TransformComponent>().MoveNext(ref active, ref trigger, ref transform))
			{
				if (this.Update(trigger, transform, query))
				{
					base.RemCompDeferred(trigger.Owner, active);
				}
			}
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x0000FD30 File Offset: 0x0000DF30
		private bool Update(StepTriggerComponent component, TransformComponent transform, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<PhysicsComponent> query)
		{
			if (!component.Active || component.Colliding.Count == 0)
			{
				return true;
			}
			foreach (EntityUid otherUid in component.Colliding)
			{
				this.UpdateColliding(component, transform, otherUid, query);
			}
			return false;
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x0000FDA0 File Offset: 0x0000DFA0
		private void UpdateColliding(StepTriggerComponent component, TransformComponent ownerTransform, EntityUid otherUid, [Nullable(new byte[]
		{
			0,
			1
		})] EntityQuery<PhysicsComponent> query)
		{
			PhysicsComponent otherPhysics;
			if (!query.TryGetComponent(otherUid, ref otherPhysics))
			{
				return;
			}
			Box2 ourAabb = this._entityLookup.GetWorldAABB(component.Owner, ownerTransform);
			Box2 otherAabb = this._entityLookup.GetWorldAABB(otherUid, null);
			if (!ourAabb.Intersects(ref otherAabb))
			{
				if (component.CurrentlySteppedOn.Remove(otherUid))
				{
					base.Dirty(component, null);
				}
				return;
			}
			if (otherPhysics.LinearVelocity.Length < component.RequiredTriggerSpeed || component.CurrentlySteppedOn.Contains(otherUid) || otherAabb.IntersectPercentage(ref ourAabb) < component.IntersectRatio || !this.CanTrigger(component.Owner, otherUid, component))
			{
				return;
			}
			StepTriggeredEvent ev = new StepTriggeredEvent
			{
				Source = component.Owner,
				Tripper = otherUid
			};
			base.RaiseLocalEvent<StepTriggeredEvent>(component.Owner, ref ev, true);
			component.CurrentlySteppedOn.Add(otherUid);
			base.Dirty(component, null);
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x0000FE84 File Offset: 0x0000E084
		private bool CanTrigger(EntityUid uid, EntityUid otherUid, StepTriggerComponent component)
		{
			if (!component.Active || component.CurrentlySteppedOn.Contains(otherUid))
			{
				return false;
			}
			StepTriggerAttemptEvent msg = new StepTriggerAttemptEvent
			{
				Source = uid,
				Tripper = otherUid
			};
			base.RaiseLocalEvent<StepTriggerAttemptEvent>(uid, ref msg, true);
			return msg.Continue && !msg.Cancelled;
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x0000FEE0 File Offset: 0x0000E0E0
		private void OnStartCollide(EntityUid uid, StepTriggerComponent component, ref StartCollideEvent args)
		{
			EntityUid otherUid = args.OtherFixture.Body.Owner;
			if (!args.OtherFixture.Hard)
			{
				return;
			}
			if (!this.CanTrigger(uid, otherUid, component))
			{
				return;
			}
			base.EnsureComp<StepTriggerActiveComponent>(uid);
			if (component.Colliding.Add(otherUid))
			{
				base.Dirty(component, null);
			}
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x0000FF38 File Offset: 0x0000E138
		private void OnEndCollide(EntityUid uid, StepTriggerComponent component, ref EndCollideEvent args)
		{
			EntityUid otherUid = args.OtherFixture.Body.Owner;
			if (!component.Colliding.Remove(otherUid))
			{
				return;
			}
			component.CurrentlySteppedOn.Remove(otherUid);
			base.Dirty(component, null);
			if (component.Colliding.Count == 0)
			{
				base.RemCompDeferred<StepTriggerActiveComponent>(uid);
			}
		}

		// Token: 0x060003F8 RID: 1016 RVA: 0x0000FF90 File Offset: 0x0000E190
		private void TriggerHandleState(EntityUid uid, StepTriggerComponent component, ref ComponentHandleState args)
		{
			StepTriggerComponentState state = args.Current as StepTriggerComponentState;
			if (state == null)
			{
				return;
			}
			component.RequiredTriggerSpeed = state.RequiredTriggerSpeed;
			component.IntersectRatio = state.IntersectRatio;
			component.Active = state.Active;
			component.CurrentlySteppedOn.Clear();
			component.Colliding.Clear();
			component.CurrentlySteppedOn.UnionWith(state.CurrentlySteppedOn);
			component.Colliding.UnionWith(state.Colliding);
			if (component.Colliding.Count > 0)
			{
				base.EnsureComp<StepTriggerActiveComponent>(uid);
				return;
			}
			base.RemCompDeferred<StepTriggerActiveComponent>(uid);
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x00010028 File Offset: 0x0000E228
		private static void TriggerGetState(EntityUid uid, StepTriggerComponent component, ref ComponentGetState args)
		{
			args.State = new StepTriggerComponentState(component.IntersectRatio, component.CurrentlySteppedOn, component.Colliding, component.RequiredTriggerSpeed, component.Active);
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x00010053 File Offset: 0x0000E253
		[NullableContext(2)]
		public void SetIntersectRatio(EntityUid uid, float ratio, StepTriggerComponent component = null)
		{
			if (!base.Resolve<StepTriggerComponent>(uid, ref component, true))
			{
				return;
			}
			if (MathHelper.CloseToPercent(component.IntersectRatio, ratio, 1E-05))
			{
				return;
			}
			component.IntersectRatio = ratio;
			base.Dirty(component, null);
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x00010089 File Offset: 0x0000E289
		[NullableContext(2)]
		public void SetRequiredTriggerSpeed(EntityUid uid, float speed, StepTriggerComponent component = null)
		{
			if (!base.Resolve<StepTriggerComponent>(uid, ref component, true))
			{
				return;
			}
			if (MathHelper.CloseToPercent(component.RequiredTriggerSpeed, speed, 1E-05))
			{
				return;
			}
			component.RequiredTriggerSpeed = speed;
			base.Dirty(component, null);
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x000100BF File Offset: 0x0000E2BF
		[NullableContext(2)]
		public void SetActive(EntityUid uid, bool active, StepTriggerComponent component = null)
		{
			if (!base.Resolve<StepTriggerComponent>(uid, ref component, true))
			{
				return;
			}
			if (active == component.Active)
			{
				return;
			}
			component.Active = active;
			base.Dirty(component, null);
		}

		// Token: 0x040003D2 RID: 978
		[Dependency]
		private readonly EntityLookupSystem _entityLookup;

		// Token: 0x020007A1 RID: 1953
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040017BC RID: 6076
			[Nullable(new byte[]
			{
				0,
				1
			})]
			public static ComponentEventRefHandler<StepTriggerComponent, ComponentGetState> <0>__TriggerGetState;
		}
	}
}
