using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Eye.Blinding
{
	// Token: 0x02000496 RID: 1174
	[NetSerializable]
	[Serializable]
	public sealed class BlindableComponentState : ComponentState
	{
		// Token: 0x06000E44 RID: 3652 RVA: 0x0002DCE0 File Offset: 0x0002BEE0
		public BlindableComponentState(int sources)
		{
			this.Sources = sources;
		}

		// Token: 0x04000D6A RID: 3434
		public readonly int Sources;
	}
}
