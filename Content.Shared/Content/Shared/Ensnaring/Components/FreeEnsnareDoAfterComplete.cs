using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Ensnaring.Components
{
	// Token: 0x020004C1 RID: 1217
	public sealed class FreeEnsnareDoAfterComplete : EntityEventArgs
	{
		// Token: 0x06000EB2 RID: 3762 RVA: 0x0002F4D5 File Offset: 0x0002D6D5
		public FreeEnsnareDoAfterComplete(EntityUid ensnaringEntity)
		{
			this.EnsnaringEntity = ensnaringEntity;
		}

		// Token: 0x04000DD9 RID: 3545
		public readonly EntityUid EnsnaringEntity;
	}
}
