using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Hands
{
	// Token: 0x0200042C RID: 1068
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class RequestSetHandEvent : EntityEventArgs
	{
		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x06000CD1 RID: 3281 RVA: 0x0002A48A File Offset: 0x0002868A
		public string HandName { get; }

		// Token: 0x06000CD2 RID: 3282 RVA: 0x0002A492 File Offset: 0x00028692
		public RequestSetHandEvent(string handName)
		{
			this.HandName = handName;
		}
	}
}
