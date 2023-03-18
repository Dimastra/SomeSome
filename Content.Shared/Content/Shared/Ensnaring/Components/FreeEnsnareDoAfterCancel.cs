using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Ensnaring.Components
{
	// Token: 0x020004C2 RID: 1218
	public sealed class FreeEnsnareDoAfterCancel : EntityEventArgs
	{
		// Token: 0x06000EB3 RID: 3763 RVA: 0x0002F4E4 File Offset: 0x0002D6E4
		public FreeEnsnareDoAfterCancel(EntityUid ensnaringEntity)
		{
			this.EnsnaringEntity = ensnaringEntity;
		}

		// Token: 0x04000DDA RID: 3546
		public readonly EntityUid EnsnaringEntity;
	}
}
