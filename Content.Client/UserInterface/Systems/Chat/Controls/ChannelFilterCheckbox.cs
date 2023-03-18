using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chat;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;

namespace Content.Client.UserInterface.Systems.Chat.Controls
{
	// Token: 0x020000A8 RID: 168
	public sealed class ChannelFilterCheckbox : CheckBox
	{
		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000454 RID: 1108 RVA: 0x00018FDC File Offset: 0x000171DC
		public bool IsHidden
		{
			get
			{
				return base.Parent == null;
			}
		}

		// Token: 0x06000455 RID: 1109 RVA: 0x00018FE8 File Offset: 0x000171E8
		public ChannelFilterCheckbox(ChatChannel channel)
		{
			this.Channel = channel;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 1);
			defaultInterpolatedStringHandler.AppendLiteral("hud-chatbox-channel-");
			defaultInterpolatedStringHandler.AppendFormatted<ChatChannel>(this.Channel);
			base.Text = Loc.GetString(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x00019038 File Offset: 0x00017238
		private void UpdateText(int? unread)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 1);
			defaultInterpolatedStringHandler.AppendLiteral("hud-chatbox-channel-");
			defaultInterpolatedStringHandler.AppendFormatted<ChatChannel>(this.Channel);
			string text = Loc.GetString(defaultInterpolatedStringHandler.ToStringAndClear());
			int? num = unread;
			int num2 = 0;
			if (num.GetValueOrDefault() > num2 & num != null)
			{
				string str = text;
				string str2 = " (";
				num = unread;
				num2 = 9;
				string text2 = (num.GetValueOrDefault() > num2 & num != null) ? "9+" : unread;
				text = str + str2 + ((text2 != null) ? text2.ToString() : null) + ")";
			}
			base.Text = text;
		}

		// Token: 0x06000457 RID: 1111 RVA: 0x000190D7 File Offset: 0x000172D7
		public void UpdateUnreadCount(int? unread)
		{
			this.UpdateText(unread);
		}

		// Token: 0x04000216 RID: 534
		public readonly ChatChannel Channel;
	}
}
