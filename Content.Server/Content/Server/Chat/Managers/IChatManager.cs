using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Chat;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Server.Chat.Managers
{
	// Token: 0x020006CB RID: 1739
	[NullableContext(1)]
	public interface IChatManager
	{
		// Token: 0x06002447 RID: 9287
		void Initialize();

		// Token: 0x06002448 RID: 9288
		void ClearCache();

		// Token: 0x06002449 RID: 9289
		void CheckMessageCoolDown(IPlayerSession player, Dictionary<NetUserId, TimeSpan> lastSendMessageStorage, int coolDown, out int remainingTime);

		// Token: 0x0600244A RID: 9290
		bool CheckSpamUserMessage(EntityUid source, string message, [Nullable(2)] IPlayerSession player, bool hideChat);

		// Token: 0x0600244B RID: 9291
		void DispatchServerAnnouncement(string message, Color? colorOverride = null);

		// Token: 0x0600244C RID: 9292
		void DispatchServerMessage(IPlayerSession player, string message, bool suppressLog = false);

		// Token: 0x0600244D RID: 9293
		void TrySendOOCMessage(IPlayerSession player, string message, OOCChatType type);

		// Token: 0x0600244E RID: 9294
		void SendHookOOC(string sender, string message);

		// Token: 0x0600244F RID: 9295
		void SendHookAdminChat(string sender, string message);

		// Token: 0x06002450 RID: 9296
		void SendAdminAnnouncement(string message);

		// Token: 0x06002451 RID: 9297
		void ChatMessageToOne(ChatChannel channel, string message, string wrappedMessage, EntityUid source, bool hideChat, INetChannel client, Color? colorOverride = null, bool recordReplay = false, [Nullable(2)] string audioPath = null, float audioVolume = 0f);

		// Token: 0x06002452 RID: 9298
		void ChatMessageToMany(ChatChannel channel, string message, string wrappedMessage, EntityUid source, bool hideChat, bool recordReplay, IEnumerable<INetChannel> clients, Color? colorOverride = null, [Nullable(2)] string audioPath = null, float audioVolume = 0f);

		// Token: 0x06002453 RID: 9299
		void ChatMessageToManyFiltered(Filter filter, ChatChannel channel, string message, string wrappedMessage, EntityUid source, bool hideChat, bool recordReplay, Color? colorOverride, [Nullable(2)] string audioPath = null, float audioVolume = 0f);

		// Token: 0x06002454 RID: 9300
		void ChatMessageToAll(ChatChannel channel, string message, string wrappedMessage, EntityUid source, bool hideChat, bool recordReplay, Color? colorOverride = null, [Nullable(2)] string audioPath = null, float audioVolume = 0f);

		// Token: 0x06002455 RID: 9301
		bool MessageCharacterLimit(IPlayerSession player, string message);
	}
}
