using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Revenant
{
	// Token: 0x020001EE RID: 494
	public sealed class HarvestDoAfterComplete : EntityEventArgs
	{
		// Token: 0x0600058B RID: 1419 RVA: 0x00014319 File Offset: 0x00012519
		public HarvestDoAfterComplete(EntityUid target)
		{
			this.Target = target;
		}

		// Token: 0x04000598 RID: 1432
		public readonly EntityUid Target;
	}
}
