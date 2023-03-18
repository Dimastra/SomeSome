using System;
using System.Runtime.CompilerServices;
using Content.Shared.Wires;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Wires.Visualizers
{
	// Token: 0x02000013 RID: 19
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class WiresVisualizerSystem : VisualizerSystem<WiresVisualsComponent>
	{
		// Token: 0x0600003C RID: 60 RVA: 0x00003340 File Offset: 0x00001540
		protected override void OnAppearanceChange(EntityUid uid, WiresVisualsComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			int num = args.Sprite.LayerMapReserveBlank(WiresVisualLayers.MaintenancePanel);
			object obj;
			if (args.AppearanceData.TryGetValue(WiresVisuals.MaintenancePanelState, out obj) && obj is bool)
			{
				bool flag = (bool)obj;
				args.Sprite.LayerSetVisible(num, flag);
				return;
			}
			args.Sprite.LayerSetVisible(num, false);
		}
	}
}
