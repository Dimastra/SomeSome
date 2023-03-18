using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Hands
{
	// Token: 0x02000436 RID: 1078
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class RequestActivateInHandEvent : EntityEventArgs
	{
		// Token: 0x170002B2 RID: 690
		// (get) Token: 0x06000CE5 RID: 3301 RVA: 0x0002A58A File Offset: 0x0002878A
		public string HandName { get; }

		// Token: 0x06000CE6 RID: 3302 RVA: 0x0002A592 File Offset: 0x00028792
		public RequestActivateInHandEvent(string handName)
		{
			this.HandName = handName;
		}
	}
}
