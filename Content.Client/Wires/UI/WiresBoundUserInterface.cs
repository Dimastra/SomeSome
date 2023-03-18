using System;
using System.Runtime.CompilerServices;
using Content.Shared.Wires;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Wires.UI
{
	// Token: 0x02000015 RID: 21
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class WiresBoundUserInterface : BoundUserInterface
	{
		// Token: 0x0600003E RID: 62 RVA: 0x000021BC File Offset: 0x000003BC
		public WiresBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000033AF File Offset: 0x000015AF
		protected override void Open()
		{
			base.Open();
			this._menu = new WiresMenu(this);
			this._menu.OnClose += base.Close;
			this._menu.OpenCenteredLeft();
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000033E5 File Offset: 0x000015E5
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			WiresMenu menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.Populate((WiresBoundUserInterfaceState)state);
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00003404 File Offset: 0x00001604
		public void PerformAction(int id, WiresAction action)
		{
			base.SendMessage(new WiresActionMessage(id, action));
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00003413 File Offset: 0x00001613
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			WiresMenu menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.Dispose();
		}

		// Token: 0x04000019 RID: 25
		[Nullable(2)]
		private WiresMenu _menu;
	}
}
