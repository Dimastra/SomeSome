using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.Guidebook.Richtext
{
	// Token: 0x020002EF RID: 751
	public sealed class Document : BoxContainer, IDocumentTag
	{
		// Token: 0x060012E6 RID: 4838 RVA: 0x00070923 File Offset: 0x0006EB23
		public Document()
		{
			base.Orientation = 1;
		}

		// Token: 0x060012E7 RID: 4839 RVA: 0x00070932 File Offset: 0x0006EB32
		[NullableContext(1)]
		public bool TryParseTag(Dictionary<string, string> args, [Nullable(2)] [NotNullWhen(true)] out Control control)
		{
			control = this;
			return true;
		}
	}
}
