using System;
using System.Runtime.CompilerServices;
using Content.Server.Popups;
using Content.Shared.Construction;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Completions
{
	// Token: 0x0200061F RID: 1567
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class PopupUser : IGraphAction
	{
		// Token: 0x1700050A RID: 1290
		// (get) Token: 0x06002167 RID: 8551 RVA: 0x000AE9B2 File Offset: 0x000ACBB2
		[DataField("cursor", false, 1, false, false, null)]
		public bool Cursor { get; }

		// Token: 0x1700050B RID: 1291
		// (get) Token: 0x06002168 RID: 8552 RVA: 0x000AE9BA File Offset: 0x000ACBBA
		[DataField("text", false, 1, false, false, null)]
		public string Text { get; } = string.Empty;

		// Token: 0x06002169 RID: 8553 RVA: 0x000AE9C4 File Offset: 0x000ACBC4
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			if (userUid == null)
			{
				return;
			}
			PopupSystem popupSystem = entityManager.EntitySysManager.GetEntitySystem<PopupSystem>();
			if (this.Cursor)
			{
				popupSystem.PopupCursor(Loc.GetString(this.Text), userUid.Value, PopupType.Small);
				return;
			}
			popupSystem.PopupEntity(Loc.GetString(this.Text), uid, userUid.Value, PopupType.Small);
		}
	}
}
