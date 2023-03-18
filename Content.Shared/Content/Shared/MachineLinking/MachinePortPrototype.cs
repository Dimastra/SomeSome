using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.MachineLinking
{
	// Token: 0x02000347 RID: 839
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class MachinePortPrototype
	{
		// Token: 0x04000996 RID: 2454
		[DataField("name", false, 1, true, false, null)]
		public string Name;

		// Token: 0x04000997 RID: 2455
		[DataField("description", false, 1, true, false, null)]
		public string Description;
	}
}
