using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Atmos.Piping.Unary.Components;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.Nodes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Atmos.Piping.Unary.EntitySystems
{
	// Token: 0x0200074A RID: 1866
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasPassiveVentSystem : EntitySystem
	{
		// Token: 0x06002733 RID: 10035 RVA: 0x000CE876 File Offset: 0x000CCA76
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GasPassiveVentComponent, AtmosDeviceUpdateEvent>(new ComponentEventHandler<GasPassiveVentComponent, AtmosDeviceUpdateEvent>(this.OnPassiveVentUpdated), null, null);
		}

		// Token: 0x06002734 RID: 10036 RVA: 0x000CE894 File Offset: 0x000CCA94
		private void OnPassiveVentUpdated(EntityUid uid, GasPassiveVentComponent vent, AtmosDeviceUpdateEvent args)
		{
			GasMixture environment = this._atmosphereSystem.GetContainingMixture(uid, true, true, null);
			if (environment == null)
			{
				return;
			}
			NodeContainerComponent nodeContainer;
			if (!this.EntityManager.TryGetComponent<NodeContainerComponent>(uid, ref nodeContainer))
			{
				return;
			}
			PipeNode inlet;
			if (!nodeContainer.TryGetNode<PipeNode>(vent.InletName, out inlet))
			{
				return;
			}
			float environmentPressure = environment.Pressure;
			float pressureDelta = MathF.Abs(environmentPressure - inlet.Air.Pressure);
			if ((environment.Temperature > 0f || inlet.Air.Temperature > 0f) && pressureDelta > 0.5f)
			{
				if (environmentPressure < inlet.Air.Pressure)
				{
					float airTemperature = (environment.Temperature > 0f) ? environment.Temperature : inlet.Air.Temperature;
					float transferMoles = pressureDelta * environment.Volume / (airTemperature * 8.314463f);
					GasMixture removed = inlet.Air.Remove(transferMoles);
					this._atmosphereSystem.Merge(environment, removed);
					return;
				}
				float airTemperature2 = (inlet.Air.Temperature > 0f) ? inlet.Air.Temperature : environment.Temperature;
				float outputVolume = inlet.Air.Volume;
				float transferMoles2 = pressureDelta * outputVolume / (airTemperature2 * 8.314463f);
				transferMoles2 = MathF.Min(transferMoles2, environment.TotalMoles * inlet.Air.Volume / environment.Volume);
				GasMixture removed2 = environment.Remove(transferMoles2);
				this._atmosphereSystem.Merge(inlet.Air, removed2);
			}
		}

		// Token: 0x04001869 RID: 6249
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;
	}
}
