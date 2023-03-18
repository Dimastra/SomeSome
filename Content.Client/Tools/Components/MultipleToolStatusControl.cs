using System;
using System.Runtime.CompilerServices;
using Content.Client.Message;
using Content.Shared.Tools.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Timing;

namespace Content.Client.Tools.Components
{
	// Token: 0x020000F1 RID: 241
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MultipleToolStatusControl : Control
	{
		// Token: 0x060006D8 RID: 1752 RVA: 0x00023FD4 File Offset: 0x000221D4
		public MultipleToolStatusControl(MultipleToolComponent parent)
		{
			this._parent = parent;
			this._label = new RichTextLabel
			{
				StyleClasses = 
				{
					"ItemStatus"
				}
			};
			this._label.SetMarkup(this._parent.StatusShowBehavior ? this._parent.CurrentQualityName : string.Empty);
			base.AddChild(this._label);
		}

		// Token: 0x060006D9 RID: 1753 RVA: 0x0002403F File Offset: 0x0002223F
		protected override void FrameUpdate(FrameEventArgs args)
		{
			base.FrameUpdate(args);
			if (this._parent.UiUpdateNeeded)
			{
				this._parent.UiUpdateNeeded = false;
				this.Update();
			}
		}

		// Token: 0x060006DA RID: 1754 RVA: 0x00024067 File Offset: 0x00022267
		public void Update()
		{
			this._label.SetMarkup(this._parent.StatusShowBehavior ? this._parent.CurrentQualityName : string.Empty);
		}

		// Token: 0x04000322 RID: 802
		private readonly MultipleToolComponent _parent;

		// Token: 0x04000323 RID: 803
		private readonly RichTextLabel _label;
	}
}
