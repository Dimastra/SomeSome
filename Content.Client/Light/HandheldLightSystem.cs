using System;
using System.Runtime.CompilerServices;
using Content.Client.Items;
using Content.Client.Light.Components;
using Content.Shared.Light;
using Content.Shared.Toggleable;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Light
{
	// Token: 0x02000262 RID: 610
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HandheldLightSystem : SharedHandheldLightSystem
	{
		// Token: 0x06000FA1 RID: 4001 RVA: 0x0005E247 File Offset: 0x0005C447
		public override void Initialize()
		{
			base.Initialize();
			ComponentEventHandler<HandheldLightComponent, ItemStatusCollectMessage> componentEventHandler;
			if ((componentEventHandler = HandheldLightSystem.<>O.<0>__OnGetStatusControl) == null)
			{
				componentEventHandler = (HandheldLightSystem.<>O.<0>__OnGetStatusControl = new ComponentEventHandler<HandheldLightComponent, ItemStatusCollectMessage>(HandheldLightSystem.OnGetStatusControl));
			}
			base.SubscribeLocalEvent<HandheldLightComponent, ItemStatusCollectMessage>(componentEventHandler, null, null);
			base.SubscribeLocalEvent<HandheldLightComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<HandheldLightComponent, AppearanceChangeEvent>(this.OnAppearanceChange), null, null);
		}

		// Token: 0x06000FA2 RID: 4002 RVA: 0x0005E286 File Offset: 0x0005C486
		private static void OnGetStatusControl(EntityUid uid, HandheldLightComponent component, ItemStatusCollectMessage args)
		{
			args.Controls.Add(new HandheldLightStatus(component));
		}

		// Token: 0x06000FA3 RID: 4003 RVA: 0x0005E29C File Offset: 0x0005C49C
		[NullableContext(2)]
		private void OnAppearanceChange(EntityUid uid, HandheldLightComponent component, ref AppearanceChangeEvent args)
		{
			if (!base.Resolve<HandheldLightComponent>(uid, ref component, true))
			{
				return;
			}
			bool flag;
			if (!this._appearance.TryGetData<bool>(uid, ToggleableLightVisuals.Enabled, ref flag, args.Component))
			{
				return;
			}
			HandheldLightPowerStates handheldLightPowerStates;
			if (!this._appearance.TryGetData<HandheldLightPowerStates>(uid, HandheldLightVisuals.Power, ref handheldLightPowerStates, args.Component))
			{
				return;
			}
			LightBehaviourComponent lightBehaviourComponent;
			if (base.TryComp<LightBehaviourComponent>(uid, ref lightBehaviourComponent))
			{
				if (lightBehaviourComponent.HasRunningBehaviours())
				{
					lightBehaviourComponent.StopLightBehaviour("", false, true);
				}
				if (!flag)
				{
					return;
				}
				switch (handheldLightPowerStates)
				{
				case HandheldLightPowerStates.FullPower:
					break;
				case HandheldLightPowerStates.LowPower:
					lightBehaviourComponent.StartLightBehaviour(component.RadiatingBehaviourId);
					return;
				case HandheldLightPowerStates.Dying:
					lightBehaviourComponent.StartLightBehaviour(component.BlinkingBehaviourId);
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x040007B7 RID: 1975
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x02000263 RID: 611
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040007B8 RID: 1976
			[Nullable(new byte[]
			{
				0,
				1,
				1
			})]
			public static ComponentEventHandler<HandheldLightComponent, ItemStatusCollectMessage> <0>__OnGetStatusControl;
		}
	}
}
