using System;
using System.Runtime.CompilerServices;
using Content.Client.Power;
using Content.Shared.Atmos.Visuals;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Atmos.Visualizers
{
	// Token: 0x02000433 RID: 1075
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class PortableScrubberSystem : VisualizerSystem<PortableScrubberVisualsComponent>
	{
		// Token: 0x06001A38 RID: 6712 RVA: 0x00095E78 File Offset: 0x00094078
		protected override void OnAppearanceChange(EntityUid uid, PortableScrubberVisualsComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			bool flag;
			bool flag2;
			if (this.AppearanceSystem.TryGetData<bool>(uid, PortableScrubberVisuals.IsFull, ref flag, args.Component) && this.AppearanceSystem.TryGetData<bool>(uid, PortableScrubberVisuals.IsRunning, ref flag2, args.Component))
			{
				string text = flag2 ? component.RunningState : component.IdleState;
				args.Sprite.LayerSetState(PortableScrubberVisualLayers.IsRunning, text);
				string text2 = flag ? component.FullState : component.ReadyState;
				args.Sprite.LayerSetState(PowerDeviceVisualLayers.Powered, text2);
			}
			bool flag3;
			if (this.AppearanceSystem.TryGetData<bool>(uid, PortableScrubberVisuals.IsDraining, ref flag3, args.Component))
			{
				args.Sprite.LayerSetVisible(PortableScrubberVisualLayers.IsDraining, flag3);
			}
		}
	}
}
