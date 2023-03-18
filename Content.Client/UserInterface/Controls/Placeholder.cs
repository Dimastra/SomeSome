using System;
using System.Runtime.CompilerServices;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Controls
{
	// Token: 0x020000E1 RID: 225
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class Placeholder : PanelContainer
	{
		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000654 RID: 1620 RVA: 0x00021D79 File Offset: 0x0001FF79
		// (set) Token: 0x06000655 RID: 1621 RVA: 0x00021D86 File Offset: 0x0001FF86
		public string PlaceholderText
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

		// Token: 0x06000656 RID: 1622 RVA: 0x00021D94 File Offset: 0x0001FF94
		public Placeholder()
		{
			this._label = new Label
			{
				VerticalAlignment = 0,
				Align = 1,
				VAlign = 1
			};
			this._label.AddStyleClass("PlaceholderText");
			base.AddChild(this._label);
		}

		// Token: 0x040002D9 RID: 729
		[Nullable(1)]
		public const string StyleClassPlaceholderText = "PlaceholderText";

		// Token: 0x040002DA RID: 730
		[Nullable(1)]
		private readonly Label _label;
	}
}
