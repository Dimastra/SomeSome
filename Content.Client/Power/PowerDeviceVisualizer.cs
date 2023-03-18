using System;
using System.Runtime.CompilerServices;
using Content.Shared.Power;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Power
{
	// Token: 0x0200019F RID: 415
	public sealed class PowerDeviceVisualizer : AppearanceVisualizer
	{
		// Token: 0x06000AFD RID: 2813 RVA: 0x00040158 File Offset: 0x0003E358
		[NullableContext(1)]
		[Obsolete("Subscribe to AppearanceChangeEvent instead.")]
		public override void OnChangeData(AppearanceComponent component)
		{
			base.OnChangeData(component);
			SpriteComponent component2 = IoCManager.Resolve<IEntityManager>().GetComponent<SpriteComponent>(component.Owner);
			bool flag2;
			bool flag = component.TryGetData<bool>(PowerDeviceVisuals.Powered, ref flag2) && flag2;
			component2.LayerSetVisible(PowerDeviceVisualLayers.Powered, flag);
		}
	}
}
