using System;
using System.Runtime.CompilerServices;
using Content.Server.Singularity.Components;
using Content.Shared.Anomaly.Components;
using Content.Shared.Anomaly.Effects;
using Content.Shared.Anomaly.Effects.Components;
using Content.Shared.Radiation.Components;
using Robust.Shared.GameObjects;

namespace Content.Server.Anomaly.Effects
{
	// Token: 0x020007C6 RID: 1990
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GravityAnomalySystem : SharedGravityAnomalySystem
	{
		// Token: 0x06002B5F RID: 11103 RVA: 0x000E384C File Offset: 0x000E1A4C
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GravityAnomalyComponent, AnomalySeverityChangedEvent>(new ComponentEventRefHandler<GravityAnomalyComponent, AnomalySeverityChangedEvent>(this.OnSeverityChanged), null, null);
			base.SubscribeLocalEvent<GravityAnomalyComponent, AnomalyStabilityChangedEvent>(new ComponentEventRefHandler<GravityAnomalyComponent, AnomalyStabilityChangedEvent>(this.OnStabilityChanged), null, null);
		}

		// Token: 0x06002B60 RID: 11104 RVA: 0x000E387C File Offset: 0x000E1A7C
		private void OnSeverityChanged(EntityUid uid, GravityAnomalyComponent component, ref AnomalySeverityChangedEvent args)
		{
			RadiationSourceComponent radSource;
			if (base.TryComp<RadiationSourceComponent>(uid, ref radSource))
			{
				radSource.Intensity = component.MaxRadiationIntensity * args.Severity;
			}
			GravityWellComponent gravityWell;
			if (!base.TryComp<GravityWellComponent>(uid, ref gravityWell))
			{
				return;
			}
			float accel = (component.MaxAccel - component.MinAccel) * args.Severity + component.MinAccel;
			gravityWell.BaseRadialAcceleration = accel;
			gravityWell.BaseTangentialAcceleration = accel * 0.2f;
		}

		// Token: 0x06002B61 RID: 11105 RVA: 0x000E38E4 File Offset: 0x000E1AE4
		private void OnStabilityChanged(EntityUid uid, GravityAnomalyComponent component, ref AnomalyStabilityChangedEvent args)
		{
			GravityWellComponent gravityWell;
			if (base.TryComp<GravityWellComponent>(uid, ref gravityWell))
			{
				gravityWell.MaxRange = component.MaxGravityWellRange * args.Stability;
			}
		}
	}
}
