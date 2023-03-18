using System;
using Content.Shared.Chat;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;

namespace Content.Client.UserInterface.Systems.Chat.Controls
{
	// Token: 0x020000AB RID: 171
	public sealed class ChannelSelectorItemButton : Button
	{
		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000472 RID: 1138 RVA: 0x00018FDC File Offset: 0x000171DC
		public bool IsHidden
		{
			get
			{
				return base.Parent == null;
			}
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x000197A8 File Offset: 0x000179A8
		public ChannelSelectorItemButton(ChatSelectChannel selector)
		{
			this.Channel = selector;
			base.AddStyleClass("chatSelectorOptionButton");
			base.Text = ChannelSelectorButton.ChannelSelectorName(selector);
			char c = ChatUIController.ChannelPrefixes[selector];
			if (c != '\0')
			{
				base.Text = Loc.GetString("hud-chatbox-select-name-prefixed", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("name", base.Text),
					new ValueTuple<string, object>("prefix", c)
				});
			}
		}

		// Token: 0x0400021E RID: 542
		public readonly ChatSelectChannel Channel;
	}
}
