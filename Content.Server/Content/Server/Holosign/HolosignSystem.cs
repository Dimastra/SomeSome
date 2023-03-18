using System;
using System.Runtime.CompilerServices;
using Content.Server.Coordinates.Helpers;
using Content.Server.Power.Components;
using Content.Server.PowerCell;
using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Holosign
{
	// Token: 0x0200045F RID: 1119
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HolosignSystem : EntitySystem
	{
		// Token: 0x06001690 RID: 5776 RVA: 0x000772D9 File Offset: 0x000754D9
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<HolosignProjectorComponent, UseInHandEvent>(new ComponentEventHandler<HolosignProjectorComponent, UseInHandEvent>(this.OnUse), null, null);
			base.SubscribeLocalEvent<HolosignProjectorComponent, ExaminedEvent>(new ComponentEventHandler<HolosignProjectorComponent, ExaminedEvent>(this.OnExamine), null, null);
		}

		// Token: 0x06001691 RID: 5777 RVA: 0x0007730C File Offset: 0x0007550C
		private void OnExamine(EntityUid uid, HolosignProjectorComponent component, ExaminedEvent args)
		{
			BatteryComponent battery;
			this._cellSystem.TryGetBatteryFromSlot(uid, out battery, null);
			int charges = this.UsesRemaining(component, battery);
			int maxCharges = this.MaxUses(component, battery);
			args.PushMarkup(Loc.GetString("emag-charges-remaining", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("charges", charges)
			}));
			if (charges > 0 && charges == maxCharges)
			{
				args.PushMarkup(Loc.GetString("emag-max-charges"));
			}
		}

		// Token: 0x06001692 RID: 5778 RVA: 0x00077380 File Offset: 0x00075580
		private void OnUse(EntityUid uid, HolosignProjectorComponent component, UseInHandEvent args)
		{
			BatteryComponent battery;
			if (args.Handled || !this._cellSystem.TryGetBatteryFromSlot(uid, out battery, null) || !battery.TryUseCharge(component.ChargeUse))
			{
				return;
			}
			EntityUid holo = this.EntityManager.SpawnEntity(component.SignProto, base.Transform(args.User).Coordinates.SnapToGrid(this.EntityManager, null));
			base.Transform(holo).Anchored = true;
			args.Handled = true;
		}

		// Token: 0x06001693 RID: 5779 RVA: 0x000773F8 File Offset: 0x000755F8
		private int UsesRemaining(HolosignProjectorComponent component, [Nullable(2)] BatteryComponent battery = null)
		{
			if (battery == null || component.ChargeUse == 0f)
			{
				return 0;
			}
			return (int)(battery.CurrentCharge / component.ChargeUse);
		}

		// Token: 0x06001694 RID: 5780 RVA: 0x0007741A File Offset: 0x0007561A
		private int MaxUses(HolosignProjectorComponent component, [Nullable(2)] BatteryComponent battery = null)
		{
			if (battery == null || component.ChargeUse == 0f)
			{
				return 0;
			}
			return (int)(battery.MaxCharge / component.ChargeUse);
		}

		// Token: 0x04000E19 RID: 3609
		[Dependency]
		private readonly PowerCellSystem _cellSystem;
	}
}
