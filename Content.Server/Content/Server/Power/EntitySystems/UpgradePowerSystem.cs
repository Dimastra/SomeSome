using System;
using System.Runtime.CompilerServices;
using Content.Server.Construction;
using Content.Server.Construction.Components;
using Content.Server.Power.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Log;

namespace Content.Server.Power.EntitySystems
{
	// Token: 0x0200029C RID: 668
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class UpgradePowerSystem : EntitySystem
	{
		// Token: 0x06000D94 RID: 3476 RVA: 0x000470B8 File Offset: 0x000452B8
		public override void Initialize()
		{
			base.SubscribeLocalEvent<UpgradePowerDrawComponent, MapInitEvent>(new ComponentEventHandler<UpgradePowerDrawComponent, MapInitEvent>(this.OnMapInit), null, null);
			base.SubscribeLocalEvent<UpgradePowerDrawComponent, RefreshPartsEvent>(new ComponentEventHandler<UpgradePowerDrawComponent, RefreshPartsEvent>(this.OnRefreshParts), null, null);
			base.SubscribeLocalEvent<UpgradePowerDrawComponent, UpgradeExamineEvent>(new ComponentEventHandler<UpgradePowerDrawComponent, UpgradeExamineEvent>(this.OnUpgradeExamine), null, null);
			base.SubscribeLocalEvent<UpgradePowerSupplierComponent, MapInitEvent>(new ComponentEventHandler<UpgradePowerSupplierComponent, MapInitEvent>(this.OnSupplierMapInit), null, null);
			base.SubscribeLocalEvent<UpgradePowerSupplierComponent, RefreshPartsEvent>(new ComponentEventHandler<UpgradePowerSupplierComponent, RefreshPartsEvent>(this.OnSupplierRefreshParts), null, null);
			base.SubscribeLocalEvent<UpgradePowerSupplierComponent, UpgradeExamineEvent>(new ComponentEventHandler<UpgradePowerSupplierComponent, UpgradeExamineEvent>(this.OnSupplierUpgradeExamine), null, null);
		}

		// Token: 0x06000D95 RID: 3477 RVA: 0x00047140 File Offset: 0x00045340
		private void OnMapInit(EntityUid uid, UpgradePowerDrawComponent component, MapInitEvent args)
		{
			PowerConsumerComponent powa;
			if (base.TryComp<PowerConsumerComponent>(uid, ref powa))
			{
				component.BaseLoad = powa.DrawRate;
				return;
			}
			ApcPowerReceiverComponent powa2;
			if (base.TryComp<ApcPowerReceiverComponent>(uid, ref powa2))
			{
				component.BaseLoad = powa2.Load;
			}
		}

		// Token: 0x06000D96 RID: 3478 RVA: 0x0004717C File Offset: 0x0004537C
		private void OnRefreshParts(EntityUid uid, UpgradePowerDrawComponent component, RefreshPartsEvent args)
		{
			float load = component.BaseLoad;
			float rating = args.PartRatings[component.MachinePartPowerDraw];
			MachineUpgradeScalingType scaling = component.Scaling;
			if (scaling != MachineUpgradeScalingType.Linear)
			{
				if (scaling != MachineUpgradeScalingType.Exponential)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 1);
					defaultInterpolatedStringHandler.AppendLiteral("invalid power scaling type for ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
					defaultInterpolatedStringHandler.AppendLiteral(".");
					Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
					load = 0f;
				}
				else
				{
					load *= MathF.Pow(component.PowerDrawMultiplier, rating - 1f);
				}
			}
			else
			{
				load += component.PowerDrawMultiplier * (rating - 1f);
			}
			ApcPowerReceiverComponent powa;
			if (base.TryComp<ApcPowerReceiverComponent>(uid, ref powa))
			{
				powa.Load = load;
			}
			PowerConsumerComponent powa2;
			if (base.TryComp<PowerConsumerComponent>(uid, ref powa2))
			{
				powa2.DrawRate = load;
			}
		}

		// Token: 0x06000D97 RID: 3479 RVA: 0x00047248 File Offset: 0x00045448
		private void OnUpgradeExamine(EntityUid uid, UpgradePowerDrawComponent component, UpgradeExamineEvent args)
		{
			ApcPowerReceiverComponent apcPowerReceiverComponent = base.CompOrNull<ApcPowerReceiverComponent>(uid);
			float? num = ((apcPowerReceiverComponent != null) ? new float?(apcPowerReceiverComponent.Load) : null) / component.BaseLoad;
			float? num2;
			if (num == null)
			{
				PowerConsumerComponent powerConsumerComponent = base.CompOrNull<PowerConsumerComponent>(uid);
				num2 = ((powerConsumerComponent != null) ? new float?(powerConsumerComponent.DrawRate) : null) / component.BaseLoad;
			}
			else
			{
				num2 = num;
			}
			float? powerDrawMultiplier = num2;
			if (powerDrawMultiplier != null)
			{
				args.AddPercentageUpgrade("upgrade-power-draw", powerDrawMultiplier.Value);
			}
		}

		// Token: 0x06000D98 RID: 3480 RVA: 0x00047318 File Offset: 0x00045518
		private void OnSupplierMapInit(EntityUid uid, UpgradePowerSupplierComponent component, MapInitEvent args)
		{
			PowerSupplierComponent supplier;
			if (base.TryComp<PowerSupplierComponent>(uid, ref supplier))
			{
				component.BaseSupplyRate = supplier.MaxSupply;
			}
		}

		// Token: 0x06000D99 RID: 3481 RVA: 0x0004733C File Offset: 0x0004553C
		private void OnSupplierRefreshParts(EntityUid uid, UpgradePowerSupplierComponent component, RefreshPartsEvent args)
		{
			float supply = component.BaseSupplyRate;
			float rating = args.PartRatings[component.MachinePartPowerSupply];
			MachineUpgradeScalingType scaling = component.Scaling;
			if (scaling != MachineUpgradeScalingType.Linear)
			{
				if (scaling != MachineUpgradeScalingType.Exponential)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 1);
					defaultInterpolatedStringHandler.AppendLiteral("invalid power scaling type for ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
					defaultInterpolatedStringHandler.AppendLiteral(".");
					Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
					supply = component.BaseSupplyRate;
				}
				else
				{
					supply *= MathF.Pow(component.PowerSupplyMultiplier, rating - 1f);
				}
			}
			else
			{
				supply += component.BaseSupplyRate * (rating - 1f);
			}
			PowerSupplierComponent powa;
			if (base.TryComp<PowerSupplierComponent>(uid, ref powa))
			{
				powa.MaxSupply = supply;
			}
		}

		// Token: 0x06000D9A RID: 3482 RVA: 0x000473F4 File Offset: 0x000455F4
		private void OnSupplierUpgradeExamine(EntityUid uid, UpgradePowerSupplierComponent component, UpgradeExamineEvent args)
		{
			PowerSupplierComponent powa;
			if (base.TryComp<PowerSupplierComponent>(uid, ref powa))
			{
				args.AddPercentageUpgrade("upgrade-power-supply", powa.MaxSupply / component.BaseSupplyRate);
			}
		}
	}
}
