using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Components;
using Content.Shared.Alert;
using Content.Shared.Clothing;
using Content.Shared.Inventory.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;

namespace Content.Server.Clothing
{
	// Token: 0x02000637 RID: 1591
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MagbootsSystem : SharedMagbootsSystem
	{
		// Token: 0x060021D6 RID: 8662 RVA: 0x000B0780 File Offset: 0x000AE980
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MagbootsComponent, GotEquippedEvent>(new ComponentEventHandler<MagbootsComponent, GotEquippedEvent>(this.OnGotEquipped), null, null);
			base.SubscribeLocalEvent<MagbootsComponent, GotUnequippedEvent>(new ComponentEventHandler<MagbootsComponent, GotUnequippedEvent>(this.OnGotUnequipped), null, null);
			base.SubscribeLocalEvent<MagbootsComponent, ComponentGetState>(new ComponentEventRefHandler<MagbootsComponent, ComponentGetState>(this.OnGetState), null, null);
		}

		// Token: 0x060021D7 RID: 8663 RVA: 0x000B07D0 File Offset: 0x000AE9D0
		[NullableContext(2)]
		protected override void UpdateMagbootEffects(EntityUid parent, EntityUid uid, bool state, MagbootsComponent component)
		{
			if (!base.Resolve<MagbootsComponent>(uid, ref component, true))
			{
				return;
			}
			state = (state && component.On);
			MovedByPressureComponent movedByPressure;
			if (base.TryComp<MovedByPressureComponent>(parent, ref movedByPressure))
			{
				movedByPressure.Enabled = !state;
			}
			if (state)
			{
				this._alertsSystem.ShowAlert(parent, AlertType.Magboots, null, null);
				return;
			}
			this._alertsSystem.ClearAlert(parent, AlertType.Magboots);
		}

		// Token: 0x060021D8 RID: 8664 RVA: 0x000B0840 File Offset: 0x000AEA40
		private void OnGotUnequipped(EntityUid uid, MagbootsComponent component, GotUnequippedEvent args)
		{
			if (args.Slot == "shoes")
			{
				this.UpdateMagbootEffects(args.Equipee, uid, false, component);
			}
		}

		// Token: 0x060021D9 RID: 8665 RVA: 0x000B0863 File Offset: 0x000AEA63
		private void OnGotEquipped(EntityUid uid, MagbootsComponent component, GotEquippedEvent args)
		{
			if (args.Slot == "shoes")
			{
				this.UpdateMagbootEffects(args.Equipee, uid, true, component);
			}
		}

		// Token: 0x060021DA RID: 8666 RVA: 0x000B0886 File Offset: 0x000AEA86
		private void OnGetState(EntityUid uid, MagbootsComponent component, ref ComponentGetState args)
		{
			args.State = new MagbootsComponent.MagbootsComponentState(component.On);
		}

		// Token: 0x040014BE RID: 5310
		[Dependency]
		private readonly AlertsSystem _alertsSystem;
	}
}
