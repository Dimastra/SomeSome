using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Research.Components
{
	// Token: 0x02000210 RID: 528
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class ConsoleUnlockTechnologyMessage : BoundUserInterfaceMessage
	{
		// Token: 0x060005ED RID: 1517 RVA: 0x00015023 File Offset: 0x00013223
		public ConsoleUnlockTechnologyMessage(string id)
		{
			this.Id = id;
		}

		// Token: 0x040005F6 RID: 1526
		public string Id;
	}
}
