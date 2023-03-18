using System;
using System.Runtime.CompilerServices;
using Content.Client.Message;
using Content.Shared.Radiation.Components;
using Content.Shared.Radiation.Systems;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Radiation.UI
{
	// Token: 0x02000178 RID: 376
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GeigerItemControl : Control
	{
		// Token: 0x060009C8 RID: 2504 RVA: 0x00038DB0 File Offset: 0x00036FB0
		public GeigerItemControl(GeigerComponent component)
		{
			this._component = component;
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

		// Token: 0x060009C9 RID: 2505 RVA: 0x00038DEC File Offset: 0x00036FEC
		protected override void FrameUpdate(FrameEventArgs args)
		{
			base.FrameUpdate(args);
			if (!this._component.UiUpdateNeeded)
			{
				return;
			}
			this.Update();
		}

		// Token: 0x060009CA RID: 2506 RVA: 0x00038E0C File Offset: 0x0003700C
		private void Update()
		{
			string @string;
			if (this._component.IsEnabled)
			{
				Color color = SharedGeigerSystem.LevelToColor(this._component.DangerLevel);
				float currentRadiation = this._component.CurrentRadiation;
				string item = currentRadiation.ToString("N1");
				@string = Loc.GetString("geiger-item-control-status", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("rads", item),
					new ValueTuple<string, object>("color", color)
				});
			}
			else
			{
				@string = Loc.GetString("geiger-item-control-disabled");
			}
			this._label.SetMarkup(@string);
			this._component.UiUpdateNeeded = false;
		}

		// Token: 0x040004DC RID: 1244
		private readonly GeigerComponent _component;

		// Token: 0x040004DD RID: 1245
		private readonly RichTextLabel _label;
	}
}
