using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;

namespace Content.Shared.Verbs
{
	// Token: 0x0200008C RID: 140
	[NetSerializable]
	[Serializable]
	public sealed class ActivationVerb : Verb
	{
		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060001AA RID: 426 RVA: 0x000094FA File Offset: 0x000076FA
		public override int TypePriority
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060001AB RID: 427 RVA: 0x000094FD File Offset: 0x000076FD
		public override bool DefaultDoContactInteraction
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060001AC RID: 428 RVA: 0x00009500 File Offset: 0x00007700
		public ActivationVerb()
		{
			this.TextStyleClass = ActivationVerb.DefaultTextStyleClass;
		}

		// Token: 0x040001D0 RID: 464
		[Nullable(1)]
		public new static string DefaultTextStyleClass = "ActivationVerb";
	}
}
