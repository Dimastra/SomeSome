using System;
using System.Runtime.CompilerServices;
using Content.Server.Light.Components;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Server.Light.EntitySystems
{
	// Token: 0x0200040F RID: 1039
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LitOnPoweredSystem : EntitySystem
	{
		// Token: 0x0600151E RID: 5406 RVA: 0x0006ECC3 File Offset: 0x0006CEC3
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<LitOnPoweredComponent, PowerChangedEvent>(new ComponentEventRefHandler<LitOnPoweredComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
			base.SubscribeLocalEvent<LitOnPoweredComponent, PowerNetBatterySupplyEvent>(new ComponentEventRefHandler<LitOnPoweredComponent, PowerNetBatterySupplyEvent>(this.OnPowerSupply), null, null);
		}

		// Token: 0x0600151F RID: 5407 RVA: 0x0006ECF4 File Offset: 0x0006CEF4
		private void OnPowerChanged(EntityUid uid, LitOnPoweredComponent component, ref PowerChangedEvent args)
		{
			PointLightComponent light;
			if (this.EntityManager.TryGetComponent<PointLightComponent>(uid, ref light))
			{
				light.Enabled = args.Powered;
			}
		}

		// Token: 0x06001520 RID: 5408 RVA: 0x0006ED20 File Offset: 0x0006CF20
		private void OnPowerSupply(EntityUid uid, LitOnPoweredComponent component, ref PowerNetBatterySupplyEvent args)
		{
			PointLightComponent light;
			if (this.EntityManager.TryGetComponent<PointLightComponent>(uid, ref light))
			{
				light.Enabled = args.Supply;
			}
		}
	}
}
