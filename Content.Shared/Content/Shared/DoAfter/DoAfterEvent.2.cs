using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.DoAfter
{
	// Token: 0x020004F5 RID: 1269
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DoAfterEvent<[Nullable(2)] T> : HandledEntityEventArgs
	{
		// Token: 0x06000F5A RID: 3930 RVA: 0x000318B7 File Offset: 0x0002FAB7
		public DoAfterEvent(T additionalData, bool cancelled, DoAfterEventArgs args)
		{
			this.AdditionalData = additionalData;
			this.Cancelled = cancelled;
			this.Args = args;
		}

		// Token: 0x04000E99 RID: 3737
		public T AdditionalData;

		// Token: 0x04000E9A RID: 3738
		public bool Cancelled;

		// Token: 0x04000E9B RID: 3739
		public readonly DoAfterEventArgs Args;
	}
}
