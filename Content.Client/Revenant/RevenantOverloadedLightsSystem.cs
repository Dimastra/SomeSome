using System;
using System.Runtime.CompilerServices;
using Content.Shared.Revenant.Components;
using Content.Shared.Revenant.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Revenant
{
	// Token: 0x02000169 RID: 361
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RevenantOverloadedLightsSystem : SharedRevenantOverloadedLightsSystem
	{
		// Token: 0x06000966 RID: 2406 RVA: 0x00036FA2 File Offset: 0x000351A2
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RevenantOverloadedLightsComponent, ComponentStartup>(new ComponentEventHandler<RevenantOverloadedLightsComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<RevenantOverloadedLightsComponent, ComponentShutdown>(new ComponentEventHandler<RevenantOverloadedLightsComponent, ComponentShutdown>(this.OnShutdown), null, null);
		}

		// Token: 0x06000967 RID: 2407 RVA: 0x00036FD4 File Offset: 0x000351D4
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			RevenantOverloadedLightsComponent revenantOverloadedLightsComponent;
			PointLightComponent pointLightComponent;
			while (base.EntityQueryEnumerator<RevenantOverloadedLightsComponent, PointLightComponent>().MoveNext(ref revenantOverloadedLightsComponent, ref pointLightComponent))
			{
				pointLightComponent.Energy = 2f * Math.Abs((float)Math.Sin(0.7853981633974483 * (double)revenantOverloadedLightsComponent.Accumulator));
			}
		}

		// Token: 0x06000968 RID: 2408 RVA: 0x00037028 File Offset: 0x00035228
		private void OnStartup(EntityUid uid, RevenantOverloadedLightsComponent component, ComponentStartup args)
		{
			PointLightComponent pointLightComponent = base.EnsureComp<PointLightComponent>(uid);
			component.OriginalEnergy = new float?(pointLightComponent.Energy);
			component.OriginalEnabled = pointLightComponent.Enabled;
			pointLightComponent.Enabled = component.OriginalEnabled;
			base.Dirty(pointLightComponent, null);
		}

		// Token: 0x06000969 RID: 2409 RVA: 0x00037070 File Offset: 0x00035270
		private void OnShutdown(EntityUid uid, RevenantOverloadedLightsComponent component, ComponentShutdown args)
		{
			PointLightComponent pointLightComponent;
			if (!base.TryComp<PointLightComponent>(component.Owner, ref pointLightComponent))
			{
				return;
			}
			if (component.OriginalEnergy == null)
			{
				base.RemComp<PointLightComponent>(component.Owner);
				return;
			}
			pointLightComponent.Energy = component.OriginalEnergy.Value;
			pointLightComponent.Enabled = component.OriginalEnabled;
			base.Dirty(pointLightComponent, null);
		}

		// Token: 0x0600096A RID: 2410 RVA: 0x0001B008 File Offset: 0x00019208
		protected override void OnZap(RevenantOverloadedLightsComponent component)
		{
		}
	}
}
