using System;
using System.Runtime.CompilerServices;
using Content.Shared.Research;
using Content.Shared.Research.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Research.UI
{
	// Token: 0x0200016D RID: 365
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DiskConsoleBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06000976 RID: 2422 RVA: 0x000021BC File Offset: 0x000003BC
		public DiskConsoleBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x06000977 RID: 2423 RVA: 0x00037290 File Offset: 0x00035490
		protected override void Open()
		{
			base.Open();
			this._menu = new DiskConsoleMenu();
			this._menu.OnClose += base.Close;
			this._menu.OpenCentered();
			this._menu.OnServerButtonPressed += delegate()
			{
				base.SendMessage(new ConsoleServerSelectionMessage());
			};
			this._menu.OnPrintButtonPressed += delegate()
			{
				base.SendMessage(new DiskConsolePrintDiskMessage());
			};
		}

		// Token: 0x06000978 RID: 2424 RVA: 0x000372FE File Offset: 0x000354FE
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			DiskConsoleMenu menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.Close();
		}

		// Token: 0x06000979 RID: 2425 RVA: 0x0003731C File Offset: 0x0003551C
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			DiskConsoleBoundUserInterfaceState diskConsoleBoundUserInterfaceState = state as DiskConsoleBoundUserInterfaceState;
			if (diskConsoleBoundUserInterfaceState == null)
			{
				return;
			}
			DiskConsoleMenu menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.Update(diskConsoleBoundUserInterfaceState);
		}

		// Token: 0x040004C2 RID: 1218
		[Nullable(2)]
		private DiskConsoleMenu _menu;
	}
}
