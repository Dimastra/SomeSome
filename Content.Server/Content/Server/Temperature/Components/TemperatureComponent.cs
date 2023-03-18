using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Temperature.Components
{
	// Token: 0x02000128 RID: 296
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class TemperatureComponent : Component
	{
		// Token: 0x170000EE RID: 238
		// (get) Token: 0x06000559 RID: 1369 RVA: 0x0001A327 File Offset: 0x00018527
		// (set) Token: 0x0600055A RID: 1370 RVA: 0x0001A32F File Offset: 0x0001852F
		[ViewVariables]
		[DataField("currentTemperature", false, 1, false, false, null)]
		public float CurrentTemperature { get; set; } = 293.15f;

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x0600055B RID: 1371 RVA: 0x0001A338 File Offset: 0x00018538
		[ViewVariables]
		public float HeatCapacity
		{
			get
			{
				PhysicsComponent physics;
				if (IoCManager.Resolve<IEntityManager>().TryGetComponent<PhysicsComponent>(base.Owner, ref physics) && physics.FixturesMass != 0f)
				{
					return this.SpecificHeat * physics.FixturesMass;
				}
				return 0.0003f;
			}
		}

		// Token: 0x04000339 RID: 825
		[DataField("heatDamageThreshold", false, 1, false, false, null)]
		[ViewVariables]
		public float HeatDamageThreshold = 360f;

		// Token: 0x0400033A RID: 826
		[DataField("coldDamageThreshold", false, 1, false, false, null)]
		[ViewVariables]
		public float ColdDamageThreshold = 260f;

		// Token: 0x0400033B RID: 827
		[ViewVariables]
		public float? ParentHeatDamageThreshold;

		// Token: 0x0400033C RID: 828
		[ViewVariables]
		public float? ParentColdDamageThreshold;

		// Token: 0x0400033D RID: 829
		[DataField("specificHeat", false, 1, false, false, null)]
		[ViewVariables]
		public float SpecificHeat = 50f;

		// Token: 0x0400033E RID: 830
		[DataField("atmosTemperatureTransferEfficiency", false, 1, false, false, null)]
		[ViewVariables]
		public float AtmosTemperatureTransferEfficiency = 0.1f;

		// Token: 0x0400033F RID: 831
		[DataField("coldDamage", false, 1, false, false, null)]
		[ViewVariables]
		public DamageSpecifier ColdDamage = new DamageSpecifier();

		// Token: 0x04000340 RID: 832
		[DataField("heatDamage", false, 1, false, false, null)]
		[ViewVariables]
		public DamageSpecifier HeatDamage = new DamageSpecifier();

		// Token: 0x04000341 RID: 833
		[DataField("damageCap", false, 1, false, false, null)]
		[ViewVariables]
		public FixedPoint2 DamageCap = FixedPoint2.New(8);

		// Token: 0x04000342 RID: 834
		public bool TakingDamage;
	}
}
