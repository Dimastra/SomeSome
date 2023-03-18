using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.White.TTS
{
	// Token: 0x0200002F RID: 47
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class RequestTTSEvent : EntityEventArgs
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000040 RID: 64 RVA: 0x00002B86 File Offset: 0x00000D86
		public string Text { get; }

		// Token: 0x06000041 RID: 65 RVA: 0x00002B8E File Offset: 0x00000D8E
		public RequestTTSEvent(string text)
		{
			this.Text = text;
		}
	}
}
