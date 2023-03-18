using System;
using System.Runtime.CompilerServices;
using Content.Server.Body.Systems;
using Content.Server.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Body.Components
{
	// Token: 0x02000710 RID: 1808
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(BloodstreamSystem),
		typeof(ChemistrySystem)
	})]
	public sealed class BloodstreamComponent : Component
	{
		// Token: 0x1700059D RID: 1437
		// (get) Token: 0x06002617 RID: 9751 RVA: 0x000C9538 File Offset: 0x000C7738
		public bool IsBleeding
		{
			get
			{
				return this.BleedAmount > 0f;
			}
		}

		// Token: 0x04001788 RID: 6024
		public static string DefaultChemicalsSolutionName = "chemicals";

		// Token: 0x04001789 RID: 6025
		public static string DefaultBloodSolutionName = "bloodstream";

		// Token: 0x0400178A RID: 6026
		public static string DefaultBloodTemporarySolutionName = "bloodstreamTemporary";

		// Token: 0x0400178B RID: 6027
		public float AccumulatedFrametime;

		// Token: 0x0400178C RID: 6028
		[ViewVariables]
		public float BleedAmount;

		// Token: 0x0400178D RID: 6029
		[DataField("bleedReductionAmount", false, 1, false, false, null)]
		public float BleedReductionAmount = 1f;

		// Token: 0x0400178E RID: 6030
		[DataField("maxBleedAmount", false, 1, false, false, null)]
		public float MaxBleedAmount = 20f;

		// Token: 0x0400178F RID: 6031
		[DataField("bloodlossThreshold", false, 1, false, false, null)]
		public float BloodlossThreshold = 0.9f;

		// Token: 0x04001790 RID: 6032
		[DataField("bloodlossDamage", false, 1, true, false, null)]
		public DamageSpecifier BloodlossDamage = new DamageSpecifier();

		// Token: 0x04001791 RID: 6033
		[DataField("bloodlossHealDamage", false, 1, true, false, null)]
		public DamageSpecifier BloodlossHealDamage = new DamageSpecifier();

		// Token: 0x04001792 RID: 6034
		[DataField("updateInterval", false, 1, false, false, null)]
		public float UpdateInterval = 5f;

		// Token: 0x04001793 RID: 6035
		[DataField("bloodRefreshAmount", false, 1, false, false, null)]
		public float BloodRefreshAmount = 0.2f;

		// Token: 0x04001794 RID: 6036
		[DataField("bleedPuddleThreshold", false, 1, false, false, null)]
		public FixedPoint2 BleedPuddleThreshold = 5f;

		// Token: 0x04001795 RID: 6037
		[DataField("damageBleedModifiers", false, 1, false, false, typeof(PrototypeIdSerializer<DamageModifierSetPrototype>))]
		public string DamageBleedModifiers = "BloodlossHuman";

		// Token: 0x04001796 RID: 6038
		[DataField("instantBloodSound", false, 1, false, false, null)]
		public SoundSpecifier InstantBloodSound = new SoundCollectionSpecifier("blood", null);

		// Token: 0x04001797 RID: 6039
		[DataField("bloodHealedSound", false, 1, false, false, null)]
		public SoundSpecifier BloodHealedSound = new SoundPathSpecifier("/Audio/Effects/lightburn.ogg", null);

		// Token: 0x04001798 RID: 6040
		[DataField("chemicalMaxVolume", false, 1, false, false, null)]
		public FixedPoint2 ChemicalMaxVolume = FixedPoint2.New(250);

		// Token: 0x04001799 RID: 6041
		[DataField("bloodMaxVolume", false, 1, false, false, null)]
		public FixedPoint2 BloodMaxVolume = FixedPoint2.New(300);

		// Token: 0x0400179A RID: 6042
		[DataField("bloodReagent", false, 1, false, false, null)]
		public string BloodReagent = "Blood";

		// Token: 0x0400179B RID: 6043
		[ViewVariables]
		[Access]
		public Solution ChemicalSolution;

		// Token: 0x0400179C RID: 6044
		[ViewVariables]
		public Solution BloodSolution;

		// Token: 0x0400179D RID: 6045
		[ViewVariables]
		public Solution BloodTemporarySolution;
	}
}
