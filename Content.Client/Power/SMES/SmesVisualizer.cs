using System;
using System.Runtime.CompilerServices;
using Content.Shared.Power;
using Content.Shared.SMES;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Power.SMES
{
	// Token: 0x020001A3 RID: 419
	public sealed class SmesVisualizer : AppearanceVisualizer
	{
		// Token: 0x06000B03 RID: 2819 RVA: 0x00040244 File Offset: 0x0003E444
		[Obsolete("Subscribe to your component being initialised instead.")]
		public override void InitializeEntity(EntityUid entity)
		{
			base.InitializeEntity(entity);
			SpriteComponent component = IoCManager.Resolve<IEntityManager>().GetComponent<SpriteComponent>(entity);
			component.LayerMapSet(SmesVisualizer.Layers.Input, component.AddLayerState("smes-oc0", null));
			component.LayerSetShader(SmesVisualizer.Layers.Input, "unshaded");
			component.LayerMapSet(SmesVisualizer.Layers.Charge, component.AddLayerState("smes-og1", null));
			component.LayerSetShader(SmesVisualizer.Layers.Charge, "unshaded");
			component.LayerSetVisible(SmesVisualizer.Layers.Charge, false);
			component.LayerMapSet(SmesVisualizer.Layers.Output, component.AddLayerState("smes-op0", null));
			component.LayerSetShader(SmesVisualizer.Layers.Output, "unshaded");
		}

		// Token: 0x06000B04 RID: 2820 RVA: 0x00040304 File Offset: 0x0003E504
		[NullableContext(1)]
		[Obsolete("Subscribe to AppearanceChangeEvent instead.")]
		public override void OnChangeData(AppearanceComponent component)
		{
			base.OnChangeData(component);
			SpriteComponent component2 = IoCManager.Resolve<IEntityManager>().GetComponent<SpriteComponent>(component.Owner);
			int num;
			if (!component.TryGetData<int>(SmesVisuals.LastChargeLevel, ref num) || num == 0)
			{
				component2.LayerSetVisible(SmesVisualizer.Layers.Charge, false);
			}
			else
			{
				component2.LayerSetVisible(SmesVisualizer.Layers.Charge, true);
				SpriteComponent spriteComponent = component2;
				object obj = SmesVisualizer.Layers.Charge;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
				defaultInterpolatedStringHandler.AppendLiteral("smes-og");
				defaultInterpolatedStringHandler.AppendFormatted<int>(num);
				spriteComponent.LayerSetState(obj, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			ChargeState chargeState;
			if (!component.TryGetData<ChargeState>(SmesVisuals.LastChargeState, ref chargeState))
			{
				component2.LayerSetState(SmesVisualizer.Layers.Input, "smes-oc0");
				component2.LayerSetState(SmesVisualizer.Layers.Output, "smes-op1");
				return;
			}
			switch (chargeState)
			{
			case ChargeState.Still:
				component2.LayerSetState(SmesVisualizer.Layers.Input, "smes-oc0");
				component2.LayerSetState(SmesVisualizer.Layers.Output, "smes-op1");
				return;
			case ChargeState.Charging:
				component2.LayerSetState(SmesVisualizer.Layers.Input, "smes-oc1");
				component2.LayerSetState(SmesVisualizer.Layers.Output, "smes-op1");
				return;
			case ChargeState.Discharging:
				component2.LayerSetState(SmesVisualizer.Layers.Input, "smes-oc0");
				component2.LayerSetState(SmesVisualizer.Layers.Output, "smes-op2");
				return;
			default:
				return;
			}
		}

		// Token: 0x020001A4 RID: 420
		private enum Layers : byte
		{
			// Token: 0x04000555 RID: 1365
			Input,
			// Token: 0x04000556 RID: 1366
			Charge,
			// Token: 0x04000557 RID: 1367
			Output
		}
	}
}
