using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.White.TTS
{
	// Token: 0x02000086 RID: 134
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class TransformSpeakerVoiceEvent : EntityEventArgs
	{
		// Token: 0x06000200 RID: 512 RVA: 0x0000B7DD File Offset: 0x000099DD
		public TransformSpeakerVoiceEvent(EntityUid sender, string voiceId)
		{
			this.Sender = sender;
			this.VoiceId = voiceId;
		}

		// Token: 0x04000177 RID: 375
		public EntityUid Sender;

		// Token: 0x04000178 RID: 376
		public string VoiceId;
	}
}
