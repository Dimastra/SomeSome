using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Eye.Blinding
{
	// Token: 0x0200049B RID: 1179
	public sealed class BlindnessChangedEvent : EntityEventArgs
	{
		// Token: 0x06000E57 RID: 3671 RVA: 0x0002E11F File Offset: 0x0002C31F
		public BlindnessChangedEvent(bool blind)
		{
			this.Blind = blind;
		}

		// Token: 0x04000D6F RID: 3439
		public bool Blind;
	}
}
