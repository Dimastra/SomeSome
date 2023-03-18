using System;
using System.Runtime.CompilerServices;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Utility;

namespace Content.Client.Message
{
	// Token: 0x02000230 RID: 560
	public static class RichTextLabelExt
	{
		// Token: 0x06000E6A RID: 3690 RVA: 0x000570AF File Offset: 0x000552AF
		[NullableContext(1)]
		public static void SetMarkup(this RichTextLabel label, string markup)
		{
			label.SetMessage(FormattedMessage.FromMarkup(markup));
		}
	}
}
