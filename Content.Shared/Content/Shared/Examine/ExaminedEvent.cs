using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;

namespace Content.Shared.Examine
{
	// Token: 0x020004AC RID: 1196
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ExaminedEvent : EntityEventArgs
	{
		// Token: 0x17000301 RID: 769
		// (get) Token: 0x06000E7D RID: 3709 RVA: 0x0002EDD1 File Offset: 0x0002CFD1
		public FormattedMessage Message { get; }

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x06000E7E RID: 3710 RVA: 0x0002EDD9 File Offset: 0x0002CFD9
		public EntityUid Examiner { get; }

		// Token: 0x17000303 RID: 771
		// (get) Token: 0x06000E7F RID: 3711 RVA: 0x0002EDE1 File Offset: 0x0002CFE1
		public EntityUid Examined { get; }

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x06000E80 RID: 3712 RVA: 0x0002EDE9 File Offset: 0x0002CFE9
		public bool IsInDetailsRange { get; }

		// Token: 0x06000E81 RID: 3713 RVA: 0x0002EDF1 File Offset: 0x0002CFF1
		public ExaminedEvent(FormattedMessage message, EntityUid examined, EntityUid examiner, bool isInDetailsRange, bool doNewLine)
		{
			this.Message = message;
			this.Examined = examined;
			this.Examiner = examiner;
			this.IsInDetailsRange = isInDetailsRange;
			this._doNewLine = doNewLine;
		}

		// Token: 0x06000E82 RID: 3714 RVA: 0x0002EE1E File Offset: 0x0002D01E
		public void PushMessage(FormattedMessage message)
		{
			if (message.Nodes.Count == 0)
			{
				return;
			}
			if (this._doNewLine)
			{
				this.Message.AddText("\n");
			}
			this.Message.AddMessage(message);
			this._doNewLine = true;
		}

		// Token: 0x06000E83 RID: 3715 RVA: 0x0002EE59 File Offset: 0x0002D059
		public void PushMarkup(string markup)
		{
			this.PushMessage(FormattedMessage.FromMarkup(markup));
		}

		// Token: 0x06000E84 RID: 3716 RVA: 0x0002EE68 File Offset: 0x0002D068
		public void PushText(string text)
		{
			FormattedMessage msg = new FormattedMessage();
			msg.AddText(text);
			this.PushMessage(msg);
		}

		// Token: 0x04000DAB RID: 3499
		private bool _doNewLine;
	}
}
