using System;
using System.Runtime.CompilerServices;
using Content.Shared.Toilet;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Toilet
{
	// Token: 0x020000F4 RID: 244
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class ToiletVisualsSystem : VisualizerSystem<ToiletComponent>
	{
		// Token: 0x060006E3 RID: 1763 RVA: 0x000240D8 File Offset: 0x000222D8
		protected override void OnAppearanceChange(EntityUid uid, ToiletComponent component, ref AppearanceChangeEvent args)
		{
			if (args.Sprite == null)
			{
				return;
			}
			bool flag;
			this.AppearanceSystem.TryGetData<bool>(uid, ToiletVisuals.LidOpen, ref flag, args.Component);
			bool flag2;
			this.AppearanceSystem.TryGetData<bool>(uid, ToiletVisuals.SeatUp, ref flag2, args.Component);
			string text;
			if (!flag)
			{
				if (flag2)
				{
					text = "closed_toilet_seat_up";
				}
				else
				{
					text = "closed_toilet_seat_down";
				}
			}
			else if (flag2)
			{
				text = "open_toilet_seat_up";
			}
			else
			{
				text = "open_toilet_seat_down";
			}
			string text2 = text;
			args.Sprite.LayerSetState(0, text2);
		}
	}
}
