using System;
using System.Runtime.CompilerServices;
using Content.Server.Tools;
using Content.Shared.Clothing.Components;
using Content.Shared.Eye.Blinding;
using Content.Shared.Eye.Blinding.EyeProtection;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.StatusEffect;
using Content.Shared.Tools.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Eye.Blinding.EyeProtection
{
	// Token: 0x02000505 RID: 1285
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EyeProtectionSystem : EntitySystem
	{
		// Token: 0x06001A83 RID: 6787 RVA: 0x0008BBDC File Offset: 0x00089DDC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RequiresEyeProtectionComponent, ToolUseAttemptEvent>(new ComponentEventHandler<RequiresEyeProtectionComponent, ToolUseAttemptEvent>(this.OnUseAttempt), null, null);
			base.SubscribeLocalEvent<RequiresEyeProtectionComponent, WelderToggledEvent>(new ComponentEventHandler<RequiresEyeProtectionComponent, WelderToggledEvent>(this.OnWelderToggled), null, null);
			base.SubscribeLocalEvent<EyeProtectionComponent, GotEquippedEvent>(new ComponentEventHandler<EyeProtectionComponent, GotEquippedEvent>(this.OnEquipped), null, null);
			base.SubscribeLocalEvent<EyeProtectionComponent, GotUnequippedEvent>(new ComponentEventHandler<EyeProtectionComponent, GotUnequippedEvent>(this.OnUnequipped), null, null);
		}

		// Token: 0x06001A84 RID: 6788 RVA: 0x0008BC40 File Offset: 0x00089E40
		private void OnUseAttempt(EntityUid uid, RequiresEyeProtectionComponent component, ToolUseAttemptEvent args)
		{
			if (!component.Toggled)
			{
				return;
			}
			BlindableComponent blindable;
			if (!base.HasComp<StatusEffectsComponent>(args.User) || !base.TryComp<BlindableComponent>(args.User, ref blindable))
			{
				return;
			}
			if (blindable.Sources > 0)
			{
				return;
			}
			float statusTime = (float)component.StatusEffectTime.TotalSeconds - blindable.BlindResistance;
			if (statusTime <= 0f)
			{
				return;
			}
			TimeSpan statusTimeSpan = TimeSpan.FromSeconds((double)(statusTime * (float)(blindable.EyeDamage + 1)));
			if (this._statusEffectsSystem.TryAddStatusEffect(args.User, "TemporaryBlindness", statusTimeSpan, false, "TemporaryBlindness", null) && blindable.BlindResistance <= 0f)
			{
				this._blindingSystem.AdjustEyeDamage(args.User, 1, blindable);
			}
		}

		// Token: 0x06001A85 RID: 6789 RVA: 0x0008BCEE File Offset: 0x00089EEE
		private void OnWelderToggled(EntityUid uid, RequiresEyeProtectionComponent component, WelderToggledEvent args)
		{
			component.Toggled = args.WelderOn;
		}

		// Token: 0x06001A86 RID: 6790 RVA: 0x0008BCFC File Offset: 0x00089EFC
		private void OnEquipped(EntityUid uid, EyeProtectionComponent component, GotEquippedEvent args)
		{
			ClothingComponent clothing;
			if (!base.TryComp<ClothingComponent>(uid, ref clothing) || clothing.Slots == SlotFlags.PREVENTEQUIP)
			{
				return;
			}
			if (!clothing.Slots.HasFlag(args.SlotFlags))
			{
				return;
			}
			component.IsActive = true;
			BlindableComponent blindComp;
			if (!base.TryComp<BlindableComponent>(args.Equipee, ref blindComp))
			{
				return;
			}
			blindComp.BlindResistance += (float)component.ProtectionTime.TotalSeconds;
		}

		// Token: 0x06001A87 RID: 6791 RVA: 0x0008BD70 File Offset: 0x00089F70
		private void OnUnequipped(EntityUid uid, EyeProtectionComponent component, GotUnequippedEvent args)
		{
			if (!component.IsActive)
			{
				return;
			}
			component.IsActive = false;
			BlindableComponent blindComp;
			if (!base.TryComp<BlindableComponent>(args.Equipee, ref blindComp))
			{
				return;
			}
			blindComp.BlindResistance -= (float)component.ProtectionTime.TotalSeconds;
		}

		// Token: 0x040010E3 RID: 4323
		[Dependency]
		private readonly StatusEffectsSystem _statusEffectsSystem;

		// Token: 0x040010E4 RID: 4324
		[Dependency]
		private readonly SharedBlindingSystem _blindingSystem;
	}
}
