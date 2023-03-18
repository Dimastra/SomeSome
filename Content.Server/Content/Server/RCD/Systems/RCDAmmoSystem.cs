using System;
using System.Runtime.CompilerServices;
using Content.Server.RCD.Components;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Server.RCD.Systems
{
	// Token: 0x0200024B RID: 587
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RCDAmmoSystem : EntitySystem
	{
		// Token: 0x06000BC0 RID: 3008 RVA: 0x0003DDEB File Offset: 0x0003BFEB
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RCDAmmoComponent, ExaminedEvent>(new ComponentEventHandler<RCDAmmoComponent, ExaminedEvent>(this.OnExamine), null, null);
			base.SubscribeLocalEvent<RCDAmmoComponent, AfterInteractEvent>(new ComponentEventHandler<RCDAmmoComponent, AfterInteractEvent>(this.OnAfterInteract), null, null);
		}

		// Token: 0x06000BC1 RID: 3009 RVA: 0x0003DE1C File Offset: 0x0003C01C
		private void OnExamine(EntityUid uid, RCDAmmoComponent component, ExaminedEvent args)
		{
			string examineMessage = Loc.GetString("rcd-ammo-component-on-examine-text", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("ammo", component.RefillAmmo)
			});
			args.PushText(examineMessage);
		}

		// Token: 0x06000BC2 RID: 3010 RVA: 0x0003DE60 File Offset: 0x0003C060
		private void OnAfterInteract(EntityUid uid, RCDAmmoComponent component, AfterInteractEvent args)
		{
			if (args.Handled || !args.CanReach)
			{
				return;
			}
			EntityUid? target2 = args.Target;
			if (target2 != null)
			{
				EntityUid target = target2.GetValueOrDefault();
				RCDComponent rcdComponent;
				if (target.Valid && this.EntityManager.TryGetComponent<RCDComponent>(target, ref rcdComponent))
				{
					if (rcdComponent.MaxAmmo - rcdComponent.CurrentAmmo < component.RefillAmmo)
					{
						rcdComponent.Owner.PopupMessage(args.User, Loc.GetString("rcd-ammo-component-after-interact-full-text"));
						args.Handled = true;
						return;
					}
					rcdComponent.CurrentAmmo = Math.Min(rcdComponent.MaxAmmo, rcdComponent.CurrentAmmo + component.RefillAmmo);
					rcdComponent.Owner.PopupMessage(args.User, Loc.GetString("rcd-ammo-component-after-interact-refilled-text"));
					this.EntityManager.QueueDeleteEntity(uid);
					args.Handled = true;
					return;
				}
			}
		}
	}
}
