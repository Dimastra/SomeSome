using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.VoiceMask
{
	// Token: 0x02000085 RID: 133
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class VoiceMaskChangeVoiceMessage : BoundUserInterfaceMessage
	{
		// Token: 0x1700003E RID: 62
		// (get) Token: 0x0600018F RID: 399 RVA: 0x00008E2A File Offset: 0x0000702A
		public string Voice { get; }

		// Token: 0x06000190 RID: 400 RVA: 0x00008E32 File Offset: 0x00007032
		public VoiceMaskChangeVoiceMessage(string voice)
		{
			this.Voice = voice;
		}
	}
}
