using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Verbs
{
	// Token: 0x0200008D RID: 141
	[NetSerializable]
	[Serializable]
	public sealed class ExamineVerb : Verb
	{
		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060001AE RID: 430 RVA: 0x0000951F File Offset: 0x0000771F
		public override int TypePriority
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060001AF RID: 431 RVA: 0x00009522 File Offset: 0x00007722
		public override bool CloseMenuDefault
		{
			get
			{
				return false;
			}
		}

		// Token: 0x040001D1 RID: 465
		public bool ShowOnExamineTooltip = true;
	}
}
