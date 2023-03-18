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
using Robust.Shared.Maths;

namespace Content.Server.Atmos.Piping.Trinary.EntitySystems
{
	// Token: 0x02000757 RID: 1879
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasMixerSystem : EntitySystem
	{
		// Token: 0x060027AF RID: 10159 RVA: 0x000D0800 File Offset: 0x000CEA00
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GasMixerComponent, ComponentInit>(new ComponentEventHandler<GasMixerComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<GasMixerComponent, AtmosDeviceUpdateEvent>(new ComponentEventHandler<GasMixerComponent, AtmosDeviceUpdateEvent>(this.OnMixerUpdated), null, null);
			base.SubscribeLocalEvent<GasMixerComponent, InteractHandEvent>(new ComponentEventHandler<GasMixerComponent, InteractHandEvent>(this.OnMixerInteractHand), null, null);
			base.SubscribeLocalEvent<GasMixerComponent, GasAnalyzerScanEvent>(new ComponentEventHandler<GasMixerComponent, GasAnalyzerScanEvent>(this.OnMixerAnalyzed), null, null);
			base.SubscribeLocalEvent<GasMixerComponent, GasMixerChangeOutputPressureMessage>(new ComponentEventHandler<GasMixerComponent, GasMixerChangeOutputPressureMessage>(this.OnOutputPressureChangeMessage), null, null);
			base.SubscribeLocalEvent<GasMixerComponent, GasMixerChangeNodePercentageMessage>(new ComponentEventHandler<GasMixerComponent, GasMixerChangeNodePercentageMessage>(this.OnChangeNodePercentageMessage), null, null);
			base.SubscribeLocalEvent<GasMixerComponent, GasMixerToggleStatusMessage>(new ComponentEventHandler<GasMixerComponent, GasMixerToggleStatusMessage>(this.OnToggleStatusMessage), null, null);
			base.SubscribeLocalEvent<GasMixerComponent, AtmosDeviceDisabledEvent>(new ComponentEventHandler<GasMixerComponent, AtmosDeviceDisabledEvent>(this.OnMixerLeaveAtmosphere), null, null);
		}

		// Token: 0x060027B0 RID: 10160 RVA: 0x000D08B3 File Offset: 0x000CEAB3
		private void OnInit(EntityUid uid, GasMixerComponent mixer, ComponentInit args)
		{
			this.UpdateAppearance(uid, mixer, null);
		}

		// Token: 0x060027B1 RID: 10161 RVA: 0x000D08C0 File Offset: 0x000CEAC0
		private void OnMixerUpdated(EntityUid uid, GasMixerComponent mixer, AtmosDeviceUpdateEvent args)
		{
			if (!mixer.Enabled)
			{
				this._ambientSoundSystem.SetAmbience(mixer.Owner, false, null);
				return;
			}
			NodeContainerComponent nodeContainer;
			if (!this.EntityManager.TryGetComponent<NodeContainerComponent>(uid, ref nodeContainer))
			{
				return;
			}
			PipeNode inletOne;
			PipeNode inletTwo;
			PipeNode outlet;
			if (!nodeContainer.TryGetNode<PipeNode>(mixer.InletOneName, out inletOne) || !nodeContainer.TryGetNode<PipeNode>(mixer.InletTwoName, out inletTwo) || !nodeContainer.TryGetNode<PipeNode>(mixer.OutletName, out outlet))
			{
				this._ambientSoundSystem.SetAmbience(mixer.Owner, false, null);
				return;
			}
			float outputStartingPressure = outlet.Air.Pressure;
			if (outputStartingPressure >= mixer.TargetPressure)
			{
				return;
			}
			float generalTransfer = (mixer.TargetPressure - outputStartingPressure) * outlet.Air.Volume / 8.314463f;
			float transferMolesOne = (inletOne.Air.Temperature > 0f) ? (mixer.InletOneConcentration * generalTransfer / inletOne.Air.Temperature) : 0f;
			float transferMolesTwo = (inletTwo.Air.Temperature > 0f) ? (mixer.InletTwoConcentration * generalTransfer / inletTwo.Air.Temperature) : 0f;
			if (mixer.InletTwoConcentration <= 0f)
			{
				if (inletOne.Air.Temperature <= 0f)
				{
					return;
				}
				transferMolesOne = MathF.Min(transferMolesOne, inletOne.Air.TotalMoles);
				transferMolesTwo = 0f;
			}
			else if (mixer.InletOneConcentration <= 0f)
			{
				if (inletTwo.Air.Temperature <= 0f)
				{
					return;
				}
				transferMolesOne = 0f;
				transferMolesTwo = MathF.Min(transferMolesTwo, inletTwo.Air.TotalMoles);
			}
			else
			{
				if (inletOne.Air.Temperature <= 0f || inletTwo.Air.Temperature <= 0f)
				{
					return;
				}
				if (transferMolesOne <= 0f || transferMolesTwo <= 0f)
				{
					this._ambientSoundSystem.SetAmbience(mixer.Owner, false, null);
					return;
				}
				if (inletOne.Air.TotalMoles < transferMolesOne || inletTwo.Air.TotalMoles < transferMolesTwo)
				{
					float ratio = MathF.Min(inletOne.Air.TotalMoles / transferMolesOne, inletTwo.Air.TotalMoles / transferMolesTwo);
					transferMolesOne *= ratio;
					transferMolesTwo *= ratio;
				}
			}
			bool transferred = false;
			if (transferMolesOne > 0f)
			{
				transferred = true;
				GasMixture removed = inletOne.Air.Remove(transferMolesOne);
				this._atmosphereSystem.Merge(outlet.Air, removed);
			}
			if (transferMolesTwo > 0f)
			{
				transferred = true;
				GasMixture removed2 = inletTwo.Air.Remove(transferMolesTwo);
				this._atmosphereSystem.Merge(outlet.Air, removed2);
			}
			if (transferred)
			{
				this._ambientSoundSystem.SetAmbience(mixer.Owner, true, null);
			}
		}

		// Token: 0x060027B2 RID: 10162 RVA: 0x000D0B63 File Offset: 0x000CED63
		private void OnMixerLeaveAtmosphere(EntityUid uid, GasMixerComponent mixer, AtmosDeviceDisabledEvent args)
		{
			mixer.Enabled = false;
			this.DirtyUI(uid, mixer);
			this.UpdateAppearance(uid, mixer, null);
			this._userInterfaceSystem.TryCloseAll(uid, GasFilterUiKey.Key, null);
		}

		// Token: 0x060027B3 RID: 10163 RVA: 0x000D0B94 File Offset: 0x000CED94
		private void OnMixerInteractHand(EntityUid uid, GasMixerComponent mixer, InteractHandEvent args)
		{
			ActorComponent actor;
			if (!this.EntityManager.TryGetComponent<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			if (this.EntityManager.GetComponent<TransformComponent>(mixer.Owner).Anchored)
			{
				this._userInterfaceSystem.TryOpen(uid, GasMixerUiKey.Key, actor.PlayerSession, null);
				this.DirtyUI(uid, mixer);
			}
			else
			{
				args.User.PopupMessageCursor(Loc.GetString("comp-gas-mixer-ui-needs-anchor"));
			}
			args.Handled = true;
		}

		// Token: 0x060027B4 RID: 10164 RVA: 0x000D0C10 File Offset: 0x000CEE10
		[NullableContext(2)]
		private void DirtyUI(EntityUid uid, GasMixerComponent mixer)
		{
			if (!base.Resolve<GasMixerComponent>(uid, ref mixer, true))
			{
				return;
			}
			this._userInterfaceSystem.TrySetUiState(uid, GasMixerUiKey.Key, new GasMixerBoundUserInterfaceState(this.EntityManager.GetComponent<MetaDataComponent>(mixer.Owner).EntityName, mixer.TargetPressure, mixer.Enabled, mixer.InletOneConcentration), null, null, true);
		}

		// Token: 0x060027B5 RID: 10165 RVA: 0x000D0C6D File Offset: 0x000CEE6D
		[NullableContext(2)]
		private void UpdateAppearance(EntityUid uid, GasMixerComponent mixer = null, AppearanceComponent appearance = null)
		{
			if (!base.Resolve<GasMixerComponent, AppearanceComponent>(uid, ref mixer, ref appearance, false))
			{
				return;
			}
			this._appearance.SetData(uid, FilterVisuals.Enabled, mixer.Enabled, appearance);
		}

		// Token: 0x060027B6 RID: 10166 RVA: 0x000D0C9C File Offset: 0x000CEE9C
		private void OnToggleStatusMessage(EntityUid uid, GasMixerComponent mixer, GasMixerToggleStatusMessage args)
		{
			mixer.Enabled = args.Enabled;
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
			this.DirtyUI(uid, mixer);
			this.UpdateAppearance(uid, mixer, null);
		}

		// Token: 0x060027B7 RID: 10167 RVA: 0x000D0D4C File Offset: 0x000CEF4C
		private void OnOutputPressureChangeMessage(EntityUid uid, GasMixerComponent mixer, GasMixerChangeOutputPressureMessage args)
		{
			mixer.TargetPressure = Math.Clamp(args.Pressure, 0f, mixer.MaxTargetPressure);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.AtmosPressureChanged;
			LogImpact impact = LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(28, 3);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(args.Session.AttachedEntity.Value), "player", "ToPrettyString(args.Session.AttachedEntity!.Value)");
			logStringHandler.AppendLiteral(" set the pressure on ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "device", "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" to ");
			logStringHandler.AppendFormatted<float>(args.Pressure, "args.Pressure");
			logStringHandler.AppendLiteral("kPa");
			adminLogger.Add(type, impact, ref logStringHandler);
			this.DirtyUI(uid, mixer);
		}

		// Token: 0x060027B8 RID: 10168 RVA: 0x000D0E10 File Offset: 0x000CF010
		private void OnChangeNodePercentageMessage(EntityUid uid, GasMixerComponent mixer, GasMixerChangeNodePercentageMessage args)
		{
			float nodeOne = Math.Clamp(args.NodeOne, 0f, 100f) / 100f;
			mixer.InletOneConcentration = nodeOne;
			mixer.InletTwoConcentration = 1f - mixer.InletOneConcentration;
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.AtmosRatioChanged;
			LogImpact impact = LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(23, 4);
			logStringHandler.AppendFormatted<EntityStringRepresentation>(this.EntityManager.ToPrettyString(args.Session.AttachedEntity.Value), "player", "EntityManager.ToPrettyString(args.Session.AttachedEntity!.Value)");
			logStringHandler.AppendLiteral(" set the ratio on ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(this.EntityManager.ToPrettyString(uid), "device", "EntityManager.ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" to ");
			logStringHandler.AppendFormatted<float>(mixer.InletOneConcentration, "mixer.InletOneConcentration");
			logStringHandler.AppendLiteral(":");
			logStringHandler.AppendFormatted<float>(mixer.InletTwoConcentration, "mixer.InletTwoConcentration");
			adminLogger.Add(type, impact, ref logStringHandler);
			this.DirtyUI(uid, mixer);
		}

		// Token: 0x060027B9 RID: 10169 RVA: 0x000D0F08 File Offset: 0x000CF108
		private void OnMixerAnalyzed(EntityUid uid, GasMixerComponent component, GasAnalyzerScanEvent args)
		{
			NodeContainerComponent nodeContainer;
			if (!this.EntityManager.TryGetComponent<NodeContainerComponent>(uid, ref nodeContainer))
			{
				return;
			}
			Dictionary<string, GasMixture> gasMixDict = new Dictionary<string, GasMixture>();
			PipeNode inletOne;
			nodeContainer.TryGetNode<PipeNode>(component.InletOneName, out inletOne);
			PipeNode inletTwo;
			nodeContainer.TryGetNode<PipeNode>(component.InletTwoName, out inletTwo);
			if (inletOne != null)
			{
				Dictionary<string, GasMixture> dictionary = gasMixDict;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
				defaultInterpolatedStringHandler.AppendFormatted<PipeDirection>(inletOne.CurrentPipeDirection);
				defaultInterpolatedStringHandler.AppendLiteral(" ");
				defaultInterpolatedStringHandler.AppendFormatted(Loc.GetString("gas-analyzer-window-text-inlet"));
				dictionary.Add(defaultInterpolatedStringHandler.ToStringAndClear(), inletOne.Air);
			}
			if (inletTwo != null)
			{
				Dictionary<string, GasMixture> dictionary2 = gasMixDict;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
				defaultInterpolatedStringHandler.AppendFormatted<PipeDirection>(inletTwo.CurrentPipeDirection);
				defaultInterpolatedStringHandler.AppendLiteral(" ");
				defaultInterpolatedStringHandler.AppendFormatted(Loc.GetString("gas-analyzer-window-text-inlet"));
				dictionary2.Add(defaultInterpolatedStringHandler.ToStringAndClear(), inletTwo.Air);
			}
			PipeNode outlet;
			if (nodeContainer.TryGetNode<PipeNode>(component.OutletName, out outlet))
			{
				gasMixDict.Add(Loc.GetString("gas-analyzer-window-text-outlet"), outlet.Air);
			}
			args.GasMixtures = gasMixDict;
			args.DeviceFlipped = (inletOne != null && inletTwo != null && inletOne.CurrentPipeDirection.ToDirection() == DirectionExtensions.GetClockwise90Degrees(inletTwo.CurrentPipeDirection.ToDirection()));
		}

		// Token: 0x040018BC RID: 6332
		[Dependency]
		private UserInterfaceSystem _userInterfaceSystem;

		// Token: 0x040018BD RID: 6333
		[Dependency]
		private IAdminLogManager _adminLogger;

		// Token: 0x040018BE RID: 6334
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x040018BF RID: 6335
		[Dependency]
		private readonly SharedAmbientSoundSystem _ambientSoundSystem;

		// Token: 0x040018C0 RID: 6336
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
