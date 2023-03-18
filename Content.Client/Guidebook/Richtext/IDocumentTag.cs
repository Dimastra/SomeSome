using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Robust.Client.UserInterface;

namespace Content.Client.Guidebook.Richtext
{
	// Token: 0x020002F0 RID: 752
	[NullableContext(1)]
	public interface IDocumentTag
	{
		// Token: 0x060012E8 RID: 4840
		bool TryParseTag(Dictionary<string, string> args, [Nullable(2)] [NotNullWhen(true)] out Control control);
	}
}
