using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Examine
{
	// Token: 0x020004AA RID: 1194
	public static class ExamineSystemMessages
	{
		// Token: 0x0200080F RID: 2063
		[NetSerializable]
		[Serializable]
		public sealed class RequestExamineInfoMessage : EntityEventArgs
		{
			// Token: 0x060018D6 RID: 6358 RVA: 0x0004EEAC File Offset: 0x0004D0AC
			public RequestExamineInfoMessage(EntityUid entityUid, int id, bool getVerbs = false)
			{
				this.EntityUid = entityUid;
				this.Id = id;
				this.GetVerbs = getVerbs;
			}

			// Token: 0x040018BB RID: 6331
			public readonly EntityUid EntityUid;

			// Token: 0x040018BC RID: 6332
			public readonly int Id;

			// Token: 0x040018BD RID: 6333
			public readonly bool GetVerbs;
		}

		// Token: 0x02000810 RID: 2064
		[NullableContext(1)]
		[Nullable(0)]
		[NetSerializable]
		[Serializable]
		public sealed class ExamineInfoResponseMessage : EntityEventArgs
		{
			// Token: 0x060018D7 RID: 6359 RVA: 0x0004EEC9 File Offset: 0x0004D0C9
			public ExamineInfoResponseMessage(EntityUid entityUid, int id, FormattedMessage message, [Nullable(new byte[]
			{
				2,
				1
			})] List<Verb> verbs = null, bool centerAtCursor = true, bool openAtOldTooltip = true, bool knowTarget = true)
			{
				this.EntityUid = entityUid;
				this.Id = id;
				this.Message = message;
				this.Verbs = verbs;
				this.CenterAtCursor = centerAtCursor;
				this.OpenAtOldTooltip = openAtOldTooltip;
				this.KnowTarget = knowTarget;
			}

			// Token: 0x040018BE RID: 6334
			public readonly EntityUid EntityUid;

			// Token: 0x040018BF RID: 6335
			public readonly int Id;

			// Token: 0x040018C0 RID: 6336
			public readonly FormattedMessage Message;

			// Token: 0x040018C1 RID: 6337
			[Nullable(new byte[]
			{
				2,
				1
			})]
			public List<Verb> Verbs;

			// Token: 0x040018C2 RID: 6338
			public readonly bool CenterAtCursor;

			// Token: 0x040018C3 RID: 6339
			public readonly bool OpenAtOldTooltip;

			// Token: 0x040018C4 RID: 6340
			public readonly bool KnowTarget;
		}
	}
}
