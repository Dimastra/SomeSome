using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Damage
{
	// Token: 0x0200052D RID: 1325
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	[Access]
	public sealed class DamageableComponent : Component
	{
		// Token: 0x04000F30 RID: 3888
		[Nullable(2)]
		[DataField("damageContainer", false, 1, false, false, typeof(PrototypeIdSerializer<DamageContainerPrototype>))]
		public string DamageContainerID;

		// Token: 0x04000F31 RID: 3889
		[Nullable(2)]
		[DataField("damageModifierSet", false, 1, false, false, typeof(PrototypeIdSerializer<DamageModifierSetPrototype>))]
		public string DamageModifierSetId;

		// Token: 0x04000F32 RID: 3890
		[DataField("damage", true, 1, false, false, null)]
		public DamageSpecifier Damage = new DamageSpecifier();

		// Token: 0x04000F33 RID: 3891
		[ViewVariables]
		public Dictionary<string, FixedPoint2> DamagePerGroup = new Dictionary<string, FixedPoint2>();

		// Token: 0x04000F34 RID: 3892
		[ViewVariables]
		public FixedPoint2 TotalDamage;

		// Token: 0x04000F35 RID: 3893
		[DataField("radiationDamageTypes", false, 1, false, false, typeof(PrototypeIdListSerializer<DamageTypePrototype>))]
		public List<string> RadiationDamageTypeIDs = new List<string>
		{
			"Radiation"
		};
	}
}
