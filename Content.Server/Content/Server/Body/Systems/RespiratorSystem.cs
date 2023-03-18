using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Atmos;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Body.Components;
using Content.Server.Popups;
using Content.Shared.Administration.Logs;
using Content.Shared.Alert;
using Content.Shared.Body.Components;
using Content.Shared.Body.Organ;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Content.Server.Body.Systems
{
	// Token: 0x0200070C RID: 1804
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RespiratorSystem : EntitySystem
	{
		// Token: 0x06002603 RID: 9731 RVA: 0x000C89BC File Offset: 0x000C6BBC
		public override void Initialize()
		{
			base.Initialize();
			base.UpdatesAfter.Add(typeof(MetabolizerSystem));
			base.SubscribeLocalEvent<RespiratorComponent, ApplyMetabolicMultiplierEvent>(new ComponentEventHandler<RespiratorComponent, ApplyMetabolicMultiplierEvent>(this.OnApplyMetabolicMultiplier), null, null);
		}

		// Token: 0x06002604 RID: 9732 RVA: 0x000C89F0 File Offset: 0x000C6BF0
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (ValueTuple<RespiratorComponent, BodyComponent> valueTuple in this.EntityManager.EntityQuery<RespiratorComponent, BodyComponent>(false))
			{
				RespiratorComponent respirator = valueTuple.Item1;
				BodyComponent body = valueTuple.Item2;
				EntityUid uid = respirator.Owner;
				if (!this._mobState.IsDead(uid, null))
				{
					respirator.AccumulatedFrametime += frameTime;
					if (respirator.AccumulatedFrametime >= respirator.CycleDelay)
					{
						respirator.AccumulatedFrametime -= respirator.CycleDelay;
						this.UpdateSaturation(respirator.Owner, -respirator.CycleDelay, respirator);
						if (!this._mobState.IsIncapacitated(uid, null))
						{
							RespiratorStatus status = respirator.Status;
							if (status != RespiratorStatus.Inhaling)
							{
								if (status == RespiratorStatus.Exhaling)
								{
									this.Exhale(uid, body);
									respirator.Status = RespiratorStatus.Inhaling;
								}
							}
							else
							{
								this.Inhale(uid, body);
								respirator.Status = RespiratorStatus.Exhaling;
							}
						}
						if (respirator.Saturation < respirator.SuffocationThreshold)
						{
							if (this._gameTiming.CurTime >= respirator.LastGaspPopupTime + respirator.GaspPopupCooldown)
							{
								respirator.LastGaspPopupTime = this._gameTiming.CurTime;
								this._popupSystem.PopupEntity(Loc.GetString("lung-behavior-gasp"), uid, PopupType.Small);
							}
							this.TakeSuffocationDamage(uid, respirator);
							respirator.SuffocationCycles++;
						}
						else
						{
							this.StopSuffocation(uid, respirator);
							respirator.SuffocationCycles = 0;
						}
					}
				}
			}
		}

		// Token: 0x06002605 RID: 9733 RVA: 0x000C8B84 File Offset: 0x000C6D84
		[NullableContext(2)]
		public void Inhale(EntityUid uid, BodyComponent body = null)
		{
			if (!base.Resolve<BodyComponent>(uid, ref body, false))
			{
				return;
			}
			List<ValueTuple<LungComponent, OrganComponent>> organs = this._bodySystem.GetBodyOrganComponents<LungComponent>(uid, body);
			InhaleLocationEvent ev = new InhaleLocationEvent();
			base.RaiseLocalEvent<InhaleLocationEvent>(uid, ev, false);
			InhaleLocationEvent inhaleLocationEvent = ev;
			if (inhaleLocationEvent.Gas == null)
			{
				inhaleLocationEvent.Gas = this._atmosSys.GetContainingMixture(uid, false, true, null);
			}
			if (ev.Gas == null)
			{
				return;
			}
			GasMixture actualGas = ev.Gas.RemoveVolume(0.5f);
			float lungRatio = 1f / (float)organs.Count;
			GasMixture gas = (organs.Count == 1) ? actualGas : actualGas.RemoveRatio(lungRatio);
			foreach (ValueTuple<LungComponent, OrganComponent> valueTuple in organs)
			{
				LungComponent lung = valueTuple.Item1;
				this._atmosSys.Merge(lung.Air, gas);
				this._lungSystem.GasToReagent(lung.Owner, lung);
			}
		}

		// Token: 0x06002606 RID: 9734 RVA: 0x000C8C84 File Offset: 0x000C6E84
		[NullableContext(2)]
		public void Exhale(EntityUid uid, BodyComponent body = null)
		{
			if (!base.Resolve<BodyComponent>(uid, ref body, false))
			{
				return;
			}
			List<ValueTuple<LungComponent, OrganComponent>> bodyOrganComponents = this._bodySystem.GetBodyOrganComponents<LungComponent>(uid, body);
			ExhaleLocationEvent ev = new ExhaleLocationEvent();
			base.RaiseLocalEvent<ExhaleLocationEvent>(uid, ev, false);
			if (ev.Gas == null)
			{
				ev.Gas = this._atmosSys.GetContainingMixture(uid, false, true, null);
				ExhaleLocationEvent exhaleLocationEvent = ev;
				if (exhaleLocationEvent.Gas == null)
				{
					exhaleLocationEvent.Gas = GasMixture.SpaceGas;
				}
			}
			GasMixture outGas = new GasMixture(ev.Gas.Volume);
			foreach (ValueTuple<LungComponent, OrganComponent> valueTuple in bodyOrganComponents)
			{
				LungComponent lung = valueTuple.Item1;
				this._atmosSys.Merge(outGas, lung.Air);
				lung.Air.Clear();
				lung.LungSolution.RemoveAllSolution();
			}
			this._atmosSys.Merge(ev.Gas, outGas);
		}

		// Token: 0x06002607 RID: 9735 RVA: 0x000C8D7C File Offset: 0x000C6F7C
		private void TakeSuffocationDamage(EntityUid uid, RespiratorComponent respirator)
		{
			if (respirator.SuffocationCycles == 2)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Asphyxiation;
				LogStringHandler logStringHandler = new LogStringHandler(20, 1);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "entity", "ToPrettyString(uid)");
				logStringHandler.AppendLiteral(" started suffocating");
				adminLogger.Add(type, ref logStringHandler);
			}
			if (respirator.SuffocationCycles >= respirator.SuffocationCycleThreshold)
			{
				this._alertsSystem.ShowAlert(uid, AlertType.LowOxygen, null, null);
			}
			this._damageableSys.TryChangeDamage(new EntityUid?(uid), respirator.Damage, true, false, null, null);
		}

		// Token: 0x06002608 RID: 9736 RVA: 0x000C8E20 File Offset: 0x000C7020
		private void StopSuffocation(EntityUid uid, RespiratorComponent respirator)
		{
			if (respirator.SuffocationCycles >= 2)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Asphyxiation;
				LogStringHandler logStringHandler = new LogStringHandler(20, 1);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "entity", "ToPrettyString(uid)");
				logStringHandler.AppendLiteral(" stopped suffocating");
				adminLogger.Add(type, ref logStringHandler);
			}
			this._alertsSystem.ClearAlert(uid, AlertType.LowOxygen);
			this._damageableSys.TryChangeDamage(new EntityUid?(uid), respirator.DamageRecovery, true, true, null, null);
		}

		// Token: 0x06002609 RID: 9737 RVA: 0x000C8EA4 File Offset: 0x000C70A4
		[NullableContext(2)]
		public void UpdateSaturation(EntityUid uid, float amount, RespiratorComponent respirator = null)
		{
			if (!base.Resolve<RespiratorComponent>(uid, ref respirator, false))
			{
				return;
			}
			respirator.Saturation += amount;
			respirator.Saturation = Math.Clamp(respirator.Saturation, respirator.MinSaturation, respirator.MaxSaturation);
		}

		// Token: 0x0600260A RID: 9738 RVA: 0x000C8EE0 File Offset: 0x000C70E0
		private void OnApplyMetabolicMultiplier(EntityUid uid, RespiratorComponent component, ApplyMetabolicMultiplierEvent args)
		{
			if (args.Apply)
			{
				component.CycleDelay *= args.Multiplier;
				component.Saturation *= args.Multiplier;
				component.MaxSaturation *= args.Multiplier;
				component.MinSaturation *= args.Multiplier;
				return;
			}
			component.CycleDelay /= args.Multiplier;
			component.Saturation /= args.Multiplier;
			component.MaxSaturation /= args.Multiplier;
			component.MinSaturation /= args.Multiplier;
			if (component.AccumulatedFrametime >= component.CycleDelay)
			{
				component.AccumulatedFrametime = component.CycleDelay;
			}
		}

		// Token: 0x04001779 RID: 6009
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x0400177A RID: 6010
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x0400177B RID: 6011
		[Dependency]
		private readonly AlertsSystem _alertsSystem;

		// Token: 0x0400177C RID: 6012
		[Dependency]
		private readonly AtmosphereSystem _atmosSys;

		// Token: 0x0400177D RID: 6013
		[Dependency]
		private readonly BodySystem _bodySystem;

		// Token: 0x0400177E RID: 6014
		[Dependency]
		private readonly DamageableSystem _damageableSys;

		// Token: 0x0400177F RID: 6015
		[Dependency]
		private readonly LungSystem _lungSystem;

		// Token: 0x04001780 RID: 6016
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x04001781 RID: 6017
		[Dependency]
		private readonly MobStateSystem _mobState;
	}
}
