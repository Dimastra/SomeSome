using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.Body.Components
{
	// Token: 0x0200070F RID: 1807
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BeingGibbedEvent : EntityEventArgs
	{
		// Token: 0x06002616 RID: 9750 RVA: 0x000C9529 File Offset: 0x000C7729
		public BeingGibbedEvent(HashSet<EntityUid> gibbedParts)
		{
			this.GibbedParts = gibbedParts;
		}

		// Token: 0x04001787 RID: 6023
		public readonly HashSet<EntityUid> GibbedParts;
	}
}
