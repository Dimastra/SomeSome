using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Roles
{
	// Token: 0x020001E2 RID: 482
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("department", 1)]
	public sealed class DepartmentPrototype : IPrototype
	{
		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x06000562 RID: 1378 RVA: 0x00013CC7 File Offset: 0x00011EC7
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x04000571 RID: 1393
		[DataField("description", false, 1, true, false, null)]
		public string Description;

		// Token: 0x04000572 RID: 1394
		[DataField("color", false, 1, true, false, null)]
		public Color Color;

		// Token: 0x04000573 RID: 1395
		[ViewVariables]
		[DataField("roles", false, 1, false, false, typeof(PrototypeIdListSerializer<JobPrototype>))]
		public List<string> Roles = new List<string>();
	}
}
