using System;
using System.Runtime.CompilerServices;
using Content.Server.Construction;
using Content.Server.Power.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.Power.EntitySystems
{
	// Token: 0x0200029B RID: 667
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class UpgradeBatterySystem : EntitySystem
	{
		// Token: 0x06000D90 RID: 3472 RVA: 0x00047002 File Offset: 0x00045202
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<UpgradeBatteryComponent, RefreshPartsEvent>(new ComponentEventHandler<UpgradeBatteryComponent, RefreshPartsEvent>(this.OnRefreshParts), null, null);
			base.SubscribeLocalEvent<UpgradeBatteryComponent, UpgradeExamineEvent>(new ComponentEventHandler<UpgradeBatteryComponent, UpgradeExamineEvent>(this.OnUpgradeExamine), null, null);
		}

		// Token: 0x06000D91 RID: 3473 RVA: 0x00047034 File Offset: 0x00045234
		public void OnRefreshParts(EntityUid uid, UpgradeBatteryComponent component, RefreshPartsEvent args)
		{
			float capacitorRating = args.PartRatings[component.MachinePartPowerCapacity];
			BatteryComponent batteryComp;
			if (base.TryComp<BatteryComponent>(uid, ref batteryComp))
			{
				batteryComp.MaxCharge = MathF.Pow(component.MaxChargeMultiplier, capacitorRating - 1f) * component.BaseMaxCharge;
			}
		}

		// Token: 0x06000D92 RID: 3474 RVA: 0x00047080 File Offset: 0x00045280
		private void OnUpgradeExamine(EntityUid uid, UpgradeBatteryComponent component, UpgradeExamineEvent args)
		{
			BatteryComponent batteryComp;
			if (base.TryComp<BatteryComponent>(uid, ref batteryComp))
			{
				args.AddPercentageUpgrade("upgrade-max-charge", batteryComp.MaxCharge / component.BaseMaxCharge);
			}
		}
	}
}
