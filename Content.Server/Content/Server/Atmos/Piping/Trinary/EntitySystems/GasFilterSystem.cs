using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Atmos.Piping.Trinary.Components;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.Nodes;
using Content.Shared.Administration.Logs;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Piping;
using Content.Shared.Atmos.Piping.Trinary.Components;
using Content.Shared.Audio;
using Content.Shared.Database;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Server.Atmos.Piping.Trinary.EntitySystems
{
	// Token: 0x02000756 RID: 1878
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasFilterSystem : EntitySystem
	{
		// Token: 0x060027A3 RID: 10147 RVA: 0x000D0044 File Offset: 0x000CE244
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GasFilterComponent, ComponentInit>(new ComponentEventHandler<GasFilterComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<GasFilterComponent, AtmosDeviceUpdateEvent>(new ComponentEventHandler<GasFilterComponent, AtmosDeviceUpdateEvent>(this.OnFilterUpdated), null, null);
			base.SubscribeLocalEvent<GasFilterComponent, AtmosDeviceDisabledEvent>(new ComponentEventHandler<GasFilterComponent, AtmosDeviceDisabledEvent>(this.OnFilterLeaveAtmosphere), null, null);
			base.SubscribeLocalEvent<GasFilterComponent, InteractHandEvent>(new ComponentEventHandler<GasFilterComponent, InteractHandEvent>(this.OnFilterInteractHand), null, null);
			base.SubscribeLocalEvent<GasFilterComponent, GasAnalyzerScanEvent>(new ComponentEventHandler<GasFilterComponent, GasAnalyzerScanEvent>(this.OnFilterAnalyzed), null, null);
			base.SubscribeLocalEvent<GasFilterComponent, GasFilterChangeRateMessage>(new ComponentEventHandler<GasFilterComponent, GasFilterChangeRateMessage>(this.OnTransferRateChangeMessage), null, null);
			base.SubscribeLocalEvent<GasFilterComponent, GasFilterSelectGasMessage>(new ComponentEventHandler<GasFilterComponent, GasFilterSelectGasMessage>(this.OnSelectGasMessage), null, null);
			base.SubscribeLocalEvent<GasFilterComponent, GasFilterToggleStatusMessage>(new ComponentEventHandler<GasFilterComponent, GasFilterToggleStatusMessage>(this.OnToggleStatusMessage), null, null);
		}

		// Token: 0x060027A4 RID: 10148 RVA: 0x000D00F7 File Offset: 0x000CE2F7
		private void OnInit(EntityUid uid, GasFilterComponent filter, ComponentInit args)
		{
			this.UpdateAppearance(uid, filter);
		}

		// Token: 0x060027A5 RID: 10149 RVA: 0x000D0104 File Offset: 0x000CE304
		private void OnFilterUpdated(EntityUid uid, GasFilterComponent filter, AtmosDeviceUpdateEvent args)
		{
			NodeContainerComponent nodeContainer;
			AtmosDeviceComponent device;
			PipeNode inletNode;
			PipeNode filterNode;
			PipeNode outletNode;
			if (!filter.Enabled || !this.EntityManager.TryGetComponent<NodeContainerComponent>(uid, ref nodeContainer) || !this.EntityManager.TryGetComponent<AtmosDeviceComponent>(uid, ref device) || !nodeContainer.TryGetNode<PipeNode>(filter.InletName, out inletNode) || !nodeContainer.TryGetNode<PipeNode>(filter.FilterName, out filterNode) || !nodeContainer.TryGetNode<PipeNode>(filter.OutletName, out outletNode) || outletNode.Air.Pressure >= 4500f)
			{
				this._ambientSoundSystem.SetAmbience(filter.Owner, false, null);
				return;
			}
			float transferVol = (float)((double)filter.TransferRate * (this._gameTiming.CurTime - device.LastProcess).TotalSeconds);
			if (transferVol <= 0f)
			{
				this._ambientSoundSystem.SetAmbience(filter.Owner, false, null);
				return;
			}
			GasMixture removed = inletNode.Air.RemoveVolume(transferVol);
			if (filter.FilteredGas != null)
			{
				GasMixture filteredOut = new GasMixture
				{
					Temperature = removed.Temperature
				};
				filteredOut.SetMoles(filter.FilteredGas.Value, removed.GetMoles(filter.FilteredGas.Value));
				removed.SetMoles(filter.FilteredGas.Value, 0f);
				PipeNode target = (filterNode.Air.Pressure < 4500f) ? filterNode : inletNode;
				this._atmosphereSystem.Merge(target.Air, filteredOut);
				this._ambientSoundSystem.SetAmbience(filter.Owner, filteredOut.TotalMoles > 0f, null);
			}
			this._atmosphereSystem.Merge(outletNode.Air, removed);
		}

		// Token: 0x060027A6 RID: 10150 RVA: 0x000D02B5 File Offset: 0x000CE4B5
		private void OnFilterLeaveAtmosphere(EntityUid uid, GasFilterComponent filter, AtmosDeviceDisabledEvent args)
		{
			filter.Enabled = false;
			this.UpdateAppearance(uid, filter);
			this._ambientSoundSystem.SetAmbience(filter.Owner, false, null);
			this.DirtyUI(uid, filter);
			this._userInterfaceSystem.TryCloseAll(uid, GasFilterUiKey.Key, null);
		}

		// Token: 0x060027A7 RID: 10151 RVA: 0x000D02F8 File Offset: 0x000CE4F8
		private void OnFilterInteractHand(EntityUid uid, GasFilterComponent filter, InteractHandEvent args)
		{
			ActorComponent actor;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			if (this.EntityManager.GetComponent<TransformComponent>(filter.Owner).Anchored)
			{
				this._userInterfaceSystem.TryOpen(uid, GasFilterUiKey.Key, actor.PlayerSession, null);
				this.DirtyUI(uid, filter);
			}
			else
			{
				this._popupSystem.PopupCursor(Loc.GetString("comp-gas-filter-ui-needs-anchor"), args.User, PopupType.Small);
			}
			args.Handled = true;
		}

		// Token: 0x060027A8 RID: 10152 RVA: 0x000D037C File Offset: 0x000CE57C
		[NullableContext(2)]
		private void DirtyUI(EntityUid uid, GasFilterComponent filter)
		{
			if (!base.Resolve<GasFilterComponent>(uid, ref filter, true))
			{
				return;
			}
			this._userInterfaceSystem.TrySetUiState(uid, GasFilterUiKey.Key, new GasFilterBoundUserInterfaceState(this.EntityManager.GetComponent<MetaDataComponent>(filter.Owner).EntityName, filter.TransferRate, filter.Enabled, filter.FilteredGas), null, null, true);
		}

		// Token: 0x060027A9 RID: 10153 RVA: 0x000D03D9 File Offset: 0x000CE5D9
		[NullableContext(2)]
		private void UpdateAppearance(EntityUid uid, GasFilterComponent filter = null)
		{
			if (!base.Resolve<GasFilterComponent>(uid, ref filter, false))
			{
				return;
			}
			this._appearanceSystem.SetData(uid, FilterVisuals.Enabled, filter.Enabled, null);
		}

		// Token: 0x060027AA RID: 10154 RVA: 0x000D0408 File Offset: 0x000CE608
		private void OnToggleStatusMessage(EntityUid uid, GasFilterComponent filter, GasFilterToggleStatusMessage args)
		{
			filter.Enabled = args.Enabled;
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
			this.DirtyUI(uid, filter);
			this.UpdateAppearance(uid, filter);
		}

		// Token: 0x060027AB RID: 10155 RVA: 0x000D04B8 File Offset: 0x000CE6B8
		private void OnTransferRateChangeMessage(EntityUid uid, GasFilterComponent filter, GasFilterChangeRateMessage args)
		{
			filter.TransferRate = Math.Clamp(args.Rate, 0f, filter.MaxTransferRate);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.AtmosVolumeChanged;
			LogImpact impact = LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(30, 3);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Session.AttachedEntity.Value), "player", "ToPrettyString(args.Session.AttachedEntity!.Value)");
			logStringHandler.AppendLiteral(" set the transfer rate on ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "device", "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" to ");
			logStringHandler.AppendFormatted<float>(args.Rate, "args.Rate");
			adminLogger.Add(type, impact, ref logStringHandler);
			this.DirtyUI(uid, filter);
		}

		// Token: 0x060027AC RID: 10156 RVA: 0x000D0570 File Offset: 0x000CE770
		private void OnSelectGasMessage(EntityUid uid, GasFilterComponent filter, GasFilterSelectGasMessage args)
		{
			if (args.ID == null)
			{
				filter.FilteredGas = null;
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.AtmosFilterChanged;
				LogImpact impact = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(27, 2);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Session.AttachedEntity.Value), "player", "ToPrettyString(args.Session.AttachedEntity!.Value)");
				logStringHandler.AppendLiteral(" set the filter on ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "device", "ToPrettyString(uid)");
				logStringHandler.AppendLiteral(" to none");
				adminLogger.Add(type, impact, ref logStringHandler);
				this.DirtyUI(uid, filter);
				return;
			}
			Gas parsedGas;
			if (Enum.TryParse<Gas>(args.ID.ToString(), true, out parsedGas))
			{
				filter.FilteredGas = new Gas?(parsedGas);
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.AtmosFilterChanged;
				LogImpact impact2 = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(23, 3);
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Session.AttachedEntity.Value), "player", "ToPrettyString(args.Session.AttachedEntity!.Value)");
				logStringHandler.AppendLiteral(" set the filter on ");
				logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "device", "ToPrettyString(uid)");
				logStringHandler.AppendLiteral(" to ");
				logStringHandler.AppendFormatted(parsedGas.ToString());
				adminLogger2.Add(type2, impact2, ref logStringHandler);
				this.DirtyUI(uid, filter);
				return;
			}
			string text = "atmos";
			object[] array = new object[1];
			int num = 0;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(56, 2);
			defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid));
			defaultInterpolatedStringHandler.AppendLiteral(" received GasFilterSelectGasMessage with an invalid ID: ");
			defaultInterpolatedStringHandler.AppendFormatted<int?>(args.ID);
			array[num] = defaultInterpolatedStringHandler.ToStringAndClear();
			Logger.Warning(text, array);
		}

		// Token: 0x060027AD RID: 10157 RVA: 0x000D0728 File Offset: 0x000CE928
		private void OnFilterAnalyzed(EntityUid uid, GasFilterComponent component, GasAnalyzerScanEvent args)
		{
			NodeContainerComponent nodeContainer;
			if (!this.EntityManager.TryGetComponent<NodeContainerComponent>(uid, ref nodeContainer))
			{
				return;
			}
			Dictionary<string, GasMixture> gasMixDict = new Dictionary<string, GasMixture>();
			PipeNode inlet;
			nodeContainer.TryGetNode<PipeNode>(component.InletName, out inlet);
			PipeNode filterNode;
			nodeContainer.TryGetNode<PipeNode>(component.FilterName, out filterNode);
			if (inlet != null)
			{
				gasMixDict.Add(Loc.GetString("gas-analyzer-window-text-inlet"), inlet.Air);
			}
			if (filterNode != null)
			{
				gasMixDict.Add(Loc.GetString("gas-analyzer-window-text-filter"), filterNode.Air);
			}
			PipeNode outlet;
			if (nodeContainer.TryGetNode<PipeNode>(component.OutletName, out outlet))
			{
				gasMixDict.Add(Loc.GetString("gas-analyzer-window-text-outlet"), outlet.Air);
			}
			args.GasMixtures = gasMixDict;
			args.DeviceFlipped = (inlet != null && filterNode != null && inlet.CurrentPipeDirection.ToDirection() == DirectionExtensions.GetClockwise90Degrees(filterNode.CurrentPipeDirection.ToDirection()));
		}

		// Token: 0x040018B5 RID: 6325
		[Dependency]
		private IGameTiming _gameTiming;

		// Token: 0x040018B6 RID: 6326
		[Dependency]
		private UserInterfaceSystem _userInterfaceSystem;

		// Token: 0x040018B7 RID: 6327
		[Dependency]
		private IAdminLogManager _adminLogger;

		// Token: 0x040018B8 RID: 6328
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x040018B9 RID: 6329
		[Dependency]
		private readonly SharedAmbientSoundSystem _ambientSoundSystem;

		// Token: 0x040018BA RID: 6330
		[Dependency]
		private readonly SharedAppearanceSystem _appearanceSystem;

		// Token: 0x040018BB RID: 6331
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;
	}
}
