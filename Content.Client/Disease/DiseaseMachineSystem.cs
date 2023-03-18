using System;
using System.Runtime.CompilerServices;
using Content.Shared.Disease;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Disease
{
	// Token: 0x02000356 RID: 854
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class DiseaseMachineSystem : VisualizerSystem<DiseaseMachineVisualsComponent>
	{
		// Token: 0x06001527 RID: 5415 RVA: 0x0007C580 File Offset: 0x0007A780
		protected override void OnAppearanceChange(EntityUid uid, DiseaseMachineVisualsComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			bool flag;
			bool flag2;
			if (this.AppearanceSystem.TryGetData<bool>(uid, DiseaseMachineVisuals.IsOn, ref flag, args.Component) && this.AppearanceSystem.TryGetData<bool>(uid, DiseaseMachineVisuals.IsRunning, ref flag2, args.Component))
			{
				string text = flag2 ? component.RunningState : component.IdleState;
				args.Sprite.LayerSetVisible(DiseaseMachineVisualLayers.IsOn, flag);
				args.Sprite.LayerSetState(DiseaseMachineVisualLayers.IsRunning, text);
			}
		}
	}
}
