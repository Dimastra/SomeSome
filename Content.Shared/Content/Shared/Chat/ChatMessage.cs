using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Chat
{
	// Token: 0x02000601 RID: 1537
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ChatMessage
	{
		// Token: 0x060012D7 RID: 4823 RVA: 0x0003DC4C File Offset: 0x0003BE4C
		public ChatMessage(ChatChannel channel, string message, string wrappedMessage, EntityUid source, bool hideChat = false, Color? colorOverride = null, [Nullable(2)] string audioPath = null, float audioVolume = 0f)
		{
			this.Channel = channel;
			this.Message = message;
			this.WrappedMessage = wrappedMessage;
			this.SenderEntity = source;
			this.HideChat = hideChat;
			this.MessageColorOverride = colorOverride;
			this.AudioPath = audioPath;
			this.AudioVolume = audioVolume;
		}

		// Token: 0x04001182 RID: 4482
		public ChatChannel Channel;

		// Token: 0x04001183 RID: 4483
		public string Message;

		// Token: 0x04001184 RID: 4484
		public string WrappedMessage;

		// Token: 0x04001185 RID: 4485
		public EntityUid SenderEntity;

		// Token: 0x04001186 RID: 4486
		public bool HideChat;

		// Token: 0x04001187 RID: 4487
		public Color? MessageColorOverride;

		// Token: 0x04001188 RID: 4488
		[Nullable(2)]
		public string AudioPath;

		// Token: 0x04001189 RID: 4489
		public float AudioVolume;

		// Token: 0x0400118A RID: 4490
		[NonSerialized]
		public bool Read;
	}
}
