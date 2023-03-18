using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Atmos.Piping.Trinary.Components;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.Nodes;
using Content.Shared.Atmos.Piping;
using Content.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Server.Atmos.Piping.Trinary.EntitySystems
{
	// Token: 0x02000758 RID: 1880
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PressureControlledValveSystem : EntitySystem
	{
		// Token: 0x060027BB RID: 10171 RVA: 0x000D1040 File Offset: 0x000CF240
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<PressureControlledValveComponent, ComponentInit>(new ComponentEventHandler<PressureControlledValveComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<PressureControlledValveComponent, AtmosDeviceUpdateEvent>(new ComponentEventHandler<PressureControlledValveComponent, AtmosDeviceUpdateEvent>(this.OnUpdate), null, null);
			base.SubscribeLocalEvent<PressureControlledValveComponent, AtmosDeviceDisabledEvent>(new ComponentEventHandler<PressureControlledValveComponent, AtmosDeviceDisabledEvent>(this.OnFilterLeaveAtmosphere), null, null);
		}

		// Token: 0x060027BC RID: 10172 RVA: 0x000D108F File Offset: 0x000CF28F
		private void OnInit(EntityUid uid, PressureControlledValveComponent comp, ComponentInit args)
		{
			this.UpdateAppearance(uid, comp, null);
		}

		// Token: 0x060027BD RID: 10173 RVA: 0x000D109C File Offset: 0x000CF29C
		private void OnUpdate(EntityUid uid, PressureControlledValveComponent comp, AtmosDeviceUpdateEvent args)
		{
			NodeContainerComponent nodeContainer;
			AtmosDeviceComponent device;
			PipeNode inletNode;
			PipeNode controlNode;
			PipeNode outletNode;
			if (!this.EntityManager.TryGetComponent<NodeContainerComponent>(uid, ref nodeContainer) || !this.EntityManager.TryGetComponent<AtmosDeviceComponent>(uid, ref device) || !nodeContainer.TryGetNode<PipeNode>(comp.InletName, out inletNode) || !nodeContainer.TryGetNode<PipeNode>(comp.ControlName, out controlNode) || !nodeContainer.TryGetNode<PipeNode>(comp.OutletName, out outletNode))
			{
				this._ambientSoundSystem.SetAmbience(comp.Owner, false, null);
				comp.Enabled = false;
				return;
			}
			if (outletNode.Air.Pressure > inletNode.Air.Pressure)
			{
				PipeNode pipeNode = outletNode;
				outletNode = inletNode;
				inletNode = pipeNode;
			}
			float control = controlNode.Air.Pressure - outletNode.Air.Pressure - comp.Threshold;
			float transferRate;
			if (control < 0f)
			{
				comp.Enabled = false;
				transferRate = 0f;
			}
			else
			{
				comp.Enabled = true;
				transferRate = Math.Min(control * comp.Gain, comp.MaxTransferRate);
			}
			this.UpdateAppearance(uid, comp, null);
			float transferVolume = (float)((double)transferRate * (this._gameTiming.CurTime - device.LastProcess).TotalSeconds);
			if (transferVolume <= 0f)
			{
				this._ambientSoundSystem.SetAmbience(comp.Owner, false, null);
				return;
			}
			this._ambientSoundSystem.SetAmbience(comp.Owner, true, null);
			GasMixture removed = inletNode.Air.RemoveVolume(transferVolume);
			this._atmosphereSystem.Merge(outletNode.Air, removed);
		}

		// Token: 0x060027BE RID: 10174 RVA: 0x000D120C File Offset: 0x000CF40C
		private void OnFilterLeaveAtmosphere(EntityUid uid, PressureControlledValveComponent comp, AtmosDeviceDisabledEvent args)
		{
			comp.Enabled = false;
			this.UpdateAppearance(uid, comp, null);
			this._ambientSoundSystem.SetAmbience(comp.Owner, false, null);
		}

		// Token: 0x060027BF RID: 10175 RVA: 0x000D1231 File Offset: 0x000CF431
		[NullableContext(2)]
		private void UpdateAppearance(EntityUid uid, PressureControlledValveComponent comp = null, AppearanceComponent appearance = null)
		{
			if (!base.Resolve<PressureControlledValveComponent, AppearanceComponent>(uid, ref comp, ref appearance, false))
			{
				return;
			}
			this._appearance.SetData(uid, FilterVisuals.Enabled, comp.Enabled, appearance);
		}

		// Token: 0x040018C1 RID: 6337
		[Dependency]
		private IGameTiming _gameTiming;

		// Token: 0x040018C2 RID: 6338
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x040018C3 RID: 6339
		[Dependency]
		private readonly SharedAmbientSoundSystem _ambientSoundSystem;

		// Token: 0x040018C4 RID: 6340
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
