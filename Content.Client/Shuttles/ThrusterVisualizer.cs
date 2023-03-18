using System;
using System.Runtime.CompilerServices;
using Content.Shared.Shuttles.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Shuttles
{
	// Token: 0x02000147 RID: 327
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ThrusterVisualizer : AppearanceVisualizer
	{
		// Token: 0x0600087B RID: 2171 RVA: 0x0003130C File Offset: 0x0002F50C
		[Obsolete("Subscribe to AppearanceChangeEvent instead.")]
		public override void OnChangeData(AppearanceComponent component)
		{
			base.OnChangeData(component);
			SpriteComponent spriteComponent;
			if (!IoCManager.Resolve<IEntityManager>().TryGetComponent<SpriteComponent>(component.Owner, ref spriteComponent))
			{
				return;
			}
			bool flag;
			component.TryGetData<bool>(ThrusterVisualState.State, ref flag);
			if (flag)
			{
				spriteComponent.LayerSetVisible(ThrusterVisualLayers.ThrustOn, true);
				bool flag2;
				if (!component.TryGetData<bool>(ThrusterVisualState.Thrusting, ref flag2) || !flag2)
				{
					this.DisableThrusting(component, spriteComponent);
					return;
				}
				int num;
				if (spriteComponent.LayerMapTryGet(ThrusterVisualLayers.Thrusting, ref num, false))
				{
					spriteComponent.LayerSetVisible(ThrusterVisualLayers.Thrusting, true);
				}
				if (spriteComponent.LayerMapTryGet(ThrusterVisualLayers.ThrustingUnshaded, ref num, false))
				{
					spriteComponent.LayerSetVisible(ThrusterVisualLayers.ThrustingUnshaded, true);
					return;
				}
			}
			else
			{
				spriteComponent.LayerSetVisible(ThrusterVisualLayers.ThrustOn, false);
				this.DisableThrusting(component, spriteComponent);
			}
		}

		// Token: 0x0600087C RID: 2172 RVA: 0x000313C4 File Offset: 0x0002F5C4
		private void DisableThrusting(AppearanceComponent component, SpriteComponent spriteComponent)
		{
			int num;
			if (spriteComponent.LayerMapTryGet(ThrusterVisualLayers.Thrusting, ref num, false))
			{
				spriteComponent.LayerSetVisible(ThrusterVisualLayers.Thrusting, false);
			}
			if (spriteComponent.LayerMapTryGet(ThrusterVisualLayers.ThrustingUnshaded, ref num, false))
			{
				spriteComponent.LayerSetVisible(ThrusterVisualLayers.ThrustingUnshaded, false);
			}
		}
	}
}
