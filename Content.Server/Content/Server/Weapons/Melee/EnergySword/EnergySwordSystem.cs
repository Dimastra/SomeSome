using System;
using System.Runtime.CompilerServices;
using Content.Server.CombatMode.Disarm;
using Content.Server.Kitchen.Components;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Light;
using Content.Shared.Light.Component;
using Content.Shared.Temperature;
using Content.Shared.Toggleable;
using Content.Shared.Tools.Components;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Server.Weapons.Melee.EnergySword
{
	// Token: 0x020000BB RID: 187
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EnergySwordSystem : EntitySystem
	{
		// Token: 0x06000310 RID: 784 RVA: 0x000108E4 File Offset: 0x0000EAE4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<EnergySwordComponent, MapInitEvent>(new ComponentEventHandler<EnergySwordComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<EnergySwordComponent, MeleeHitEvent>(new ComponentEventHandler<EnergySwordComponent, MeleeHitEvent>(this.OnMeleeHit), null, null);
			base.SubscribeLocalEvent<EnergySwordComponent, UseInHandEvent>(new ComponentEventHandler<EnergySwordComponent, UseInHandEvent>(this.OnUseInHand), null, null);
			base.SubscribeLocalEvent<EnergySwordComponent, InteractUsingEvent>(new ComponentEventHandler<EnergySwordComponent, InteractUsingEvent>(this.OnInteractUsing), null, null);
			base.SubscribeLocalEvent<EnergySwordComponent, IsHotEvent>(new ComponentEventHandler<EnergySwordComponent, IsHotEvent>(this.OnIsHotEvent), null, null);
			base.SubscribeLocalEvent<EnergySwordComponent, EnergySwordDeactivatedEvent>(new ComponentEventRefHandler<EnergySwordComponent, EnergySwordDeactivatedEvent>(this.TurnOff), null, null);
			base.SubscribeLocalEvent<EnergySwordComponent, EnergySwordActivatedEvent>(new ComponentEventRefHandler<EnergySwordComponent, EnergySwordActivatedEvent>(this.TurnOn), null, null);
			base.SubscribeLocalEvent<EnergySwordComponent, DroppedEvent>(new ComponentEventHandler<EnergySwordComponent, DroppedEvent>(this.OnDropped), null, null);
		}

		// Token: 0x06000311 RID: 785 RVA: 0x00010998 File Offset: 0x0000EB98
		private void OnDropped(EntityUid uid, EnergySwordComponent component, DroppedEvent args)
		{
			if (!component.Activated)
			{
				return;
			}
			EnergySwordDeactivatedEvent ev = new EnergySwordDeactivatedEvent();
			base.RaiseLocalEvent<EnergySwordDeactivatedEvent>(uid, ref ev, false);
			this.UpdateAppearance(uid, component);
		}

		// Token: 0x06000312 RID: 786 RVA: 0x000109C7 File Offset: 0x0000EBC7
		private void OnMapInit(EntityUid uid, EnergySwordComponent comp, MapInitEvent args)
		{
			if (comp.ColorOptions.Count != 0)
			{
				comp.BladeColor = RandomExtensions.Pick<Color>(this._random, comp.ColorOptions);
			}
		}

		// Token: 0x06000313 RID: 787 RVA: 0x000109ED File Offset: 0x0000EBED
		private void OnMeleeHit(EntityUid uid, EnergySwordComponent comp, MeleeHitEvent args)
		{
			if (!comp.Activated)
			{
				return;
			}
			args.BonusDamage = comp.LitDamageBonus;
		}

		// Token: 0x06000314 RID: 788 RVA: 0x00010A04 File Offset: 0x0000EC04
		private void OnUseInHand(EntityUid uid, EnergySwordComponent comp, UseInHandEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			args.Handled = true;
			if (comp.Activated)
			{
				EnergySwordDeactivatedEvent ev = new EnergySwordDeactivatedEvent();
				base.RaiseLocalEvent<EnergySwordDeactivatedEvent>(uid, ref ev, false);
			}
			else
			{
				EnergySwordActivatedEvent ev2 = new EnergySwordActivatedEvent();
				base.RaiseLocalEvent<EnergySwordActivatedEvent>(uid, ref ev2, false);
			}
			this.UpdateAppearance(uid, comp);
		}

		// Token: 0x06000315 RID: 789 RVA: 0x00010A58 File Offset: 0x0000EC58
		private void TurnOff(EntityUid uid, EnergySwordComponent comp, ref EnergySwordDeactivatedEvent args)
		{
			ItemComponent item;
			if (base.TryComp<ItemComponent>(uid, ref item))
			{
				this._item.SetSize(uid, 5, item);
			}
			DisarmMalusComponent malus;
			if (base.TryComp<DisarmMalusComponent>(uid, ref malus))
			{
				malus.Malus -= comp.LitDisarmMalus;
			}
			MeleeWeaponComponent weaponComp;
			if (base.TryComp<MeleeWeaponComponent>(uid, ref weaponComp))
			{
				weaponComp.HitSound = comp.OnHitOff;
				if (comp.Secret)
				{
					weaponComp.HideFromExamine = true;
				}
			}
			if (comp.IsSharp)
			{
				base.RemComp<SharpComponent>(uid);
			}
			this._audio.Play(comp.DeActivateSound, Filter.Pvs(uid, 2f, this.EntityManager, null, null), uid, true, new AudioParams?(comp.DeActivateSound.Params));
			comp.Activated = false;
		}

		// Token: 0x06000316 RID: 790 RVA: 0x00010B10 File Offset: 0x0000ED10
		private void TurnOn(EntityUid uid, EnergySwordComponent comp, ref EnergySwordActivatedEvent args)
		{
			ItemComponent item;
			if (base.TryComp<ItemComponent>(uid, ref item))
			{
				this._item.SetSize(uid, 9999, item);
			}
			if (comp.IsSharp)
			{
				base.EnsureComp<SharpComponent>(uid);
			}
			MeleeWeaponComponent weaponComp;
			if (base.TryComp<MeleeWeaponComponent>(uid, ref weaponComp))
			{
				weaponComp.HitSound = comp.OnHitOn;
				if (comp.Secret)
				{
					weaponComp.HideFromExamine = false;
				}
			}
			DisarmMalusComponent malus;
			if (base.TryComp<DisarmMalusComponent>(uid, ref malus))
			{
				malus.Malus += comp.LitDisarmMalus;
			}
			this._audio.Play(comp.ActivateSound, Filter.Pvs(uid, 2f, this.EntityManager, null, null), uid, true, new AudioParams?(comp.ActivateSound.Params));
			comp.Activated = true;
		}

		// Token: 0x06000317 RID: 791 RVA: 0x00010BCC File Offset: 0x0000EDCC
		private void UpdateAppearance(EntityUid uid, EnergySwordComponent component)
		{
			AppearanceComponent appearanceComponent;
			if (!base.TryComp<AppearanceComponent>(uid, ref appearanceComponent))
			{
				return;
			}
			this._appearance.SetData(uid, ToggleableLightVisuals.Enabled, component.Activated, appearanceComponent);
			this._appearance.SetData(uid, ToggleableLightVisuals.Color, component.BladeColor, appearanceComponent);
		}

		// Token: 0x06000318 RID: 792 RVA: 0x00010C24 File Offset: 0x0000EE24
		private void OnInteractUsing(EntityUid uid, EnergySwordComponent comp, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			ToolComponent tool;
			if (!base.TryComp<ToolComponent>(args.Used, ref tool) || !tool.Qualities.ContainsAny(new string[]
			{
				"Pulsing"
			}))
			{
				return;
			}
			args.Handled = true;
			comp.Hacked = !comp.Hacked;
			if (comp.Hacked)
			{
				RgbLightControllerComponent rgb = base.EnsureComp<RgbLightControllerComponent>(uid);
				this._rgbSystem.SetCycleRate(uid, comp.CycleRate, rgb);
				return;
			}
			base.RemComp<RgbLightControllerComponent>(uid);
		}

		// Token: 0x06000319 RID: 793 RVA: 0x00010CA8 File Offset: 0x0000EEA8
		private void OnIsHotEvent(EntityUid uid, EnergySwordComponent energySword, IsHotEvent args)
		{
			args.IsHot = energySword.Activated;
		}

		// Token: 0x04000211 RID: 529
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000212 RID: 530
		[Dependency]
		private readonly SharedRgbLightControllerSystem _rgbSystem;

		// Token: 0x04000213 RID: 531
		[Dependency]
		private readonly SharedItemSystem _item;

		// Token: 0x04000214 RID: 532
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000215 RID: 533
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
