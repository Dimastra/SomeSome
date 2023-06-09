﻿using System;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Client.UserInterface.Systems.Chat.Controls;
using Content.Shared.Chat;
using Content.Shared.Input;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Audio;
using Robust.Shared.Input;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.Systems.Chat.Widgets
{
	// Token: 0x020000A4 RID: 164
	[NullableContext(1)]
	[Nullable(0)]
	[GenerateTypedNameReferences]
	public class ChatBox : UIWidget
	{
		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000428 RID: 1064 RVA: 0x000180E0 File Offset: 0x000162E0
		// (set) Token: 0x06000429 RID: 1065 RVA: 0x000180E8 File Offset: 0x000162E8
		public bool Main { get; set; }

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x0600042A RID: 1066 RVA: 0x000180F1 File Offset: 0x000162F1
		public ChatSelectChannel SelectedChannel
		{
			get
			{
				return this.ChatInput.ChannelSelector.SelectedChannel;
			}
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x00018104 File Offset: 0x00016304
		public ChatBox()
		{
			ChatBox.!XamlIlPopulateTrampoline(this);
			this.ChatInput.Input.OnTextEntered += this.OnTextEntered;
			this.ChatInput.Input.OnKeyBindDown += this.OnKeyBindDown;
			this.ChatInput.Input.OnTextChanged += this.OnTextChanged;
			this.ChatInput.Input.OnFocusEnter += this.OnFocusEnter;
			this.ChatInput.Input.OnFocusExit += this.OnFocusExit;
			this.ChatInput.ChannelSelector.OnChannelSelect += this.OnChannelSelect;
			this.ChatInput.FilterButton.ChatFilterPopup.OnChannelFilter += this.OnChannelFilter;
			this._controller = base.UserInterfaceManager.GetUIController<ChatUIController>();
			this._controller.MessageAdded += this.OnMessageAdded;
			this._controller.RegisterChat(this);
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x0001821A File Offset: 0x0001641A
		private void OnTextEntered(LineEdit.LineEditEventArgs args)
		{
			this._controller.SendMessage(this, this.SelectedChannel);
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x00018230 File Offset: 0x00016430
		private void OnMessageAdded(ChatMessage msg)
		{
			string text = "chat";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 2);
			defaultInterpolatedStringHandler.AppendFormatted<ChatChannel>(msg.Channel);
			defaultInterpolatedStringHandler.AppendLiteral(": ");
			defaultInterpolatedStringHandler.AppendFormatted(msg.Message);
			Logger.DebugS(text, defaultInterpolatedStringHandler.ToStringAndClear());
			if (!this.ChatInput.FilterButton.ChatFilterPopup.IsActive(msg.Channel))
			{
				return;
			}
			if (msg != null && !msg.Read && msg.AudioPath != null)
			{
				SoundSystem.Play(msg.AudioPath, Filter.Local(), new AudioParams?(new AudioParams().WithVolume(msg.AudioVolume)));
			}
			msg.Read = true;
			Color color = (msg.MessageColorOverride != null) ? msg.MessageColorOverride.Value : msg.Channel.TextColor();
			this.AddLine(msg.WrappedMessage, color);
		}

		// Token: 0x0600042E RID: 1070 RVA: 0x00018311 File Offset: 0x00016511
		private void OnChannelSelect(ChatSelectChannel channel)
		{
			this._controller.UpdateSelectedChannel(this);
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x00018320 File Offset: 0x00016520
		public void Repopulate()
		{
			this.Contents.Clear();
			foreach (ValueTuple<GameTick, ChatMessage> valueTuple in this._controller.History)
			{
				this.OnMessageAdded(valueTuple.Item2);
			}
		}

		// Token: 0x06000430 RID: 1072 RVA: 0x00018388 File Offset: 0x00016588
		private void OnChannelFilter(ChatChannel channel, bool active)
		{
			this.Contents.Clear();
			foreach (ValueTuple<GameTick, ChatMessage> valueTuple in this._controller.History)
			{
				this.OnMessageAdded(valueTuple.Item2);
			}
			if (active)
			{
				this._controller.ClearUnfilteredUnreads(channel);
			}
		}

		// Token: 0x06000431 RID: 1073 RVA: 0x00018400 File Offset: 0x00016600
		public void AddLine(string message, Color color)
		{
			FormattedMessage formattedMessage = new FormattedMessage(3);
			formattedMessage.PushColor(color);
			formattedMessage.AddMarkup(message);
			formattedMessage.Pop();
			this.Contents.AddMessage(formattedMessage);
		}

		// Token: 0x06000432 RID: 1074 RVA: 0x00018434 File Offset: 0x00016634
		public void Focus(ChatSelectChannel? channel = null)
		{
			HistoryLineEdit input = this.ChatInput.Input;
			Index end = Index.End;
			if (channel != null)
			{
				this.ChatInput.ChannelSelector.Select(channel.Value);
			}
			input.IgnoreNext = true;
			input.GrabKeyboardFocus();
			input.CursorPosition = input.Text.Length;
			input.SelectionStart = end.GetOffset(input.Text.Length);
		}

		// Token: 0x06000433 RID: 1075 RVA: 0x000184AC File Offset: 0x000166AC
		public void CycleChatChannel(bool forward)
		{
			int num = Array.IndexOf<ChatSelectChannel>(ChannelSelectorPopup.ChannelSelectorOrder, this.SelectedChannel);
			do
			{
				num += (forward ? 1 : -1);
				num = MathHelper.Mod(num, ChannelSelectorPopup.ChannelSelectorOrder.Length);
			}
			while ((this._controller.SelectableChannels & ChannelSelectorPopup.ChannelSelectorOrder[num]) == ChatSelectChannel.None);
			this.SafelySelectChannel(ChannelSelectorPopup.ChannelSelectorOrder[num]);
		}

		// Token: 0x06000434 RID: 1076 RVA: 0x00018504 File Offset: 0x00016704
		public void SafelySelectChannel(ChatSelectChannel toSelect)
		{
			toSelect = this._controller.MapLocalIfGhost(toSelect);
			if ((this._controller.SelectableChannels & toSelect) == ChatSelectChannel.None)
			{
				return;
			}
			this.ChatInput.ChannelSelector.Select(toSelect);
		}

		// Token: 0x06000435 RID: 1077 RVA: 0x00018538 File Offset: 0x00016738
		private void OnKeyBindDown(GUIBoundKeyEventArgs args)
		{
			if (args.Function == EngineKeyFunctions.TextReleaseFocus)
			{
				this.ChatInput.Input.ReleaseKeyboardFocus();
				this.ChatInput.Input.Clear();
				args.Handle();
				return;
			}
			if (args.Function == ContentKeyFunctions.CycleChatChannelForward)
			{
				this.CycleChatChannel(true);
				args.Handle();
				return;
			}
			if (args.Function == ContentKeyFunctions.CycleChatChannelBackward)
			{
				this.CycleChatChannel(false);
				args.Handle();
			}
		}

		// Token: 0x06000436 RID: 1078 RVA: 0x000185BD File Offset: 0x000167BD
		private void OnTextChanged(LineEdit.LineEditEventArgs args)
		{
			this._controller.UpdateSelectedChannel(this);
			this._controller.NotifyChatTextChange();
		}

		// Token: 0x06000437 RID: 1079 RVA: 0x000185D6 File Offset: 0x000167D6
		private void OnFocusEnter(LineEdit.LineEditEventArgs args)
		{
			this._controller.NotifyChatFocus(true);
		}

		// Token: 0x06000438 RID: 1080 RVA: 0x000185E4 File Offset: 0x000167E4
		private void OnFocusExit(LineEdit.LineEditEventArgs args)
		{
			this._controller.NotifyChatFocus(false);
		}

		// Token: 0x06000439 RID: 1081 RVA: 0x000185F4 File Offset: 0x000167F4
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			this._controller.UnregisterChat(this);
			this.ChatInput.Input.OnTextEntered -= this.OnTextEntered;
			this.ChatInput.Input.OnKeyBindDown -= this.OnKeyBindDown;
			this.ChatInput.Input.OnTextChanged -= this.OnTextChanged;
			this.ChatInput.ChannelSelector.OnChannelSelect -= this.OnChannelSelect;
		}

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x0600043A RID: 1082 RVA: 0x00018688 File Offset: 0x00016888
		[Nullable(0)]
		protected OutputPanel Contents
		{
			[NullableContext(0)]
			get
			{
				return base.FindControl<OutputPanel>("Contents");
			}
		}

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x0600043B RID: 1083 RVA: 0x00018695 File Offset: 0x00016895
		[Nullable(0)]
		public ChatInputBox ChatInput
		{
			[NullableContext(0)]
			get
			{
				return base.FindControl<ChatInputBox>("ChatInput");
			}
		}

		// Token: 0x0600043C RID: 1084 RVA: 0x000186A4 File Offset: 0x000168A4
		static void xaml(IServiceProvider A_0, ChatBox A_1)
		{
			XamlIlContext.Context<ChatBox> context = new XamlIlContext.Context<ChatBox>(A_0, null, "resm:Content.Client.UserInterface.Systems.Chat.Widgets.ChatBox.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.MouseFilter = 0;
			A_1.HorizontalExpand = true;
			A_1.VerticalExpand = true;
			A_1.MinSize = new Vector2(465f, 225f);
			PanelContainer panelContainer = new PanelContainer();
			panelContainer.HorizontalExpand = true;
			panelContainer.VerticalExpand = true;
			panelContainer.PanelOverride = new StyleBoxFlat
			{
				BackgroundColor = Color.FromXaml("#252525AA")
			};
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			boxContainer.SeparationOverride = new int?(4);
			boxContainer.HorizontalExpand = true;
			boxContainer.VerticalExpand = true;
			OutputPanel outputPanel = new OutputPanel();
			outputPanel.Name = "Contents";
			Control control = outputPanel;
			context.RobustNameScope.Register("Contents", control);
			outputPanel.HorizontalExpand = true;
			outputPanel.VerticalExpand = true;
			control = outputPanel;
			boxContainer.XamlChildren.Add(control);
			ChatInputBox chatInputBox = new ChatInputBox();
			chatInputBox.HorizontalExpand = true;
			chatInputBox.Name = "ChatInput";
			control = chatInputBox;
			context.RobustNameScope.Register("ChatInput", control);
			chatInputBox.Access = new AccessLevel?(0);
			chatInputBox.Margin = new Thickness(2f, 2f, 2f, 2f);
			control = chatInputBox;
			boxContainer.XamlChildren.Add(control);
			control = boxContainer;
			panelContainer.XamlChildren.Add(control);
			control = panelContainer;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x0600043D RID: 1085 RVA: 0x000188B2 File Offset: 0x00016AB2
		private static void !XamlIlPopulateTrampoline(ChatBox A_0)
		{
			ChatBox.Populate:Content.Client.UserInterface.Systems.Chat.Widgets.ChatBox.xaml(null, A_0);
		}

		// Token: 0x04000201 RID: 513
		private readonly ChatUIController _controller;
	}
}
