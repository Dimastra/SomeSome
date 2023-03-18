using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Server.Xenoarchaeology.XenoArtifacts.Triggers.Components
{
	// Token: 0x02000030 RID: 48
	[RegisterComponent]
	public sealed class ArtifactDamageTriggerComponent : Component
	{
		// Token: 0x04000077 RID: 119
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("damageTypes", false, 1, false, false, typeof(PrototypeIdListSerializer<DamageTypePrototype>))]
		public List<string> DamageTypes;

		// Token: 0x04000078 RID: 120
		[DataField("damageThreshold", false, 1, true, false, null)]
		public float DamageThreshold;

		// Token: 0x04000079 RID: 121
		[ViewVariables]
		public float AccumulatedDamage;
	}
}
