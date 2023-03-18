using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions.ActionTypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.RatKing
{
	// Token: 0x02000250 RID: 592
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class RatKingComponent : Component
	{
		// Token: 0x04000758 RID: 1880
		[DataField("actionRaiseArmy", false, 1, true, false, null)]
		public InstantAction ActionRaiseArmy = new InstantAction();

		// Token: 0x04000759 RID: 1881
		[ViewVariables]
		[DataField("hungerPerArmyUse", false, 1, true, false, null)]
		public float HungerPerArmyUse = 25f;

		// Token: 0x0400075A RID: 1882
		[ViewVariables]
		[DataField("armyMobSpawnId", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string ArmyMobSpawnId = "MobRatServant";

		// Token: 0x0400075B RID: 1883
		[DataField("actionDomain", false, 1, true, false, null)]
		public InstantAction ActionDomain = new InstantAction();

		// Token: 0x0400075C RID: 1884
		[ViewVariables]
		[DataField("hungerPerDomainUse", false, 1, true, false, null)]
		public float HungerPerDomainUse = 50f;

		// Token: 0x0400075D RID: 1885
		[DataField("molesMiasmaPerDomain", false, 1, false, false, null)]
		public float MolesMiasmaPerDomain = 100f;
	}
}
