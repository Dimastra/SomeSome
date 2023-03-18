using System;
using System.Runtime.CompilerServices;
using Content.Server.Damage.Systems;
using Content.Server.Explosion.EntitySystems;
using Content.Server.Popups;
using Content.Shared.Interaction.Events;
using Content.Shared.Mousetrap;
using Content.Shared.Popups;
using Content.Shared.StepTrigger.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Physics.Components;

namespace Content.Server.Mousetrap
{
	// Token: 0x02000395 RID: 917
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MousetrapSystem : EntitySystem
	{
		// Token: 0x060012C3 RID: 4803 RVA: 0x000611D0 File Offset: 0x0005F3D0
		public override void Initialize()
		{
			base.SubscribeLocalEvent<MousetrapComponent, UseInHandEvent>(new ComponentEventHandler<MousetrapComponent, UseInHandEvent>(this.OnUseInHand), null, null);
			base.SubscribeLocalEvent<MousetrapComponent, BeforeDamageUserOnTriggerEvent>(new ComponentEventHandler<MousetrapComponent, BeforeDamageUserOnTriggerEvent>(this.BeforeDamageOnTrigger), null, null);
			base.SubscribeLocalEvent<MousetrapComponent, StepTriggerAttemptEvent>(new ComponentEventRefHandler<MousetrapComponent, StepTriggerAttemptEvent>(this.OnStepTriggerAttempt), null, null);
			base.SubscribeLocalEvent<MousetrapComponent, TriggerEvent>(new ComponentEventHandler<MousetrapComponent, TriggerEvent>(this.OnTrigger), null, null);
		}

		// Token: 0x060012C4 RID: 4804 RVA: 0x00061230 File Offset: 0x0005F430
		private void OnUseInHand(EntityUid uid, MousetrapComponent component, UseInHandEvent args)
		{
			component.IsActive = !component.IsActive;
			this._popupSystem.PopupEntity(component.IsActive ? Loc.GetString("mousetrap-on-activate") : Loc.GetString("mousetrap-on-deactivate"), uid, args.User, PopupType.Small);
			this.UpdateVisuals(uid, null, null);
		}

		// Token: 0x060012C5 RID: 4805 RVA: 0x00061286 File Offset: 0x0005F486
		private void OnStepTriggerAttempt(EntityUid uid, MousetrapComponent component, ref StepTriggerAttemptEvent args)
		{
			args.Continue |= component.IsActive;
		}

		// Token: 0x060012C6 RID: 4806 RVA: 0x00061298 File Offset: 0x0005F498
		private void BeforeDamageOnTrigger(EntityUid uid, MousetrapComponent component, BeforeDamageUserOnTriggerEvent args)
		{
			PhysicsComponent physics;
			if (base.TryComp<PhysicsComponent>(args.Tripper, ref physics) && physics.Mass != 0f)
			{
				double scaledDamage = -50.0 * Math.Atan((double)(physics.Mass - (float)component.MassBalance)) + 78.53981633974483;
				args.Damage *= scaledDamage;
			}
		}

		// Token: 0x060012C7 RID: 4807 RVA: 0x00061302 File Offset: 0x0005F502
		private void OnTrigger(EntityUid uid, MousetrapComponent component, TriggerEvent args)
		{
			component.IsActive = false;
			this.UpdateVisuals(uid, null, null);
		}

		// Token: 0x060012C8 RID: 4808 RVA: 0x00061314 File Offset: 0x0005F514
		[NullableContext(2)]
		private void UpdateVisuals(EntityUid uid, MousetrapComponent mousetrap = null, AppearanceComponent appearance = null)
		{
			if (!base.Resolve<MousetrapComponent, AppearanceComponent>(uid, ref mousetrap, ref appearance, false))
			{
				return;
			}
			this._appearance.SetData(uid, MousetrapVisuals.Visual, mousetrap.IsActive ? MousetrapVisuals.Armed : MousetrapVisuals.Unarmed, appearance);
		}

		// Token: 0x04000B7B RID: 2939
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04000B7C RID: 2940
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
