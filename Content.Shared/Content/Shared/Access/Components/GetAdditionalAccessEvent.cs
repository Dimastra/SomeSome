using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Access.Components
{
	// Token: 0x0200077A RID: 1914
	[ByRefEvent]
	public struct GetAdditionalAccessEvent
	{
		// Token: 0x060017A8 RID: 6056 RVA: 0x0004CDFF File Offset: 0x0004AFFF
		public GetAdditionalAccessEvent()
		{
			this.Entities = new HashSet<EntityUid>();
		}

		// Token: 0x04001759 RID: 5977
		[Nullable(1)]
		public HashSet<EntityUid> Entities;
	}
}
