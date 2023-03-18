using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Objectives;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CharacterInfo
{
	// Token: 0x02000610 RID: 1552
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class CharacterInfoEvent : EntityEventArgs
	{
		// Token: 0x060012F3 RID: 4851 RVA: 0x0003E1B6 File Offset: 0x0003C3B6
		public CharacterInfoEvent(EntityUid entityUid, string jobTitle, Dictionary<string, List<ConditionInfo>> objectives, string briefing)
		{
			this.EntityUid = entityUid;
			this.JobTitle = jobTitle;
			this.Objectives = objectives;
			this.Briefing = briefing;
		}

		// Token: 0x040011BD RID: 4541
		public readonly EntityUid EntityUid;

		// Token: 0x040011BE RID: 4542
		public readonly string JobTitle;

		// Token: 0x040011BF RID: 4543
		public readonly Dictionary<string, List<ConditionInfo>> Objectives;

		// Token: 0x040011C0 RID: 4544
		public readonly string Briefing;
	}
}
