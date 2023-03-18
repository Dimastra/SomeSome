using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.Speech
{
	// Token: 0x020001AE RID: 430
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AccentGetEvent : EntityEventArgs
	{
		// Token: 0x1700016B RID: 363
		// (get) Token: 0x06000879 RID: 2169 RVA: 0x0002B4BB File Offset: 0x000296BB
		public EntityUid Entity { get; }

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x0600087A RID: 2170 RVA: 0x0002B4C3 File Offset: 0x000296C3
		// (set) Token: 0x0600087B RID: 2171 RVA: 0x0002B4CB File Offset: 0x000296CB
		public string Message { get; set; }

		// Token: 0x0600087C RID: 2172 RVA: 0x0002B4D4 File Offset: 0x000296D4
		public AccentGetEvent(EntityUid entity, string message)
		{
			this.Entity = entity;
			this.Message = message;
		}
	}
}
