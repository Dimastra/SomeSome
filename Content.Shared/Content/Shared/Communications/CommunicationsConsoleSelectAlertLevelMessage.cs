using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Communications
{
	// Token: 0x02000594 RID: 1428
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class CommunicationsConsoleSelectAlertLevelMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06001178 RID: 4472 RVA: 0x00039271 File Offset: 0x00037471
		public CommunicationsConsoleSelectAlertLevelMessage(string level)
		{
			this.Level = level;
		}

		// Token: 0x04001027 RID: 4135
		public readonly string Level;
	}
}
