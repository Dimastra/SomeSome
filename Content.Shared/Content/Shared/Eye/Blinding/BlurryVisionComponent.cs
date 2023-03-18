using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Eye.Blinding
{
	// Token: 0x02000498 RID: 1176
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class BlurryVisionComponent : Component
	{
		// Token: 0x170002FD RID: 765
		// (get) Token: 0x06000E46 RID: 3654 RVA: 0x0002DCF7 File Offset: 0x0002BEF7
		public bool Active
		{
			get
			{
				return this.Magnitude < 10f;
			}
		}

		// Token: 0x04000D6C RID: 3436
		[DataField("mangitude", false, 1, false, false, null)]
		public float Magnitude = 1f;
	}
}
