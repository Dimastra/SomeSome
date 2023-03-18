using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Hands
{
	// Token: 0x02000438 RID: 1080
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class RequestMoveHandItemEvent : EntityEventArgs
	{
		// Token: 0x170002B4 RID: 692
		// (get) Token: 0x06000CE9 RID: 3305 RVA: 0x0002A5B8 File Offset: 0x000287B8
		public string HandName { get; }

		// Token: 0x06000CEA RID: 3306 RVA: 0x0002A5C0 File Offset: 0x000287C0
		public RequestMoveHandItemEvent(string handName)
		{
			this.HandName = handName;
		}
	}
}
