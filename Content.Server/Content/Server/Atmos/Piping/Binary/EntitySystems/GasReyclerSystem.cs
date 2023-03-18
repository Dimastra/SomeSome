using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Binary.Components;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Construction;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.Nodes;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Piping;
using Content.Shared.Audio;
using Content.Shared.Examine;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Atmos.Piping.Binary.EntitySystems
{
	// Token: 0x0200076A RID: 1898
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasReyclerSystem : EntitySystem
	{
		// Token: 0x0600282F RID: 10287 RVA: 0x000D2764 File Offset: 0x000D0964
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GasRecyclerComponent, AtmosDeviceEnabledEvent>(new ComponentEventHandler<GasRecyclerComponent, AtmosDeviceEnabledEvent>(this.OnEnabled), null, null);
			base.SubscribeLocalEvent<GasRecyclerComponent, AtmosDeviceUpdateEvent>(new ComponentEventHandler<GasRecyclerComponent, AtmosDeviceUpdateEvent>(this.OnUpdate), null, null);
			base.SubscribeLocalEvent<GasRecyclerComponent, AtmosDeviceDisabledEvent>(new ComponentEventHandler<GasRecyclerComponent, AtmosDeviceDisabledEvent>(this.OnDisabled), null, null);
			base.SubscribeLocalEvent<GasRecyclerComponent, ExaminedEvent>(new ComponentEventHandler<GasRecyclerComponent, ExaminedEvent>(this.OnExamined), null, null);
			base.SubscribeLocalEvent<GasRecyclerComponent, RefreshPartsEvent>(new ComponentEventHandler<GasRecyclerComponent, RefreshPartsEvent>(this.OnRefreshParts), null, null);
			base.SubscribeLocalEvent<GasRecyclerComponent, UpgradeExamineEvent>(new ComponentEventHandler<GasRecyclerComponent, UpgradeExamineEvent>(this.OnUpgradeExamine), null, null);
		}

		// Token: 0x06002830 RID: 10288 RVA: 0x000D27EF File Offset: 0x000D09EF
		private void OnEnabled(EntityUid uid, GasRecyclerComponent comp, AtmosDeviceEnabledEvent args)
		{
			this.UpdateAppearance(uid, comp);
		}

		// Token: 0x06002831 RID: 10289 RVA: 0x000D27FC File Offset: 0x000D09FC
		private void OnExamined(EntityUid uid, GasRecyclerComponent comp, ExaminedEvent args)
		{
			if (!this.EntityManager.GetComponent<TransformComponent>(comp.Owner).Anchored || !args.IsInDetailsRange)
			{
				return;
			}
			NodeContainerComponent nodeContainer;
			PipeNode inlet;
			PipeNode pipeNode;
			if (!this.EntityManager.TryGetComponent<NodeContainerComponent>(uid, ref nodeContainer) || !nodeContainer.TryGetNode<PipeNode>(comp.InletName, out inlet) || !nodeContainer.TryGetNode<PipeNode>(comp.OutletName, out pipeNode))
			{
				return;
			}
			if (comp.Reacting)
			{
				args.PushMarkup(Loc.GetString("gas-recycler-reacting"));
				return;
			}
			if (inlet.Air.Pressure < comp.MinPressure)
			{
				args.PushMarkup(Loc.GetString("gas-recycler-low-pressure"));
			}
			if (inlet.Air.Temperature < comp.MinTemp)
			{
				args.PushMarkup(Loc.GetString("gas-recycler-low-temperature"));
			}
		}

		// Token: 0x06002832 RID: 10290 RVA: 0x000D28BC File Offset: 0x000D0ABC
		private void OnUpdate(EntityUid uid, GasRecyclerComponent comp, AtmosDeviceUpdateEvent args)
		{
			NodeContainerComponent nodeContainer;
			PipeNode inlet;
			PipeNode outlet;
			if (!this.EntityManager.TryGetComponent<NodeContainerComponent>(uid, ref nodeContainer) || !nodeContainer.TryGetNode<PipeNode>(comp.InletName, out inlet) || !nodeContainer.TryGetNode<PipeNode>(comp.OutletName, out outlet))
			{
				this._ambientSoundSystem.SetAmbience(comp.Owner, false, null);
				return;
			}
			comp.Reacting = (inlet.Air.Temperature >= comp.MinTemp && inlet.Air.Pressure >= comp.MinPressure);
			GasMixture removed = inlet.Air.RemoveVolume(this.PassiveTransferVol(inlet.Air, outlet.Air));
			if (comp.Reacting)
			{
				float nCO2 = removed.GetMoles(Gas.CarbonDioxide);
				removed.AdjustMoles(Gas.CarbonDioxide, -nCO2);
				removed.AdjustMoles(Gas.Oxygen, nCO2);
				float nN2O = removed.GetMoles(Gas.NitrousOxide);
				removed.AdjustMoles(Gas.NitrousOxide, -nN2O);
				removed.AdjustMoles(Gas.Nitrogen, nN2O);
			}
			this._atmosphereSystem.Merge(outlet.Air, removed);
			this.UpdateAppearance(uid, comp);
			this._ambientSoundSystem.SetAmbience(comp.Owner, true, null);
		}

		// Token: 0x06002833 RID: 10291 RVA: 0x000D29CC File Offset: 0x000D0BCC
		public float PassiveTransferVol(GasMixture inlet, GasMixture outlet)
		{
			if (inlet.Pressure < outlet.Pressure)
			{
				return 0f;
			}
			float overPressConst = 300f;
			return 200f / (float)Math.Sqrt((double)(overPressConst * 101.325f)) * (float)Math.Sqrt((double)(inlet.Pressure - outlet.Pressure));
		}

		// Token: 0x06002834 RID: 10292 RVA: 0x000D2A1C File Offset: 0x000D0C1C
		private void OnDisabled(EntityUid uid, GasRecyclerComponent comp, AtmosDeviceDisabledEvent args)
		{
			comp.Reacting = false;
			this.UpdateAppearance(uid, comp);
		}

		// Token: 0x06002835 RID: 10293 RVA: 0x000D2A2D File Offset: 0x000D0C2D
		[NullableContext(2)]
		private void UpdateAppearance(EntityUid uid, GasRecyclerComponent comp = null)
		{
			if (!base.Resolve<GasRecyclerComponent>(uid, ref comp, false))
			{
				return;
			}
			this._appearance.SetData(uid, PumpVisuals.Enabled, comp.Reacting, null);
		}

		// Token: 0x06002836 RID: 10294 RVA: 0x000D2A5C File Offset: 0x000D0C5C
		private void OnRefreshParts(EntityUid uid, GasRecyclerComponent component, RefreshPartsEvent args)
		{
			float ratingTemp = args.PartRatings[component.MachinePartMinTemp];
			float ratingPressure = args.PartRatings[component.MachinePartMinPressure];
			component.MinTemp = component.BaseMinTemp * MathF.Pow(component.PartRatingMinTempMultiplier, ratingTemp - 1f);
			component.MinPressure = component.BaseMinPressure * MathF.Pow(component.PartRatingMinPressureMultiplier, ratingPressure - 1f);
		}

		// Token: 0x06002837 RID: 10295 RVA: 0x000D2ACB File Offset: 0x000D0CCB
		private void OnUpgradeExamine(EntityUid uid, GasRecyclerComponent component, UpgradeExamineEvent args)
		{
			args.AddPercentageUpgrade("gas-recycler-upgrade-min-temp", component.MinTemp / component.BaseMinTemp);
			args.AddPercentageUpgrade("gas-recycler-upgrade-min-pressure", component.MinPressure / component.BaseMinPressure);
		}

		// Token: 0x040018FD RID: 6397
		[Dependency]
		private readonly AppearanceSystem _appearance;

		// Token: 0x040018FE RID: 6398
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x040018FF RID: 6399
		[Dependency]
		private readonly SharedAmbientSoundSystem _ambientSoundSystem;
	}
}
