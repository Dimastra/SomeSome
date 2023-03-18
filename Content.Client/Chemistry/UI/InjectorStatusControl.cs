using System;
using System.Runtime.CompilerServices;
using Content.Client.Chemistry.Components;
using Content.Client.Message;
using Content.Shared.Chemistry.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Content.Client.Chemistry.UI
{
	// Token: 0x020003D9 RID: 985
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class InjectorStatusControl : Control
	{
		// Token: 0x0600183B RID: 6203 RVA: 0x0008BF1A File Offset: 0x0008A11A
		public InjectorStatusControl(InjectorComponent parent)
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

		// Token: 0x0600183C RID: 6204 RVA: 0x0008BF56 File Offset: 0x0008A156
		protected override void FrameUpdate(FrameEventArgs args)
		{
			base.FrameUpdate(args);
			if (!this._parent.UiUpdateNeeded)
			{
				return;
			}
			this.Update();
		}

		// Token: 0x0600183D RID: 6205 RVA: 0x0008BF74 File Offset: 0x0008A174
		public void Update()
		{
			this._parent.UiUpdateNeeded = false;
			SharedInjectorComponent.InjectorToggleMode currentMode = this._parent.CurrentMode;
			string @string;
			if (currentMode != SharedInjectorComponent.InjectorToggleMode.Inject)
			{
				if (currentMode == SharedInjectorComponent.InjectorToggleMode.Draw)
				{
					@string = Loc.GetString("injector-draw-text");
				}
				else
				{
					@string = Loc.GetString("injector-invalid-injector-toggle-mode");
				}
			}
			else
			{
				@string = Loc.GetString("injector-inject-text");
			}
			string item = @string;
			this._label.SetMarkup(Loc.GetString("injector-volume-label", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("currentVolume", this._parent.CurrentVolume),
				new ValueTuple<string, object>("totalVolume", this._parent.TotalVolume),
				new ValueTuple<string, object>("modeString", item)
			}));
		}

		// Token: 0x04000C64 RID: 3172
		private readonly InjectorComponent _parent;

		// Token: 0x04000C65 RID: 3173
		private readonly RichTextLabel _label;
	}
}
