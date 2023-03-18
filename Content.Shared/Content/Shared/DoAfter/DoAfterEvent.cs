using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.DoAfter
{
	// Token: 0x020004F4 RID: 1268
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class DoAfterEvent : HandledEntityEventArgs
	{
		// Token: 0x06000F59 RID: 3929 RVA: 0x000318A1 File Offset: 0x0002FAA1
		public DoAfterEvent(bool cancelled, DoAfterEventArgs args)
		{
			this.Cancelled = cancelled;
			this.Args = args;
		}

		// Token: 0x04000E97 RID: 3735
		public bool Cancelled;

		// Token: 0x04000E98 RID: 3736
		public readonly DoAfterEventArgs Args;
	}
}
