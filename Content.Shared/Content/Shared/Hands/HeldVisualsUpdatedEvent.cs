using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Hands
{
	// Token: 0x02000429 RID: 1065
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HeldVisualsUpdatedEvent : EntityEventArgs
	{
		// Token: 0x06000CCC RID: 3276 RVA: 0x0002A446 File Offset: 0x00028646
		public HeldVisualsUpdatedEvent(EntityUid user, HashSet<string> revealedLayers)
		{
			this.User = user;
			this.RevealedLayers = revealedLayers;
		}

		// Token: 0x04000C9B RID: 3227
		public readonly EntityUid User;

		// Token: 0x04000C9C RID: 3228
		public HashSet<string> RevealedLayers;
	}
}
