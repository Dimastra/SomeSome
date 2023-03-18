using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Hands
{
	// Token: 0x02000439 RID: 1081
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class RequestHandAltInteractEvent : EntityEventArgs
	{
		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x06000CEB RID: 3307 RVA: 0x0002A5CF File Offset: 0x000287CF
		public string HandName { get; }

		// Token: 0x06000CEC RID: 3308 RVA: 0x0002A5D7 File Offset: 0x000287D7
		public RequestHandAltInteractEvent(string handName)
		{
			this.HandName = handName;
		}
	}
}
