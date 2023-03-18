using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;

namespace Content.Shared.Verbs
{
	// Token: 0x0200008B RID: 139
	[NetSerializable]
	[Serializable]
	public sealed class AlternativeVerb : Verb
	{
		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060001A6 RID: 422 RVA: 0x000094D5 File Offset: 0x000076D5
		public override int TypePriority
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060001A7 RID: 423 RVA: 0x000094D8 File Offset: 0x000076D8
		public override bool DefaultDoContactInteraction
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x000094DB File Offset: 0x000076DB
		public AlternativeVerb()
		{
			this.TextStyleClass = AlternativeVerb.DefaultTextStyleClass;
		}

		// Token: 0x040001CF RID: 463
		[Nullable(1)]
		public new static string DefaultTextStyleClass = "AlternativeVerb";
	}
}
