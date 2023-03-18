using System;
using System.Runtime.CompilerServices;
using Content.Server.Cargo.Systems;
using Content.Server.Power.Components;
using Content.Shared.Examine;
using Content.Shared.Rejuvenate;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Server.Power.EntitySystems
{
	// Token: 0x0200028A RID: 650
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BatterySystem : EntitySystem
	{
		// Token: 0x06000D09 RID: 3337 RVA: 0x00044188 File Offset: 0x00042388
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<ExaminableBatteryComponent, ExaminedEvent>(new ComponentEventHandler<ExaminableBatteryComponent, ExaminedEvent>(this.OnExamine), null, null);
			base.SubscribeLocalEvent<PowerNetworkBatteryComponent, RejuvenateEvent>(new ComponentEventHandler<PowerNetworkBatteryComponent, RejuvenateEvent>(this.OnNetBatteryRejuvenate), null, null);
			base.SubscribeLocalEvent<BatteryComponent, RejuvenateEvent>(new ComponentEventHandler<BatteryComponent, RejuvenateEvent>(this.OnBatteryRejuvenate), null, null);
			base.SubscribeLocalEvent<BatteryComponent, PriceCalculationEvent>(new ComponentEventRefHandler<BatteryComponent, PriceCalculationEvent>(this.CalculateBatteryPrice), null, null);
			base.SubscribeLocalEvent<NetworkBatteryPreSync>(new EntityEventHandler<NetworkBatteryPreSync>(this.PreSync), null, null);
			base.SubscribeLocalEvent<NetworkBatteryPostSync>(new EntityEventHandler<NetworkBatteryPostSync>(this.PostSync), null, null);
		}

		// Token: 0x06000D0A RID: 3338 RVA: 0x00044213 File Offset: 0x00042413
		private void OnNetBatteryRejuvenate(EntityUid uid, PowerNetworkBatteryComponent component, RejuvenateEvent args)
		{
			component.NetworkBattery.CurrentStorage = component.NetworkBattery.Capacity;
		}

		// Token: 0x06000D0B RID: 3339 RVA: 0x0004422B File Offset: 0x0004242B
		private void OnBatteryRejuvenate(EntityUid uid, BatteryComponent component, RejuvenateEvent args)
		{
			component.CurrentCharge = component.MaxCharge;
		}

		// Token: 0x06000D0C RID: 3340 RVA: 0x0004423C File Offset: 0x0004243C
		private void OnExamine(EntityUid uid, ExaminableBatteryComponent component, ExaminedEvent args)
		{
			BatteryComponent batteryComponent;
			if (!base.TryComp<BatteryComponent>(uid, ref batteryComponent))
			{
				return;
			}
			if (args.IsInDetailsRange)
			{
				float effectiveMax = batteryComponent.MaxCharge;
				if (effectiveMax == 0f)
				{
					effectiveMax = 1f;
				}
				int chargePercentRounded = (int)(batteryComponent.CurrentCharge / effectiveMax * 100f);
				args.PushMarkup(Loc.GetString("examinable-battery-component-examine-detail", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("percent", chargePercentRounded),
					new ValueTuple<string, object>("markupPercentColor", "green")
				}));
			}
		}

		// Token: 0x06000D0D RID: 3341 RVA: 0x000442C8 File Offset: 0x000424C8
		private void PreSync(NetworkBatteryPreSync ev)
		{
			foreach (ValueTuple<PowerNetworkBatteryComponent, BatteryComponent> valueTuple in this.EntityManager.EntityQuery<PowerNetworkBatteryComponent, BatteryComponent>(false))
			{
				PowerNetworkBatteryComponent netBat = valueTuple.Item1;
				BatteryComponent bat = valueTuple.Item2;
				netBat.NetworkBattery.Capacity = bat.MaxCharge;
				netBat.NetworkBattery.CurrentStorage = bat.CurrentCharge;
			}
		}

		// Token: 0x06000D0E RID: 3342 RVA: 0x00044344 File Offset: 0x00042544
		private void PostSync(NetworkBatteryPostSync ev)
		{
			foreach (ValueTuple<PowerNetworkBatteryComponent, BatteryComponent> valueTuple in this.EntityManager.EntityQuery<PowerNetworkBatteryComponent, BatteryComponent>(false))
			{
				PowerNetworkBatteryComponent netBat = valueTuple.Item1;
				valueTuple.Item2.CurrentCharge = netBat.NetworkBattery.CurrentStorage;
			}
		}

		// Token: 0x06000D0F RID: 3343 RVA: 0x000443AC File Offset: 0x000425AC
		public override void Update(float frameTime)
		{
			foreach (ValueTuple<BatterySelfRechargerComponent, BatteryComponent> valueTuple in this.EntityManager.EntityQuery<BatterySelfRechargerComponent, BatteryComponent>(false))
			{
				BatterySelfRechargerComponent comp = valueTuple.Item1;
				BatteryComponent batt = valueTuple.Item2;
				if (comp.AutoRecharge && !batt.IsFullyCharged)
				{
					batt.CurrentCharge += comp.AutoRechargeRate * frameTime;
				}
			}
		}

		// Token: 0x06000D10 RID: 3344 RVA: 0x0004442C File Offset: 0x0004262C
		private void CalculateBatteryPrice(EntityUid uid, BatteryComponent component, ref PriceCalculationEvent args)
		{
			args.Price += (double)(component.CurrentCharge * component.PricePerJoule);
		}
	}
}
