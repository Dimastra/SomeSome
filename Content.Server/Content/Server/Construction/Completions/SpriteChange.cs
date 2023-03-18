using System;
using System.Runtime.CompilerServices;
using Content.Shared.Construction;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Server.Construction.Completions
{
	// Token: 0x02000626 RID: 1574
	[NullableContext(2)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class SpriteChange : IGraphAction
	{
		// Token: 0x17000517 RID: 1303
		// (get) Token: 0x06002186 RID: 8582 RVA: 0x000AECFD File Offset: 0x000ACEFD
		// (set) Token: 0x06002187 RID: 8583 RVA: 0x000AED05 File Offset: 0x000ACF05
		[DataField("layer", false, 1, false, false, null)]
		public int Layer { get; private set; }

		// Token: 0x17000518 RID: 1304
		// (get) Token: 0x06002188 RID: 8584 RVA: 0x000AED0E File Offset: 0x000ACF0E
		// (set) Token: 0x06002189 RID: 8585 RVA: 0x000AED16 File Offset: 0x000ACF16
		[DataField("specifier", false, 1, false, false, null)]
		public SpriteSpecifier SpriteSpecifier { get; private set; } = SpriteSpecifier.Invalid;

		// Token: 0x0600218A RID: 8586 RVA: 0x000AED20 File Offset: 0x000ACF20
		[NullableContext(1)]
		public void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager)
		{
			if (this.SpriteSpecifier == null || this.SpriteSpecifier == SpriteSpecifier.Invalid)
			{
				return;
			}
			SpriteComponent sprite;
			if (!entityManager.TryGetComponent<SpriteComponent>(uid, ref sprite))
			{
				return;
			}
			if (sprite.LayerCount <= this.Layer)
			{
				return;
			}
			sprite.LayerSetSprite(this.Layer, this.SpriteSpecifier);
		}
	}
}
