using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Revenant
{
	// Token: 0x020001EC RID: 492
	public sealed class SoulSearchDoAfterComplete : EntityEventArgs
	{
		// Token: 0x06000589 RID: 1417 RVA: 0x00014302 File Offset: 0x00012502
		public SoulSearchDoAfterComplete(EntityUid target)
		{
			this.Target = target;
		}

		// Token: 0x04000597 RID: 1431
		public readonly EntityUid Target;
	}
}
