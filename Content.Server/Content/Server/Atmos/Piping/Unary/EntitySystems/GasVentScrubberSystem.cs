using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Monitor.Systems;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Atmos.Piping.Unary.Components;
using Content.Server.DeviceNetwork;
using Content.Server.DeviceNetwork.Components;
using Content.Server.DeviceNetwork.Systems;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Power.Components;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Monitor;
using Content.Shared.Atmos.Piping.Unary.Components;
using Content.Shared.Atmos.Piping.Unary.Visuals;
using Content.Shared.Audio;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Server.Atmos.Piping.Unary.EntitySystems
{
	// Token: 0x0200074E RID: 1870
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasVentScrubberSystem : EntitySystem
	{
		// Token: 0x06002751 RID: 10065 RVA: 0x000CF574 File Offset: 0x000CD774
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GasVentScrubberComponent, AtmosDeviceUpdateEvent>(new ComponentEventHandler<GasVentScrubberComponent, AtmosDeviceUpdateEvent>(this.OnVentScrubberUpdated), null, null);
			base.SubscribeLocalEvent<GasVentScrubberComponent, AtmosDeviceEnabledEvent>(new ComponentEventHandler<GasVentScrubberComponent, AtmosDeviceEnabledEvent>(this.OnVentScrubberEnterAtmosphere), null, null);
			base.SubscribeLocalEvent<GasVentScrubberComponent, AtmosDeviceDisabledEvent>(new ComponentEventHandler<GasVentScrubberComponent, AtmosDeviceDisabledEvent>(this.OnVentScrubberLeaveAtmosphere), null, null);
			base.SubscribeLocalEvent<GasVentScrubberComponent, AtmosAlarmEvent>(new ComponentEventHandler<GasVentScrubberComponent, AtmosAlarmEvent>(this.OnAtmosAlarm), null, null);
			base.SubscribeLocalEvent<GasVentScrubberComponent, PowerChangedEvent>(new ComponentEventRefHandler<GasVentScrubberComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
			base.SubscribeLocalEvent<GasVentScrubberComponent, DeviceNetworkPacketEvent>(new ComponentEventHandler<GasVentScrubberComponent, DeviceNetworkPacketEvent>(this.OnPacketRecv), null, null);
		}

		// Token: 0x06002752 RID: 10066 RVA: 0x000CF600 File Offset: 0x000CD800
		private void OnVentScrubberUpdated(EntityUid uid, GasVentScrubberComponent scrubber, AtmosDeviceUpdateEvent args)
		{
			if (scrubber.Welded)
			{
				return;
			}
			AtmosDeviceComponent device;
			if (!base.TryComp<AtmosDeviceComponent>(uid, ref device))
			{
				return;
			}
			float timeDelta = (float)(this._gameTiming.CurTime - device.LastProcess).TotalSeconds;
			NodeContainerComponent nodeContainer;
			PipeNode outlet;
			if (!scrubber.Enabled || !this.EntityManager.TryGetComponent<NodeContainerComponent>(uid, ref nodeContainer) || !nodeContainer.TryGetNode<PipeNode>(scrubber.OutletName, out outlet))
			{
				return;
			}
			TransformComponent xform = base.Transform(uid);
			if (xform.GridUid == null)
			{
				return;
			}
			Vector2i position = this._transformSystem.GetGridOrMapTilePosition(uid, xform);
			GasMixture environment = this._atmosphereSystem.GetTileMixture(xform.GridUid, xform.MapUid, position, true);
			this.Scrub(timeDelta, scrubber, environment, outlet);
			if (!scrubber.WideNet)
			{
				return;
			}
			foreach (GasMixture adjacent in this._atmosphereSystem.GetAdjacentTileMixtures(xform.GridUid.Value, position, false, true))
			{
				this.Scrub(timeDelta, scrubber, adjacent, outlet);
			}
		}

		// Token: 0x06002753 RID: 10067 RVA: 0x000CF730 File Offset: 0x000CD930
		private void OnVentScrubberLeaveAtmosphere(EntityUid uid, GasVentScrubberComponent component, AtmosDeviceDisabledEvent args)
		{
			this.UpdateState(uid, component, null);
		}

		// Token: 0x06002754 RID: 10068 RVA: 0x000CF73B File Offset: 0x000CD93B
		private void OnVentScrubberEnterAtmosphere(EntityUid uid, GasVentScrubberComponent component, AtmosDeviceEnabledEvent args)
		{
			this.UpdateState(uid, component, null);
		}

		// Token: 0x06002755 RID: 10069 RVA: 0x000CF746 File Offset: 0x000CD946
		private void Scrub(float timeDelta, GasVentScrubberComponent scrubber, [Nullable(2)] GasMixture tile, PipeNode outlet)
		{
			this.Scrub(timeDelta, scrubber.TransferRate, scrubber.PumpDirection, scrubber.FilterGases, tile, outlet.Air);
		}

		// Token: 0x06002756 RID: 10070 RVA: 0x000CF76C File Offset: 0x000CD96C
		public bool Scrub(float timeDelta, float transferRate, ScrubberPumpDirection mode, HashSet<Gas> filterGases, [Nullable(2)] GasMixture tile, GasMixture destination)
		{
			if (tile == null || destination.Pressure >= 5066.25f)
			{
				return false;
			}
			float ratio = MathF.Min(1f, timeDelta * transferRate * 2.5f / tile.Volume);
			GasMixture removed = tile.RemoveRatio(ratio);
			if (MathHelper.CloseToPercent(removed.TotalMoles, 0f, 1E-05))
			{
				return false;
			}
			if (mode == ScrubberPumpDirection.Scrubbing)
			{
				this._atmosphereSystem.ScrubInto(removed, destination, filterGases);
				this._atmosphereSystem.Merge(tile, removed);
			}
			else if (mode == ScrubberPumpDirection.Siphoning)
			{
				this._atmosphereSystem.Merge(destination, removed);
			}
			return true;
		}

		// Token: 0x06002757 RID: 10071 RVA: 0x000CF805 File Offset: 0x000CDA05
		private void OnAtmosAlarm(EntityUid uid, GasVentScrubberComponent component, AtmosAlarmEvent args)
		{
			if (args.AlarmType == AtmosAlarmType.Danger)
			{
				component.Enabled = false;
			}
			else if (args.AlarmType == AtmosAlarmType.Normal)
			{
				component.Enabled = true;
			}
			this.UpdateState(uid, component, null);
		}

		// Token: 0x06002758 RID: 10072 RVA: 0x000CF832 File Offset: 0x000CDA32
		private void OnPowerChanged(EntityUid uid, GasVentScrubberComponent component, ref PowerChangedEvent args)
		{
			component.Enabled = args.Powered;
			this.UpdateState(uid, component, null);
		}

		// Token: 0x06002759 RID: 10073 RVA: 0x000CF84C File Offset: 0x000CDA4C
		private void OnPacketRecv(EntityUid uid, GasVentScrubberComponent component, DeviceNetworkPacketEvent args)
		{
			DeviceNetworkComponent netConn;
			object cmd;
			if (!this.EntityManager.TryGetComponent<DeviceNetworkComponent>(uid, ref netConn) || !args.Data.TryGetValue("command", out cmd))
			{
				return;
			}
			NetworkPayload payload = new NetworkPayload();
			string text = cmd as string;
			if (text != null)
			{
				if (text == "atmos_sync_data")
				{
					payload.Add("command", "atmos_sync_data");
					payload.Add("atmos_sync_data", component.ToAirAlarmData());
					DeviceNetworkSystem deviceNetSystem = this._deviceNetSystem;
					string senderAddress = args.SenderAddress;
					NetworkPayload data = payload;
					DeviceNetworkComponent device = netConn;
					deviceNetSystem.QueuePacket(uid, senderAddress, data, null, device);
					return;
				}
				if (!(text == "set_state"))
				{
					return;
				}
				GasVentScrubberData setData;
				if (args.Data.TryGetValue<GasVentScrubberData>("set_state", out setData))
				{
					component.FromAirAlarmData(setData);
					this.UpdateState(uid, component, null);
					return;
				}
			}
		}

		// Token: 0x0600275A RID: 10074 RVA: 0x000CF91C File Offset: 0x000CDB1C
		private void UpdateState(EntityUid uid, GasVentScrubberComponent scrubber, [Nullable(2)] AppearanceComponent appearance = null)
		{
			if (!base.Resolve<AppearanceComponent>(uid, ref appearance, false))
			{
				return;
			}
			this._ambientSoundSystem.SetAmbience(uid, true, null);
			if (!scrubber.Enabled)
			{
				this._ambientSoundSystem.SetAmbience(uid, false, null);
				this._appearance.SetData(uid, ScrubberVisuals.State, ScrubberState.Off, appearance);
				return;
			}
			if (scrubber.PumpDirection == ScrubberPumpDirection.Scrubbing)
			{
				this._appearance.SetData(uid, ScrubberVisuals.State, scrubber.WideNet ? ScrubberState.WideScrub : ScrubberState.Scrub, appearance);
				return;
			}
			if (scrubber.PumpDirection == ScrubberPumpDirection.Siphoning)
			{
				this._appearance.SetData(uid, ScrubberVisuals.State, ScrubberState.Siphon, appearance);
				return;
			}
			if (scrubber.Welded)
			{
				this._ambientSoundSystem.SetAmbience(uid, false, null);
				this._appearance.SetData(uid, ScrubberVisuals.State, ScrubberState.Welded, appearance);
			}
		}

		// Token: 0x04001875 RID: 6261
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04001876 RID: 6262
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x04001877 RID: 6263
		[Dependency]
		private readonly DeviceNetworkSystem _deviceNetSystem;

		// Token: 0x04001878 RID: 6264
		[Dependency]
		private readonly SharedAmbientSoundSystem _ambientSoundSystem;

		// Token: 0x04001879 RID: 6265
		[Dependency]
		private readonly TransformSystem _transformSystem;

		// Token: 0x0400187A RID: 6266
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
