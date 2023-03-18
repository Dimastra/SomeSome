using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Movement.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;

namespace Content.Shared.Movement.Systems
{
	// Token: 0x020002DF RID: 735
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SlowContactsSystem : EntitySystem
	{
		// Token: 0x0600084A RID: 2122 RVA: 0x0001C570 File Offset: 0x0001A770
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SlowContactsComponent, StartCollideEvent>(new ComponentEventRefHandler<SlowContactsComponent, StartCollideEvent>(this.OnEntityEnter), null, null);
			base.SubscribeLocalEvent<SlowContactsComponent, EndCollideEvent>(new ComponentEventRefHandler<SlowContactsComponent, EndCollideEvent>(this.OnEntityExit), null, null);
			base.SubscribeLocalEvent<SlowedByContactComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<SlowedByContactComponent, RefreshMovementSpeedModifiersEvent>(this.MovementSpeedCheck), null, null);
			base.SubscribeLocalEvent<SlowContactsComponent, ComponentHandleState>(new ComponentEventRefHandler<SlowContactsComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeLocalEvent<SlowContactsComponent, ComponentGetState>(new ComponentEventRefHandler<SlowContactsComponent, ComponentGetState>(this.OnGetState), null, null);
			base.UpdatesAfter.Add(typeof(SharedPhysicsSystem));
		}

		// Token: 0x0600084B RID: 2123 RVA: 0x0001C5FC File Offset: 0x0001A7FC
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this._toRemove.Clear();
			foreach (EntityUid ent in this._toUpdate)
			{
				this._speedModifierSystem.RefreshMovementSpeedModifiers(ent, null);
			}
			foreach (EntityUid ent2 in this._toRemove)
			{
				base.RemComp<SlowedByContactComponent>(ent2);
			}
			this._toUpdate.Clear();
		}

		// Token: 0x0600084C RID: 2124 RVA: 0x0001C6B8 File Offset: 0x0001A8B8
		private void OnGetState(EntityUid uid, SlowContactsComponent component, ref ComponentGetState args)
		{
			args.State = new SlowContactsComponentState(component.WalkSpeedModifier, component.SprintSpeedModifier);
		}

		// Token: 0x0600084D RID: 2125 RVA: 0x0001C6D4 File Offset: 0x0001A8D4
		private void OnHandleState(EntityUid uid, SlowContactsComponent component, ref ComponentHandleState args)
		{
			SlowContactsComponentState state = args.Current as SlowContactsComponentState;
			if (state == null)
			{
				return;
			}
			component.WalkSpeedModifier = state.WalkSpeedModifier;
			component.SprintSpeedModifier = state.SprintSpeedModifier;
		}

		// Token: 0x0600084E RID: 2126 RVA: 0x0001C70C File Offset: 0x0001A90C
		private void MovementSpeedCheck(EntityUid uid, SlowedByContactComponent component, RefreshMovementSpeedModifiersEvent args)
		{
			PhysicsComponent physicsComponent;
			if (!this.EntityManager.TryGetComponent<PhysicsComponent>(uid, ref physicsComponent))
			{
				return;
			}
			float walkSpeed = 1f;
			float sprintSpeed = 1f;
			bool remove = true;
			foreach (PhysicsComponent physicsComponent2 in this._physics.GetContactingEntities(physicsComponent, false))
			{
				EntityUid ent = physicsComponent2.Owner;
				SlowContactsComponent slowContactsComponent;
				if (base.TryComp<SlowContactsComponent>(ent, ref slowContactsComponent) && (slowContactsComponent.IgnoreWhitelist == null || !slowContactsComponent.IgnoreWhitelist.IsValid(uid, null)))
				{
					walkSpeed = Math.Min(walkSpeed, slowContactsComponent.WalkSpeedModifier);
					sprintSpeed = Math.Min(sprintSpeed, slowContactsComponent.SprintSpeedModifier);
					remove = false;
				}
			}
			args.ModifySpeed(walkSpeed, sprintSpeed);
			if (remove)
			{
				this._toRemove.Add(uid);
			}
		}

		// Token: 0x0600084F RID: 2127 RVA: 0x0001C7E0 File Offset: 0x0001A9E0
		private void OnEntityExit(EntityUid uid, SlowContactsComponent component, ref EndCollideEvent args)
		{
			EntityUid otherUid = args.OtherFixture.Body.Owner;
			if (base.HasComp<MovementSpeedModifierComponent>(otherUid))
			{
				this._toUpdate.Add(otherUid);
			}
		}

		// Token: 0x06000850 RID: 2128 RVA: 0x0001C814 File Offset: 0x0001AA14
		private void OnEntityEnter(EntityUid uid, SlowContactsComponent component, ref StartCollideEvent args)
		{
			EntityUid otherUid = args.OtherFixture.Body.Owner;
			if (!base.HasComp<MovementSpeedModifierComponent>(otherUid))
			{
				return;
			}
			base.EnsureComp<SlowedByContactComponent>(otherUid);
			this._toUpdate.Add(otherUid);
		}

		// Token: 0x04000861 RID: 2145
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x04000862 RID: 2146
		[Dependency]
		private readonly MovementSpeedModifierSystem _speedModifierSystem;

		// Token: 0x04000863 RID: 2147
		private HashSet<EntityUid> _toUpdate = new HashSet<EntityUid>();

		// Token: 0x04000864 RID: 2148
		private HashSet<EntityUid> _toRemove = new HashSet<EntityUid>();
	}
}
