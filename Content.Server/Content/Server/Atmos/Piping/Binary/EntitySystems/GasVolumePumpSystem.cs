using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Binary.Components;
using Content.Server.Atmos.Piping.Components;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.Nodes;
using Content.Shared.Administration.Logs;
using Content.Shared.Atmos.Piping;
using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.Audio;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Server.Atmos.Piping.Binary.EntitySystems
{
	// Token: 0x0200076C RID: 1900
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasVolumePumpSystem : EntitySystem
	{
		// Token: 0x06002840 RID: 10304 RVA: 0x000D2D1C File Offset: 0x000D0F1C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GasVolumePumpComponent, ComponentInit>(new ComponentEventHandler<GasVolumePumpComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<GasVolumePumpComponent, AtmosDeviceUpdateEvent>(new ComponentEventHandler<GasVolumePumpComponent, AtmosDeviceUpdateEvent>(this.OnVolumePumpUpdated), null, null);
			base.SubscribeLocalEvent<GasVolumePumpComponent, AtmosDeviceDisabledEvent>(new ComponentEventHandler<GasVolumePumpComponent, AtmosDeviceDisabledEvent>(this.OnVolumePumpLeaveAtmosphere), null, null);
			base.SubscribeLocalEvent<GasVolumePumpComponent, ExaminedEvent>(new ComponentEventHandler<GasVolumePumpComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<GasVolumePumpComponent, InteractHandEvent>(new ComponentEventHandler<GasVolumePumpComponent, InteractHandEvent>(this.OnPumpInteractHand), null, null);
			base.SubscribeLocalEvent<GasVolumePumpComponent, GasVolumePumpChangeTransferRateMessage>(new ComponentEventHandler<GasVolumePumpComponent, GasVolumePumpChangeTransferRateMessage>(this.OnTransferRateChangeMessage), null, null);
			base.SubscribeLocalEvent<GasVolumePumpComponent, GasVolumePumpToggleStatusMessage>(new ComponentEventHandler<GasVolumePumpComponent, GasVolumePumpToggleStatusMessage>(this.OnToggleStatusMessage), null, null);
		}

		// Token: 0x06002841 RID: 10305 RVA: 0x000D2DBB File Offset: 0x000D0FBB
		private void OnInit(EntityUid uid, GasVolumePumpComponent pump, ComponentInit args)
		{
			this.UpdateAppearance(uid, pump, null);
		}

		// Token: 0x06002842 RID: 10306 RVA: 0x000D2DC8 File Offset: 0x000D0FC8
		private void OnExamined(EntityUid uid, GasVolumePumpComponent pump, ExaminedEvent args)
		{
			if (!this.EntityManager.GetComponent<TransformComponent>(pump.Owner).Anchored || !args.IsInDetailsRange)
			{
				return;
			}
			string str;
			if (Loc.TryGetString("gas-volume-pump-system-examined", ref str, new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("statusColor", "lightblue"),
				new ValueTuple<string, object>("rate", pump.TransferRate)
			}))
			{
				args.PushMarkup(str);
			}
		}

		// Token: 0x06002843 RID: 10307 RVA: 0x000D2E44 File Offset: 0x000D1044
		private void OnVolumePumpUpdated(EntityUid uid, GasVolumePumpComponent pump, AtmosDeviceUpdateEvent args)
		{
			NodeContainerComponent nodeContainer;
			AtmosDeviceComponent device;
			PipeNode inlet;
			PipeNode outlet;
			if (!pump.Enabled || !base.TryComp<NodeContainerComponent>(uid, ref nodeContainer) || !base.TryComp<AtmosDeviceComponent>(uid, ref device) || !nodeContainer.TryGetNode<PipeNode>(pump.InletName, out inlet) || !nodeContainer.TryGetNode<PipeNode>(pump.OutletName, out outlet))
			{
				this._ambientSoundSystem.SetAmbience(uid, false, null);
				return;
			}
			float inputStartingPressure = inlet.Air.Pressure;
			float outputStartingPressure = outlet.Air.Pressure;
			if (inputStartingPressure < pump.LowerThreshold || (outputStartingPressure > pump.HigherThreshold && !pump.Overclocked))
			{
				return;
			}
			if (outputStartingPressure - inputStartingPressure > pump.OverclockThreshold && pump.Overclocked)
			{
				return;
			}
			GasMixture removed = inlet.Air.RemoveVolume((float)((double)pump.TransferRate * (this._gameTiming.CurTime - device.LastProcess).TotalSeconds));
			if (pump.Overclocked)
			{
				TransformComponent transform = base.Transform(uid);
				Vector2i indices = this._transformSystem.GetGridOrMapTilePosition(uid, transform);
				GasMixture tile = this._atmosphereSystem.GetTileMixture(transform.GridUid, null, indices, true);
				if (tile != null)
				{
					GasMixture leaked = removed.RemoveRatio(pump.LeakRatio);
					this._atmosphereSystem.Merge(tile, leaked);
				}
			}
			this._atmosphereSystem.Merge(outlet.Air, removed);
			this._ambientSoundSystem.SetAmbience(uid, removed.TotalMoles > 0f, null);
		}

		// Token: 0x06002844 RID: 10308 RVA: 0x000D2FB2 File Offset: 0x000D11B2
		private void OnVolumePumpLeaveAtmosphere(EntityUid uid, GasVolumePumpComponent pump, AtmosDeviceDisabledEvent args)
		{
			pump.Enabled = false;
			this.UpdateAppearance(uid, pump, null);
			this.DirtyUI(uid, pump);
			this._userInterfaceSystem.TryCloseAll(uid, GasVolumePumpUiKey.Key, null);
		}

		// Token: 0x06002845 RID: 10309 RVA: 0x000D2FE0 File Offset: 0x000D11E0
		private void OnPumpInteractHand(EntityUid uid, GasVolumePumpComponent pump, InteractHandEvent args)
		{
			ActorComponent actor;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			if (this.EntityManager.GetComponent<TransformComponent>(pump.Owner).Anchored)
			{
				this._userInterfaceSystem.TryOpen(uid, GasVolumePumpUiKey.Key, actor.PlayerSession, null);
				this.DirtyUI(uid, pump);
			}
			else
			{
				args.User.PopupMessageCursor(Loc.GetString("comp-gas-pump-ui-needs-anchor"));
			}
			args.Handled = true;
		}

		// Token: 0x06002846 RID: 10310 RVA: 0x000D305C File Offset: 0x000D125C
		private void OnToggleStatusMessage(EntityUid uid, GasVolumePumpComponent pump, GasVolumePumpToggleStatusMessage args)
		{
			pump.Enabled = args.Enabled;
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.AtmosPowerChanged;
			LogImpact impact = LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(22, 3);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Session.AttachedEntity.Value), "player", "ToPrettyString(args.Session.AttachedEntity!.Value)");
			logStringHandler.AppendLiteral(" set the power on ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "device", "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" to ");
			logStringHandler.AppendFormatted<bool>(args.Enabled, "args.Enabled");
			adminLogger.Add(type, impact, ref logStringHandler);
			this.DirtyUI(uid, pump);
			this.UpdateAppearance(uid, pump, null);
		}

		// Token: 0x06002847 RID: 10311 RVA: 0x000D310C File Offset: 0x000D130C
		private void OnTransferRateChangeMessage(EntityUid uid, GasVolumePumpComponent pump, GasVolumePumpChangeTransferRateMessage args)
		{
			pump.TransferRate = Math.Clamp(args.TransferRate, 0f, pump.MaxTransferRate);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.AtmosVolumeChanged;
			LogImpact impact = LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(30, 3);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Session.AttachedEntity.Value), "player", "ToPrettyString(args.Session.AttachedEntity!.Value)");
			logStringHandler.AppendLiteral(" set the transfer rate on ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "device", "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" to ");
			logStringHandler.AppendFormatted<float>(args.TransferRate, "args.TransferRate");
			adminLogger.Add(type, impact, ref logStringHandler);
			this.DirtyUI(uid, pump);
		}

		// Token: 0x06002848 RID: 10312 RVA: 0x000D31C4 File Offset: 0x000D13C4
		[NullableContext(2)]
		private void DirtyUI(EntityUid uid, GasVolumePumpComponent pump)
		{
			if (!base.Resolve<GasVolumePumpComponent>(uid, ref pump, true))
			{
				return;
			}
			this._userInterfaceSystem.TrySetUiState(uid, GasVolumePumpUiKey.Key, new GasVolumePumpBoundUserInterfaceState(this.EntityManager.GetComponent<MetaDataComponent>(pump.Owner).EntityName, pump.TransferRate, pump.Enabled), null, null, true);
		}

		// Token: 0x06002849 RID: 10313 RVA: 0x000D321B File Offset: 0x000D141B
		[NullableContext(2)]
		private void UpdateAppearance(EntityUid uid, GasVolumePumpComponent pump = null, AppearanceComponent appearance = null)
		{
			if (!base.Resolve<GasVolumePumpComponent, AppearanceComponent>(uid, ref pump, ref appearance, false))
			{
				return;
			}
			this._appearance.SetData(uid, PumpVisuals.Enabled, pump.Enabled, appearance);
		}

		// Token: 0x04001902 RID: 6402
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04001903 RID: 6403
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x04001904 RID: 6404
		[Dependency]
		private readonly TransformSystem _transformSystem;

		// Token: 0x04001905 RID: 6405
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x04001906 RID: 6406
		[Dependency]
		private readonly UserInterfaceSystem _userInterfaceSystem;

		// Token: 0x04001907 RID: 6407
		[Dependency]
		private readonly SharedAmbientSoundSystem _ambientSoundSystem;

		// Token: 0x04001908 RID: 6408
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
