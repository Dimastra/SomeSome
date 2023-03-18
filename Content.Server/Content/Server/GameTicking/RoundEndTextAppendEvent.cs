using System;
using System.Runtime.CompilerServices;

namespace Content.Server.GameTicking
{
	// Token: 0x020004B3 RID: 1203
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RoundEndTextAppendEvent
	{
		// Token: 0x1700036B RID: 875
		// (get) Token: 0x06001879 RID: 6265 RVA: 0x0007F86F File Offset: 0x0007DA6F
		// (set) Token: 0x0600187A RID: 6266 RVA: 0x0007F877 File Offset: 0x0007DA77
		public string Text { get; private set; } = string.Empty;

		// Token: 0x0600187B RID: 6267 RVA: 0x0007F880 File Offset: 0x0007DA80
		public void AddLine(string text)
		{
			if (this._doNewLine)
			{
				this.Text += "\n";
			}
			this.Text += text;
			this._doNewLine = true;
		}

		// Token: 0x04000F3B RID: 3899
		private bool _doNewLine;
	}
}
