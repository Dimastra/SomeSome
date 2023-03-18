using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Store
{
	// Token: 0x02000126 RID: 294
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class StoreInitializeState : BoundUserInterfaceState
	{
		// Token: 0x06000367 RID: 871 RVA: 0x0000E750 File Offset: 0x0000C950
		public StoreInitializeState(string name)
		{
			this.Name = name;
		}

		// Token: 0x04000387 RID: 903
		public readonly string Name;
	}
}
