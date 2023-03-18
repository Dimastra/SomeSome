using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Verbs
{
	// Token: 0x0200008A RID: 138
	[NetSerializable]
	[Serializable]
	public sealed class InnateVerb : Verb
	{
		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060001A4 RID: 420 RVA: 0x000094BF File Offset: 0x000076BF
		public override int TypePriority
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x000094C2 File Offset: 0x000076C2
		public InnateVerb()
		{
			this.TextStyleClass = InteractionVerb.DefaultTextStyleClass;
		}
	}
}
