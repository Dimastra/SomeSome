using System;
using System.Runtime.CompilerServices;
using Content.Client.Message;
using Content.Client.Tools.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Content.Client.Tools.UI
{
	// Token: 0x020000F0 RID: 240
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class WelderStatusControl : Control
	{
		// Token: 0x060006D5 RID: 1749 RVA: 0x00023E9C File Offset: 0x0002209C
		public WelderStatusControl(WelderComponent parent)
		{
			this._parent = parent;
			this._label = new RichTextLabel
			{
				StyleClasses = 
				{
					"ItemStatus"
				}
			};
			base.AddChild(this._label);
			base.UpdateDraw();
		}

		// Token: 0x060006D6 RID: 1750 RVA: 0x00023ED8 File Offset: 0x000220D8
		protected override void FrameUpdate(FrameEventArgs args)
		{
			base.FrameUpdate(args);
			if (!this._parent.UiUpdateNeeded)
			{
				return;
			}
			this.Update();
		}

		// Token: 0x060006D7 RID: 1751 RVA: 0x00023EF8 File Offset: 0x000220F8
		public void Update()
		{
			this._parent.UiUpdateNeeded = false;
			float fuelCapacity = this._parent.FuelCapacity;
			float fuel = this._parent.Fuel;
			bool lit = this._parent.Lit;
			this._label.SetMarkup(Loc.GetString("welder-component-on-examine-detailed-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("colorName", (fuel < fuelCapacity / 4f) ? "darkorange" : "orange"),
				new ValueTuple<string, object>("fuelLeft", Math.Round((double)fuel, 1)),
				new ValueTuple<string, object>("fuelCapacity", fuelCapacity),
				new ValueTuple<string, object>("status", Loc.GetString(lit ? "welder-component-on-examine-welder-lit-message" : "welder-component-on-examine-welder-not-lit-message"))
			}));
		}

		// Token: 0x04000320 RID: 800
		private readonly WelderComponent _parent;

		// Token: 0x04000321 RID: 801
		private readonly RichTextLabel _label;
	}
}
