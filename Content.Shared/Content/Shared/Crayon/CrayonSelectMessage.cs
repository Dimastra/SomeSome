using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Crayon
{
	// Token: 0x02000553 RID: 1363
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class CrayonSelectMessage : BoundUserInterfaceMessage
	{
		// Token: 0x06001096 RID: 4246 RVA: 0x0003624B File Offset: 0x0003444B
		public CrayonSelectMessage(string selected)
		{
			this.State = selected;
		}

		// Token: 0x04000F8A RID: 3978
		public readonly string State;
	}
}
