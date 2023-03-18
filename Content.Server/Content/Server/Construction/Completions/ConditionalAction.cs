using System;
using System.Runtime.CompilerServices;
using Content.Shared.Construction;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Completions
{
	// Token: 0x02000613 RID: 1555
	[NullableContext(2)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class ConditionalAction : IGraphAction
	{
		// Token: 0x170004FE RID: 1278
		// (get) Token: 0x06002140 RID: 8512 RVA: 0x000AE44A File Offset: 0x000AC64A
		[DataField("passUser", false, 1, false, false, null)]
		public bool PassUser { get; }

		// Token: 0x170004FF RID: 1279
		// (get) Token: 0x06002141 RID: 8513 RVA: 0x000AE452 File Offset: 0x000AC652
		[DataField("condition", false, 1, true, false, null)]
		public IGraphCondition Condition { get; }

		// Token: 0x17000500 RID: 1280
		// (get) Token: 0x06002142 RID: 8514 RVA: 0x000AE45A File Offset: 0x000AC65A
		[DataField("action", false, 1, true, false, null)]
		public IGraphAction Action { get; }

		// Token: 0x17000501 RID: 1281
		// (get) Token: 0x06002143 RID: 8515 RVA: 0x000AE462 File Offset: 0x000AC662
		[DataField("else", false, 1, false, false, null)]
		public IGraphAction Else { get; }

		// Token: 0x06002144 RID: 8516 RVA: 0x000AE46C File Offset: 0x000AC66C
		[NullableContext(1)]
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			if (this.Condition == null || this.Action == null)
			{
				return;
			}
			if (this.Condition.Condition((this.PassUser && userUid != null) ? userUid.Value : uid, entityManager))
			{
				this.Action.PerformAction(uid, userUid, entityManager);
				return;
			}
			IGraphAction @else = this.Else;
			if (@else == null)
			{
				return;
			}
			@else.PerformAction(uid, userUid, entityManager);
		}
	}
}
