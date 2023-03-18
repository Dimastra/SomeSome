using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.Analyzers;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

namespace Content.Shared.Damage
{
	// Token: 0x0200052F RID: 1327
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	[NetSerializable]
	[Virtual]
	[Serializable]
	public class DamageModifierSet
	{
		// Token: 0x04000F38 RID: 3896
		[DataField("coefficients", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<float, DamageTypePrototype>))]
		public Dictionary<string, float> Coefficients = new Dictionary<string, float>();

		// Token: 0x04000F39 RID: 3897
		[DataField("flatReductions", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<float, DamageTypePrototype>))]
		public Dictionary<string, float> FlatReduction = new Dictionary<string, float>();
	}
}
