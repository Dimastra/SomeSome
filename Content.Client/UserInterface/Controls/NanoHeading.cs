using System;
using System.Runtime.CompilerServices;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Controls
{
	// Token: 0x020000E0 RID: 224
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class NanoHeading : Container
	{
		// Token: 0x06000651 RID: 1617 RVA: 0x00021D04 File Offset: 0x0001FF04
		public NanoHeading()
		{
			PanelContainer panelContainer = new PanelContainer();
			Control.OrderedChildCollection children = panelContainer.Children;
			Label label = new Label();
			label.StyleClasses.Add("LabelHeading");
			Label label2 = label;
			this._label = label;
			children.Add(label2);
			this._panel = panelContainer;
			base.AddChild(this._panel);
			base.HorizontalAlignment = 1;
		}

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000652 RID: 1618 RVA: 0x00021D5E File Offset: 0x0001FF5E
		// (set) Token: 0x06000653 RID: 1619 RVA: 0x00021D6B File Offset: 0x0001FF6B
		public string Text
		{
			get
			{
				return this._label.Text;
			}
			set
			{
				this._label.Text = value;
			}
		}

		// Token: 0x040002D7 RID: 727
		[Nullable(1)]
		private readonly Label _label;

		// Token: 0x040002D8 RID: 728
		[Nullable(1)]
		private readonly PanelContainer _panel;
	}
}
