using System;
using System.Runtime.CompilerServices;
using Content.Shared.Construction;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Completions
{
	// Token: 0x02000620 RID: 1568
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class RaiseEvent : IGraphAction
	{
		// Token: 0x1700050C RID: 1292
		// (get) Token: 0x0600216B RID: 8555 RVA: 0x000AEA36 File Offset: 0x000ACC36
		[DataField("event", false, 1, true, false, null)]
		public EntityEventArgs Event { get; }

		// Token: 0x1700050D RID: 1293
		// (get) Token: 0x0600216C RID: 8556 RVA: 0x000AEA3E File Offset: 0x000ACC3E
		[DataField("directed", false, 1, false, false, null)]
		public bool Directed { get; } = 1;

		// Token: 0x1700050E RID: 1294
		// (get) Token: 0x0600216D RID: 8557 RVA: 0x000AEA46 File Offset: 0x000ACC46
		[DataField("broadcast", false, 1, false, false, null)]
		public bool Broadcast { get; } = 1;

		// Token: 0x0600216E RID: 8558 RVA: 0x000AEA50 File Offset: 0x000ACC50
		[NullableContext(1)]
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			if (this.Event == null || (!this.Directed && !this.Broadcast))
			{
				return;
			}
			if (this.Directed)
			{
				entityManager.EventBus.RaiseLocalEvent(uid, this.Event, false);
			}
			if (this.Broadcast)
			{
				entityManager.EventBus.RaiseEvent(1, this.Event);
			}
		}
	}
}
