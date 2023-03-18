using System;
using System.Runtime.CompilerServices;
using Content.Shared.Materials;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Materials
{
	// Token: 0x02000246 RID: 582
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MaterialStorageSystem : SharedMaterialStorageSystem
	{
		// Token: 0x06000EBF RID: 3775 RVA: 0x000590E9 File Offset: 0x000572E9
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MaterialStorageComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<MaterialStorageComponent, AppearanceChangeEvent>(this.OnAppearanceChange), null, null);
		}

		// Token: 0x06000EC0 RID: 3776 RVA: 0x00059108 File Offset: 0x00057308
		private void OnAppearanceChange(EntityUid uid, MaterialStorageComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			int num;
			if (!args.Sprite.LayerMapTryGet(MaterialStorageVisualLayers.Inserting, ref num, false))
			{
				return;
			}
			bool flag;
			if (!this._appearance.TryGetData<bool>(uid, MaterialStorageVisuals.Inserting, ref flag, args.Component))
			{
				return;
			}
			InsertingMaterialStorageComponent insertingMaterialStorageComponent;
			if (flag && base.TryComp<InsertingMaterialStorageComponent>(uid, ref insertingMaterialStorageComponent))
			{
				args.Sprite.LayerSetAnimationTime(num, 0f);
				args.Sprite.LayerSetVisible(num, true);
				if (insertingMaterialStorageComponent.MaterialColor != null)
				{
					args.Sprite.LayerSetColor(num, insertingMaterialStorageComponent.MaterialColor.Value);
					return;
				}
			}
			else
			{
				args.Sprite.LayerSetVisible(num, false);
			}
		}

		// Token: 0x06000EC1 RID: 3777 RVA: 0x000591B0 File Offset: 0x000573B0
		[NullableContext(2)]
		public override bool TryInsertMaterialEntity(EntityUid user, EntityUid toInsert, EntityUid receiver, MaterialStorageComponent component = null)
		{
			if (!base.TryInsertMaterialEntity(user, toInsert, receiver, component))
			{
				return false;
			}
			this._transform.DetachParentToNull(toInsert, base.Transform(toInsert));
			return true;
		}

		// Token: 0x0400074D RID: 1869
		[Dependency]
		private readonly AppearanceSystem _appearance;

		// Token: 0x0400074E RID: 1870
		[Dependency]
		private readonly TransformSystem _transform;
	}
}
