using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Anomaly.Components
{
	// Token: 0x0200070B RID: 1803
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class AnomalyComponent : Component
	{
		// Token: 0x040015FD RID: 5629
		[ViewVariables]
		public float Stability;

		// Token: 0x040015FE RID: 5630
		[ViewVariables]
		public float Severity;

		// Token: 0x040015FF RID: 5631
		[ViewVariables]
		public float Health = 1f;

		// Token: 0x04001600 RID: 5632
		[DataField("decayhreshold", false, 1, false, false, null)]
		[ViewVariables]
		public float DecayThreshold = 0.15f;

		// Token: 0x04001601 RID: 5633
		[DataField("healthChangePerSecond", false, 1, false, false, null)]
		[ViewVariables]
		public float HealthChangePerSecond = -0.01f;

		// Token: 0x04001602 RID: 5634
		[DataField("growthThreshold", false, 1, false, false, null)]
		[ViewVariables]
		public float GrowthThreshold = 0.5f;

		// Token: 0x04001603 RID: 5635
		[DataField("severityGrowthCoefficient", false, 1, false, false, null)]
		[ViewVariables]
		public float SeverityGrowthCoefficient = 0.07f;

		// Token: 0x04001604 RID: 5636
		[DataField("nextPulseTime", false, 1, false, false, typeof(TimeOffsetSerializer))]
		[ViewVariables]
		public TimeSpan NextPulseTime = TimeSpan.MaxValue;

		// Token: 0x04001605 RID: 5637
		[DataField("minPulseLength", false, 1, false, false, null)]
		public TimeSpan MinPulseLength = TimeSpan.FromMinutes(1.0);

		// Token: 0x04001606 RID: 5638
		[DataField("maxPulseLength", false, 1, false, false, null)]
		public TimeSpan MaxPulseLength = TimeSpan.FromMinutes(2.0);

		// Token: 0x04001607 RID: 5639
		[DataField("pulseVariation", false, 1, false, false, null)]
		public float PulseVariation = 0.1f;

		// Token: 0x04001608 RID: 5640
		[DataField("pulseStabilityVariation", false, 1, false, false, null)]
		public float PulseStabilityVariation = 0.05f;

		// Token: 0x04001609 RID: 5641
		[Nullable(2)]
		[DataField("pulseSound", false, 1, false, false, null)]
		public SoundSpecifier PulseSound = new SoundCollectionSpecifier("RadiationPulse", null);

		// Token: 0x0400160A RID: 5642
		[Nullable(2)]
		[DataField("supercriticalSound", false, 1, false, false, null)]
		public SoundSpecifier SupercriticalSound = new SoundCollectionSpecifier("explosion", null);

		// Token: 0x0400160B RID: 5643
		[DataField("initialStabilityRange", false, 1, false, false, null)]
		public ValueTuple<float, float> InitialStabilityRange = new ValueTuple<float, float>(0.4f, 0.6f);

		// Token: 0x0400160C RID: 5644
		[DataField("initialSeverityRange", false, 1, false, false, null)]
		public ValueTuple<float, float> InitialSeverityRange = new ValueTuple<float, float>(0.1f, 0.5f);

		// Token: 0x0400160D RID: 5645
		[DataField("severityParticleType", false, 1, false, false, null)]
		public AnomalousParticleType SeverityParticleType;

		// Token: 0x0400160E RID: 5646
		[DataField("severityPerSeverityHit", false, 1, false, false, null)]
		public float SeverityPerSeverityHit = 0.025f;

		// Token: 0x0400160F RID: 5647
		[DataField("destabilizingParticleType", false, 1, false, false, null)]
		public AnomalousParticleType DestabilizingParticleType;

		// Token: 0x04001610 RID: 5648
		[DataField("stabilityPerDestabilizingHit", false, 1, false, false, null)]
		public float StabilityPerDestabilizingHit = 0.04f;

		// Token: 0x04001611 RID: 5649
		[DataField("weakeningParticleType", false, 1, false, false, null)]
		public AnomalousParticleType WeakeningParticleType;

		// Token: 0x04001612 RID: 5650
		[DataField("healthPerWeakeningeHit", false, 1, false, false, null)]
		public float HealthPerWeakeningeHit = -0.05f;

		// Token: 0x04001613 RID: 5651
		[DataField("stabilityPerWeakeningeHit", false, 1, false, false, null)]
		public float StabilityPerWeakeningeHit = -0.1f;

		// Token: 0x04001614 RID: 5652
		[ViewVariables]
		public EntityUid? ConnectedVessel;

		// Token: 0x04001615 RID: 5653
		[DataField("minPointsPerSecond", false, 1, false, false, null)]
		public int MinPointsPerSecond;

		// Token: 0x04001616 RID: 5654
		[DataField("maxPointsPerSecond", false, 1, false, false, null)]
		public int MaxPointsPerSecond = 100;

		// Token: 0x04001617 RID: 5655
		[DataField("growingPointMultiplier", false, 1, false, false, null)]
		public float GrowingPointMultiplier = 1.5f;

		// Token: 0x04001618 RID: 5656
		[Nullable(1)]
		[DataField("anomalyContactDamage", false, 1, true, false, null)]
		public DamageSpecifier AnomalyContactDamage;

		// Token: 0x04001619 RID: 5657
		[Nullable(1)]
		[DataField("anomalyContactDamageSound", false, 1, false, false, null)]
		public SoundSpecifier AnomalyContactDamageSound = new SoundPathSpecifier("/Audio/Effects/lightburn.ogg", null);

		// Token: 0x0400161A RID: 5658
		[ViewVariables]
		[DataField("animationTime", false, 1, false, false, null)]
		public readonly float AnimationTime = 2f;

		// Token: 0x0400161B RID: 5659
		[ViewVariables]
		[DataField("offset", false, 1, false, false, null)]
		public readonly Vector2 FloatingOffset = new ValueTuple<float, float>(0f, 0.15f);

		// Token: 0x0400161C RID: 5660
		[Nullable(1)]
		public readonly string AnimationKey = "anomalyfloat";
	}
}
