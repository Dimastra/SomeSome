using System;
using System.Runtime.CompilerServices;
using Content.Shared.SubFloor;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.ViewVariables;

namespace Content.Client.SubFloor
{
	// Token: 0x0200010F RID: 271
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SubFloorHideSystem : SharedSubFloorHideSystem
	{
		// Token: 0x17000154 RID: 340
		// (get) Token: 0x06000799 RID: 1945 RVA: 0x00027C37 File Offset: 0x00025E37
		// (set) Token: 0x0600079A RID: 1946 RVA: 0x00027C3F File Offset: 0x00025E3F
		[ViewVariables]
		public bool ShowAll
		{
			get
			{
				return this._showAll;
			}
			set
			{
				if (this._showAll == value)
				{
					return;
				}
				this._showAll = value;
				this.UpdateAll();
			}
		}

		// Token: 0x0600079B RID: 1947 RVA: 0x00027C58 File Offset: 0x00025E58
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SubFloorHideComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<SubFloorHideComponent, AppearanceChangeEvent>(this.OnAppearanceChanged), null, null);
		}

		// Token: 0x0600079C RID: 1948 RVA: 0x00027C74 File Offset: 0x00025E74
		private void OnAppearanceChanged(EntityUid uid, SubFloorHideComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			bool flag;
			this._appearance.TryGetData<bool>(uid, SubFloorVisuals.Covered, ref flag, args.Component);
			bool flag2;
			this._appearance.TryGetData<bool>(uid, SubFloorVisuals.ScannerRevealed, ref flag2, args.Component);
			flag2 &= !this.ShowAll;
			bool flag3 = !flag || this.ShowAll || flag2;
			float num = flag2 ? component.ScannerTransparency : 1f;
			foreach (ISpriteLayer spriteLayer in args.Sprite.AllLayers)
			{
				spriteLayer.Visible = flag3;
				if (spriteLayer.Visible)
				{
					spriteLayer.Color = spriteLayer.Color.WithAlpha(num);
				}
			}
			bool flag4 = false;
			foreach (Enum @enum in component.VisibleLayers)
			{
				int num2;
				if (args.Sprite.LayerMapTryGet(@enum, ref num2, false))
				{
					ISpriteLayer spriteLayer2 = args.Sprite[num2];
					spriteLayer2.Visible = true;
					spriteLayer2.Color = spriteLayer2.Color.WithAlpha(1f);
					flag4 = true;
				}
			}
			args.Sprite.Visible = (flag4 || flag3);
		}

		// Token: 0x0600079D RID: 1949 RVA: 0x00027DEC File Offset: 0x00025FEC
		private void UpdateAll()
		{
			foreach (ValueTuple<SubFloorHideComponent, AppearanceComponent> valueTuple in this.EntityManager.EntityQuery<SubFloorHideComponent, AppearanceComponent>(true))
			{
				AppearanceComponent item = valueTuple.Item2;
				this._appearance.MarkDirty(item, true);
			}
		}

		// Token: 0x0400037F RID: 895
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04000380 RID: 896
		private bool _showAll;
	}
}
