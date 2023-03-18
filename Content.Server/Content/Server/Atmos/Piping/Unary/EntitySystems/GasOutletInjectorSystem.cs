using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Atmos.Piping.Unary.Components;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.Nodes;
using Content.Shared.Atmos.Piping;
using Content.Shared.Interaction;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Server.Atmos.Piping.Unary.EntitySystems
{
	// Token: 0x02000749 RID: 1865
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasOutletInjectorSystem : EntitySystem
	{
		// Token: 0x0600272D RID: 10029 RVA: 0x000CE6EC File Offset: 0x000CC8EC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GasOutletInjectorComponent, AtmosDeviceUpdateEvent>(new ComponentEventHandler<GasOutletInjectorComponent, AtmosDeviceUpdateEvent>(this.OnOutletInjectorUpdated), null, null);
			base.SubscribeLocalEvent<GasOutletInjectorComponent, ActivateInWorldEvent>(new ComponentEventHandler<GasOutletInjectorComponent, ActivateInWorldEvent>(this.OnActivate), null, null);
			base.SubscribeLocalEvent<GasOutletInjectorComponent, MapInitEvent>(new ComponentEventHandler<GasOutletInjectorComponent, MapInitEvent>(this.OnMapInit), null, null);
		}

		// Token: 0x0600272E RID: 10030 RVA: 0x000CE73B File Offset: 0x000CC93B
		private void OnMapInit(EntityUid uid, GasOutletInjectorComponent component, MapInitEvent args)
		{
			this.UpdateAppearance(uid, component, null);
		}

		// Token: 0x0600272F RID: 10031 RVA: 0x000CE746 File Offset: 0x000CC946
		private void OnActivate(EntityUid uid, GasOutletInjectorComponent component, ActivateInWorldEvent args)
		{
			component.Enabled = !component.Enabled;
			this.UpdateAppearance(uid, component, null);
		}

		// Token: 0x06002730 RID: 10032 RVA: 0x000CE760 File Offset: 0x000CC960
		public void UpdateAppearance(EntityUid uid, GasOutletInjectorComponent component, [Nullable(2)] AppearanceComponent appearance = null)
		{
			if (!base.Resolve<AppearanceComponent>(component.Owner, ref appearance, false))
			{
				return;
			}
			this._appearance.SetData(uid, OutletInjectorVisuals.Enabled, component.Enabled, appearance);
		}

		// Token: 0x06002731 RID: 10033 RVA: 0x000CE794 File Offset: 0x000CC994
		private void OnOutletInjectorUpdated(EntityUid uid, GasOutletInjectorComponent injector, AtmosDeviceUpdateEvent args)
		{
			if (!injector.Enabled)
			{
				return;
			}
			NodeContainerComponent nodeContainer;
			if (!this.EntityManager.TryGetComponent<NodeContainerComponent>(uid, ref nodeContainer))
			{
				return;
			}
			AtmosDeviceComponent device;
			if (!base.TryComp<AtmosDeviceComponent>(uid, ref device))
			{
				return;
			}
			PipeNode inlet;
			if (!nodeContainer.TryGetNode<PipeNode>(injector.InletName, out inlet))
			{
				return;
			}
			GasMixture environment = this._atmosphereSystem.GetContainingMixture(uid, true, true, null);
			if (environment == null)
			{
				return;
			}
			if (inlet.Air.Temperature < 0f)
			{
				return;
			}
			if (environment.Pressure > injector.MaxPressure)
			{
				return;
			}
			float timeDelta = (float)(this._gameTiming.CurTime - device.LastProcess).TotalSeconds;
			float ratio = MathF.Min(1f, timeDelta * injector.TransferRate / inlet.Air.Volume);
			GasMixture removed = inlet.Air.RemoveRatio(ratio);
			this._atmosphereSystem.Merge(environment, removed);
		}

		// Token: 0x04001866 RID: 6246
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x04001867 RID: 6247
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04001868 RID: 6248
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
