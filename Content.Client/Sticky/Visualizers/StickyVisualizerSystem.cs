using System;
using System.Runtime.CompilerServices;
using Content.Shared.Sticky.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Sticky.Visualizers
{
	// Token: 0x0200012E RID: 302
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class StickyVisualizerSystem : VisualizerSystem<StickyVisualizerComponent>
	{
		// Token: 0x06000824 RID: 2084 RVA: 0x0002F647 File Offset: 0x0002D847
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<StickyVisualizerComponent, ComponentInit>(new ComponentEventHandler<StickyVisualizerComponent, ComponentInit>(this.OnInit), null, null);
		}

		// Token: 0x06000825 RID: 2085 RVA: 0x0002F664 File Offset: 0x0002D864
		private void OnInit(EntityUid uid, StickyVisualizerComponent component, ComponentInit args)
		{
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				return;
			}
			component.DefaultDrawDepth = spriteComponent.DrawDepth;
		}

		// Token: 0x06000826 RID: 2086 RVA: 0x0002F68C File Offset: 0x0002D88C
		protected override void OnAppearanceChange(EntityUid uid, StickyVisualizerComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			bool flag;
			if (!this.AppearanceSystem.TryGetData<bool>(uid, StickyVisuals.IsStuck, ref flag, args.Component))
			{
				return;
			}
			int drawDepth = flag ? component.StuckDrawDepth : component.DefaultDrawDepth;
			args.Sprite.DrawDepth = drawDepth;
		}
	}
}
