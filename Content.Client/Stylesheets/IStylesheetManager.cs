using System;
using System.Runtime.CompilerServices;
using Robust.Client.UserInterface;

namespace Content.Client.Stylesheets
{
	// Token: 0x02000110 RID: 272
	[NullableContext(1)]
	public interface IStylesheetManager
	{
		// Token: 0x17000155 RID: 341
		// (get) Token: 0x0600079F RID: 1951
		Stylesheet SheetNano { get; }

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x060007A0 RID: 1952
		Stylesheet SheetSpace { get; }

		// Token: 0x060007A1 RID: 1953
		void Initialize();
	}
}
