using System;
using System.Runtime.CompilerServices;
using Content.Shared.Power;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.PowerCell
{
	// Token: 0x02000196 RID: 406
	public sealed class PowerChargerVisualizer : AppearanceVisualizer
	{
		// Token: 0x06000AD5 RID: 2773 RVA: 0x0003EE94 File Offset: 0x0003D094
		[Obsolete("Subscribe to your component being initialised instead.")]
		public override void InitializeEntity(EntityUid entity)
		{
			base.InitializeEntity(entity);
			SpriteComponent component = IoCManager.Resolve<IEntityManager>().GetComponent<SpriteComponent>(entity);
			component.LayerMapSet(PowerChargerVisualizer.Layers.Base, component.AddLayerState("empty", null));
			component.LayerMapSet(PowerChargerVisualizer.Layers.Light, component.AddLayerState("light-off", null));
			component.LayerSetShader(PowerChargerVisualizer.Layers.Light, "unshaded");
		}

		// Token: 0x06000AD6 RID: 2774 RVA: 0x0003EF08 File Offset: 0x0003D108
		[NullableContext(1)]
		[Obsolete("Subscribe to AppearanceChangeEvent instead.")]
		public override void OnChangeData(AppearanceComponent component)
		{
			base.OnChangeData(component);
			SpriteComponent component2 = IoCManager.Resolve<IEntityManager>().GetComponent<SpriteComponent>(component.Owner);
			bool flag;
			if (component.TryGetData<bool>(CellVisual.Occupied, ref flag))
			{
				component2.LayerSetState(PowerChargerVisualizer.Layers.Base, flag ? "full" : "empty");
			}
			else
			{
				component2.LayerSetState(PowerChargerVisualizer.Layers.Base, "empty");
			}
			CellChargerStatus cellChargerStatus;
			if (!component.TryGetData<CellChargerStatus>(CellVisual.Light, ref cellChargerStatus))
			{
				component2.LayerSetState(PowerChargerVisualizer.Layers.Light, "light-off");
				return;
			}
			switch (cellChargerStatus)
			{
			case CellChargerStatus.Off:
				component2.LayerSetState(PowerChargerVisualizer.Layers.Light, "light-off");
				return;
			case CellChargerStatus.Empty:
				component2.LayerSetState(PowerChargerVisualizer.Layers.Light, "light-empty");
				return;
			case CellChargerStatus.Charging:
				component2.LayerSetState(PowerChargerVisualizer.Layers.Light, "light-charging");
				return;
			case CellChargerStatus.Charged:
				component2.LayerSetState(PowerChargerVisualizer.Layers.Light, "light-charged");
				return;
			default:
				component2.LayerSetState(PowerChargerVisualizer.Layers.Light, "light-off");
				return;
			}
		}

		// Token: 0x02000197 RID: 407
		private enum Layers : byte
		{
			// Token: 0x04000544 RID: 1348
			Base,
			// Token: 0x04000545 RID: 1349
			Light
		}
	}
}
