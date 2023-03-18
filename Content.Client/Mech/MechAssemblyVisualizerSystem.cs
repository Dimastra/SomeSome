using System;
using System.Runtime.CompilerServices;
using Content.Shared.Mech;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Mech
{
	// Token: 0x0200023A RID: 570
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class MechAssemblyVisualizerSystem : VisualizerSystem<MechAssemblyVisualsComponent>
	{
		// Token: 0x06000E86 RID: 3718 RVA: 0x00057AF0 File Offset: 0x00055CF0
		protected override void OnAppearanceChange(EntityUid uid, MechAssemblyVisualsComponent component, ref AppearanceChangeEvent args)
		{
			base.OnAppearanceChange(uid, component, ref args);
			int num;
			if (!this.AppearanceSystem.TryGetData<int>(uid, MechAssemblyVisuals.State, ref num, args.Component))
			{
				return;
			}
			string text = component.StatePrefix + num.ToString();
			SpriteComponent sprite = args.Sprite;
			if (sprite == null)
			{
				return;
			}
			sprite.LayerSetState(0, text);
		}
	}
}
