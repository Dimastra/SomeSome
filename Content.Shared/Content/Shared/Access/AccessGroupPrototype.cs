using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Shared.Access
{
	// Token: 0x0200076D RID: 1901
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("accessGroup", 1)]
	public sealed class AccessGroupPrototype : IPrototype
	{
		// Token: 0x170004E7 RID: 1255
		// (get) Token: 0x06001778 RID: 6008 RVA: 0x0004C4C2 File Offset: 0x0004A6C2
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x04001742 RID: 5954
		[DataField("tags", false, 1, true, false, typeof(PrototypeIdHashSetSerializer<AccessLevelPrototype>))]
		public HashSet<string> Tags;
	}
}
