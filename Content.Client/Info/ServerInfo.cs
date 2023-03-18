using System;
using System.Runtime.CompilerServices;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Utility;

namespace Content.Client.Info
{
	// Token: 0x020002C3 RID: 707
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ServerInfo : BoxContainer
	{
		// Token: 0x060011CF RID: 4559 RVA: 0x000698A5 File Offset: 0x00067AA5
		public ServerInfo()
		{
			base.Orientation = 1;
			this._richTextLabel = new RichTextLabel
			{
				VerticalExpand = true
			};
			base.AddChild(this._richTextLabel);
		}

		// Token: 0x060011D0 RID: 4560 RVA: 0x000698D2 File Offset: 0x00067AD2
		public void SetInfoBlob(string markup)
		{
			this._richTextLabel.SetMessage(FormattedMessage.FromMarkup(markup));
		}

		// Token: 0x040008BF RID: 2239
		private readonly RichTextLabel _richTextLabel;
	}
}
