using System;
using System.Runtime.CompilerServices;
using Content.Shared.Movement.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Movement.Systems
{
	// Token: 0x020002D8 RID: 728
	public sealed class MovementSpeedModifierSystem : EntitySystem
	{
		// Token: 0x060007F4 RID: 2036 RVA: 0x0001A602 File Offset: 0x00018802
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MovementSpeedModifierComponent, ComponentGetState>(new ComponentEventRefHandler<MovementSpeedModifierComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<MovementSpeedModifierComponent, ComponentHandleState>(new ComponentEventRefHandler<MovementSpeedModifierComponent, ComponentHandleState>(this.OnHandleState), null, null);
		}

		// Token: 0x060007F5 RID: 2037 RVA: 0x0001A632 File Offset: 0x00018832
		[NullableContext(1)]
		private void OnGetState(EntityUid uid, MovementSpeedModifierComponent component, ref ComponentGetState args)
		{
			args.State = new MovementSpeedModifierSystem.MovementSpeedModifierComponentState
			{
				BaseWalkSpeed = component.BaseWalkSpeed,
				BaseSprintSpeed = component.BaseSprintSpeed,
				WalkSpeedModifier = component.WalkSpeedModifier,
				SprintSpeedModifier = component.SprintSpeedModifier
			};
		}

		// Token: 0x060007F6 RID: 2038 RVA: 0x0001A670 File Offset: 0x00018870
		[NullableContext(1)]
		private void OnHandleState(EntityUid uid, MovementSpeedModifierComponent component, ref ComponentHandleState args)
		{
			MovementSpeedModifierSystem.MovementSpeedModifierComponentState state = args.Current as MovementSpeedModifierSystem.MovementSpeedModifierComponentState;
			if (state == null)
			{
				return;
			}
			component.BaseWalkSpeed = state.BaseWalkSpeed;
			component.BaseSprintSpeed = state.BaseSprintSpeed;
			component.WalkSpeedModifier = state.WalkSpeedModifier;
			component.SprintSpeedModifier = state.SprintSpeedModifier;
		}

		// Token: 0x060007F7 RID: 2039 RVA: 0x0001A6C0 File Offset: 0x000188C0
		[NullableContext(2)]
		public void RefreshMovementSpeedModifiers(EntityUid uid, MovementSpeedModifierComponent move = null)
		{
			if (!base.Resolve<MovementSpeedModifierComponent>(uid, ref move, false))
			{
				return;
			}
			RefreshMovementSpeedModifiersEvent ev = new RefreshMovementSpeedModifiersEvent();
			base.RaiseLocalEvent<RefreshMovementSpeedModifiersEvent>(uid, ev, false);
			if (MathHelper.CloseTo(ev.WalkSpeedModifier, move.WalkSpeedModifier, 1E-07f) && MathHelper.CloseTo(ev.SprintSpeedModifier, move.SprintSpeedModifier, 1E-07f))
			{
				return;
			}
			move.WalkSpeedModifier = ev.WalkSpeedModifier;
			move.SprintSpeedModifier = ev.SprintSpeedModifier;
			base.Dirty(move, null);
		}

		// Token: 0x060007F8 RID: 2040 RVA: 0x0001A73A File Offset: 0x0001893A
		[NullableContext(2)]
		public void ChangeBaseSpeed(EntityUid uid, float baseWalkSpeed, float baseSprintSpeed, float acceleration, MovementSpeedModifierComponent move = null)
		{
			if (!base.Resolve<MovementSpeedModifierComponent>(uid, ref move, false))
			{
				return;
			}
			move.BaseWalkSpeed = baseWalkSpeed;
			move.BaseSprintSpeed = baseSprintSpeed;
			move.Acceleration = acceleration;
			base.Dirty(move, null);
		}

		// Token: 0x020007CA RID: 1994
		[NetSerializable]
		[Serializable]
		private sealed class MovementSpeedModifierComponentState : ComponentState
		{
			// Token: 0x04001813 RID: 6163
			public float BaseWalkSpeed;

			// Token: 0x04001814 RID: 6164
			public float BaseSprintSpeed;

			// Token: 0x04001815 RID: 6165
			public float WalkSpeedModifier;

			// Token: 0x04001816 RID: 6166
			public float SprintSpeedModifier;
		}
	}
}
