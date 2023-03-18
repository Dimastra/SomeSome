using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos.Monitor;
using Content.Shared.Power;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client.Atmos.Monitor
{
	// Token: 0x02000448 RID: 1096
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class AtmosAlarmableVisualsSystem : VisualizerSystem<AtmosAlarmableVisualsComponent>
	{
		// Token: 0x06001B09 RID: 6921 RVA: 0x0009BF18 File Offset: 0x0009A118
		protected override void OnAppearanceChange(EntityUid uid, AtmosAlarmableVisualsComponent component, ref AppearanceChangeEvent args)
		{
			int num;
			if (args.Sprite == null || !args.Sprite.LayerMapTryGet(component.LayerMap, ref num, false))
			{
				return;
			}
			object obj;
			if (args.AppearanceData.TryGetValue(PowerDeviceVisuals.Powered, out obj) && obj is bool)
			{
				bool flag = (bool)obj;
				if (component.HideOnDepowered != null)
				{
					foreach (string text in component.HideOnDepowered)
					{
						int num2;
						if (args.Sprite.LayerMapTryGet(text, ref num2, false))
						{
							args.Sprite.LayerSetVisible(num2, flag);
						}
					}
				}
				if (component.SetOnDepowered != null && !flag)
				{
					foreach (KeyValuePair<string, string> keyValuePair in component.SetOnDepowered)
					{
						string text2;
						string text3;
						keyValuePair.Deconstruct(out text2, out text3);
						string text4 = text2;
						string text5 = text3;
						int num3;
						if (args.Sprite.LayerMapTryGet(text4, ref num3, false))
						{
							args.Sprite.LayerSetState(num3, new RSI.StateId(text5));
						}
					}
				}
				object obj2;
				AtmosAlarmType key;
				bool flag2;
				if (args.AppearanceData.TryGetValue(AtmosMonitorVisuals.AlarmType, out obj2))
				{
					if (obj2 is AtmosAlarmType)
					{
						key = (AtmosAlarmType)obj2;
						flag2 = true;
					}
					else
					{
						flag2 = false;
					}
				}
				else
				{
					flag2 = false;
				}
				string text6;
				if (flag2 && flag && component.AlarmStates.TryGetValue(key, out text6))
				{
					args.Sprite.LayerSetState(num, new RSI.StateId(text6));
				}
				return;
			}
		}
	}
}
