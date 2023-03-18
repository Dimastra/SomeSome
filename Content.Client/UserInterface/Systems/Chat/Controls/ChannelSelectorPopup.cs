using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Chat;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Systems.Chat.Controls
{
	// Token: 0x020000AC RID: 172
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ChannelSelectorPopup : Popup
	{
		// Token: 0x1400001F RID: 31
		// (add) Token: 0x06000474 RID: 1140 RVA: 0x0001982C File Offset: 0x00017A2C
		// (remove) Token: 0x06000475 RID: 1141 RVA: 0x00019864 File Offset: 0x00017A64
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action<ChatSelectChannel> Selected;

		// Token: 0x06000476 RID: 1142 RVA: 0x0001989C File Offset: 0x00017A9C
		public ChannelSelectorPopup()
		{
			this._channelSelectorHBox = new BoxContainer
			{
				Orientation = 0,
				SeparationOverride = new int?(1)
			};
			this._chatUIController = base.UserInterfaceManager.GetUIController<ChatUIController>();
			this._chatUIController.SelectableChannelsChanged += this.SetChannels;
			this.SetChannels(this._chatUIController.SelectableChannels);
			base.AddChild(this._channelSelectorHBox);
		}

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000477 RID: 1143 RVA: 0x00019920 File Offset: 0x00017B20
		public ChatSelectChannel? FirstChannel
		{
			get
			{
				foreach (ChannelSelectorItemButton channelSelectorItemButton in this._selectorStates.Values)
				{
					if (!channelSelectorItemButton.IsHidden)
					{
						return new ChatSelectChannel?(channelSelectorItemButton.Channel);
					}
				}
				return null;
			}
		}

		// Token: 0x06000478 RID: 1144 RVA: 0x00019994 File Offset: 0x00017B94
		private bool IsPreferredAvailable()
		{
			ChatSelectChannel key = this._chatUIController.MapLocalIfGhost(this._chatUIController.GetPreferredChannel());
			ChannelSelectorItemButton channelSelectorItemButton;
			return this._selectorStates.TryGetValue(key, out channelSelectorItemButton) && !channelSelectorItemButton.IsHidden;
		}

		// Token: 0x06000479 RID: 1145 RVA: 0x000199D4 File Offset: 0x00017BD4
		public void SetChannels(ChatSelectChannel channels)
		{
			bool flag = this.IsPreferredAvailable();
			this._channelSelectorHBox.RemoveAllChildren();
			foreach (ChatSelectChannel chatSelectChannel in ChannelSelectorPopup.ChannelSelectorOrder)
			{
				ChannelSelectorItemButton channelSelectorItemButton;
				if (!this._selectorStates.TryGetValue(chatSelectChannel, out channelSelectorItemButton))
				{
					channelSelectorItemButton = new ChannelSelectorItemButton(chatSelectChannel);
					this._selectorStates.Add(chatSelectChannel, channelSelectorItemButton);
					channelSelectorItemButton.OnPressed += this.OnSelectorPressed;
				}
				if ((channels & chatSelectChannel) == ChatSelectChannel.None)
				{
					if (channelSelectorItemButton.Parent == this._channelSelectorHBox)
					{
						this._channelSelectorHBox.RemoveChild(channelSelectorItemButton);
					}
				}
				else if (channelSelectorItemButton.IsHidden)
				{
					this._channelSelectorHBox.AddChild(channelSelectorItemButton);
				}
			}
			bool flag2 = this.IsPreferredAvailable();
			if (!flag && flag2)
			{
				this.Select(this._chatUIController.GetPreferredChannel());
				return;
			}
			if (flag && !flag2)
			{
				this.Select(ChatSelectChannel.OOC);
			}
		}

		// Token: 0x0600047A RID: 1146 RVA: 0x00019AB4 File Offset: 0x00017CB4
		private void OnSelectorPressed(BaseButton.ButtonEventArgs args)
		{
			ChannelSelectorItemButton channelSelectorItemButton = (ChannelSelectorItemButton)args.Button;
			this.Select(channelSelectorItemButton.Channel);
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x00019AD9 File Offset: 0x00017CD9
		private void Select(ChatSelectChannel channel)
		{
			Action<ChatSelectChannel> selected = this.Selected;
			if (selected == null)
			{
				return;
			}
			selected(channel);
		}

		// Token: 0x0600047C RID: 1148 RVA: 0x00019AEC File Offset: 0x00017CEC
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			this._chatUIController.SelectableChannelsChanged -= this.SetChannels;
		}

		// Token: 0x0600047D RID: 1149 RVA: 0x00019B10 File Offset: 0x00017D10
		// Note: this type is marked as 'beforefieldinit'.
		static ChannelSelectorPopup()
		{
			ChatSelectChannel[] array = new ChatSelectChannel[8];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.91EEEBEAE3749D9A41DD6582CBA7CEC3C480F5F31EC26AF1F9DCFDCCEC1EC735).FieldHandle);
			ChannelSelectorPopup.ChannelSelectorOrder = array;
		}

		// Token: 0x0400021F RID: 543
		public static readonly ChatSelectChannel[] ChannelSelectorOrder;

		// Token: 0x04000220 RID: 544
		private readonly BoxContainer _channelSelectorHBox;

		// Token: 0x04000221 RID: 545
		private readonly Dictionary<ChatSelectChannel, ChannelSelectorItemButton> _selectorStates = new Dictionary<ChatSelectChannel, ChannelSelectorItemButton>();

		// Token: 0x04000222 RID: 546
		private readonly ChatUIController _chatUIController;
	}
}
