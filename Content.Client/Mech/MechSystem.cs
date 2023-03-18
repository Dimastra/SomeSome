using System;
using System.Runtime.CompilerServices;
using Content.Shared.DrawDepth;
using Content.Shared.Mech;
using Content.Shared.Mech.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Mech
{
	// Token: 0x0200023D RID: 573
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MechSystem : SharedMechSystem
	{
		// Token: 0x06000E8A RID: 3722 RVA: 0x00057B70 File Offset: 0x00055D70
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<MechComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<MechComponent, AppearanceChangeEvent>(this.OnAppearanceChanged), null, null);
		}

		// Token: 0x06000E8B RID: 3723 RVA: 0x00057B8C File Offset: 0x00055D8C
		private void OnAppearanceChanged(EntityUid uid, MechComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			SpriteComponent.Layer layer;
			if (!args.Sprite.TryGetLayer(0, ref layer, false))
			{
				return;
			}
			string text = component.BaseState;
			DrawDepth drawDepth = DrawDepth.Mobs;
			bool flag;
			bool flag2;
			if (component.BrokenState != null && this._appearance.TryGetData<bool>(uid, MechVisuals.Broken, ref flag, args.Component) && flag)
			{
				text = component.BrokenState;
				drawDepth = DrawDepth.SmallMobs;
			}
			else if (component.OpenState != null && this._appearance.TryGetData<bool>(uid, MechVisuals.Open, ref flag2, args.Component) && flag2)
			{
				text = component.OpenState;
				drawDepth = DrawDepth.SmallMobs;
			}
			layer.SetState(text);
			args.Sprite.DrawDepth = (int)drawDepth;
		}

		// Token: 0x0400073C RID: 1852
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
