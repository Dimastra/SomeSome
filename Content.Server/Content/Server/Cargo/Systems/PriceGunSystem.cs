using System;
using System.Runtime.CompilerServices;
using Content.Server.Cargo.Components;
using Content.Server.Popups;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Cargo.Systems
{
	// Token: 0x020006E1 RID: 1761
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PriceGunSystem : EntitySystem
	{
		// Token: 0x060024FE RID: 9470 RVA: 0x000C15B6 File Offset: 0x000BF7B6
		public override void Initialize()
		{
			base.SubscribeLocalEvent<PriceGunComponent, AfterInteractEvent>(new ComponentEventHandler<PriceGunComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
			base.SubscribeLocalEvent<PriceGunComponent, GetVerbsEvent<UtilityVerb>>(new ComponentEventHandler<PriceGunComponent, GetVerbsEvent<UtilityVerb>>(this.OnUtilityVerb), null, null);
		}

		// Token: 0x060024FF RID: 9471 RVA: 0x000C15E0 File Offset: 0x000BF7E0
		private void OnUtilityVerb(EntityUid uid, PriceGunComponent component, GetVerbsEvent<UtilityVerb> args)
		{
			if (!args.CanAccess || !args.CanInteract)
			{
				return;
			}
			EntityUid target = args.Target;
			UseDelayComponent useDelay;
			if (base.TryComp<UseDelayComponent>(args.Using, ref useDelay) && useDelay.ActiveDelay)
			{
				return;
			}
			double price = this._pricingSystem.GetPrice(args.Target);
			UtilityVerb verb = new UtilityVerb
			{
				Act = delegate()
				{
					SharedPopupSystem popupSystem = this._popupSystem;
					string text = "price-gun-pricing-result";
					ValueTuple<string, object>[] array = new ValueTuple<string, object>[2];
					array[0] = new ValueTuple<string, object>("object", Identity.Entity(args.Target, this.EntityManager));
					int num = 1;
					string item = "price";
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
					defaultInterpolatedStringHandler.AppendFormatted<double>(price, "F2");
					array[num] = new ValueTuple<string, object>(item, defaultInterpolatedStringHandler.ToStringAndClear());
					popupSystem.PopupEntity(Loc.GetString(text, array), args.User, args.User, PopupType.Small);
					this._useDelay.BeginDelay(uid, useDelay);
				},
				Text = Loc.GetString("price-gun-verb-text"),
				Message = Loc.GetString("price-gun-verb-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("object", Identity.Entity(args.Target, this.EntityManager))
				})
			};
			args.Verbs.Add(verb);
		}

		// Token: 0x06002500 RID: 9472 RVA: 0x000C16F4 File Offset: 0x000BF8F4
		private void OnAfterInteract(EntityUid uid, PriceGunComponent component, AfterInteractEvent args)
		{
			if (!args.CanReach || args.Target == null)
			{
				return;
			}
			UseDelayComponent useDelay;
			if (base.TryComp<UseDelayComponent>(args.Used, ref useDelay) && useDelay.ActiveDelay)
			{
				return;
			}
			double price = this._pricingSystem.GetPrice(args.Target.Value);
			SharedPopupSystem popupSystem = this._popupSystem;
			string text = "price-gun-pricing-result";
			ValueTuple<string, object>[] array = new ValueTuple<string, object>[2];
			array[0] = new ValueTuple<string, object>("object", Identity.Entity(args.Target.Value, this.EntityManager));
			int num = 1;
			string item = "price";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<double>(price, "F2");
			array[num] = new ValueTuple<string, object>(item, defaultInterpolatedStringHandler.ToStringAndClear());
			popupSystem.PopupEntity(Loc.GetString(text, array), args.User, args.User, PopupType.Small);
			this._useDelay.BeginDelay(uid, useDelay);
		}

		// Token: 0x040016AE RID: 5806
		[Dependency]
		private readonly UseDelaySystem _useDelay;

		// Token: 0x040016AF RID: 5807
		[Dependency]
		private readonly PricingSystem _pricingSystem;

		// Token: 0x040016B0 RID: 5808
		[Dependency]
		private readonly PopupSystem _popupSystem;
	}
}
