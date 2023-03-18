using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Verbs
{
	// Token: 0x02000091 RID: 145
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ExecuteVerbEvent : EntityEventArgs
	{
		// Token: 0x060001B5 RID: 437 RVA: 0x000097AF File Offset: 0x000079AF
		public ExecuteVerbEvent(EntityUid target, Verb requestedVerb)
		{
			this.Target = target;
			this.RequestedVerb = requestedVerb;
		}

		// Token: 0x040001EF RID: 495
		public readonly EntityUid Target;

		// Token: 0x040001F0 RID: 496
		public readonly Verb RequestedVerb;
	}
}
