using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Buckle.Components
{
	// Token: 0x02000643 RID: 1603
	public sealed class BuckleChangeEvent : EntityEventArgs
	{
		// Token: 0x04001351 RID: 4945
		public EntityUid Strap;

		// Token: 0x04001352 RID: 4946
		public EntityUid BuckledEntity;

		// Token: 0x04001353 RID: 4947
		public bool Buckling;
	}
}
