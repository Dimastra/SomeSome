using System;
using System.Runtime.CompilerServices;
using Content.Server.AlertLevel;
using Content.Server.Audio;
using Content.Server.Light.Components;
using Content.Server.Power.Components;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Shared.Examine;
using Content.Shared.Light;
using Content.Shared.Light.Component;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Server.Light.EntitySystems
{
	// Token: 0x0200040A RID: 1034
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EmergencyLightSystem : SharedEmergencyLightSystem
	{
		// Token: 0x060014E2 RID: 5346 RVA: 0x0006D55C File Offset: 0x0006B75C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<EmergencyLightComponent, EmergencyLightEvent>(new ComponentEventHandler<EmergencyLightComponent, EmergencyLightEvent>(this.OnEmergencyLightEvent), null, null);
			base.SubscribeLocalEvent<AlertLevelChangedEvent>(new EntityEventHandler<AlertLevelChangedEvent>(this.OnAlertLevelChanged), null, null);
			base.SubscribeLocalEvent<EmergencyLightComponent, ComponentGetState>(new ComponentEventRefHandler<EmergencyLightComponent, ComponentGetState>(this.GetCompState), null, null);
			base.SubscribeLocalEvent<EmergencyLightComponent, PointLightToggleEvent>(new ComponentEventHandler<EmergencyLightComponent, PointLightToggleEvent>(this.HandleLightToggle), null, null);
			base.SubscribeLocalEvent<EmergencyLightComponent, ExaminedEvent>(new ComponentEventHandler<EmergencyLightComponent, ExaminedEvent>(this.OnEmergencyExamine), null, null);
			base.SubscribeLocalEvent<EmergencyLightComponent, PowerChangedEvent>(new ComponentEventRefHandler<EmergencyLightComponent, PowerChangedEvent>(this.OnEmergencyPower), null, null);
		}

		// Token: 0x060014E3 RID: 5347 RVA: 0x0006D5E7 File Offset: 0x0006B7E7
		private void OnEmergencyPower(EntityUid uid, EmergencyLightComponent component, ref PowerChangedEvent args)
		{
			if (base.MetaData(uid).EntityLifeStage >= 4)
			{
				return;
			}
			this.UpdateState(component);
		}

		// Token: 0x060014E4 RID: 5348 RVA: 0x0006D600 File Offset: 0x0006B800
		private void OnEmergencyExamine(EntityUid uid, EmergencyLightComponent component, ExaminedEvent args)
		{
			args.PushMarkup(Loc.GetString("emergency-light-component-on-examine", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("batteryStateText", Loc.GetString(component.BatteryStateText[component.State]))
			}));
			AlertLevelComponent alerts;
			if (!base.TryComp<AlertLevelComponent>(this._station.GetOwningStation(uid, null), ref alerts))
			{
				return;
			}
			if (alerts.AlertLevels == null)
			{
				return;
			}
			string name = alerts.CurrentLevel;
			Color color = Color.White;
			AlertLevelDetail details;
			if (alerts.AlertLevels.Levels.TryGetValue(alerts.CurrentLevel, out details))
			{
				color = details.Color;
			}
			args.PushMarkup(Loc.GetString("emergency-light-component-on-examine-alert", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("color", color.ToHex()),
				new ValueTuple<string, object>("level", name)
			}));
		}

		// Token: 0x060014E5 RID: 5349 RVA: 0x0006D6D9 File Offset: 0x0006B8D9
		private void HandleLightToggle(EntityUid uid, EmergencyLightComponent component, PointLightToggleEvent args)
		{
			if (component.Enabled == args.Enabled)
			{
				return;
			}
			component.Enabled = args.Enabled;
			base.Dirty(component, null);
		}

		// Token: 0x060014E6 RID: 5350 RVA: 0x0006D6FE File Offset: 0x0006B8FE
		private void GetCompState(EntityUid uid, EmergencyLightComponent component, ref ComponentGetState args)
		{
			args.State = new EmergencyLightComponentState(component.Enabled);
		}

		// Token: 0x060014E7 RID: 5351 RVA: 0x0006D714 File Offset: 0x0006B914
		private void OnEmergencyLightEvent(EntityUid uid, EmergencyLightComponent component, EmergencyLightEvent args)
		{
			switch (args.State)
			{
			case EmergencyLightState.Charging:
			case EmergencyLightState.On:
				base.EnsureComp<ActiveEmergencyLightComponent>(uid);
				return;
			case EmergencyLightState.Full:
			case EmergencyLightState.Empty:
				base.RemComp<ActiveEmergencyLightComponent>(uid);
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x060014E8 RID: 5352 RVA: 0x0006D758 File Offset: 0x0006B958
		private void OnAlertLevelChanged(AlertLevelChangedEvent ev)
		{
			AlertLevelComponent alert;
			if (!base.TryComp<AlertLevelComponent>(ev.Station, ref alert))
			{
				return;
			}
			AlertLevelDetail details;
			if (alert.AlertLevels == null || !alert.AlertLevels.Levels.TryGetValue(ev.AlertLevel, out details))
			{
				return;
			}
			foreach (ValueTuple<EmergencyLightComponent, PointLightComponent, AppearanceComponent, TransformComponent> valueTuple in base.EntityQuery<EmergencyLightComponent, PointLightComponent, AppearanceComponent, TransformComponent>(false))
			{
				EmergencyLightComponent light = valueTuple.Item1;
				PointLightComponent pointLight = valueTuple.Item2;
				AppearanceComponent appearance = valueTuple.Item3;
				TransformComponent xform = valueTuple.Item4;
				StationMemberComponent stationMemberComponent = base.CompOrNull<StationMemberComponent>(xform.GridUid);
				EntityUid? entityUid = (stationMemberComponent != null) ? new EntityUid?(stationMemberComponent.Station) : null;
				EntityUid station = ev.Station;
				if (entityUid != null && (entityUid == null || !(entityUid.GetValueOrDefault() != station)))
				{
					pointLight.Color = details.EmergencyLightColor;
					this._appearance.SetData(appearance.Owner, EmergencyLightVisuals.Color, details.EmergencyLightColor, appearance);
					if (details.ForceEnableEmergencyLights && !light.ForciblyEnabled)
					{
						light.ForciblyEnabled = true;
						this.TurnOn(light);
					}
					else if (!details.ForceEnableEmergencyLights && light.ForciblyEnabled)
					{
						light.ForciblyEnabled = false;
						this.UpdateState(light);
					}
				}
			}
		}

		// Token: 0x060014E9 RID: 5353 RVA: 0x0006D8C0 File Offset: 0x0006BAC0
		public void SetState(EmergencyLightComponent component, EmergencyLightState state)
		{
			if (component.State == state)
			{
				return;
			}
			component.State = state;
			base.RaiseLocalEvent<EmergencyLightEvent>(component.Owner, new EmergencyLightEvent(component, state), false);
		}

		// Token: 0x060014EA RID: 5354 RVA: 0x0006D8E8 File Offset: 0x0006BAE8
		public override void Update(float frameTime)
		{
			foreach (ValueTuple<ActiveEmergencyLightComponent, EmergencyLightComponent, BatteryComponent> valueTuple in base.EntityQuery<ActiveEmergencyLightComponent, EmergencyLightComponent, BatteryComponent>(false))
			{
				EmergencyLightComponent activeLight = valueTuple.Item2;
				BatteryComponent battery = valueTuple.Item3;
				this.Update(activeLight, battery, frameTime);
			}
		}

		// Token: 0x060014EB RID: 5355 RVA: 0x0006D944 File Offset: 0x0006BB44
		private void Update(EmergencyLightComponent component, BatteryComponent battery, float frameTime)
		{
			if (component.State == EmergencyLightState.On)
			{
				if (!battery.TryUseCharge(component.Wattage * frameTime))
				{
					this.SetState(component, EmergencyLightState.Empty);
					this.TurnOff(component);
					return;
				}
			}
			else
			{
				battery.CurrentCharge += component.ChargingWattage * frameTime * component.ChargingEfficiency;
				if (battery.IsFullyCharged)
				{
					ApcPowerReceiverComponent receiver;
					if (base.TryComp<ApcPowerReceiverComponent>(component.Owner, ref receiver))
					{
						receiver.Load = 1f;
					}
					this.SetState(component, EmergencyLightState.Full);
				}
			}
		}

		// Token: 0x060014EC RID: 5356 RVA: 0x0006D9C4 File Offset: 0x0006BBC4
		public void UpdateState(EmergencyLightComponent component)
		{
			ApcPowerReceiverComponent receiver;
			if (!base.TryComp<ApcPowerReceiverComponent>(component.Owner, ref receiver))
			{
				return;
			}
			if (receiver.Powered && !component.ForciblyEnabled)
			{
				receiver.Load = (float)((int)Math.Abs(component.Wattage));
				this.TurnOff(component);
				this.SetState(component, EmergencyLightState.Charging);
				return;
			}
			this.TurnOn(component);
			this.SetState(component, EmergencyLightState.On);
		}

		// Token: 0x060014ED RID: 5357 RVA: 0x0006DA24 File Offset: 0x0006BC24
		private void TurnOff(EmergencyLightComponent component)
		{
			PointLightComponent light;
			if (base.TryComp<PointLightComponent>(component.Owner, ref light))
			{
				light.Enabled = false;
			}
			AppearanceComponent appearance;
			if (base.TryComp<AppearanceComponent>(component.Owner, ref appearance))
			{
				this._appearance.SetData(appearance.Owner, EmergencyLightVisuals.On, false, appearance);
			}
			this._ambient.SetAmbience(component.Owner, false, null);
		}

		// Token: 0x060014EE RID: 5358 RVA: 0x0006DA8C File Offset: 0x0006BC8C
		private void TurnOn(EmergencyLightComponent component)
		{
			PointLightComponent light;
			if (base.TryComp<PointLightComponent>(component.Owner, ref light))
			{
				light.Enabled = true;
			}
			AppearanceComponent appearance;
			if (base.TryComp<AppearanceComponent>(component.Owner, ref appearance))
			{
				this._appearance.SetData(appearance.Owner, EmergencyLightVisuals.On, true, appearance);
			}
			this._ambient.SetAmbience(component.Owner, true, null);
		}

		// Token: 0x04000CFF RID: 3327
		[Dependency]
		private readonly AmbientSoundSystem _ambient;

		// Token: 0x04000D00 RID: 3328
		[Dependency]
		private readonly StationSystem _station;

		// Token: 0x04000D01 RID: 3329
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
