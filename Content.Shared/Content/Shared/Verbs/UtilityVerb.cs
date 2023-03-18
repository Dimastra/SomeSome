using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Verbs
{
	// Token: 0x02000089 RID: 137
	[NetSerializable]
	[Serializable]
	public sealed class UtilityVerb : Verb
	{
		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060001A1 RID: 417 RVA: 0x000094A6 File Offset: 0x000076A6
		public override int TypePriority
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060001A2 RID: 418 RVA: 0x000094A9 File Offset: 0x000076A9
		public override bool DefaultDoContactInteraction
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x000094AC File Offset: 0x000076AC
		public UtilityVerb()
		{
			this.TextStyleClass = InteractionVerb.DefaultTextStyleClass;
		}
	}
}
