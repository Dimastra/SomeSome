using System;
using System.Runtime.CompilerServices;
using Content.Client.Message;
using Content.Shared.Stacks;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Content.Client.Stack
{
	// Token: 0x02000133 RID: 307
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StackStatusControl : Control
	{
		// Token: 0x06000844 RID: 2116 RVA: 0x000300F4 File Offset: 0x0002E2F4
		public StackStatusControl(StackComponent parent)
		{
			this._parent = parent;
			this._label = new RichTextLabel
			{
				StyleClasses = 
				{
					"ItemStatus"
				}
			};
			this._label.SetMarkup(Loc.GetString("comp-stack-status", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("count", this._parent.Count)
			}));
			base.AddChild(this._label);
		}

		// Token: 0x06000845 RID: 2117 RVA: 0x00030174 File Offset: 0x0002E374
		protected override void FrameUpdate(FrameEventArgs args)
		{
			base.FrameUpdate(args);
			if (!this._parent.UiUpdateNeeded)
			{
				return;
			}
			this._parent.UiUpdateNeeded = false;
			this._label.SetMarkup(Loc.GetString("comp-stack-status", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("count", this._parent.Count)
			}));
		}

		// Token: 0x0400042B RID: 1067
		private readonly StackComponent _parent;

		// Token: 0x0400042C RID: 1068
		private readonly RichTextLabel _label;
	}
}
