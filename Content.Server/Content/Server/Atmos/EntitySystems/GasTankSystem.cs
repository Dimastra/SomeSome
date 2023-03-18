using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.Components;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Server.Cargo.Systems;
using Content.Server.Explosion.Components;
using Content.Server.Explosion.EntitySystems;
using Content.Server.UserInterface;
using Content.Shared.Actions;
using Content.Shared.Atmos.Components;
using Content.Shared.Examine;
using Content.Shared.Toggleable;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;

namespace Content.Server.Atmos.EntitySystems
{
	// Token: 0x0200079C RID: 1948
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasTankSystem : EntitySystem
	{
		// Token: 0x06002A2D RID: 10797 RVA: 0x000DE6AC File Offset: 0x000DC8AC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GasTankComponent, ComponentShutdown>(new ComponentEventHandler<GasTankComponent, ComponentShutdown>(this.OnGasShutdown), null, null);
			base.SubscribeLocalEvent<GasTankComponent, BeforeActivatableUIOpenEvent>(new ComponentEventHandler<GasTankComponent, BeforeActivatableUIOpenEvent>(this.BeforeUiOpen), null, null);
			base.SubscribeLocalEvent<GasTankComponent, GetItemActionsEvent>(new ComponentEventHandler<GasTankComponent, GetItemActionsEvent>(this.OnGetActions), null, null);
			base.SubscribeLocalEvent<GasTankComponent, ExaminedEvent>(new ComponentEventHandler<GasTankComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<GasTankComponent, ToggleActionEvent>(new ComponentEventHandler<GasTankComponent, ToggleActionEvent>(this.OnActionToggle), null, null);
			base.SubscribeLocalEvent<GasTankComponent, EntParentChangedMessage>(new ComponentEventRefHandler<GasTankComponent, EntParentChangedMessage>(this.OnParentChange), null, null);
			base.SubscribeLocalEvent<GasTankComponent, GasTankSetPressureMessage>(new ComponentEventHandler<GasTankComponent, GasTankSetPressureMessage>(this.OnGasTankSetPressure), null, null);
			base.SubscribeLocalEvent<GasTankComponent, GasTankToggleInternalsMessage>(new ComponentEventHandler<GasTankComponent, GasTankToggleInternalsMessage>(this.OnGasTankToggleInternals), null, null);
			base.SubscribeLocalEvent<GasTankComponent, GasAnalyzerScanEvent>(new ComponentEventHandler<GasTankComponent, GasAnalyzerScanEvent>(this.OnAnalyzed), null, null);
			base.SubscribeLocalEvent<GasTankComponent, PriceCalculationEvent>(new ComponentEventRefHandler<GasTankComponent, PriceCalculationEvent>(this.OnGasTankPrice), null, null);
		}

		// Token: 0x06002A2E RID: 10798 RVA: 0x000DE787 File Offset: 0x000DC987
		private void OnGasShutdown(EntityUid uid, GasTankComponent component, ComponentShutdown args)
		{
			this.DisconnectFromInternals(component);
		}

		// Token: 0x06002A2F RID: 10799 RVA: 0x000DE790 File Offset: 0x000DC990
		private void OnGasTankToggleInternals(EntityUid uid, GasTankComponent component, GasTankToggleInternalsMessage args)
		{
			IPlayerSession playerSession = args.Session as IPlayerSession;
			if (playerSession != null)
			{
				EntityUid? attachedEntity = playerSession.AttachedEntity;
				if (attachedEntity != null)
				{
					attachedEntity.GetValueOrDefault();
					this.ToggleInternals(component);
					return;
				}
			}
		}

		// Token: 0x06002A30 RID: 10800 RVA: 0x000DE7CE File Offset: 0x000DC9CE
		private void OnGasTankSetPressure(EntityUid uid, GasTankComponent component, GasTankSetPressureMessage args)
		{
			component.OutputPressure = args.Pressure;
		}

		// Token: 0x06002A31 RID: 10801 RVA: 0x000DE7DC File Offset: 0x000DC9DC
		public void UpdateUserInterface(GasTankComponent component, bool initialUpdate = false)
		{
			this.GetInternalsComponent(component, null);
			UserInterfaceSystem ui = this._ui;
			EntityUid owner = component.Owner;
			Enum @enum = SharedGasTankUiKey.Key;
			GasTankBoundUserInterfaceState gasTankBoundUserInterfaceState = new GasTankBoundUserInterfaceState();
			GasMixture air = component.Air;
			gasTankBoundUserInterfaceState.TankPressure = ((air != null) ? air.Pressure : 0f);
			gasTankBoundUserInterfaceState.OutputPressure = (initialUpdate ? new float?(component.OutputPressure) : null);
			gasTankBoundUserInterfaceState.InternalsConnected = component.IsConnected;
			gasTankBoundUserInterfaceState.CanConnectInternals = this.CanConnectToInternals(component);
			ui.TrySetUiState(owner, @enum, gasTankBoundUserInterfaceState, null, null, true);
		}

		// Token: 0x06002A32 RID: 10802 RVA: 0x000DE86E File Offset: 0x000DCA6E
		private void BeforeUiOpen(EntityUid uid, GasTankComponent component, BeforeActivatableUIOpenEvent args)
		{
			this.UpdateUserInterface(component, true);
		}

		// Token: 0x06002A33 RID: 10803 RVA: 0x000DE878 File Offset: 0x000DCA78
		private void OnParentChange(EntityUid uid, GasTankComponent component, ref EntParentChangedMessage args)
		{
			component.CheckUser = true;
		}

		// Token: 0x06002A34 RID: 10804 RVA: 0x000DE881 File Offset: 0x000DCA81
		private void OnGetActions(EntityUid uid, GasTankComponent component, GetItemActionsEvent args)
		{
			args.Actions.Add(component.ToggleAction);
		}

		// Token: 0x06002A35 RID: 10805 RVA: 0x000DE898 File Offset: 0x000DCA98
		private void OnExamined(EntityUid uid, GasTankComponent component, ExaminedEvent args)
		{
			if (args.IsInDetailsRange)
			{
				string text = "comp-gas-tank-examine";
				ValueTuple<string, object>[] array = new ValueTuple<string, object>[1];
				int num = 0;
				string item = "pressure";
				GasMixture air = component.Air;
				array[num] = new ValueTuple<string, object>(item, Math.Round((double)((air != null) ? air.Pressure : 0f)));
				args.PushMarkup(Loc.GetString(text, array));
			}
			if (component.IsConnected)
			{
				args.PushMarkup(Loc.GetString("comp-gas-tank-connected"));
			}
		}

		// Token: 0x06002A36 RID: 10806 RVA: 0x000DE90D File Offset: 0x000DCB0D
		private void OnActionToggle(EntityUid uid, GasTankComponent component, ToggleActionEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			this.ToggleInternals(component);
			args.Handled = true;
		}

		// Token: 0x06002A37 RID: 10807 RVA: 0x000DE928 File Offset: 0x000DCB28
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			this._timer += frameTime;
			if (this._timer < 0.5f)
			{
				return;
			}
			this._timer -= 0.5f;
			foreach (GasTankComponent gasTank in this.EntityManager.EntityQuery<GasTankComponent>(false))
			{
				if (gasTank.CheckUser)
				{
					gasTank.CheckUser = false;
					if (base.Transform(gasTank.Owner).ParentUid != gasTank.User)
					{
						this.DisconnectFromInternals(gasTank);
						continue;
					}
				}
				this._atmosphereSystem.React(gasTank.Air, gasTank);
				this.CheckStatus(gasTank);
				if (this._ui.IsUiOpen(gasTank.Owner, SharedGasTankUiKey.Key, null))
				{
					this.UpdateUserInterface(gasTank, false);
				}
			}
		}

		// Token: 0x06002A38 RID: 10808 RVA: 0x000DEA38 File Offset: 0x000DCC38
		private void ToggleInternals(GasTankComponent component)
		{
			if (component.IsConnected)
			{
				this.DisconnectFromInternals(component);
				return;
			}
			this.ConnectToInternals(component);
		}

		// Token: 0x06002A39 RID: 10809 RVA: 0x000DEA51 File Offset: 0x000DCC51
		[return: Nullable(2)]
		public GasMixture RemoveAir(GasTankComponent component, float amount)
		{
			GasMixture air = component.Air;
			GasMixture result = (air != null) ? air.Remove(amount) : null;
			this.CheckStatus(component);
			return result;
		}

		// Token: 0x06002A3A RID: 10810 RVA: 0x000DEA70 File Offset: 0x000DCC70
		public GasMixture RemoveAirVolume(GasTankComponent component, float volume)
		{
			if (component.Air == null)
			{
				return new GasMixture(volume);
			}
			float molesNeeded = component.OutputPressure * volume / (8.314463f * component.Air.Temperature);
			GasMixture air = this.RemoveAir(component, molesNeeded);
			if (air != null)
			{
				air.Volume = volume;
				return air;
			}
			return new GasMixture(volume);
		}

		// Token: 0x06002A3B RID: 10811 RVA: 0x000DEAC4 File Offset: 0x000DCCC4
		public bool CanConnectToInternals(GasTankComponent component)
		{
			InternalsComponent internals = this.GetInternalsComponent(component, null);
			return internals != null && internals.BreathToolEntity != null;
		}

		// Token: 0x06002A3C RID: 10812 RVA: 0x000DEAF8 File Offset: 0x000DCCF8
		public void ConnectToInternals(GasTankComponent component)
		{
			if (component.IsConnected || !this.CanConnectToInternals(component))
			{
				return;
			}
			InternalsComponent internals = this.GetInternalsComponent(component, null);
			if (internals == null)
			{
				return;
			}
			if (this._internals.TryConnectTank(internals, component.Owner))
			{
				component.User = new EntityUid?(internals.Owner);
			}
			this._actions.SetToggled(component.ToggleAction, component.IsConnected);
			if (!component.IsConnected)
			{
				return;
			}
			IPlayingAudioStream connectStream = component.ConnectStream;
			if (connectStream != null)
			{
				connectStream.Stop();
			}
			if (component.ConnectSound != null)
			{
				component.ConnectStream = this._audioSys.PlayPvs(component.ConnectSound, component.Owner, null);
			}
			this.UpdateUserInterface(component, false);
		}

		// Token: 0x06002A3D RID: 10813 RVA: 0x000DEBB8 File Offset: 0x000DCDB8
		public void DisconnectFromInternals(GasTankComponent component)
		{
			if (component.User == null)
			{
				return;
			}
			InternalsComponent internals = this.GetInternalsComponent(component, null);
			component.User = null;
			this._actions.SetToggled(component.ToggleAction, false);
			this._internals.DisconnectTank(internals);
			IPlayingAudioStream disconnectStream = component.DisconnectStream;
			if (disconnectStream != null)
			{
				disconnectStream.Stop();
			}
			if (component.DisconnectSound != null)
			{
				component.DisconnectStream = this._audioSys.PlayPvs(component.DisconnectSound, component.Owner, null);
			}
			this.UpdateUserInterface(component, false);
		}

		// Token: 0x06002A3E RID: 10814 RVA: 0x000DEC58 File Offset: 0x000DCE58
		[return: Nullable(2)]
		private InternalsComponent GetInternalsComponent(GasTankComponent component, EntityUid? owner = null)
		{
			EntityUid? entityUid = owner;
			if (entityUid == null)
			{
				owner = component.User;
			}
			if (base.Deleted(component.Owner, null))
			{
				return null;
			}
			if (owner != null)
			{
				return base.CompOrNull<InternalsComponent>(owner.Value);
			}
			IContainer container;
			if (!this._containers.TryGetContainingContainer(component.Owner, ref container, null, null))
			{
				return null;
			}
			return base.CompOrNull<InternalsComponent>(container.Owner);
		}

		// Token: 0x06002A3F RID: 10815 RVA: 0x000DECC5 File Offset: 0x000DCEC5
		public void AssumeAir(GasTankComponent component, GasMixture giver)
		{
			this._atmosphereSystem.Merge(component.Air, giver);
			this.CheckStatus(component);
		}

		// Token: 0x06002A40 RID: 10816 RVA: 0x000DECE0 File Offset: 0x000DCEE0
		public void CheckStatus(GasTankComponent component)
		{
			if (component.Air == null)
			{
				return;
			}
			float pressure = component.Air.Pressure;
			if (pressure > component.TankFragmentPressure)
			{
				for (int i = 0; i < 6; i++)
				{
					this._atmosphereSystem.React(component.Air, component);
				}
				pressure = component.Air.Pressure;
				float range = (pressure - component.TankFragmentPressure) / component.TankFragmentScale;
				if (range > 80f)
				{
					range = 80f;
				}
				ExplosionSystem explosions = this._explosions;
				EntityUid owner = component.Owner;
				ExplosiveComponent explosive = null;
				bool delete = true;
				float? radius = new float?(range);
				explosions.TriggerExplosive(owner, explosive, delete, null, radius, null);
				return;
			}
			if (pressure > component.TankRupturePressure)
			{
				if (component.Integrity <= 0)
				{
					GasMixture environment = this._atmosphereSystem.GetContainingMixture(component.Owner, false, true, null);
					if (environment != null)
					{
						this._atmosphereSystem.Merge(environment, component.Air);
					}
					this._audioSys.Play(component.RuptureSound, Filter.Pvs(component.Owner, 2f, null, null, null), base.Transform(component.Owner).Coordinates, true, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.125f))));
					base.QueueDel(component.Owner);
					return;
				}
				component.Integrity--;
				return;
			}
			else
			{
				if (pressure <= component.TankLeakPressure)
				{
					if (component.Integrity < 3)
					{
						component.Integrity++;
					}
					return;
				}
				if (component.Integrity > 0)
				{
					component.Integrity--;
					return;
				}
				GasMixture environment2 = this._atmosphereSystem.GetContainingMixture(component.Owner, false, true, null);
				if (environment2 == null)
				{
					return;
				}
				GasMixture leakedGas = component.Air.RemoveRatio(0.25f);
				this._atmosphereSystem.Merge(environment2, leakedGas);
				return;
			}
		}

		// Token: 0x06002A41 RID: 10817 RVA: 0x000DEEAB File Offset: 0x000DD0AB
		private void OnAnalyzed(EntityUid uid, GasTankComponent component, GasAnalyzerScanEvent args)
		{
			args.GasMixtures = new Dictionary<string, GasMixture>
			{
				{
					base.Name(uid, null),
					component.Air
				}
			};
		}

		// Token: 0x06002A42 RID: 10818 RVA: 0x000DEECC File Offset: 0x000DD0CC
		private void OnGasTankPrice(EntityUid uid, GasTankComponent component, ref PriceCalculationEvent args)
		{
			args.Price += this._atmosphereSystem.GetPrice(component.Air);
		}

		// Token: 0x04001A13 RID: 6675
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x04001A14 RID: 6676
		[Dependency]
		private readonly ExplosionSystem _explosions;

		// Token: 0x04001A15 RID: 6677
		[Dependency]
		private readonly InternalsSystem _internals;

		// Token: 0x04001A16 RID: 6678
		[Dependency]
		private readonly SharedAudioSystem _audioSys;

		// Token: 0x04001A17 RID: 6679
		[Dependency]
		private readonly SharedContainerSystem _containers;

		// Token: 0x04001A18 RID: 6680
		[Dependency]
		private readonly SharedActionsSystem _actions;

		// Token: 0x04001A19 RID: 6681
		[Dependency]
		private readonly UserInterfaceSystem _ui;

		// Token: 0x04001A1A RID: 6682
		private const float TimerDelay = 0.5f;

		// Token: 0x04001A1B RID: 6683
		private float _timer;
	}
}
