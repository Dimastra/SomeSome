using System;
using System.Runtime.CompilerServices;
using Content.Shared.Construction;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Completions
{
	// Token: 0x02000627 RID: 1575
	[NullableContext(2)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class SpriteStateChange : IGraphAction
	{
		// Token: 0x17000519 RID: 1305
		// (get) Token: 0x0600218C RID: 8588 RVA: 0x000AED83 File Offset: 0x000ACF83
		// (set) Token: 0x0600218D RID: 8589 RVA: 0x000AED8B File Offset: 0x000ACF8B
		[DataField("layer", false, 1, false, false, null)]
		public int Layer { get; private set; }

		// Token: 0x1700051A RID: 1306
		// (get) Token: 0x0600218E RID: 8590 RVA: 0x000AED94 File Offset: 0x000ACF94
		// (set) Token: 0x0600218F RID: 8591 RVA: 0x000AED9C File Offset: 0x000ACF9C
		[DataField("state", false, 1, false, false, null)]
		public string State { get; private set; } = string.Empty;

		// Token: 0x06002190 RID: 8592 RVA: 0x000AEDA8 File Offset: 0x000ACFA8
		[NullableContext(1)]
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			SpriteComponent sprite;
			if (string.IsNullOrEmpty(this.State) || !entityManager.TryGetComponent<SpriteComponent>(uid, ref sprite))
			{
				return;
			}
			if (sprite.LayerCount <= this.Layer)
			{
				return;
			}
			sprite.LayerSetState(this.Layer, this.State);
		}
	}
}
