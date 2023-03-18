using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Atmos.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Alert;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.FixedPoint;
using Content.Shared.Inventory;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Atmos.EntitySystems
{
	// Token: 0x02000799 RID: 1945
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BarotraumaSystem : EntitySystem
	{
		// Token: 0x06002A07 RID: 10759 RVA: 0x000DCF60 File Offset: 0x000DB160
		public override void Initialize()
		{
			base.SubscribeLocalEvent<PressureProtectionComponent, HighPressureEvent>(new ComponentEventHandler<PressureProtectionComponent, HighPressureEvent>(this.OnHighPressureEvent), null, null);
			base.SubscribeLocalEvent<PressureProtectionComponent, LowPressureEvent>(new ComponentEventHandler<PressureProtectionComponent, LowPressureEvent>(this.OnLowPressureEvent), null, null);
			base.SubscribeLocalEvent<PressureImmunityComponent, HighPressureEvent>(new ComponentEventHandler<PressureImmunityComponent, HighPressureEvent>(this.OnHighPressureImmuneEvent), null, null);
			base.SubscribeLocalEvent<PressureImmunityComponent, LowPressureEvent>(new ComponentEventHandler<PressureImmunityComponent, LowPressureEvent>(this.OnLowPressureImmuneEvent), null, null);
		}

		// Token: 0x06002A08 RID: 10760 RVA: 0x000DCFBD File Offset: 0x000DB1BD
		private void OnHighPressureEvent(EntityUid uid, PressureProtectionComponent component, HighPressureEvent args)
		{
			args.Modifier += component.HighPressureModifier;
			args.Multiplier *= component.HighPressureMultiplier;
		}

		// Token: 0x06002A09 RID: 10761 RVA: 0x000DCFE5 File Offset: 0x000DB1E5
		private void OnLowPressureEvent(EntityUid uid, PressureProtectionComponent component, LowPressureEvent args)
		{
			args.Modifier += component.LowPressureModifier;
			args.Multiplier *= component.LowPressureMultiplier;
		}

		// Token: 0x06002A0A RID: 10762 RVA: 0x000DD00D File Offset: 0x000DB20D
		private void OnHighPressureImmuneEvent(EntityUid uid, PressureImmunityComponent component, HighPressureEvent args)
		{
			args.Multiplier = 0f;
		}

		// Token: 0x06002A0B RID: 10763 RVA: 0x000DD01A File Offset: 0x000DB21A
		private void OnLowPressureImmuneEvent(EntityUid uid, PressureImmunityComponent component, LowPressureEvent args)
		{
			args.Modifier = 100f;
			args.Multiplier = 10000f;
		}

		// Token: 0x06002A0C RID: 10764 RVA: 0x000DD034 File Offset: 0x000DB234
		public float GetFeltLowPressure(BarotraumaComponent baro, float environmentPressure)
		{
			float modifier = float.MaxValue;
			float multiplier = float.MaxValue;
			InventoryComponent inv;
			base.TryComp<InventoryComponent>(baro.Owner, ref inv);
			ContainerManagerComponent contMan;
			base.TryComp<ContainerManagerComponent>(baro.Owner, ref contMan);
			foreach (string slot in baro.ProtectionSlots)
			{
				EntityUid? equipment;
				PressureProtectionComponent protection;
				if (!this._inventorySystem.TryGetSlotEntity(baro.Owner, slot, out equipment, inv, contMan) || !base.TryComp<PressureProtectionComponent>(equipment, ref protection))
				{
					modifier = 0f;
					multiplier = 1f;
					break;
				}
				modifier = Math.Min(protection.LowPressureModifier, modifier);
				multiplier = Math.Min(protection.LowPressureMultiplier, multiplier);
			}
			LowPressureEvent lowPressureEvent = new LowPressureEvent(environmentPressure);
			base.RaiseLocalEvent<LowPressureEvent>(baro.Owner, lowPressureEvent, false);
			return (environmentPressure + modifier + lowPressureEvent.Modifier) * (multiplier * lowPressureEvent.Multiplier);
		}

		// Token: 0x06002A0D RID: 10765 RVA: 0x000DD128 File Offset: 0x000DB328
		public float GetFeltHighPressure(BarotraumaComponent baro, float environmentPressure)
		{
			float modifier = float.MinValue;
			float multiplier = float.MinValue;
			InventoryComponent inv;
			base.TryComp<InventoryComponent>(baro.Owner, ref inv);
			ContainerManagerComponent contMan;
			base.TryComp<ContainerManagerComponent>(baro.Owner, ref contMan);
			foreach (string slot in baro.ProtectionSlots)
			{
				EntityUid? equipment;
				PressureProtectionComponent protection;
				if (!this._inventorySystem.TryGetSlotEntity(baro.Owner, slot, out equipment, inv, contMan) || !base.TryComp<PressureProtectionComponent>(equipment, ref protection))
				{
					modifier = 0f;
					multiplier = 1f;
					break;
				}
				modifier = Math.Max(protection.LowPressureModifier, modifier);
				multiplier = Math.Max(protection.LowPressureMultiplier, multiplier);
			}
			HighPressureEvent highPressureEvent = new HighPressureEvent(environmentPressure);
			base.RaiseLocalEvent<HighPressureEvent>(baro.Owner, highPressureEvent, false);
			return (environmentPressure + modifier + highPressureEvent.Modifier) * (multiplier * highPressureEvent.Multiplier);
		}

		// Token: 0x06002A0E RID: 10766 RVA: 0x000DD21C File Offset: 0x000DB41C
		public override void Update(float frameTime)
		{
			this._timer += frameTime;
			if (this._timer < 1f)
			{
				return;
			}
			this._timer -= 1f;
			foreach (ValueTuple<BarotraumaComponent, DamageableComponent, TransformComponent> valueTuple in this.EntityManager.EntityQuery<BarotraumaComponent, DamageableComponent, TransformComponent>(false))
			{
				BarotraumaComponent barotrauma = valueTuple.Item1;
				DamageableComponent damageable = valueTuple.Item2;
				EntityUid uid = barotrauma.Owner;
				FixedPoint2 totalDamage = FixedPoint2.Zero;
				foreach (KeyValuePair<string, FixedPoint2> keyValuePair in barotrauma.Damage.DamageDict)
				{
					string text;
					FixedPoint2 fixedPoint;
					keyValuePair.Deconstruct(out text, out fixedPoint);
					string barotraumaDamageType = text;
					FixedPoint2 damage;
					if (damageable.Damage.DamageDict.TryGetValue(barotraumaDamageType, out damage))
					{
						totalDamage += damage;
					}
				}
				if (!(totalDamage >= barotrauma.MaxDamage))
				{
					float pressure = 1f;
					GasMixture mixture = this._atmosphereSystem.GetContainingMixture(uid, false, false, null);
					if (mixture != null)
					{
						pressure = MathF.Max(mixture.Pressure, 1f);
					}
					if (pressure > 50f)
					{
						if (pressure >= 385f)
						{
							pressure = this.GetFeltHighPressure(barotrauma, pressure);
							if (pressure >= 385f)
							{
								float damageScale = MathF.Min(pressure / 550f * 4f, 4f);
								this._damageableSystem.TryChangeDamage(new EntityUid?(barotrauma.Owner), barotrauma.Damage * damageScale, true, false, null, null);
								if (!barotrauma.TakingDamage)
								{
									barotrauma.TakingDamage = true;
									ISharedAdminLogManager adminLogger = this._adminLogger;
									LogType type = LogType.Barotrauma;
									LogStringHandler logStringHandler = new LogStringHandler(36, 1);
									logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(barotrauma.Owner), "entity", "ToPrettyString(barotrauma.Owner)");
									logStringHandler.AppendLiteral(" started taking high pressure damage");
									adminLogger.Add(type, ref logStringHandler);
								}
								if (pressure >= 550f)
								{
									this._alertsSystem.ShowAlert(barotrauma.Owner, AlertType.HighPressure, new short?((short)2), null);
									continue;
								}
								this._alertsSystem.ShowAlert(barotrauma.Owner, AlertType.HighPressure, new short?((short)1), null);
								continue;
							}
						}
					}
					else
					{
						pressure = this.GetFeltLowPressure(barotrauma, pressure);
						if (pressure <= 50f)
						{
							this._damageableSystem.TryChangeDamage(new EntityUid?(barotrauma.Owner), barotrauma.Damage * 4f, true, false, null, null);
							if (!barotrauma.TakingDamage)
							{
								barotrauma.TakingDamage = true;
								ISharedAdminLogManager adminLogger2 = this._adminLogger;
								LogType type2 = LogType.Barotrauma;
								LogStringHandler logStringHandler = new LogStringHandler(35, 1);
								logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(barotrauma.Owner), "entity", "ToPrettyString(barotrauma.Owner)");
								logStringHandler.AppendLiteral(" started taking low pressure damage");
								adminLogger2.Add(type2, ref logStringHandler);
							}
							if (pressure <= 20f)
							{
								this._alertsSystem.ShowAlert(barotrauma.Owner, AlertType.LowPressure, new short?((short)2), null);
								continue;
							}
							this._alertsSystem.ShowAlert(barotrauma.Owner, AlertType.LowPressure, new short?((short)1), null);
							continue;
						}
					}
					if (barotrauma.TakingDamage)
					{
						barotrauma.TakingDamage = false;
						ISharedAdminLogManager adminLogger3 = this._adminLogger;
						LogType type3 = LogType.Barotrauma;
						LogStringHandler logStringHandler = new LogStringHandler(31, 1);
						logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(barotrauma.Owner), "entity", "ToPrettyString(barotrauma.Owner)");
						logStringHandler.AppendLiteral(" stopped taking pressure damage");
						adminLogger3.Add(type3, ref logStringHandler);
					}
					this._alertsSystem.ClearAlertCategory(barotrauma.Owner, AlertCategory.Pressure);
				}
			}
		}

		// Token: 0x040019F6 RID: 6646
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x040019F7 RID: 6647
		[Dependency]
		private readonly DamageableSystem _damageableSystem;

		// Token: 0x040019F8 RID: 6648
		[Dependency]
		private readonly AlertsSystem _alertsSystem;

		// Token: 0x040019F9 RID: 6649
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x040019FA RID: 6650
		[Dependency]
		private readonly InventorySystem _inventorySystem;

		// Token: 0x040019FB RID: 6651
		private const float UpdateTimer = 1f;

		// Token: 0x040019FC RID: 6652
		private float _timer;
	}
}
