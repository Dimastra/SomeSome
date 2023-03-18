using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Verbs
{
	// Token: 0x02000090 RID: 144
	[NetSerializable]
	[Serializable]
	public sealed class VerbsResponseEvent : EntityEventArgs
	{
		// Token: 0x060001B4 RID: 436 RVA: 0x00009790 File Offset: 0x00007990
		public VerbsResponseEvent(EntityUid entity, [Nullable(new byte[]
		{
			2,
			1
		})] SortedSet<Verb> verbs)
		{
			this.Entity = entity;
			if (verbs == null)
			{
				return;
			}
			this.Verbs = new List<Verb>(verbs);
		}

		// Token: 0x040001ED RID: 493
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public readonly List<Verb> Verbs;

		// Token: 0x040001EE RID: 494
		public readonly EntityUid Entity;
	}
}
