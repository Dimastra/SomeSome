using System;
using System.Runtime.CompilerServices;
using Content.Shared.Chat;
using Content.Shared.Radio;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Systems.Chat.Controls
{
	// Token: 0x020000AA RID: 170
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ChannelSelectorButton : Button
	{
		// Token: 0x1400001E RID: 30
		// (add) Token: 0x06000465 RID: 1125 RVA: 0x000194DC File Offset: 0x000176DC
		// (remove) Token: 0x06000466 RID: 1126 RVA: 0x00019514 File Offset: 0x00017714
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action<ChatSelectChannel> OnChannelSelect;

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000467 RID: 1127 RVA: 0x00019549 File Offset: 0x00017749
		// (set) Token: 0x06000468 RID: 1128 RVA: 0x00019551 File Offset: 0x00017751
		public ChatSelectChannel SelectedChannel { get; private set; }

		// Token: 0x06000469 RID: 1129 RVA: 0x0001955C File Offset: 0x0001775C
		public ChannelSelectorButton()
		{
			base.Name = "ChannelSelector";
			base.Mode = 0;
			base.EnableAllKeybinds = true;
			base.ToggleMode = true;
			base.OnToggled += this.OnSelectorButtonToggled;
			this._channelSelectorPopup = base.UserInterfaceManager.CreatePopup<ChannelSelectorPopup>();
			this._channelSelectorPopup.Selected += this.OnChannelSelected;
			this._channelSelectorPopup.OnVisibilityChanged += this.OnPopupVisibilityChanged;
			ChatSelectChannel? firstChannel = this._channelSelectorPopup.FirstChannel;
			if (firstChannel != null)
			{
				ChatSelectChannel valueOrDefault = firstChannel.GetValueOrDefault();
				this.Select(valueOrDefault);
			}
		}

		// Token: 0x0600046A RID: 1130 RVA: 0x00019604 File Offset: 0x00017804
		private void OnChannelSelected(ChatSelectChannel channel)
		{
			this.Select(channel);
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x00018DCE File Offset: 0x00016FCE
		private void OnPopupVisibilityChanged(Control control)
		{
			base.Pressed = control.Visible;
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x00018E76 File Offset: 0x00017076
		protected override void KeyBindDown(GUIBoundKeyEventArgs args)
		{
			if (args.Function == EngineKeyFunctions.Use)
			{
				return;
			}
			base.KeyBindDown(args);
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x0001960D File Offset: 0x0001780D
		public void Select(ChatSelectChannel channel)
		{
			if (this._channelSelectorPopup.Visible)
			{
				this._channelSelectorPopup.Close();
			}
			if (this.SelectedChannel == channel)
			{
				return;
			}
			this.SelectedChannel = channel;
			Action<ChatSelectChannel> onChannelSelect = this.OnChannelSelect;
			if (onChannelSelect == null)
			{
				return;
			}
			onChannelSelect(channel);
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x0001964C File Offset: 0x0001784C
		public static string ChannelSelectorName(ChatSelectChannel channel)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
			defaultInterpolatedStringHandler.AppendLiteral("hud-chatbox-select-channel-");
			defaultInterpolatedStringHandler.AppendFormatted<ChatSelectChannel>(channel);
			return Loc.GetString(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x0600046F RID: 1135 RVA: 0x00019684 File Offset: 0x00017884
		public Color ChannelSelectColor(ChatSelectChannel channel)
		{
			if (channel <= ChatSelectChannel.LOOC)
			{
				if (channel == ChatSelectChannel.Radio)
				{
					return Color.LimeGreen;
				}
				if (channel == ChatSelectChannel.LOOC)
				{
					return Color.MediumTurquoise;
				}
			}
			else
			{
				if (channel == ChatSelectChannel.OOC)
				{
					return Color.LightSkyBlue;
				}
				if (channel == ChatSelectChannel.Dead)
				{
					return Color.MediumPurple;
				}
				if (channel == ChatSelectChannel.Admin)
				{
					return Color.HotPink;
				}
			}
			return Color.DarkGray;
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x000196E8 File Offset: 0x000178E8
		[NullableContext(2)]
		public void UpdateChannelSelectButton(ChatSelectChannel channel, RadioChannelPrototype radio)
		{
			base.Text = ((radio != null) ? Loc.GetString(radio.Name) : ChannelSelectorButton.ChannelSelectorName(channel));
			base.Modulate = ((radio != null) ? radio.Color : this.ChannelSelectColor(channel));
		}

		// Token: 0x06000471 RID: 1137 RVA: 0x00019720 File Offset: 0x00017920
		private void OnSelectorButtonToggled(BaseButton.ButtonToggledEventArgs args)
		{
			if (args.Pressed)
			{
				float x = base.GlobalPosition.X;
				float item = base.GlobalPosition.Y + base.Height;
				UIBox2 value = UIBox2.FromDimensions(new ValueTuple<float, float>(x, item), new ValueTuple<float, float>(base.SizeBox.Width, 38f));
				this._channelSelectorPopup.Open(new UIBox2?(value), null);
				return;
			}
			this._channelSelectorPopup.Close();
		}

		// Token: 0x0400021A RID: 538
		private readonly ChannelSelectorPopup _channelSelectorPopup;

		// Token: 0x0400021D RID: 541
		private const int SelectorDropdownOffset = 38;
	}
}
