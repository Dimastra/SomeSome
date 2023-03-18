using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Light.Component;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Light
{
	// Token: 0x02000369 RID: 873
	[NullableContext(2)]
	[Nullable(0)]
	public abstract class SharedRgbLightControllerSystem : EntitySystem
	{
		// Token: 0x06000A3E RID: 2622 RVA: 0x000221D2 File Offset: 0x000203D2
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RgbLightControllerComponent, ComponentGetState>(new ComponentEventRefHandler<RgbLightControllerComponent, ComponentGetState>(this.OnGetState), null, null);
		}

		// Token: 0x06000A3F RID: 2623 RVA: 0x000221EE File Offset: 0x000203EE
		[NullableContext(1)]
		private void OnGetState(EntityUid uid, RgbLightControllerComponent component, ref ComponentGetState args)
		{
			args.State = new RgbLightControllerState(component.CycleRate, component.Layers);
		}

		// Token: 0x06000A40 RID: 2624 RVA: 0x00022207 File Offset: 0x00020407
		public void SetLayers(EntityUid uid, List<int> layers, RgbLightControllerComponent rgb = null)
		{
			if (!base.Resolve<RgbLightControllerComponent>(uid, ref rgb, true))
			{
				return;
			}
			rgb.Layers = layers;
			base.Dirty(rgb, null);
		}

		// Token: 0x06000A41 RID: 2625 RVA: 0x00022225 File Offset: 0x00020425
		public void SetCycleRate(EntityUid uid, float rate, RgbLightControllerComponent rgb = null)
		{
			if (!base.Resolve<RgbLightControllerComponent>(uid, ref rgb, true))
			{
				return;
			}
			rgb.CycleRate = Math.Clamp(0.01f, rate, 1f);
			base.Dirty(rgb, null);
		}
	}
}
