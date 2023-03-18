using System;
using System.Runtime.CompilerServices;
using Content.Client.Message;
using Content.Shared.Implants.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Content.Client.Implants.UI
{
	// Token: 0x020002C5 RID: 709
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ImplanterStatusControl : Control
	{
		// Token: 0x060011D5 RID: 4565 RVA: 0x0006996F File Offset: 0x00067B6F
		public ImplanterStatusControl(ImplanterComponent parent)
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

		// Token: 0x060011D6 RID: 4566 RVA: 0x000699AB File Offset: 0x00067BAB
		protected override void FrameUpdate(FrameEventArgs args)
		{
			base.FrameUpdate(args);
			if (!this._parent.UiUpdateNeeded)
			{
				return;
			}
			this.Update();
		}

		// Token: 0x060011D7 RID: 4567 RVA: 0x000699C8 File Offset: 0x00067BC8
		private void Update()
		{
			this._parent.UiUpdateNeeded = false;
			ImplanterToggleMode currentMode = this._parent.CurrentMode;
			string @string;
			if (currentMode != ImplanterToggleMode.Inject)
			{
				if (currentMode == ImplanterToggleMode.Draw)
				{
					@string = Loc.GetString("implanter-draw-text");
				}
				else
				{
					@string = Loc.GetString("injector-invalid-injector-toggle-mode");
				}
			}
			else
			{
				@string = Loc.GetString("implanter-inject-text");
			}
			string item = @string;
			if (!this._parent.ImplanterSlot.HasItem)
			{
				@string = Loc.GetString("implanter-empty-text");
			}
			else
			{
				@string = Loc.GetString("implanter-implant-text", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("implantName", this._parent.ImplantData.Item1),
					new ValueTuple<string, object>("implantDescription", this._parent.ImplantData.Item2),
					new ValueTuple<string, object>("lineBreak", "\n")
				});
			}
			string item2 = @string;
			this._label.SetMarkup(Loc.GetString("implanter-label", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("currentEntities", item2),
				new ValueTuple<string, object>("modeString", item),
				new ValueTuple<string, object>("lineBreak", "\n")
			}));
		}

		// Token: 0x040008C0 RID: 2240
		private readonly ImplanterComponent _parent;

		// Token: 0x040008C1 RID: 2241
		private readonly RichTextLabel _label;
	}
}
