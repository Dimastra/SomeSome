using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Verbs
{
	// Token: 0x0200008F RID: 143
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class RequestServerVerbsEvent : EntityEventArgs
	{
		// Token: 0x060001B3 RID: 435 RVA: 0x00009710 File Offset: 0x00007910
		public RequestServerVerbsEvent(EntityUid entityUid, List<Type> verbTypes, EntityUid? slotOwner = null, bool adminRequest = false)
		{
			this.EntityUid = entityUid;
			this.SlotOwner = slotOwner;
			this.AdminRequest = adminRequest;
			foreach (Type type in verbTypes)
			{
				this.VerbTypes.Add(type.Name);
			}
		}

		// Token: 0x040001E9 RID: 489
		public readonly EntityUid EntityUid;

		// Token: 0x040001EA RID: 490
		public readonly List<string> VerbTypes = new List<string>();

		// Token: 0x040001EB RID: 491
		public readonly EntityUid? SlotOwner;

		// Token: 0x040001EC RID: 492
		public readonly bool AdminRequest;
	}
}
