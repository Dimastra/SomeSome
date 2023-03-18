using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Client.Administration.Managers;
using Content.Client.Chat;
using Content.Client.Chat.Managers;
using Content.Client.Chat.TypingIndicator;
using Content.Client.Chat.UI;
using Content.Client.Examine;
using Content.Client.Gameplay;
using Content.Client.Ghost;
using Content.Client.UserInterface.Systems.Chat.Widgets;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Content.Shared.Chat;
using Content.Shared.Examine;
using Content.Shared.Input;
using Content.Shared.Radio;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Players;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.Systems.Chat
{
	// Token: 0x020000A0 RID: 160
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ChatUIController : UIController
	{
		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x060003D0 RID: 976 RVA: 0x000169E5 File Offset: 0x00014BE5
		public IReadOnlySet<ChatBox> Chats
		{
			get
			{
				return this._chats;
			}
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060003D1 RID: 977 RVA: 0x000169ED File Offset: 0x00014BED
		public int MaxMessageLength
		{
			get
			{
				return this._config.GetCVar<int>(CCVars.ChatMaxMessageLength);
			}
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060003D2 RID: 978 RVA: 0x000169FF File Offset: 0x00014BFF
		// (set) Token: 0x060003D3 RID: 979 RVA: 0x00016A07 File Offset: 0x00014C07
		public ChatSelectChannel CanSendChannels { get; private set; }

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060003D4 RID: 980 RVA: 0x00016A10 File Offset: 0x00014C10
		// (set) Token: 0x060003D5 RID: 981 RVA: 0x00016A18 File Offset: 0x00014C18
		public ChatChannel FilterableChannels { get; private set; }

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x060003D6 RID: 982 RVA: 0x00016A21 File Offset: 0x00014C21
		// (set) Token: 0x060003D7 RID: 983 RVA: 0x00016A29 File Offset: 0x00014C29
		public ChatSelectChannel SelectableChannels { get; private set; }

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x060003D8 RID: 984 RVA: 0x00016A32 File Offset: 0x00014C32
		// (set) Token: 0x060003D9 RID: 985 RVA: 0x00016A3A File Offset: 0x00014C3A
		private ChatSelectChannel PreferredChannel { get; set; } = ChatSelectChannel.OOC;

		// Token: 0x14000018 RID: 24
		// (add) Token: 0x060003DA RID: 986 RVA: 0x00016A44 File Offset: 0x00014C44
		// (remove) Token: 0x060003DB RID: 987 RVA: 0x00016A7C File Offset: 0x00014C7C
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action<ChatSelectChannel> CanSendChannelsChanged;

		// Token: 0x14000019 RID: 25
		// (add) Token: 0x060003DC RID: 988 RVA: 0x00016AB4 File Offset: 0x00014CB4
		// (remove) Token: 0x060003DD RID: 989 RVA: 0x00016AEC File Offset: 0x00014CEC
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action<ChatChannel> FilterableChannelsChanged;

		// Token: 0x1400001A RID: 26
		// (add) Token: 0x060003DE RID: 990 RVA: 0x00016B24 File Offset: 0x00014D24
		// (remove) Token: 0x060003DF RID: 991 RVA: 0x00016B5C File Offset: 0x00014D5C
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action<ChatSelectChannel> SelectableChannelsChanged;

		// Token: 0x1400001B RID: 27
		// (add) Token: 0x060003E0 RID: 992 RVA: 0x00016B94 File Offset: 0x00014D94
		// (remove) Token: 0x060003E1 RID: 993 RVA: 0x00016BCC File Offset: 0x00014DCC
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action<ChatChannel, int?> UnreadMessageCountsUpdated;

		// Token: 0x1400001C RID: 28
		// (add) Token: 0x060003E2 RID: 994 RVA: 0x00016C04 File Offset: 0x00014E04
		// (remove) Token: 0x060003E3 RID: 995 RVA: 0x00016C3C File Offset: 0x00014E3C
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<ChatMessage> MessageAdded;

		// Token: 0x060003E4 RID: 996 RVA: 0x00016C74 File Offset: 0x00014E74
		public override void Initialize()
		{
			this._sawmill = Logger.GetSawmill("chat");
			this._sawmill.Level = new LogLevel?(2);
			this._admin.AdminStatusUpdated += this.UpdateChannelPermissions;
			this._player.LocalPlayerChanged += this.OnLocalPlayerChanged;
			this._state.OnStateChanged += this.StateChanged;
			this._net.RegisterNetMessage<MsgChatMessage>(new ProcessMessage<MsgChatMessage>(this.OnChatMessage), 3);
			this._speechBubbleRoot = new LayoutContainer();
			this.OnLocalPlayerChanged(new LocalPlayerChangedEventArgs(null, this._player.LocalPlayer));
			this._input.SetInputCommand(ContentKeyFunctions.FocusChat, InputCmdHandler.FromDelegate(delegate(ICommonSession _)
			{
				this.FocusChat();
			}, null, true, true));
			this._input.SetInputCommand(ContentKeyFunctions.FocusLocalChat, InputCmdHandler.FromDelegate(delegate(ICommonSession _)
			{
				this.FocusChannel(ChatSelectChannel.Local);
			}, null, true, true));
			this._input.SetInputCommand(ContentKeyFunctions.FocusWhisperChat, InputCmdHandler.FromDelegate(delegate(ICommonSession _)
			{
				this.FocusChannel(ChatSelectChannel.Whisper);
			}, null, true, true));
			this._input.SetInputCommand(ContentKeyFunctions.FocusOOC, InputCmdHandler.FromDelegate(delegate(ICommonSession _)
			{
				this.FocusChannel(ChatSelectChannel.OOC);
			}, null, true, true));
			this._input.SetInputCommand(ContentKeyFunctions.FocusAdminChat, InputCmdHandler.FromDelegate(delegate(ICommonSession _)
			{
				this.FocusChannel(ChatSelectChannel.Admin);
			}, null, true, true));
			this._input.SetInputCommand(ContentKeyFunctions.FocusRadio, InputCmdHandler.FromDelegate(delegate(ICommonSession _)
			{
				this.FocusChannel(ChatSelectChannel.Radio);
			}, null, true, true));
			this._input.SetInputCommand(ContentKeyFunctions.FocusDeadChat, InputCmdHandler.FromDelegate(delegate(ICommonSession _)
			{
				this.FocusChannel(ChatSelectChannel.Dead);
			}, null, true, true));
			this._input.SetInputCommand(ContentKeyFunctions.FocusConsoleChat, InputCmdHandler.FromDelegate(delegate(ICommonSession _)
			{
				this.FocusChannel(ChatSelectChannel.Console);
			}, null, true, true));
			this._input.SetInputCommand(ContentKeyFunctions.CycleChatChannelForward, InputCmdHandler.FromDelegate(delegate(ICommonSession _)
			{
				this.CycleChatChannel(true);
			}, null, true, true));
			this._input.SetInputCommand(ContentKeyFunctions.CycleChatChannelBackward, InputCmdHandler.FromDelegate(delegate(ICommonSession _)
			{
				this.CycleChatChannel(false);
			}, null, true, true));
		}

		// Token: 0x060003E5 RID: 997 RVA: 0x00016E8C File Offset: 0x0001508C
		public void SetMainChat(bool setting)
		{
			UIScreen activeScreen = this.UIManager.ActiveScreen;
			ChatBox chatBox = (activeScreen != null) ? activeScreen.GetWidget<ChatBox>() : null;
			if (chatBox == null)
			{
				UIScreen activeScreen2 = this.UIManager.ActiveScreen;
				chatBox = ((activeScreen2 != null) ? activeScreen2.GetWidget<ResizableChatBox>() : null);
				if (chatBox == null)
				{
					return;
				}
			}
			chatBox.Main = setting;
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x00016ED8 File Offset: 0x000150D8
		private void FocusChat()
		{
			foreach (ChatBox chatBox in this._chats)
			{
				if (chatBox.Main)
				{
					chatBox.Focus(null);
					break;
				}
			}
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x00016F40 File Offset: 0x00015140
		private void FocusChannel(ChatSelectChannel channel)
		{
			foreach (ChatBox chatBox in this._chats)
			{
				if (chatBox.Main)
				{
					chatBox.Focus(new ChatSelectChannel?(channel));
					break;
				}
			}
		}

		// Token: 0x060003E8 RID: 1000 RVA: 0x00016FA4 File Offset: 0x000151A4
		private void CycleChatChannel(bool forward)
		{
			foreach (ChatBox chatBox in this._chats)
			{
				if (chatBox.Main)
				{
					chatBox.CycleChatChannel(forward);
					break;
				}
			}
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x00017004 File Offset: 0x00015204
		private void StateChanged(StateChangedEventArgs args)
		{
			if (args.NewState is GameplayState)
			{
				this.PreferredChannel = ChatSelectChannel.Local;
			}
			this.UpdateChannelPermissions();
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x00017020 File Offset: 0x00015220
		public void SetSpeechBubbleRoot(LayoutContainer root)
		{
			this._speechBubbleRoot.Orphan();
			root.AddChild(this._speechBubbleRoot);
			LayoutContainer.SetAnchorPreset(this._speechBubbleRoot, 15, false);
			this._speechBubbleRoot.SetPositionLast();
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x00017054 File Offset: 0x00015254
		private void OnLocalPlayerChanged(LocalPlayerChangedEventArgs obj)
		{
			if (obj.OldPlayer != null)
			{
				obj.OldPlayer.EntityAttached -= this.OnLocalPlayerEntityAttached;
				obj.OldPlayer.EntityDetached -= this.OnLocalPlayerEntityDetached;
			}
			if (obj.NewPlayer != null)
			{
				obj.NewPlayer.EntityAttached += this.OnLocalPlayerEntityAttached;
				obj.NewPlayer.EntityDetached += this.OnLocalPlayerEntityDetached;
			}
			this.UpdateChannelPermissions();
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x000170D3 File Offset: 0x000152D3
		private void OnLocalPlayerEntityAttached(EntityAttachedEventArgs obj)
		{
			this.UpdateChannelPermissions();
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x000170D3 File Offset: 0x000152D3
		private void OnLocalPlayerEntityDetached(EntityDetachedEventArgs obj)
		{
			this.UpdateChannelPermissions();
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x000170DC File Offset: 0x000152DC
		private void AddSpeechBubble(ChatMessage msg, SpeechBubble.SpeechType speechType)
		{
			if (!this._entities.EntityExists(msg.SenderEntity))
			{
				this._sawmill.Debug("Got local chat message with invalid sender entity: {0}", new object[]
				{
					msg.SenderEntity
				});
				return;
			}
			string msg2 = FormattedMessage.RemoveMarkup(msg.Message);
			foreach (string contents in this.SplitMessage(msg2))
			{
				this.EnqueueSpeechBubble(msg.SenderEntity, contents, speechType);
			}
		}

		// Token: 0x060003EF RID: 1007 RVA: 0x0001717C File Offset: 0x0001537C
		private void CreateSpeechBubble(EntityUid entity, ChatUIController.SpeechBubbleData speechData)
		{
			SpeechBubble speechBubble = SpeechBubble.CreateSpeechBubble(speechData.Type, speechData.Message, entity, this._eye, this._manager, this._entities);
			speechBubble.OnDied += this.SpeechBubbleDied;
			List<SpeechBubble> list;
			if (this._activeSpeechBubbles.TryGetValue(entity, out list))
			{
				using (List<SpeechBubble>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SpeechBubble speechBubble2 = enumerator.Current;
						speechBubble2.VerticalOffset += speechBubble.ContentSize.Y;
					}
					goto IL_9C;
				}
			}
			list = new List<SpeechBubble>();
			this._activeSpeechBubbles.Add(entity, list);
			IL_9C:
			list.Add(speechBubble);
			this._speechBubbleRoot.AddChild(speechBubble);
			if (list.Count > 4)
			{
				list[0].FadeNow();
			}
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x00017260 File Offset: 0x00015460
		private void SpeechBubbleDied(EntityUid entity, SpeechBubble bubble)
		{
			this.RemoveSpeechBubble(entity, bubble);
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x0001726C File Offset: 0x0001546C
		private void EnqueueSpeechBubble(EntityUid entity, string contents, SpeechBubble.SpeechType speechType)
		{
			if (this._entities.GetComponent<TransformComponent>(entity).MapID != this._eye.CurrentMap)
			{
				return;
			}
			ChatUIController.SpeechBubbleQueueData speechBubbleQueueData;
			if (!this._queuedSpeechBubbles.TryGetValue(entity, out speechBubbleQueueData))
			{
				speechBubbleQueueData = new ChatUIController.SpeechBubbleQueueData();
				this._queuedSpeechBubbles.Add(entity, speechBubbleQueueData);
			}
			speechBubbleQueueData.MessageQueue.Enqueue(new ChatUIController.SpeechBubbleData(contents, speechType));
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x000172D2 File Offset: 0x000154D2
		public void RemoveSpeechBubble(EntityUid entityUid, SpeechBubble bubble)
		{
			bubble.Dispose();
			List<SpeechBubble> list = this._activeSpeechBubbles[entityUid];
			list.Remove(bubble);
			if (list.Count == 0)
			{
				this._activeSpeechBubbles.Remove(entityUid);
			}
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x00017304 File Offset: 0x00015504
		private void UpdateChannelPermissions()
		{
			this.CanSendChannels = ChatSelectChannel.None;
			this.FilterableChannels = ChatChannel.None;
			this.CanSendChannels |= ChatSelectChannel.Console;
			this.CanSendChannels |= ChatSelectChannel.OOC;
			this.CanSendChannels |= ChatSelectChannel.LOOC;
			this.FilterableChannels |= ChatChannel.OOC;
			this.FilterableChannels |= ChatChannel.LOOC;
			this.FilterableChannels |= ChatChannel.Server;
			if (this._state.CurrentState is GameplayStateBase)
			{
				this.FilterableChannels |= ChatChannel.Local;
				this.FilterableChannels |= ChatChannel.Whisper;
				this.FilterableChannels |= ChatChannel.Radio;
				this.FilterableChannels |= ChatChannel.Emotes;
				GhostSystem ghost = this._ghost;
				if (ghost == null || !ghost.IsGhost)
				{
					this.CanSendChannels |= ChatSelectChannel.Local;
					this.CanSendChannels |= ChatSelectChannel.Whisper;
					this.CanSendChannels |= ChatSelectChannel.Radio;
					this.CanSendChannels |= ChatSelectChannel.Emotes;
				}
			}
			if (!this._admin.HasFlag(AdminFlags.Admin))
			{
				GhostSystem ghost = this._ghost;
				if (ghost == null || !ghost.IsGhost)
				{
					goto IL_14F;
				}
			}
			this.FilterableChannels |= ChatChannel.Dead;
			this.CanSendChannels |= ChatSelectChannel.Dead;
			IL_14F:
			if (this._admin.HasFlag(AdminFlags.Admin))
			{
				this.FilterableChannels |= ChatChannel.Admin;
				this.FilterableChannels |= ChatChannel.AdminChat;
				this.CanSendChannels |= ChatSelectChannel.Admin;
			}
			this.SelectableChannels = this.CanSendChannels;
			Action<ChatSelectChannel> canSendChannelsChanged = this.CanSendChannelsChanged;
			if (canSendChannelsChanged != null)
			{
				canSendChannelsChanged(this.CanSendChannels);
			}
			Action<ChatChannel> filterableChannelsChanged = this.FilterableChannelsChanged;
			if (filterableChannelsChanged != null)
			{
				filterableChannelsChanged(this.FilterableChannels);
			}
			Action<ChatSelectChannel> selectableChannelsChanged = this.SelectableChannelsChanged;
			if (selectableChannelsChanged == null)
			{
				return;
			}
			selectableChannelsChanged(this.SelectableChannels);
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x000174F4 File Offset: 0x000156F4
		public void ClearUnfilteredUnreads(ChatChannel channels)
		{
			foreach (ChatChannel chatChannel in this._unreadMessages.Keys.ToArray<ChatChannel>())
			{
				if ((channels & chatChannel) != ChatChannel.None)
				{
					this._unreadMessages[chatChannel] = 0;
					Action<ChatChannel, int?> unreadMessageCountsUpdated = this.UnreadMessageCountsUpdated;
					if (unreadMessageCountsUpdated != null)
					{
						unreadMessageCountsUpdated(chatChannel, new int?(0));
					}
				}
			}
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x0001754E File Offset: 0x0001574E
		public override void FrameUpdate(FrameEventArgs delta)
		{
			this.UpdateQueuedSpeechBubbles(delta);
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x00017558 File Offset: 0x00015758
		private void UpdateQueuedSpeechBubbles(FrameEventArgs delta)
		{
			if (this._queuedSpeechBubbles.Count == 0 || this._examine == null)
			{
				return;
			}
			foreach (KeyValuePair<EntityUid, ChatUIController.SpeechBubbleQueueData> keyValuePair in Extensions.ShallowClone<EntityUid, ChatUIController.SpeechBubbleQueueData>(this._queuedSpeechBubbles))
			{
				EntityUid entityUid;
				ChatUIController.SpeechBubbleQueueData speechBubbleQueueData;
				keyValuePair.Deconstruct(out entityUid, out speechBubbleQueueData);
				EntityUid entityUid2 = entityUid;
				ChatUIController.SpeechBubbleQueueData speechBubbleQueueData2 = speechBubbleQueueData;
				if (!this._entities.EntityExists(entityUid2))
				{
					this._queuedSpeechBubbles.Remove(entityUid2);
				}
				else
				{
					speechBubbleQueueData2.TimeLeft -= delta.DeltaSeconds;
					if (speechBubbleQueueData2.TimeLeft <= 0f)
					{
						if (speechBubbleQueueData2.MessageQueue.Count == 0)
						{
							this._queuedSpeechBubbles.Remove(entityUid2);
						}
						else
						{
							ChatUIController.SpeechBubbleData speechData = speechBubbleQueueData2.MessageQueue.Dequeue();
							speechBubbleQueueData2.TimeLeft += 0.2f + (float)speechData.Message.Length * 0.008f;
							this.CreateSpeechBubble(entityUid2, speechData);
						}
					}
				}
			}
			LocalPlayer localPlayer = this._player.LocalPlayer;
			EntityUid? entityUid3 = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			Func<EntityUid, ValueTuple<EntityUid, EntityUid?>, bool> predicate = (EntityUid uid, [TupleElementNames(new string[]
			{
				"compOwner",
				"attachedEntity"
			})] ValueTuple<EntityUid, EntityUid?> data) => uid == data.Item1 || uid == data.Item2;
			MapCoordinates origin = (entityUid3 != null) ? this._entities.GetComponent<TransformComponent>(entityUid3.Value).MapPosition : MapCoordinates.Nullspace;
			bool flag = entityUid3 != null && this._examine.IsOccluded(entityUid3.Value);
			foreach (KeyValuePair<EntityUid, List<SpeechBubble>> keyValuePair2 in this._activeSpeechBubbles)
			{
				EntityUid entityUid;
				List<SpeechBubble> list;
				keyValuePair2.Deconstruct(out entityUid, out list);
				EntityUid entityUid4 = entityUid;
				List<SpeechBubble> bubbles = list;
				if (this._entities.Deleted(entityUid4))
				{
					this.SetBubbles(bubbles, false);
				}
				else
				{
					entityUid = entityUid4;
					EntityUid? entityUid5 = entityUid3;
					if (entityUid == entityUid5)
					{
						this.SetBubbles(bubbles, true);
					}
					else
					{
						MapCoordinates mapPosition = this._entities.GetComponent<TransformComponent>(entityUid4).MapPosition;
						if (flag && !ExamineSystemShared.InRangeUnOccluded<ValueTuple<EntityUid, EntityUid?>>(origin, mapPosition, 0f, new ValueTuple<EntityUid, EntityUid?>(entityUid4, entityUid3), predicate, true, null))
						{
							this.SetBubbles(bubbles, false);
						}
						else
						{
							this.SetBubbles(bubbles, true);
						}
					}
				}
			}
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x000177E8 File Offset: 0x000159E8
		private void SetBubbles(List<SpeechBubble> bubbles, bool visible)
		{
			foreach (SpeechBubble speechBubble in bubbles)
			{
				speechBubble.Visible = visible;
			}
		}

		// Token: 0x060003F8 RID: 1016 RVA: 0x00017834 File Offset: 0x00015A34
		private List<string> SplitMessage(string msg)
		{
			string[] array = msg.Split(' ', StringSplitOptions.None);
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			int num = 0;
			string[] array2 = array;
			int i = 0;
			while (i < array2.Length)
			{
				string text = array2[i];
				num += text.Length + 1;
				if (num <= 100)
				{
					goto IL_76;
				}
				list.Add(string.Join(" ", list2));
				list2.Clear();
				num = text.Length;
				if (num <= 100)
				{
					goto IL_76;
				}
				list.Add(text.Substring(0, 97) + "...");
				num = 0;
				IL_7E:
				i++;
				continue;
				IL_76:
				list2.Add(text);
				goto IL_7E;
			}
			if (list2.Count != 0)
			{
				list.Add(string.Join(" ", list2));
			}
			return list;
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x000178E8 File Offset: 0x00015AE8
		public ChatSelectChannel MapLocalIfGhost(ChatSelectChannel channel)
		{
			if (channel == ChatSelectChannel.Local)
			{
				GhostSystem ghost = this._ghost;
				if (ghost != null && ghost.IsGhost)
				{
					return ChatSelectChannel.Dead;
				}
			}
			return channel;
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x00017914 File Offset: 0x00015B14
		private bool TryGetRadioChannel(string text, [Nullable(2)] out RadioChannelPrototype radioChannel)
		{
			radioChannel = null;
			LocalPlayer localPlayer = this._player.LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			if (entityUid != null)
			{
				EntityUid valueOrDefault = entityUid.GetValueOrDefault();
				if (valueOrDefault.Valid && this._chatSys != null)
				{
					string text2;
					return this._chatSys.TryProccessRadioMessage(valueOrDefault, text, out text2, out radioChannel, true);
				}
			}
			return false;
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x0001797C File Offset: 0x00015B7C
		public void UpdateSelectedChannel(ChatBox box)
		{
			ValueTuple<ChatSelectChannel, string, RadioChannelPrototype> valueTuple = this.SplitInputContents(box.ChatInput.Input.Text);
			ChatSelectChannel item = valueTuple.Item1;
			RadioChannelPrototype item2 = valueTuple.Item3;
			if (item == ChatSelectChannel.None)
			{
				box.ChatInput.ChannelSelector.UpdateChannelSelectButton(box.SelectedChannel, null);
				return;
			}
			box.ChatInput.ChannelSelector.UpdateChannelSelectButton(item, item2);
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x000179DC File Offset: 0x00015BDC
		[return: TupleElementNames(new string[]
		{
			"chatChannel",
			"text",
			"radioChannel"
		})]
		[return: Nullable(new byte[]
		{
			0,
			1,
			2
		})]
		public ValueTuple<ChatSelectChannel, string, RadioChannelPrototype> SplitInputContents(string text)
		{
			text = text.Trim();
			if (text.Length == 0)
			{
				return new ValueTuple<ChatSelectChannel, string, RadioChannelPrototype>(ChatSelectChannel.None, text, null);
			}
			RadioChannelPrototype item;
			ChatSelectChannel chatSelectChannel;
			if (this.TryGetRadioChannel(text, out item))
			{
				chatSelectChannel = ChatSelectChannel.Radio;
			}
			else
			{
				chatSelectChannel = ChatUIController.PrefixToChannel.GetValueOrDefault(text[0]);
			}
			if ((this.CanSendChannels & chatSelectChannel) == ChatSelectChannel.None)
			{
				return new ValueTuple<ChatSelectChannel, string, RadioChannelPrototype>(ChatSelectChannel.None, text, null);
			}
			if (chatSelectChannel == ChatSelectChannel.Radio)
			{
				return new ValueTuple<ChatSelectChannel, string, RadioChannelPrototype>(chatSelectChannel, text, item);
			}
			if (chatSelectChannel == ChatSelectChannel.Local)
			{
				GhostSystem ghost = this._ghost;
				if (ghost == null || !ghost.IsGhost)
				{
					return new ValueTuple<ChatSelectChannel, string, RadioChannelPrototype>(chatSelectChannel, text, null);
				}
				chatSelectChannel = ChatSelectChannel.Dead;
			}
			ChatSelectChannel item2 = chatSelectChannel;
			string text2 = text;
			return new ValueTuple<ChatSelectChannel, string, RadioChannelPrototype>(item2, text2.Substring(1, text2.Length - 1).TrimStart(), null);
		}

		// Token: 0x060003FD RID: 1021 RVA: 0x00017A8C File Offset: 0x00015C8C
		public void SendMessage(ChatBox box, ChatSelectChannel channel)
		{
			TypingIndicatorSystem typingIndicator = this._typingIndicator;
			if (typingIndicator != null)
			{
				typingIndicator.ClientSubmittedChatText();
			}
			string text = box.ChatInput.Input.Text;
			box.ChatInput.Input.Clear();
			box.ChatInput.Input.ReleaseKeyboardFocus();
			this.UpdateSelectedChannel(box);
			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}
			ValueTuple<ChatSelectChannel, string, RadioChannelPrototype> valueTuple = this.SplitInputContents(text);
			ChatSelectChannel item = valueTuple.Item1;
			text = valueTuple.Item2;
			if (text.Length > this.MaxMessageLength)
			{
				string @string = Loc.GetString("chat-manager-max-message-length", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("maxMessageLength", this.MaxMessageLength)
				});
				box.AddLine(@string, Color.Orange);
				return;
			}
			if (item != ChatSelectChannel.None)
			{
				channel = item;
			}
			else if (channel == ChatSelectChannel.Radio)
			{
				text = ";" + text;
			}
			this._manager.SendMessage(text, (item == ChatSelectChannel.None) ? channel : item);
		}

		// Token: 0x060003FE RID: 1022 RVA: 0x00017B73 File Offset: 0x00015D73
		private void OnChatMessage(MsgChatMessage message)
		{
			this.ProcessChatMessage(message.Message, true);
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x00017B84 File Offset: 0x00015D84
		public void ProcessChatMessage(ChatMessage msg, bool speechBubble = true)
		{
			if (!msg.HideChat)
			{
				this.History.Add(new ValueTuple<GameTick, ChatMessage>(this._timing.CurTick, msg));
				Action<ChatMessage> messageAdded = this.MessageAdded;
				if (messageAdded != null)
				{
					messageAdded(msg);
				}
				if (!msg.Read)
				{
					ISawmill sawmill = this._sawmill;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 2);
					defaultInterpolatedStringHandler.AppendLiteral("Message filtered: ");
					defaultInterpolatedStringHandler.AppendFormatted<ChatChannel>(msg.Channel);
					defaultInterpolatedStringHandler.AppendLiteral(": ");
					defaultInterpolatedStringHandler.AppendFormatted(msg.Message);
					sawmill.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
					int num;
					if (!this._unreadMessages.TryGetValue(msg.Channel, out num))
					{
						num = 0;
					}
					num++;
					this._unreadMessages[msg.Channel] = num;
					Action<ChatChannel, int?> unreadMessageCountsUpdated = this.UnreadMessageCountsUpdated;
					if (unreadMessageCountsUpdated != null)
					{
						unreadMessageCountsUpdated(msg.Channel, new int?(num));
					}
				}
			}
			if (!speechBubble || msg.SenderEntity == default(EntityUid))
			{
				return;
			}
			ChatChannel channel = msg.Channel;
			if (channel > ChatChannel.Whisper)
			{
				if (channel != ChatChannel.Emotes)
				{
					if (channel != ChatChannel.Dead)
					{
						return;
					}
					GhostSystem ghost = this._ghost;
					if (ghost != null && ghost.IsGhost)
					{
						this.AddSpeechBubble(msg, SpeechBubble.SpeechType.Say);
						return;
					}
				}
				else
				{
					this.AddSpeechBubble(msg, SpeechBubble.SpeechType.Emote);
				}
				return;
			}
			if (channel == ChatChannel.Local)
			{
				this.AddSpeechBubble(msg, SpeechBubble.SpeechType.Say);
				return;
			}
			if (channel != ChatChannel.Whisper)
			{
				return;
			}
			this.AddSpeechBubble(msg, SpeechBubble.SpeechType.Whisper);
		}

		// Token: 0x06000400 RID: 1024 RVA: 0x00017CE4 File Offset: 0x00015EE4
		public void RegisterChat(ChatBox chat)
		{
			this._chats.Add(chat);
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x00017CF3 File Offset: 0x00015EF3
		public void UnregisterChat(ChatBox chat)
		{
			this._chats.Remove(chat);
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x00017D02 File Offset: 0x00015F02
		public ChatSelectChannel GetPreferredChannel()
		{
			return this.MapLocalIfGhost(this.PreferredChannel);
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x00017D10 File Offset: 0x00015F10
		public void NotifyChatTextChange()
		{
			TypingIndicatorSystem typingIndicator = this._typingIndicator;
			if (typingIndicator == null)
			{
				return;
			}
			typingIndicator.ClientChangedChatText();
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x00017D24 File Offset: 0x00015F24
		public void Repopulate()
		{
			foreach (ChatBox chatBox in this._chats)
			{
				chatBox.Repopulate();
			}
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x00017D74 File Offset: 0x00015F74
		public void NotifyChatFocus(bool isFocused)
		{
			TypingIndicatorSystem typingIndicator = this._typingIndicator;
			if (typingIndicator == null)
			{
				return;
			}
			typingIndicator.ClientChangedChatFocus(isFocused);
		}

		// Token: 0x040001D7 RID: 471
		[Dependency]
		private readonly IClientAdminManager _admin;

		// Token: 0x040001D8 RID: 472
		[Dependency]
		private readonly IChatManager _manager;

		// Token: 0x040001D9 RID: 473
		[Dependency]
		private readonly IConfigurationManager _config;

		// Token: 0x040001DA RID: 474
		[Dependency]
		private readonly IEntityManager _entities;

		// Token: 0x040001DB RID: 475
		[Dependency]
		private readonly IEyeManager _eye;

		// Token: 0x040001DC RID: 476
		[Dependency]
		private readonly IInputManager _input;

		// Token: 0x040001DD RID: 477
		[Dependency]
		private readonly IClientNetManager _net;

		// Token: 0x040001DE RID: 478
		[Dependency]
		private readonly IPlayerManager _player;

		// Token: 0x040001DF RID: 479
		[Dependency]
		private readonly IStateManager _state;

		// Token: 0x040001E0 RID: 480
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x040001E1 RID: 481
		[Nullable(2)]
		[UISystemDependency]
		private readonly ExamineSystem _examine;

		// Token: 0x040001E2 RID: 482
		[Nullable(2)]
		[UISystemDependency]
		private readonly GhostSystem _ghost;

		// Token: 0x040001E3 RID: 483
		[Nullable(2)]
		[UISystemDependency]
		private readonly TypingIndicatorSystem _typingIndicator;

		// Token: 0x040001E4 RID: 484
		[Nullable(2)]
		[UISystemDependency]
		private readonly ChatSystem _chatSys;

		// Token: 0x040001E5 RID: 485
		private ISawmill _sawmill;

		// Token: 0x040001E6 RID: 486
		public static readonly Dictionary<char, ChatSelectChannel> PrefixToChannel = new Dictionary<char, ChatSelectChannel>
		{
			{
				'.',
				ChatSelectChannel.Local
			},
			{
				',',
				ChatSelectChannel.Whisper
			},
			{
				'/',
				ChatSelectChannel.Console
			},
			{
				'(',
				ChatSelectChannel.LOOC
			},
			{
				'[',
				ChatSelectChannel.OOC
			},
			{
				'@',
				ChatSelectChannel.Emotes
			},
			{
				']',
				ChatSelectChannel.Admin
			},
			{
				';',
				ChatSelectChannel.Radio
			},
			{
				'\\',
				ChatSelectChannel.Dead
			}
		};

		// Token: 0x040001E7 RID: 487
		public static readonly Dictionary<ChatSelectChannel, char> ChannelPrefixes = ChatUIController.PrefixToChannel.ToDictionary((KeyValuePair<char, ChatSelectChannel> kv) => kv.Value, (KeyValuePair<char, ChatSelectChannel> kv) => kv.Key);

		// Token: 0x040001E8 RID: 488
		private const int SingleBubbleCharLimit = 100;

		// Token: 0x040001E9 RID: 489
		private const float BubbleDelayBase = 0.2f;

		// Token: 0x040001EA RID: 490
		private const float BubbleDelayFactor = 0.008f;

		// Token: 0x040001EB RID: 491
		private const int SpeechBubbleCap = 4;

		// Token: 0x040001EC RID: 492
		private LayoutContainer _speechBubbleRoot;

		// Token: 0x040001ED RID: 493
		private readonly Dictionary<EntityUid, List<SpeechBubble>> _activeSpeechBubbles = new Dictionary<EntityUid, List<SpeechBubble>>();

		// Token: 0x040001EE RID: 494
		private readonly Dictionary<EntityUid, ChatUIController.SpeechBubbleQueueData> _queuedSpeechBubbles = new Dictionary<EntityUid, ChatUIController.SpeechBubbleQueueData>();

		// Token: 0x040001EF RID: 495
		private readonly HashSet<ChatBox> _chats = new HashSet<ChatBox>();

		// Token: 0x040001F0 RID: 496
		private readonly Dictionary<ChatChannel, int> _unreadMessages = new Dictionary<ChatChannel, int>();

		// Token: 0x040001F1 RID: 497
		[Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		public readonly List<ValueTuple<GameTick, ChatMessage>> History = new List<ValueTuple<GameTick, ChatMessage>>();

		// Token: 0x020000A1 RID: 161
		[Nullable(0)]
		private readonly struct SpeechBubbleData : IEquatable<ChatUIController.SpeechBubbleData>
		{
			// Token: 0x06000412 RID: 1042 RVA: 0x00017EED File Offset: 0x000160ED
			public SpeechBubbleData(string Message, SpeechBubble.SpeechType Type)
			{
				this.Message = Message;
				this.Type = Type;
			}

			// Token: 0x170000A7 RID: 167
			// (get) Token: 0x06000413 RID: 1043 RVA: 0x00017EFD File Offset: 0x000160FD
			// (set) Token: 0x06000414 RID: 1044 RVA: 0x00017F05 File Offset: 0x00016105
			public string Message { get; set; }

			// Token: 0x170000A8 RID: 168
			// (get) Token: 0x06000415 RID: 1045 RVA: 0x00017F0E File Offset: 0x0001610E
			// (set) Token: 0x06000416 RID: 1046 RVA: 0x00017F16 File Offset: 0x00016116
			public SpeechBubble.SpeechType Type { get; set; }

			// Token: 0x06000417 RID: 1047 RVA: 0x00017F20 File Offset: 0x00016120
			[NullableContext(0)]
			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("SpeechBubbleData");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06000418 RID: 1048 RVA: 0x00017F6C File Offset: 0x0001616C
			[NullableContext(0)]
			[CompilerGenerated]
			private bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Message = ");
				builder.Append(this.Message);
				builder.Append(", Type = ");
				builder.Append(this.Type.ToString());
				return true;
			}

			// Token: 0x06000419 RID: 1049 RVA: 0x00017FBA File Offset: 0x000161BA
			[CompilerGenerated]
			public static bool operator !=(ChatUIController.SpeechBubbleData left, ChatUIController.SpeechBubbleData right)
			{
				return !(left == right);
			}

			// Token: 0x0600041A RID: 1050 RVA: 0x00017FC6 File Offset: 0x000161C6
			[CompilerGenerated]
			public static bool operator ==(ChatUIController.SpeechBubbleData left, ChatUIController.SpeechBubbleData right)
			{
				return left.Equals(right);
			}

			// Token: 0x0600041B RID: 1051 RVA: 0x00017FD0 File Offset: 0x000161D0
			[CompilerGenerated]
			public override int GetHashCode()
			{
				return EqualityComparer<string>.Default.GetHashCode(this.<Message>k__BackingField) * -1521134295 + EqualityComparer<SpeechBubble.SpeechType>.Default.GetHashCode(this.<Type>k__BackingField);
			}

			// Token: 0x0600041C RID: 1052 RVA: 0x00017FF9 File Offset: 0x000161F9
			[NullableContext(0)]
			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				return obj is ChatUIController.SpeechBubbleData && this.Equals((ChatUIController.SpeechBubbleData)obj);
			}

			// Token: 0x0600041D RID: 1053 RVA: 0x00018011 File Offset: 0x00016211
			[CompilerGenerated]
			public bool Equals(ChatUIController.SpeechBubbleData other)
			{
				return EqualityComparer<string>.Default.Equals(this.<Message>k__BackingField, other.<Message>k__BackingField) && EqualityComparer<SpeechBubble.SpeechType>.Default.Equals(this.<Type>k__BackingField, other.<Type>k__BackingField);
			}

			// Token: 0x0600041E RID: 1054 RVA: 0x00018043 File Offset: 0x00016243
			[CompilerGenerated]
			public void Deconstruct(out string Message, out SpeechBubble.SpeechType Type)
			{
				Message = this.Message;
				Type = this.Type;
			}
		}

		// Token: 0x020000A2 RID: 162
		[Nullable(0)]
		private sealed class SpeechBubbleQueueData
		{
			// Token: 0x170000A9 RID: 169
			// (get) Token: 0x0600041F RID: 1055 RVA: 0x00018055 File Offset: 0x00016255
			// (set) Token: 0x06000420 RID: 1056 RVA: 0x0001805D File Offset: 0x0001625D
			public float TimeLeft { get; set; }

			// Token: 0x170000AA RID: 170
			// (get) Token: 0x06000421 RID: 1057 RVA: 0x00018066 File Offset: 0x00016266
			public Queue<ChatUIController.SpeechBubbleData> MessageQueue { get; } = new Queue<ChatUIController.SpeechBubbleData>();
		}
	}
}
