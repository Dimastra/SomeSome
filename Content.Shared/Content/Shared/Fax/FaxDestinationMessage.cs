using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Fax
{
	// Token: 0x02000494 RID: 1172
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class FaxDestinationMessage : BoundUserInterfaceMessage
	{
		// Token: 0x170002FC RID: 764
		// (get) Token: 0x06000E41 RID: 3649 RVA: 0x0002DCC1 File Offset: 0x0002BEC1
		public string Address { get; }

		// Token: 0x06000E42 RID: 3650 RVA: 0x0002DCC9 File Offset: 0x0002BEC9
		public FaxDestinationMessage(string address)
		{
			this.Address = address;
		}
	}
}
