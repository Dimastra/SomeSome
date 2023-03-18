using System;
using System.Runtime.CompilerServices;
using Content.Client.Power.APC.UI;
using Content.Shared.APC;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Power.APC
{
	// Token: 0x020001A5 RID: 421
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ApcBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06000B06 RID: 2822 RVA: 0x00040466 File Offset: 0x0003E666
		protected override void Open()
		{
			base.Open();
			this._menu = new ApcMenu(this);
			this._menu.OnClose += base.Close;
			this._menu.OpenCentered();
		}

		// Token: 0x06000B07 RID: 2823 RVA: 0x000021BC File Offset: 0x000003BC
		public ApcBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06000B08 RID: 2824 RVA: 0x0004049C File Offset: 0x0003E69C
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			ApcBoundInterfaceState state2 = (ApcBoundInterfaceState)state;
			ApcMenu menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.UpdateState(state2);
		}

		// Token: 0x06000B09 RID: 2825 RVA: 0x000404C8 File Offset: 0x0003E6C8
		public void BreakerPressed()
		{
			base.SendMessage(new ApcToggleMainBreakerMessage());
		}

		// Token: 0x06000B0A RID: 2826 RVA: 0x000404D5 File Offset: 0x0003E6D5
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				ApcMenu menu = this._menu;
				if (menu == null)
				{
					return;
				}
				menu.Dispose();
			}
		}

		// Token: 0x04000558 RID: 1368
		[Nullable(2)]
		[ViewVariables]
		private ApcMenu _menu;
	}
}
