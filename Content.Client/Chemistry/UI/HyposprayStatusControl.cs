using System;
using System.Runtime.CompilerServices;
using Content.Client.Chemistry.Components;
using Content.Client.Message;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Content.Client.Chemistry.UI
{
	// Token: 0x020003D8 RID: 984
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HyposprayStatusControl : Control
	{
		// Token: 0x06001838 RID: 6200 RVA: 0x0008BE49 File Offset: 0x0008A049
		public HyposprayStatusControl(HyposprayComponent parent)
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
			this.Update();
		}

		// Token: 0x06001839 RID: 6201 RVA: 0x0008BE85 File Offset: 0x0008A085
		protected override void FrameUpdate(FrameEventArgs args)
		{
			base.FrameUpdate(args);
			if (!this._parent.UiUpdateNeeded)
			{
				return;
			}
			this.Update();
		}

		// Token: 0x0600183A RID: 6202 RVA: 0x0008BEA4 File Offset: 0x0008A0A4
		public void Update()
		{
			this._parent.UiUpdateNeeded = false;
			this._label.SetMarkup(Loc.GetString("hypospray-volume-text", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("currentVolume", this._parent.CurrentVolume),
				new ValueTuple<string, object>("totalVolume", this._parent.TotalVolume)
			}));
		}

		// Token: 0x04000C62 RID: 3170
		private readonly HyposprayComponent _parent;

		// Token: 0x04000C63 RID: 3171
		private readonly RichTextLabel _label;
	}
}
