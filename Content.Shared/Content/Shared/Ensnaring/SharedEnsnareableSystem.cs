using System;
using System.Runtime.CompilerServices;
using Content.Shared.Ensnaring.Components;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;

namespace Content.Shared.Ensnaring
{
	// Token: 0x020004BA RID: 1210
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedEnsnareableSystem : EntitySystem
	{
		// Token: 0x06000EA3 RID: 3747 RVA: 0x0002F294 File Offset: 0x0002D494
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<EnsnareableComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<EnsnareableComponent, RefreshMovementSpeedModifiersEvent>(this.MovementSpeedModify), null, null);
			base.SubscribeLocalEvent<EnsnareableComponent, EnsnareEvent>(new ComponentEventHandler<EnsnareableComponent, EnsnareEvent>(this.OnEnsnare), null, null);
			base.SubscribeLocalEvent<EnsnareableComponent, EnsnareRemoveEvent>(new ComponentEventHandler<EnsnareableComponent, EnsnareRemoveEvent>(this.OnEnsnareRemove), null, null);
			base.SubscribeLocalEvent<EnsnareableComponent, EnsnaredChangedEvent>(new ComponentEventHandler<EnsnareableComponent, EnsnaredChangedEvent>(this.OnEnsnareChange), null, null);
			base.SubscribeLocalEvent<EnsnareableComponent, ComponentGetState>(new ComponentEventRefHandler<EnsnareableComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<EnsnareableComponent, ComponentHandleState>(new ComponentEventRefHandler<EnsnareableComponent, ComponentHandleState>(this.OnHandleState), null, null);
		}

		// Token: 0x06000EA4 RID: 3748 RVA: 0x0002F320 File Offset: 0x0002D520
		private void OnHandleState(EntityUid uid, EnsnareableComponent component, ref ComponentHandleState args)
		{
			EnsnareableComponentState state = args.Current as EnsnareableComponentState;
			if (state == null)
			{
				return;
			}
			if (state.IsEnsnared == component.IsEnsnared)
			{
				return;
			}
			component.IsEnsnared = state.IsEnsnared;
			base.RaiseLocalEvent<EnsnaredChangedEvent>(uid, new EnsnaredChangedEvent(component.IsEnsnared), false);
		}

		// Token: 0x06000EA5 RID: 3749 RVA: 0x0002F36B File Offset: 0x0002D56B
		private void OnGetState(EntityUid uid, EnsnareableComponent component, ref ComponentGetState args)
		{
			args.State = new EnsnareableComponentState(component.IsEnsnared);
		}

		// Token: 0x06000EA6 RID: 3750 RVA: 0x0002F380 File Offset: 0x0002D580
		private void OnEnsnare(EntityUid uid, EnsnareableComponent component, EnsnareEvent args)
		{
			component.WalkSpeed = args.WalkSpeed;
			component.SprintSpeed = args.SprintSpeed;
			this._speedModifier.RefreshMovementSpeedModifiers(uid, null);
			EnsnaredChangedEvent ev = new EnsnaredChangedEvent(component.IsEnsnared);
			base.RaiseLocalEvent<EnsnaredChangedEvent>(uid, ev, false);
		}

		// Token: 0x06000EA7 RID: 3751 RVA: 0x0002F3C8 File Offset: 0x0002D5C8
		private void OnEnsnareRemove(EntityUid uid, EnsnareableComponent component, EnsnareRemoveEvent args)
		{
			this._speedModifier.RefreshMovementSpeedModifiers(uid, null);
			EnsnaredChangedEvent ev = new EnsnaredChangedEvent(component.IsEnsnared);
			base.RaiseLocalEvent<EnsnaredChangedEvent>(uid, ev, false);
		}

		// Token: 0x06000EA8 RID: 3752 RVA: 0x0002F3F7 File Offset: 0x0002D5F7
		private void OnEnsnareChange(EntityUid uid, EnsnareableComponent component, EnsnaredChangedEvent args)
		{
			this.UpdateAppearance(uid, component, null);
		}

		// Token: 0x06000EA9 RID: 3753 RVA: 0x0002F402 File Offset: 0x0002D602
		private void UpdateAppearance(EntityUid uid, EnsnareableComponent component, [Nullable(2)] AppearanceComponent appearance = null)
		{
			this.Appearance.SetData(uid, EnsnareableVisuals.IsEnsnared, component.IsEnsnared, appearance);
		}

		// Token: 0x06000EAA RID: 3754 RVA: 0x0002F422 File Offset: 0x0002D622
		private void MovementSpeedModify(EntityUid uid, EnsnareableComponent component, RefreshMovementSpeedModifiersEvent args)
		{
			if (!component.IsEnsnared)
			{
				return;
			}
			args.ModifySpeed(component.WalkSpeed, component.SprintSpeed);
		}

		// Token: 0x04000DC6 RID: 3526
		[Dependency]
		private readonly MovementSpeedModifierSystem _speedModifier;

		// Token: 0x04000DC7 RID: 3527
		[Dependency]
		protected readonly SharedAppearanceSystem Appearance;
	}
}
