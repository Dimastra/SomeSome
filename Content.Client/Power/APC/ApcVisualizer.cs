using System;
using System.Runtime.CompilerServices;
using Content.Shared.APC;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Power.APC
{
	// Token: 0x020001A6 RID: 422
	public sealed class ApcVisualizer : AppearanceVisualizer
	{
		// Token: 0x06000B0B RID: 2827 RVA: 0x000404F4 File Offset: 0x0003E6F4
		[Obsolete("Subscribe to your component being initialised instead.")]
		public override void InitializeEntity(EntityUid entity)
		{
			base.InitializeEntity(entity);
			SpriteComponent component = IoCManager.Resolve<IEntityManager>().GetComponent<SpriteComponent>(entity);
			component.LayerMapSet(ApcVisualizer.Layers.Panel, component.AddLayerState("apc0", null));
			component.LayerMapSet(ApcVisualizer.Layers.ChargeState, component.AddLayerState("apco3-0", null));
			component.LayerSetShader(ApcVisualizer.Layers.ChargeState, "unshaded");
			component.LayerMapSet(ApcVisualizer.Layers.Lock, component.AddLayerState("apcox-0", null));
			component.LayerSetShader(ApcVisualizer.Layers.Lock, "unshaded");
			component.LayerMapSet(ApcVisualizer.Layers.Equipment, component.AddLayerState("apco0-3", null));
			component.LayerSetShader(ApcVisualizer.Layers.Equipment, "unshaded");
			component.LayerMapSet(ApcVisualizer.Layers.Lighting, component.AddLayerState("apco1-3", null));
			component.LayerSetShader(ApcVisualizer.Layers.Lighting, "unshaded");
			component.LayerMapSet(ApcVisualizer.Layers.Environment, component.AddLayerState("apco2-3", null));
			component.LayerSetShader(ApcVisualizer.Layers.Environment, "unshaded");
		}

		// Token: 0x06000B0C RID: 2828 RVA: 0x0004062C File Offset: 0x0003E82C
		[NullableContext(1)]
		[Obsolete("Subscribe to AppearanceChangeEvent instead.")]
		public override void OnChangeData(AppearanceComponent component)
		{
			base.OnChangeData(component);
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			SpriteComponent component2 = entityManager.GetComponent<SpriteComponent>(component.Owner);
			ApcPanelState apcPanelState;
			if (component.TryGetData<ApcPanelState>(ApcVisuals.PanelState, ref apcPanelState))
			{
				if (apcPanelState != ApcPanelState.Closed)
				{
					if (apcPanelState == ApcPanelState.Open)
					{
						component2.LayerSetState(ApcVisualizer.Layers.Panel, "apcframe");
					}
				}
				else
				{
					component2.LayerSetState(ApcVisualizer.Layers.Panel, "apc0");
				}
			}
			ApcChargeState apcChargeState;
			if (component.TryGetData<ApcChargeState>(ApcVisuals.ChargeState, ref apcChargeState))
			{
				switch (apcChargeState)
				{
				case ApcChargeState.Lack:
					component2.LayerSetState(ApcVisualizer.Layers.ChargeState, "apco3-0");
					break;
				case ApcChargeState.Charging:
					component2.LayerSetState(ApcVisualizer.Layers.ChargeState, "apco3-1");
					break;
				case ApcChargeState.Full:
					component2.LayerSetState(ApcVisualizer.Layers.ChargeState, "apco3-2");
					break;
				case ApcChargeState.Emag:
					component2.LayerSetState(ApcVisualizer.Layers.ChargeState, "emag-unlit");
					break;
				}
				SharedPointLightComponent sharedPointLightComponent;
				if (entityManager.TryGetComponent<SharedPointLightComponent>(component.Owner, ref sharedPointLightComponent))
				{
					SharedPointLightComponent sharedPointLightComponent2 = sharedPointLightComponent;
					Color color;
					switch (apcChargeState)
					{
					case ApcChargeState.Lack:
						color = ApcVisualizer.LackColor;
						break;
					case ApcChargeState.Charging:
						color = ApcVisualizer.ChargingColor;
						break;
					case ApcChargeState.Full:
						color = ApcVisualizer.FullColor;
						break;
					case ApcChargeState.Emag:
						color = ApcVisualizer.EmagColor;
						break;
					default:
						color = ApcVisualizer.LackColor;
						break;
					}
					sharedPointLightComponent2.Color = color;
					return;
				}
			}
			else
			{
				component2.LayerSetState(ApcVisualizer.Layers.ChargeState, "apco3-0");
			}
		}

		// Token: 0x04000559 RID: 1369
		public static readonly Color LackColor = Color.FromHex("#d1332e", null);

		// Token: 0x0400055A RID: 1370
		public static readonly Color ChargingColor = Color.FromHex("#2e8ad1", null);

		// Token: 0x0400055B RID: 1371
		public static readonly Color FullColor = Color.FromHex("#3db83b", null);

		// Token: 0x0400055C RID: 1372
		public static readonly Color EmagColor = Color.FromHex("#1f48d6", null);

		// Token: 0x020001A7 RID: 423
		private enum Layers : byte
		{
			// Token: 0x0400055E RID: 1374
			ChargeState,
			// Token: 0x0400055F RID: 1375
			Lock,
			// Token: 0x04000560 RID: 1376
			Equipment,
			// Token: 0x04000561 RID: 1377
			Lighting,
			// Token: 0x04000562 RID: 1378
			Environment,
			// Token: 0x04000563 RID: 1379
			Panel
		}
	}
}
