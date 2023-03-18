using System;
using System.Runtime.CompilerServices;
using Content.Server.Power.Components;
using Content.Server.Power.Events;
using Content.Server.Stunnable.Components;
using Content.Shared.Audio;
using Content.Shared.Damage.Events;
using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Popups;
using Content.Shared.Toggleable;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;

namespace Content.Server.Stunnable.Systems
{
	// Token: 0x0200014D RID: 333
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StunbatonSystem : EntitySystem
	{
		// Token: 0x0600064C RID: 1612 RVA: 0x0001E530 File Offset: 0x0001C730
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<StunbatonComponent, UseInHandEvent>(new ComponentEventHandler<StunbatonComponent, UseInHandEvent>(this.OnUseInHand), null, null);
			base.SubscribeLocalEvent<StunbatonComponent, ExaminedEvent>(new ComponentEventHandler<StunbatonComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<StunbatonComponent, StaminaDamageOnHitAttemptEvent>(new ComponentEventRefHandler<StunbatonComponent, StaminaDamageOnHitAttemptEvent>(this.OnStaminaHitAttempt), null, null);
			base.SubscribeLocalEvent<StunbatonComponent, MeleeHitEvent>(new ComponentEventHandler<StunbatonComponent, MeleeHitEvent>(this.OnMeleeHit), null, null);
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x0001E593 File Offset: 0x0001C793
		private void OnMeleeHit(EntityUid uid, StunbatonComponent component, MeleeHitEvent args)
		{
			if (!component.Activated)
			{
				return;
			}
			args.BonusDamage -= args.BaseDamage;
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x0001E5B8 File Offset: 0x0001C7B8
		private void OnStaminaHitAttempt(EntityUid uid, StunbatonComponent component, ref StaminaDamageOnHitAttemptEvent args)
		{
			BatteryComponent battery;
			if (!component.Activated || !base.TryComp<BatteryComponent>(uid, ref battery) || !battery.TryUseCharge(component.EnergyPerUse))
			{
				args.Cancelled = true;
				return;
			}
			args.HitSoundOverride = component.StunSound;
			if (battery.CurrentCharge < component.EnergyPerUse)
			{
				SoundSystem.Play(component.SparksSound.GetSound(null, null), Filter.Pvs(component.Owner, 2f, this.EntityManager, null, null), uid, new AudioParams?(AudioHelpers.WithVariation(0.25f)));
				this.TurnOff(uid, component);
			}
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x0001E64B File Offset: 0x0001C84B
		private void OnUseInHand(EntityUid uid, StunbatonComponent comp, UseInHandEvent args)
		{
			if (comp.Activated)
			{
				this.TurnOff(uid, comp);
				return;
			}
			this.TurnOn(uid, comp, args.User);
		}

		// Token: 0x06000650 RID: 1616 RVA: 0x0001E66C File Offset: 0x0001C86C
		private void OnExamined(EntityUid uid, StunbatonComponent comp, ExaminedEvent args)
		{
			string msg = comp.Activated ? Loc.GetString("comp-stunbaton-examined-on") : Loc.GetString("comp-stunbaton-examined-off");
			args.PushMarkup(msg);
			BatteryComponent battery;
			if (base.TryComp<BatteryComponent>(uid, ref battery))
			{
				args.PushMarkup(Loc.GetString("stunbaton-component-on-examine-charge", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("charge", (int)(battery.CurrentCharge / battery.MaxCharge * 100f))
				}));
			}
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x0001E6EC File Offset: 0x0001C8EC
		private void TurnOff(EntityUid uid, StunbatonComponent comp)
		{
			if (!comp.Activated)
			{
				return;
			}
			AppearanceComponent appearance;
			ItemComponent item;
			if (base.TryComp<AppearanceComponent>(comp.Owner, ref appearance) && base.TryComp<ItemComponent>(comp.Owner, ref item))
			{
				this._item.SetHeldPrefix(comp.Owner, "off", item);
				this._appearance.SetData(uid, ToggleVisuals.Toggled, false, appearance);
			}
			SoundSystem.Play(comp.SparksSound.GetSound(null, null), Filter.Pvs(comp.Owner, 2f, null, null, null), comp.Owner, new AudioParams?(AudioHelpers.WithVariation(0.25f)));
			comp.Activated = false;
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x0001E794 File Offset: 0x0001C994
		private void TurnOn(EntityUid uid, StunbatonComponent comp, EntityUid user)
		{
			if (comp.Activated)
			{
				return;
			}
			Filter playerFilter = Filter.Pvs(comp.Owner, 2f, this.EntityManager, null, null);
			BatteryComponent battery;
			if (!base.TryComp<BatteryComponent>(comp.Owner, ref battery) || battery.CurrentCharge < comp.EnergyPerUse)
			{
				SoundSystem.Play(comp.TurnOnFailSound.GetSound(null, null), playerFilter, comp.Owner, new AudioParams?(AudioHelpers.WithVariation(0.25f)));
				user.PopupMessage(Loc.GetString("stunbaton-component-low-charge"));
				return;
			}
			AppearanceComponent appearance;
			ItemComponent item;
			if (this.EntityManager.TryGetComponent<AppearanceComponent>(comp.Owner, ref appearance) && this.EntityManager.TryGetComponent<ItemComponent>(comp.Owner, ref item))
			{
				this._item.SetHeldPrefix(comp.Owner, "on", item);
				this._appearance.SetData(uid, ToggleVisuals.Toggled, true, appearance);
			}
			SoundSystem.Play(comp.SparksSound.GetSound(null, null), playerFilter, comp.Owner, new AudioParams?(AudioHelpers.WithVariation(0.25f)));
			comp.Activated = true;
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
		private void SendPowerPulse(EntityUid target, EntityUid? user, EntityUid used)
		{
			base.RaiseLocalEvent<PowerPulseEvent>(target, new PowerPulseEvent
			{
				Used = new EntityUid?(used),
				User = user
			}, false);
		}

		// Token: 0x040003AA RID: 938
		[Dependency]
		private readonly SharedItemSystem _item;

		// Token: 0x040003AB RID: 939
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
