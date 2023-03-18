using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;

namespace Content.Shared.Verbs
{
	// Token: 0x02000088 RID: 136
	[NetSerializable]
	[Serializable]
	public sealed class InteractionVerb : Verb
	{
		// Token: 0x17000042 RID: 66
		// (get) Token: 0x0600019D RID: 413 RVA: 0x00009481 File Offset: 0x00007681
		public override int TypePriority
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x0600019E RID: 414 RVA: 0x00009484 File Offset: 0x00007684
		public override bool DefaultDoContactInteraction
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600019F RID: 415 RVA: 0x00009487 File Offset: 0x00007687
		public InteractionVerb()
		{
			this.TextStyleClass = InteractionVerb.DefaultTextStyleClass;
		}

		// Token: 0x040001CE RID: 462
		[Nullable(1)]
		public new static string DefaultTextStyleClass = "InteractionVerb";
	}
}
