using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chat;
using Content.Shared.Input;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.Localization;

namespace Content.Client.UserInterface.Systems.Chat.Controls
{
	// Token: 0x020000AD RID: 173
	[NullableContext(1)]
	[Nullable(0)]
	[Virtual]
	public class ChatInputBox : PanelContainer
	{
		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x0600047E RID: 1150 RVA: 0x00019B28 File Offset: 0x00017D28
		// (set) Token: 0x0600047F RID: 1151 RVA: 0x00019B30 File Offset: 0x00017D30
		private protected ChatChannel ActiveChannel { protected get; private set; } = ChatChannel.Local;

		// Token: 0x06000480 RID: 1152 RVA: 0x00019B3C File Offset: 0x00017D3C
		public ChatInputBox()
		{
			this.Container = new BoxContainer
			{
				Orientation = 0,
				SeparationOverride = new int?(4)
			};
			base.AddChild(this.Container);
			this.ChannelSelector = new ChannelSelectorButton
			{
				Name = "ChannelSelector",
				ToggleMode = true,
				StyleClasses = 
				{
					"chatSelectorOptionButton"
				},
				MinWidth = 75f
			};
			this.Container.AddChild(this.ChannelSelector);
			this.Input = new HistoryLineEdit
			{
				Name = "Input",
				PlaceHolder = Loc.GetString("hud-chatbox-info", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("talk-key", BoundKeyHelper.ShortKeyName(ContentKeyFunctions.FocusChat)),
					new ValueTuple<string, object>("cycle-key", BoundKeyHelper.ShortKeyName(ContentKeyFunctions.CycleChatChannelForward))
				}),
				HorizontalExpand = true,
				StyleClasses = 
				{
					"chatLineEdit"
				}
			};
			this.Container.AddChild(this.Input);
			this.FilterButton = new ChannelFilterButton
			{
				Name = "FilterButton",
				StyleClasses = 
				{
					"chatFilterOptionButton"
				}
			};
			this.Container.AddChild(this.FilterButton);
			this.ChannelSelector.OnChannelSelect += this.UpdateActiveChannel;
		}

		// Token: 0x06000481 RID: 1153 RVA: 0x00019CA3 File Offset: 0x00017EA3
		private void UpdateActiveChannel(ChatSelectChannel selectedChannel)
		{
			this.ActiveChannel = (ChatChannel)selectedChannel;
		}

		// Token: 0x04000224 RID: 548
		public readonly ChannelSelectorButton ChannelSelector;

		// Token: 0x04000225 RID: 549
		public readonly HistoryLineEdit Input;

		// Token: 0x04000226 RID: 550
		public readonly ChannelFilterButton FilterButton;

		// Token: 0x04000227 RID: 551
		protected readonly BoxContainer Container;
	}
}
