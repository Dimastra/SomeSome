using System;
using System.Runtime.CompilerServices;
using Content.Shared.Pinpointer;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Pinpointer
{
	// Token: 0x020001B9 RID: 441
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class PinpointerVisualizerSystem : VisualizerSystem<PinpointerComponent>
	{
		// Token: 0x06000B5C RID: 2908 RVA: 0x00041ED0 File Offset: 0x000400D0
		protected override void OnAppearanceChange(EntityUid uid, PinpointerComponent component, ref AppearanceChangeEvent args)
		{
			base.OnAppearanceChange(uid, component, ref args);
			SpriteComponent spriteComponent;
			if (!base.TryComp<SpriteComponent>(component.Owner, ref spriteComponent))
			{
				return;
			}
			bool flag;
			if (!this.AppearanceSystem.TryGetData<bool>(uid, PinpointerVisuals.IsActive, ref flag, args.Component) || !flag)
			{
				spriteComponent.LayerSetVisible(PinpointerLayers.Screen, false);
				return;
			}
			spriteComponent.LayerSetVisible(PinpointerLayers.Screen, true);
			Distance distance;
			Angle angle;
			if (!this.AppearanceSystem.TryGetData<Distance>(uid, PinpointerVisuals.TargetDistance, ref distance, args.Component) || !this.AppearanceSystem.TryGetData<Angle>(uid, PinpointerVisuals.ArrowAngle, ref angle, args.Component))
			{
				spriteComponent.LayerSetState(PinpointerLayers.Screen, "pinonnull");
				spriteComponent.LayerSetRotation(PinpointerLayers.Screen, Angle.Zero);
				return;
			}
			switch (distance)
			{
			case Distance.Unknown:
				spriteComponent.LayerSetState(PinpointerLayers.Screen, "pinonnull");
				spriteComponent.LayerSetRotation(PinpointerLayers.Screen, Angle.Zero);
				return;
			case Distance.Reached:
				spriteComponent.LayerSetState(PinpointerLayers.Screen, "pinondirect");
				spriteComponent.LayerSetRotation(PinpointerLayers.Screen, Angle.Zero);
				return;
			case Distance.Close:
				spriteComponent.LayerSetState(PinpointerLayers.Screen, "pinonclose");
				spriteComponent.LayerSetRotation(PinpointerLayers.Screen, angle);
				return;
			case Distance.Medium:
				spriteComponent.LayerSetState(PinpointerLayers.Screen, "pinonmedium");
				spriteComponent.LayerSetRotation(PinpointerLayers.Screen, angle);
				return;
			case Distance.Far:
				spriteComponent.LayerSetState(PinpointerLayers.Screen, "pinonfar");
				spriteComponent.LayerSetRotation(PinpointerLayers.Screen, angle);
				return;
			default:
				return;
			}
		}
	}
}
