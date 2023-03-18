using System;
using System.Runtime.CompilerServices;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Nuke;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;

namespace Content.Client.Nuke
{
	// Token: 0x02000206 RID: 518
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NukeBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06000D8D RID: 3469 RVA: 0x000021BC File Offset: 0x000003BC
		public NukeBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06000D8E RID: 3470 RVA: 0x00051DF0 File Offset: 0x0004FFF0
		protected override void Open()
		{
			this._menu = new NukeMenu();
			this._menu.OpenCentered();
			this._menu.OnClose += base.Close;
			this._menu.OnKeypadButtonPressed += delegate(int i)
			{
				base.SendMessage(new NukeKeypadMessage(i));
			};
			this._menu.OnEnterButtonPressed += delegate()
			{
				base.SendMessage(new NukeKeypadEnterMessage());
			};
			this._menu.OnClearButtonPressed += delegate()
			{
				base.SendMessage(new NukeKeypadClearMessage());
			};
			this._menu.EjectButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ItemSlotButtonPressedEvent("Nuke", true, true));
			};
			this._menu.AnchorButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new NukeAnchorMessage());
			};
			this._menu.ArmButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new NukeArmedMessage());
			};
		}

		// Token: 0x06000D8F RID: 3471 RVA: 0x00051EC4 File Offset: 0x000500C4
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			if (this._menu == null)
			{
				return;
			}
			NukeUiState nukeUiState = state as NukeUiState;
			if (nukeUiState != null)
			{
				this._menu.UpdateState(nukeUiState);
			}
		}

		// Token: 0x06000D90 RID: 3472 RVA: 0x00051EF7 File Offset: 0x000500F7
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			NukeMenu menu = this._menu;
			if (menu != null)
			{
				menu.Close();
			}
			NukeMenu menu2 = this._menu;
			if (menu2 == null)
			{
				return;
			}
			menu2.Dispose();
		}

		// Token: 0x040006B9 RID: 1721
		[Nullable(2)]
		private NukeMenu _menu;
	}
}
