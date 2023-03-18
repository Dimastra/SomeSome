using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Shared.Damage.Prototypes
{
	// Token: 0x0200053B RID: 1339
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("damageGroup", 2)]
	[NetSerializable]
	[Serializable]
	public sealed class DamageGroupPrototype : IPrototype
	{
		// Token: 0x17000336 RID: 822
		// (get) Token: 0x0600105A RID: 4186 RVA: 0x00035C4E File Offset: 0x00033E4E
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000337 RID: 823
		// (get) Token: 0x0600105B RID: 4187 RVA: 0x00035C56 File Offset: 0x00033E56
		[DataField("damageTypes", false, 1, true, false, typeof(PrototypeIdListSerializer<DamageTypePrototype>))]
		public List<string> DamageTypes { get; }
	}
}
