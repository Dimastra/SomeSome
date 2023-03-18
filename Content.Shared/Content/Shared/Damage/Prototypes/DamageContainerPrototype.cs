using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Damage.Prototypes
{
	// Token: 0x0200053A RID: 1338
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("damageContainer", 1)]
	[NetSerializable]
	[Serializable]
	public sealed class DamageContainerPrototype : IPrototype
	{
		// Token: 0x17000335 RID: 821
		// (get) Token: 0x06001058 RID: 4184 RVA: 0x00035C28 File Offset: 0x00033E28
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x04000F5D RID: 3933
		[DataField("supportedGroups", false, 1, false, false, typeof(PrototypeIdListSerializer<DamageGroupPrototype>))]
		public List<string> SupportedGroups = new List<string>();

		// Token: 0x04000F5E RID: 3934
		[DataField("supportedTypes", false, 1, false, false, typeof(PrototypeIdListSerializer<DamageTypePrototype>))]
		public List<string> SupportedTypes = new List<string>();
	}
}
