using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Hands
{
	// Token: 0x02000437 RID: 1079
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class RequestHandInteractUsingEvent : EntityEventArgs
	{
		// Token: 0x170002B3 RID: 691
		// (get) Token: 0x06000CE7 RID: 3303 RVA: 0x0002A5A1 File Offset: 0x000287A1
		public string HandName { get; }

		// Token: 0x06000CE8 RID: 3304 RVA: 0x0002A5A9 File Offset: 0x000287A9
		public RequestHandInteractUsingEvent(string handName)
		{
			this.HandName = handName;
		}
	}
}
