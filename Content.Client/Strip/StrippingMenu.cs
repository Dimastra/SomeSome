using System;
using System.Runtime.CompilerServices;
using Content.Client.Inventory;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.Strip
{
	// Token: 0x02000118 RID: 280
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class StrippingMenu : DefaultWindow
	{
		// Token: 0x060007BE RID: 1982 RVA: 0x0002CF54 File Offset: 0x0002B154
		public StrippingMenu(string title, StrippableBoundUserInterface bui)
		{
			base.Title = title;
			this._bui = bui;
			BoxContainer boxContainer = new BoxContainer
			{
				Orientation = 1,
				Margin = new Thickness(0f, 8f)
			};
			base.Contents.AddChild(boxContainer);
			boxContainer.AddChild(this.SnareContainer);
			boxContainer.AddChild(this.HandsContainer);
			boxContainer.AddChild(this.InventoryContainer);
		}

		// Token: 0x060007BF RID: 1983 RVA: 0x0002CFF6 File Offset: 0x0002B1F6
		public void ClearButtons()
		{
			this.InventoryContainer.DisposeAllChildren();
			this.HandsContainer.DisposeAllChildren();
			this.SnareContainer.DisposeAllChildren();
		}

		// Token: 0x060007C0 RID: 1984 RVA: 0x0002D019 File Offset: 0x0002B219
		protected override void FrameUpdate(FrameEventArgs args)
		{
			if (!this.Dirty)
			{
				return;
			}
			this.Dirty = false;
			this._bui.UpdateMenu();
		}

		// Token: 0x040003ED RID: 1005
		public LayoutContainer InventoryContainer = new LayoutContainer();

		// Token: 0x040003EE RID: 1006
		public BoxContainer HandsContainer = new BoxContainer
		{
			Orientation = 0
		};

		// Token: 0x040003EF RID: 1007
		public BoxContainer SnareContainer = new BoxContainer();

		// Token: 0x040003F0 RID: 1008
		private StrippableBoundUserInterface _bui;

		// Token: 0x040003F1 RID: 1009
		public bool Dirty = true;
	}
}
