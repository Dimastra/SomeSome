using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Logs;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Binary.Components;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Atmos.Piping.Unary.EntitySystems;
using Content.Server.Audio;
using Content.Server.Construction;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.NodeGroups;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Power.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Atmos.Piping.Unary.Components;
using Content.Shared.Atmos.Visuals;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.Examine;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Server.Atmos.Portable
{
	// Token: 0x02000747 RID: 1863
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PortableScrubberSystem : EntitySystem
	{
		// Token: 0x0600270E RID: 9998 RVA: 0x000CD544 File Offset: 0x000CB744
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PortableScrubberComponent, AtmosDeviceUpdateEvent>(new ComponentEventHandler<PortableScrubberComponent, AtmosDeviceUpdateEvent>(this.OnDeviceUpdated), null, null);
			base.SubscribeLocalEvent<PortableScrubberComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<PortableScrubberComponent, AnchorStateChangedEvent>(this.OnAnchorChanged), null, null);
			base.SubscribeLocalEvent<PortableScrubberComponent, PowerChangedEvent>(new ComponentEventRefHandler<PortableScrubberComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
			base.SubscribeLocalEvent<PortableScrubberComponent, ExaminedEvent>(new ComponentEventHandler<PortableScrubberComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<PortableScrubberComponent, DestructionEventArgs>(new ComponentEventHandler<PortableScrubberComponent, DestructionEventArgs>(this.OnDestroyed), null, null);
			base.SubscribeLocalEvent<PortableScrubberComponent, GasAnalyzerScanEvent>(new ComponentEventHandler<PortableScrubberComponent, GasAnalyzerScanEvent>(this.OnScrubberAnalyzed), null, null);
			base.SubscribeLocalEvent<PortableScrubberComponent, RefreshPartsEvent>(new ComponentEventHandler<PortableScrubberComponent, RefreshPartsEvent>(this.OnRefreshParts), null, null);
			base.SubscribeLocalEvent<PortableScrubberComponent, UpgradeExamineEvent>(new ComponentEventHandler<PortableScrubberComponent, UpgradeExamineEvent>(this.OnUpgradeExamine), null, null);
		}

		// Token: 0x0600270F RID: 9999 RVA: 0x000CD5F7 File Offset: 0x000CB7F7
		private bool IsFull(PortableScrubberComponent component)
		{
			return component.Air.Pressure >= component.MaxPressure;
		}

		// Token: 0x06002710 RID: 10000 RVA: 0x000CD610 File Offset: 0x000CB810
		private void OnDeviceUpdated(EntityUid uid, PortableScrubberComponent component, AtmosDeviceUpdateEvent args)
		{
			AtmosDeviceComponent device;
			if (!base.TryComp<AtmosDeviceComponent>(uid, ref device))
			{
				return;
			}
			float timeDelta = (float)(this._gameTiming.CurTime - device.LastProcess).TotalSeconds;
			if (!component.Enabled)
			{
				return;
			}
			NodeContainerComponent nodeContainer;
			PortablePipeNode portableNode;
			if (base.TryComp<NodeContainerComponent>(uid, ref nodeContainer) && nodeContainer.TryGetNode<PortablePipeNode>(component.PortName, out portableNode) && portableNode.ConnectionsEnabled)
			{
				this._atmosphereSystem.React(component.Air, portableNode);
				PipeNet net = portableNode.NodeGroup as PipeNet;
				if (net != null && net.NodeCount > 1)
				{
					this._canisterSystem.MixContainerWithPipeNet(component.Air, net.Air);
				}
			}
			if (this.IsFull(component))
			{
				this.UpdateAppearance(uid, true, false);
				return;
			}
			TransformComponent xform = base.Transform(uid);
			if (xform.GridUid == null)
			{
				return;
			}
			Vector2i position = this._transformSystem.GetGridOrMapTilePosition(uid, xform);
			GasMixture environment = this._atmosphereSystem.GetTileMixture(xform.GridUid, xform.MapUid, position, true);
			bool running = this.Scrub(timeDelta, component, environment);
			this.UpdateAppearance(uid, false, running);
			if (!running)
			{
				return;
			}
			foreach (GasMixture adjacent in this._atmosphereSystem.GetAdjacentTileMixtures(xform.GridUid.Value, position, false, true))
			{
				this.Scrub(timeDelta, component, adjacent);
			}
		}

		// Token: 0x06002711 RID: 10001 RVA: 0x000CD798 File Offset: 0x000CB998
		private void OnAnchorChanged(EntityUid uid, PortableScrubberComponent component, ref AnchorStateChangedEvent args)
		{
			NodeContainerComponent nodeContainer;
			if (!base.TryComp<NodeContainerComponent>(uid, ref nodeContainer))
			{
				return;
			}
			PipeNode portableNode;
			if (!nodeContainer.TryGetNode<PipeNode>(component.PortName, out portableNode))
			{
				return;
			}
			GasPortComponent gasPortComponent;
			portableNode.ConnectionsEnabled = (args.Anchored && this._gasPortableSystem.FindGasPortIn(base.Transform(uid).GridUid, base.Transform(uid).Coordinates, out gasPortComponent));
			this._appearance.SetData(uid, PortableScrubberVisuals.IsDraining, portableNode.ConnectionsEnabled, null);
		}

		// Token: 0x06002712 RID: 10002 RVA: 0x000CD816 File Offset: 0x000CBA16
		private void OnPowerChanged(EntityUid uid, PortableScrubberComponent component, ref PowerChangedEvent args)
		{
			this.UpdateAppearance(uid, this.IsFull(component), args.Powered);
			component.Enabled = args.Powered;
		}

		// Token: 0x06002713 RID: 10003 RVA: 0x000CD838 File Offset: 0x000CBA38
		private void OnExamined(EntityUid uid, PortableScrubberComponent component, ExaminedEvent args)
		{
			if (args.IsInDetailsRange)
			{
				double percentage = Math.Round((double)(component.Air.Pressure / component.MaxPressure * 100f));
				args.PushMarkup(Loc.GetString("portable-scrubber-fill-level", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("percent", percentage)
				}));
			}
		}

		// Token: 0x06002714 RID: 10004 RVA: 0x000CD89C File Offset: 0x000CBA9C
		private void OnDestroyed(EntityUid uid, PortableScrubberComponent component, DestructionEventArgs args)
		{
			GasMixture environment = this._atmosphereSystem.GetContainingMixture(uid, false, true, null);
			if (environment != null)
			{
				this._atmosphereSystem.Merge(environment, component.Air);
			}
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.CanisterPurged;
			LogImpact impact = LogImpact.Medium;
			LogStringHandler logStringHandler = new LogStringHandler(64, 2);
			logStringHandler.AppendLiteral("Portable scrubber ");
			logStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(uid), "canister", "ToPrettyString(uid)");
			logStringHandler.AppendLiteral(" purged its contents of ");
			logStringHandler.AppendFormatted<GasMixture>(component.Air, "component.Air");
			logStringHandler.AppendLiteral(" into the environment.");
			adminLogger.Add(type, impact, ref logStringHandler);
			component.Air.Clear();
		}

		// Token: 0x06002715 RID: 10005 RVA: 0x000CD941 File Offset: 0x000CBB41
		private bool Scrub(float timeDelta, PortableScrubberComponent scrubber, [Nullable(2)] GasMixture tile)
		{
			return this._scrubberSystem.Scrub(timeDelta, scrubber.TransferRate, ScrubberPumpDirection.Scrubbing, scrubber.FilterGases, tile, scrubber.Air);
		}

		// Token: 0x06002716 RID: 10006 RVA: 0x000CD964 File Offset: 0x000CBB64
		private void UpdateAppearance(EntityUid uid, bool isFull, bool isRunning)
		{
			this._ambientSound.SetAmbience(uid, isRunning, null);
			this._appearance.SetData(uid, PortableScrubberVisuals.IsFull, isFull, null);
			this._appearance.SetData(uid, PortableScrubberVisuals.IsRunning, isRunning, null);
		}

		// Token: 0x06002717 RID: 10007 RVA: 0x000CD9B4 File Offset: 0x000CBBB4
		private void OnScrubberAnalyzed(EntityUid uid, PortableScrubberComponent component, GasAnalyzerScanEvent args)
		{
			Dictionary<string, GasMixture> gasMixDict = new Dictionary<string, GasMixture>
			{
				{
					base.Name(uid, null),
					component.Air
				}
			};
			NodeContainerComponent nodeContainer;
			PipeNode port;
			if (base.TryComp<NodeContainerComponent>(uid, ref nodeContainer) && nodeContainer.TryGetNode<PipeNode>(component.PortName, out port))
			{
				gasMixDict.Add(component.PortName, port.Air);
			}
			args.GasMixtures = gasMixDict;
		}

		// Token: 0x06002718 RID: 10008 RVA: 0x000CDA10 File Offset: 0x000CBC10
		private void OnRefreshParts(EntityUid uid, PortableScrubberComponent component, RefreshPartsEvent args)
		{
			float pressureRating = args.PartRatings[component.MachinePartMaxPressure];
			float transferRating = args.PartRatings[component.MachinePartTransferRate];
			component.MaxPressure = component.BaseMaxPressure * MathF.Pow(component.PartRatingMaxPressureModifier, pressureRating - 1f);
			component.TransferRate = component.BaseTransferRate * MathF.Pow(component.PartRatingTransferRateModifier, transferRating - 1f);
		}

		// Token: 0x06002719 RID: 10009 RVA: 0x000CDA7F File Offset: 0x000CBC7F
		private void OnUpgradeExamine(EntityUid uid, PortableScrubberComponent component, UpgradeExamineEvent args)
		{
			args.AddPercentageUpgrade("portable-scrubber-component-upgrade-max-pressure", component.MaxPressure / component.BaseMaxPressure);
			args.AddPercentageUpgrade("portable-scrubber-component-upgrade-transfer-rate", component.TransferRate / component.BaseTransferRate);
		}

		// Token: 0x04001853 RID: 6227
		[Dependency]
		private readonly GasVentScrubberSystem _scrubberSystem;

		// Token: 0x04001854 RID: 6228
		[Dependency]
		private readonly GasCanisterSystem _canisterSystem;

		// Token: 0x04001855 RID: 6229
		[Dependency]
		private readonly GasPortableSystem _gasPortableSystem;

		// Token: 0x04001856 RID: 6230
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x04001857 RID: 6231
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04001858 RID: 6232
		[Dependency]
		private readonly TransformSystem _transformSystem;

		// Token: 0x04001859 RID: 6233
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x0400185A RID: 6234
		[Dependency]
		private readonly AmbientSoundSystem _ambientSound;

		// Token: 0x0400185B RID: 6235
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
