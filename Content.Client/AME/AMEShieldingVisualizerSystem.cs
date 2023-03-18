using System;
using System.Runtime.CompilerServices;
using Content.Client.AME.Components;
using Content.Shared.AME;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.AME
{
	// Token: 0x02000475 RID: 1141
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class AMEShieldingVisualizerSystem : VisualizerSystem<AMEShieldingVisualsComponent>
	{
		// Token: 0x06001C37 RID: 7223 RVA: 0x000A3C42 File Offset: 0x000A1E42
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AMEShieldingVisualsComponent, ComponentInit>(new ComponentEventHandler<AMEShieldingVisualsComponent, ComponentInit>(this.OnComponentInit), null, null);
		}

		// Token: 0x06001C38 RID: 7224 RVA: 0x000A3C60 File Offset: 0x000A1E60
		private void OnComponentInit(EntityUid uid, AMEShieldingVisualsComponent component, ComponentInit args)
		{
			SpriteComponent spriteComponent;
			if (base.TryComp<SpriteComponent>(uid, ref spriteComponent))
			{
				spriteComponent.LayerMapSet(AMEShieldingVisualsLayer.Core, spriteComponent.AddLayerState("core", null));
				spriteComponent.LayerSetVisible(AMEShieldingVisualsLayer.Core, false);
				spriteComponent.LayerMapSet(AMEShieldingVisualsLayer.CoreState, spriteComponent.AddLayerState("core_weak", null));
				spriteComponent.LayerSetVisible(AMEShieldingVisualsLayer.CoreState, false);
			}
		}

		// Token: 0x06001C39 RID: 7225 RVA: 0x000A3CD4 File Offset: 0x000A1ED4
		protected override void OnAppearanceChange(EntityUid uid, AMEShieldingVisualsComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			string a;
			if (this.AppearanceSystem.TryGetData<string>(uid, SharedAMEShieldComponent.AMEShieldVisuals.Core, ref a, args.Component))
			{
				if (a == "isCore")
				{
					args.Sprite.LayerSetState(AMEShieldingVisualsLayer.Core, "core");
					args.Sprite.LayerSetVisible(AMEShieldingVisualsLayer.Core, true);
				}
				else
				{
					args.Sprite.LayerSetVisible(AMEShieldingVisualsLayer.Core, false);
				}
			}
			string a2;
			if (this.AppearanceSystem.TryGetData<string>(uid, SharedAMEShieldComponent.AMEShieldVisuals.CoreState, ref a2, args.Component))
			{
				if (a2 == "weak")
				{
					args.Sprite.LayerSetState(AMEShieldingVisualsLayer.CoreState, "core_weak");
					args.Sprite.LayerSetVisible(AMEShieldingVisualsLayer.CoreState, true);
					return;
				}
				if (a2 == "strong")
				{
					args.Sprite.LayerSetState(AMEShieldingVisualsLayer.CoreState, "core_strong");
					args.Sprite.LayerSetVisible(AMEShieldingVisualsLayer.CoreState, true);
					return;
				}
				if (!(a2 == "off"))
				{
					return;
				}
				args.Sprite.LayerSetVisible(AMEShieldingVisualsLayer.CoreState, false);
			}
		}
	}
}
