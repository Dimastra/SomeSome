using System;
using System.Runtime.CompilerServices;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Controls
{
	// Token: 0x020000DA RID: 218
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ListContainerButton : ContainerButton
	{
		// Token: 0x0600061C RID: 1564 RVA: 0x00021469 File Offset: 0x0001F669
		public ListContainerButton(ListData data)
		{
			this.Data = data;
		}

		// Token: 0x040002C2 RID: 706
		public readonly ListData Data;
	}
}
