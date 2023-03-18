using System;
using System.Runtime.CompilerServices;
using Content.Client.AME.Components;
using Content.Shared.AME;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.AME
{
	// Token: 0x02000473 RID: 1139
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class AMEControllerVisualizerSystem : VisualizerSystem<AMEControllerVisualsComponent>
	{
		// Token: 0x06001C33 RID: 7219 RVA: 0x000A3AB0 File Offset: 0x000A1CB0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AMEControllerVisualsComponent, ComponentInit>(new ComponentEventHandler<AMEControllerVisualsComponent, ComponentInit>(this.OnComponentInit), null, null);
		}

		// Token: 0x06001C34 RID: 7220 RVA: 0x000A3ACC File Offset: 0x000A1CCC
		private void OnComponentInit(EntityUid uid, AMEControllerVisualsComponent component, ComponentInit args)
		{
			SpriteComponent spriteComponent;
			if (base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				spriteComponent.LayerMapSet(AMEControllerVisualLayers.Display, spriteComponent.AddLayerState("control_on", null));
				spriteComponent.LayerSetVisible(AMEControllerVisualLayers.Display, false);
			}
		}

		// Token: 0x06001C35 RID: 7221 RVA: 0x000A3B14 File Offset: 0x000A1D14
		protected override void OnAppearanceChange(EntityUid uid, AMEControllerVisualsComponent component, ref AppearanceChangeEvent args)
		{
			base.OnAppearanceChange(uid, component, ref args);
			string a;
			if (args.Sprite == null || !this.AppearanceSystem.TryGetData<string>(uid, SharedAMEControllerComponent.AMEControllerVisuals.DisplayState, ref a, args.Component))
			{
				return;
			}
			if (a == "on")
			{
				args.Sprite.LayerSetState(AMEControllerVisualLayers.Display, "control_on");
				args.Sprite.LayerSetVisible(AMEControllerVisualLayers.Display, true);
				return;
			}
			if (a == "critical")
			{
				args.Sprite.LayerSetState(AMEControllerVisualLayers.Display, "control_critical");
				args.Sprite.LayerSetVisible(AMEControllerVisualLayers.Display, true);
				return;
			}
			if (a == "fuck")
			{
				args.Sprite.LayerSetState(AMEControllerVisualLayers.Display, "control_fuck");
				args.Sprite.LayerSetVisible(AMEControllerVisualLayers.Display, true);
				return;
			}
			if (!(a == "off"))
			{
				args.Sprite.LayerSetVisible(AMEControllerVisualLayers.Display, false);
				return;
			}
			args.Sprite.LayerSetVisible(AMEControllerVisualLayers.Display, false);
		}
	}
}
