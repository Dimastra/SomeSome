using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Communications
{
	// Token: 0x02000595 RID: 1429
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class CommunicationsConsoleAnnounceMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06001179 RID: 4473 RVA: 0x00039280 File Offset: 0x00037480
		public CommunicationsConsoleAnnounceMessage(string message)
		{
			this.Message = message;
		}

		// Token: 0x04001028 RID: 4136
		public readonly string Message;
	}
}
