using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Temperature
{
	// Token: 0x020000DC RID: 220
	public sealed class IsHotEvent : EntityEventArgs
	{
		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000266 RID: 614 RVA: 0x0000BABD File Offset: 0x00009CBD
		// (set) Token: 0x06000267 RID: 615 RVA: 0x0000BAC5 File Offset: 0x00009CC5
		public bool IsHot { get; set; }
	}
}
