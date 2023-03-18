using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Roles
{
	// Token: 0x020001E6 RID: 486
	public sealed class DepartmentTimeRequirement : JobRequirement
	{
		// Token: 0x04000589 RID: 1417
		[Nullable(1)]
		[DataField("department", false, 1, false, false, typeof(PrototypeIdSerializer<DepartmentPrototype>))]
		public string Department;

		// Token: 0x0400058A RID: 1418
		[DataField("time", false, 1, false, false, null)]
		public TimeSpan Time;

		// Token: 0x0400058B RID: 1419
		[DataField("inverted", false, 1, false, false, null)]
		public bool Inverted;
	}
}
