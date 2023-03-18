using System;
using System.Runtime.CompilerServices;
using Content.Shared.Examine;
using Content.Shared.Radiation.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Shared.Radiation.Systems
{
	// Token: 0x02000228 RID: 552
	public abstract class SharedGeigerSystem : EntitySystem
	{
		// Token: 0x06000626 RID: 1574 RVA: 0x00015B83 File Offset: 0x00013D83
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<GeigerComponent, ExaminedEvent>(new ComponentEventHandler<GeigerComponent, ExaminedEvent>(this.OnExamine), null, null);
		}

		// Token: 0x06000627 RID: 1575 RVA: 0x00015BA0 File Offset: 0x00013DA0
		[NullableContext(1)]
		private void OnExamine(EntityUid uid, GeigerComponent component, ExaminedEvent args)
		{
			if (!component.ShowExamine || !component.IsEnabled || !args.IsInDetailsRange)
			{
				return;
			}
			float currentRads = component.CurrentRadiation;
			string rads = currentRads.ToString("N1");
			Color color = SharedGeigerSystem.LevelToColor(component.DangerLevel);
			string msg = Loc.GetString("geiger-component-examine", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("rads", rads),
				new ValueTuple<string, object>("color", color)
			});
			args.PushMarkup(msg);
		}

		// Token: 0x06000628 RID: 1576 RVA: 0x00015C27 File Offset: 0x00013E27
		public static Color LevelToColor(GeigerDangerLevel level)
		{
			switch (level)
			{
			case GeigerDangerLevel.None:
				return Color.Green;
			case GeigerDangerLevel.Low:
				return Color.Yellow;
			case GeigerDangerLevel.Med:
				return Color.DarkOrange;
			case GeigerDangerLevel.High:
			case GeigerDangerLevel.Extreme:
				return Color.Red;
			default:
				return Color.White;
			}
		}
	}
}
