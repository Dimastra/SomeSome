using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Verbs
{
	// Token: 0x02000092 RID: 146
	public sealed class GetVerbsEvent<TVerb> : EntityEventArgs where TVerb : Verb
	{
		// Token: 0x060001B6 RID: 438 RVA: 0x000097C5 File Offset: 0x000079C5
		[NullableContext(2)]
		public GetVerbsEvent(EntityUid user, EntityUid target, EntityUid? @using, SharedHandsComponent hands, bool canInteract, bool canAccess)
		{
			this.User = user;
			this.Target = target;
			this.Using = @using;
			this.Hands = hands;
			this.CanAccess = canAccess;
			this.CanInteract = canInteract;
		}

		// Token: 0x040001F1 RID: 497
		[Nullable(1)]
		public readonly SortedSet<TVerb> Verbs = new SortedSet<TVerb>();

		// Token: 0x040001F2 RID: 498
		public readonly bool CanAccess;

		// Token: 0x040001F3 RID: 499
		public readonly EntityUid Target;

		// Token: 0x040001F4 RID: 500
		public readonly EntityUid User;

		// Token: 0x040001F5 RID: 501
		public readonly bool CanInteract;

		// Token: 0x040001F6 RID: 502
		[Nullable(2)]
		public readonly SharedHandsComponent Hands;

		// Token: 0x040001F7 RID: 503
		public readonly EntityUid? Using;
	}
}
