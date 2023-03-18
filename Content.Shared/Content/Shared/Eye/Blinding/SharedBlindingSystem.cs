using System;
using System.Runtime.CompilerServices;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Rejuvenate;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Eye.Blinding
{
	// Token: 0x02000499 RID: 1177
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SharedBlindingSystem : EntitySystem
	{
		// Token: 0x06000E48 RID: 3656 RVA: 0x0002DD1C File Offset: 0x0002BF1C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<BlindableComponent, ComponentGetState>(new ComponentEventRefHandler<BlindableComponent, ComponentGetState>(this.OnGetBlindableState), null, null);
			base.SubscribeLocalEvent<BlindableComponent, ComponentHandleState>(new ComponentEventRefHandler<BlindableComponent, ComponentHandleState>(this.OnHandleBlindableState), null, null);
			base.SubscribeLocalEvent<BlindfoldComponent, GotEquippedEvent>(new ComponentEventHandler<BlindfoldComponent, GotEquippedEvent>(this.OnEquipped), null, null);
			base.SubscribeLocalEvent<BlindfoldComponent, GotUnequippedEvent>(new ComponentEventHandler<BlindfoldComponent, GotUnequippedEvent>(this.OnUnequipped), null, null);
			base.SubscribeLocalEvent<VisionCorrectionComponent, GotEquippedEvent>(new ComponentEventHandler<VisionCorrectionComponent, GotEquippedEvent>(this.OnGlassesEquipped), null, null);
			base.SubscribeLocalEvent<VisionCorrectionComponent, GotUnequippedEvent>(new ComponentEventHandler<VisionCorrectionComponent, GotUnequippedEvent>(this.OnGlassesUnequipped), null, null);
			base.SubscribeLocalEvent<BlurryVisionComponent, ComponentGetState>(new ComponentEventRefHandler<BlurryVisionComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<TemporaryBlindnessComponent, ComponentInit>(new ComponentEventHandler<TemporaryBlindnessComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<TemporaryBlindnessComponent, ComponentShutdown>(new ComponentEventHandler<TemporaryBlindnessComponent, ComponentShutdown>(this.OnShutdown), null, null);
			base.SubscribeLocalEvent<BlindableComponent, RejuvenateEvent>(new ComponentEventHandler<BlindableComponent, RejuvenateEvent>(this.OnRejuvenate), null, null);
		}

		// Token: 0x06000E49 RID: 3657 RVA: 0x0002DDF7 File Offset: 0x0002BFF7
		private void OnGetBlindableState(EntityUid uid, BlindableComponent component, ref ComponentGetState args)
		{
			args.State = new BlindableComponentState(component.Sources);
		}

		// Token: 0x06000E4A RID: 3658 RVA: 0x0002DE0C File Offset: 0x0002C00C
		private void OnHandleBlindableState(EntityUid uid, BlindableComponent component, ref ComponentHandleState args)
		{
			BlindableComponentState cast = args.Current as BlindableComponentState;
			if (cast == null)
			{
				return;
			}
			component.Sources = cast.Sources;
		}

		// Token: 0x06000E4B RID: 3659 RVA: 0x0002DE38 File Offset: 0x0002C038
		private void OnEquipped(EntityUid uid, BlindfoldComponent component, GotEquippedEvent args)
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
			this.AdjustBlindSources(args.Equipee, 1, blindComp);
		}

		// Token: 0x06000E4C RID: 3660 RVA: 0x0002DEA0 File Offset: 0x0002C0A0
		private void OnUnequipped(EntityUid uid, BlindfoldComponent component, GotUnequippedEvent args)
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
			this.AdjustBlindSources(args.Equipee, -1, blindComp);
		}

		// Token: 0x06000E4D RID: 3661 RVA: 0x0002DEDC File Offset: 0x0002C0DC
		private void OnGlassesEquipped(EntityUid uid, VisionCorrectionComponent component, GotEquippedEvent args)
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
			BlurryVisionComponent blur;
			if (!base.TryComp<BlurryVisionComponent>(args.Equipee, ref blur))
			{
				return;
			}
			component.IsActive = true;
			blur.Magnitude += component.VisionBonus;
			base.Dirty(blur, null);
		}

		// Token: 0x06000E4E RID: 3662 RVA: 0x0002DF50 File Offset: 0x0002C150
		private void OnGlassesUnequipped(EntityUid uid, VisionCorrectionComponent component, GotUnequippedEvent args)
		{
			BlurryVisionComponent blur;
			if (!component.IsActive || !base.TryComp<BlurryVisionComponent>(args.Equipee, ref blur))
			{
				return;
			}
			component.IsActive = false;
			blur.Magnitude -= component.VisionBonus;
			base.Dirty(blur, null);
		}

		// Token: 0x06000E4F RID: 3663 RVA: 0x0002DF98 File Offset: 0x0002C198
		private void OnGetState(EntityUid uid, BlurryVisionComponent component, ref ComponentGetState args)
		{
			args.State = new BlurryVisionComponentState(component.Magnitude);
		}

		// Token: 0x06000E50 RID: 3664 RVA: 0x0002DFAB File Offset: 0x0002C1AB
		private void OnInit(EntityUid uid, TemporaryBlindnessComponent component, ComponentInit args)
		{
			this.AdjustBlindSources(uid, 1, null);
		}

		// Token: 0x06000E51 RID: 3665 RVA: 0x0002DFB6 File Offset: 0x0002C1B6
		private void OnShutdown(EntityUid uid, TemporaryBlindnessComponent component, ComponentShutdown args)
		{
			this.AdjustBlindSources(uid, -1, null);
		}

		// Token: 0x06000E52 RID: 3666 RVA: 0x0002DFC1 File Offset: 0x0002C1C1
		private void OnRejuvenate(EntityUid uid, BlindableComponent component, RejuvenateEvent args)
		{
			this.AdjustEyeDamage(uid, -component.EyeDamage, component);
		}

		// Token: 0x06000E53 RID: 3667 RVA: 0x0002DFD4 File Offset: 0x0002C1D4
		[NullableContext(2)]
		public void AdjustBlindSources(EntityUid uid, int amount, BlindableComponent blindable = null)
		{
			if (!base.Resolve<BlindableComponent>(uid, ref blindable, false))
			{
				return;
			}
			int oldSources = blindable.Sources;
			blindable.Sources += amount;
			blindable.Sources = Math.Max(blindable.Sources, 0);
			if (oldSources == 0 && blindable.Sources > 0)
			{
				BlindnessChangedEvent ev = new BlindnessChangedEvent(true);
				base.RaiseLocalEvent<BlindnessChangedEvent>(uid, ev, false);
			}
			else if (blindable.Sources == 0 && oldSources > 0)
			{
				BlindnessChangedEvent ev2 = new BlindnessChangedEvent(false);
				base.RaiseLocalEvent<BlindnessChangedEvent>(uid, ev2, false);
			}
			base.Dirty(blindable, null);
		}

		// Token: 0x06000E54 RID: 3668 RVA: 0x0002E058 File Offset: 0x0002C258
		[NullableContext(2)]
		public void AdjustEyeDamage(EntityUid uid, int amount, BlindableComponent blindable = null)
		{
			if (!base.Resolve<BlindableComponent>(uid, ref blindable, false))
			{
				return;
			}
			blindable.EyeDamage += amount;
			if (blindable.EyeDamage > 0)
			{
				BlurryVisionComponent blurry = base.EnsureComp<BlurryVisionComponent>(uid);
				blurry.Magnitude = (float)(9 - blindable.EyeDamage);
				base.Dirty(blurry, null);
			}
			else
			{
				base.RemComp<BlurryVisionComponent>(uid);
			}
			if (!blindable.EyeTooDamaged && blindable.EyeDamage >= 8)
			{
				blindable.EyeTooDamaged = true;
				this.AdjustBlindSources(uid, 1, blindable);
			}
			if (blindable.EyeTooDamaged && blindable.EyeDamage < 8)
			{
				blindable.EyeTooDamaged = false;
				this.AdjustBlindSources(uid, -1, blindable);
			}
			blindable.EyeDamage = Math.Clamp(blindable.EyeDamage, 0, 8);
		}

		// Token: 0x04000D6D RID: 3437
		public const string BlindingStatusEffect = "TemporaryBlindness";
	}
}
