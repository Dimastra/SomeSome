using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Binary.Components;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Chat.Managers;
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

namespace Content.Server.Atmos.Piping.Binary.EntitySystems
{
	// Token: 0x02000769 RID: 1897
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasPressurePumpSystem : EntitySystem
	{
		// Token: 0x06002824 RID: 10276 RVA: 0x000D2170 File Offset: 0x000D0370
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GasPressurePumpComponent, ComponentInit>(new ComponentEventHandler<GasPressurePumpComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<GasPressurePumpComponent, AtmosDeviceUpdateEvent>(new ComponentEventHandler<GasPressurePumpComponent, AtmosDeviceUpdateEvent>(this.OnPumpUpdated), null, null);
			base.SubscribeLocalEvent<GasPressurePumpComponent, AtmosDeviceDisabledEvent>(new ComponentEventHandler<GasPressurePumpComponent, AtmosDeviceDisabledEvent>(this.OnPumpLeaveAtmosphere), null, null);
			base.SubscribeLocalEvent<GasPressurePumpComponent, ExaminedEvent>(new ComponentEventHandler<GasPressurePumpComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<GasPressurePumpComponent, InteractHandEvent>(new ComponentEventHandler<GasPressurePumpComponent, InteractHandEvent>(this.OnPumpInteractHand), null, null);
			base.SubscribeLocalEvent<GasPressurePumpComponent, GasPressurePumpChangeOutputPressureMessage>(new ComponentEventHandler<GasPressurePumpComponent, GasPressurePumpChangeOutputPressureMessage>(this.OnOutputPressureChangeMessage), null, null);
			base.SubscribeLocalEvent<GasPressurePumpComponent, GasPressurePumpToggleStatusMessage>(new ComponentEventHandler<GasPressurePumpComponent, GasPressurePumpToggleStatusMessage>(this.OnToggleStatusMessage), null, null);
		}

		// Token: 0x06002825 RID: 10277 RVA: 0x000D220F File Offset: 0x000D040F
		private void OnInit(EntityUid uid, GasPressurePumpComponent pump, ComponentInit args)
		{
			this.UpdateAppearance(uid, pump, null);
		}

		// Token: 0x06002826 RID: 10278 RVA: 0x000D221C File Offset: 0x000D041C
		private void OnExamined(EntityUid uid, GasPressurePumpComponent pump, ExaminedEvent args)
		{
			if (!this.EntityManager.GetComponent<TransformComponent>(pump.Owner).Anchored || !args.IsInDetailsRange)
			{
				return;
			}
			string str;
			if (Loc.TryGetString("gas-pressure-pump-system-examined", ref str, new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("statusColor", "lightblue"),
				new ValueTuple<string, object>("pressure", pump.TargetPressure)
			}))
			{
				args.PushMarkup(str);
			}
		}

		// Token: 0x06002827 RID: 10279 RVA: 0x000D2298 File Offset: 0x000D0498
		private void OnPumpUpdated(EntityUid uid, GasPressurePumpComponent pump, AtmosDeviceUpdateEvent args)
		{
			NodeContainerComponent nodeContainer;
			PipeNode inlet;
			PipeNode outlet;
			if (!pump.Enabled || !this.EntityManager.TryGetComponent<NodeContainerComponent>(uid, ref nodeContainer) || !nodeContainer.TryGetNode<PipeNode>(pump.InletName, out inlet) || !nodeContainer.TryGetNode<PipeNode>(pump.OutletName, out outlet))
			{
				this._ambientSoundSystem.SetAmbience(pump.Owner, false, null);
				return;
			}
			float outputStartingPressure = outlet.Air.Pressure;
			if (outputStartingPressure >= pump.TargetPressure)
			{
				this._ambientSoundSystem.SetAmbience(pump.Owner, false, null);
				return;
			}
			if (inlet.Air.TotalMoles > 0f && inlet.Air.Temperature > 0f)
			{
				float transferMoles = (pump.TargetPressure - outputStartingPressure) * outlet.Air.Volume / (inlet.Air.Temperature * 8.314463f);
				GasMixture removed = inlet.Air.Remove(transferMoles);
				this._atmosphereSystem.Merge(outlet.Air, removed);
				this._ambientSoundSystem.SetAmbience(pump.Owner, removed.TotalMoles > 0f, null);
			}
		}

		// Token: 0x06002828 RID: 10280 RVA: 0x000D23A8 File Offset: 0x000D05A8
		private void OnPumpLeaveAtmosphere(EntityUid uid, GasPressurePumpComponent pump, AtmosDeviceDisabledEvent args)
		{
			pump.Enabled = false;
			this.UpdateAppearance(uid, pump, null);
			this.DirtyUI(uid, pump);
			this._userInterfaceSystem.TryCloseAll(uid, GasPressurePumpUiKey.Key, null);
		}

		// Token: 0x06002829 RID: 10281 RVA: 0x000D23D8 File Offset: 0x000D05D8
		private void OnPumpInteractHand(EntityUid uid, GasPressurePumpComponent pump, InteractHandEvent args)
		{
			ActorComponent actor;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			if (this.EntityManager.GetComponent<TransformComponent>(pump.Owner).Anchored)
			{
				this._userInterfaceSystem.TryOpen(uid, GasPressurePumpUiKey.Key, actor.PlayerSession, null);
				this.DirtyUI(uid, pump);
			}
			else
			{
				args.User.PopupMessageCursor(Loc.GetString("comp-gas-pump-ui-needs-anchor"));
			}
			args.Handled = true;
		}

		// Token: 0x0600282A RID: 10282 RVA: 0x000D2454 File Offset: 0x000D0654
		private void OnToggleStatusMessage(EntityUid uid, GasPressurePumpComponent pump, GasPressurePumpToggleStatusMessage args)
		{
			pump.Enabled = args.Enabled;
			EntityUid player = args.Session.AttachedEntity.Value;
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.AtmosPowerChanged;
			LogImpact impact = LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(22, 3);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(player), "player", "ToPrettyString(player)");
			logStringHandler.AppendLiteral(" set the power on ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "device", "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" to ");
			logStringHandler.AppendFormatted<bool>(args.Enabled, "args.Enabled");
			adminLogger.Add(type, impact, ref logStringHandler);
			if (this._entityManager.GetComponent<MetaDataComponent>(uid).EntityName == "plasma pump" && args.Enabled)
			{
				this._chatManager.SendAdminAnnouncement(Loc.GetString("admin-chatalert-plasma-pump-enabled", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("pump", base.ToPrettyString(uid)),
					new ValueTuple<string, object>("player", base.ToPrettyString(player))
				}));
			}
			this.DirtyUI(uid, pump);
			this.UpdateAppearance(uid, pump, null);
		}

		// Token: 0x0600282B RID: 10283 RVA: 0x000D2580 File Offset: 0x000D0780
		private void OnOutputPressureChangeMessage(EntityUid uid, GasPressurePumpComponent pump, GasPressurePumpChangeOutputPressureMessage args)
		{
			pump.TargetPressure = Math.Clamp(args.Pressure, 0f, 4500f);
			EntityUid player = args.Session.AttachedEntity.Value;
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.AtmosPressureChanged;
			LogImpact impact = LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(28, 3);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(player), "player", "ToPrettyString(player)");
			logStringHandler.AppendLiteral(" set the pressure on ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "device", "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" to ");
			logStringHandler.AppendFormatted<float>(args.Pressure, "args.Pressure");
			logStringHandler.AppendLiteral("kPa");
			adminLogger.Add(type, impact, ref logStringHandler);
			if (this._entityManager.GetComponent<MetaDataComponent>(uid).EntityName == "plasma pump")
			{
				this._chatManager.SendAdminAnnouncement(Loc.GetString("admin-chatalert-plasma-pump-pressure-change", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("pump", base.ToPrettyString(uid)),
					new ValueTuple<string, object>("player", base.ToPrettyString(player)),
					new ValueTuple<string, object>("pressure", args.Pressure)
				}));
			}
			this.DirtyUI(uid, pump);
		}

		// Token: 0x0600282C RID: 10284 RVA: 0x000D26D4 File Offset: 0x000D08D4
		[NullableContext(2)]
		private void DirtyUI(EntityUid uid, GasPressurePumpComponent pump)
		{
			if (!base.Resolve<GasPressurePumpComponent>(uid, ref pump, true))
			{
				return;
			}
			this._userInterfaceSystem.TrySetUiState(uid, GasPressurePumpUiKey.Key, new GasPressurePumpBoundUserInterfaceState(this.EntityManager.GetComponent<MetaDataComponent>(pump.Owner).EntityName, pump.TargetPressure, pump.Enabled), null, null, true);
		}

		// Token: 0x0600282D RID: 10285 RVA: 0x000D272B File Offset: 0x000D092B
		[NullableContext(2)]
		private void UpdateAppearance(EntityUid uid, GasPressurePumpComponent pump = null, AppearanceComponent appearance = null)
		{
			if (!base.Resolve<GasPressurePumpComponent, AppearanceComponent>(uid, ref pump, ref appearance, false))
			{
				return;
			}
			this._appearance.SetData(uid, PumpVisuals.Enabled, pump.Enabled, appearance);
		}

		// Token: 0x040018F6 RID: 6390
		[Dependency]
		private readonly UserInterfaceSystem _userInterfaceSystem;

		// Token: 0x040018F7 RID: 6391
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x040018F8 RID: 6392
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x040018F9 RID: 6393
		[Dependency]
		private readonly SharedAmbientSoundSystem _ambientSoundSystem;

		// Token: 0x040018FA RID: 6394
		[Dependency]
		private readonly EntityManager _entityManager;

		// Token: 0x040018FB RID: 6395
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x040018FC RID: 6396
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
