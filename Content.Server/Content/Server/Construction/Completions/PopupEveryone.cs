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
	// Token: 0x0200061E RID: 1566
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class PopupEveryone : IGraphAction
	{
		// Token: 0x17000509 RID: 1289
		// (get) Token: 0x06002164 RID: 8548 RVA: 0x000AE978 File Offset: 0x000ACB78
		[DataField("text", false, 1, false, false, null)]
		public string Text { get; } = string.Empty;

		// Token: 0x06002165 RID: 8549 RVA: 0x000AE980 File Offset: 0x000ACB80
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			entityManager.EntitySysManager.GetEntitySystem<PopupSystem>().PopupEntity(Loc.GetString(this.Text), uid, PopupType.Small);
		}
	}
}
