using System;
using System.Runtime.CompilerServices;
using Content.Shared.Research.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Research.UI
{
	// Token: 0x0200016F RID: 367
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ResearchClientBoundUserInterface : BoundUserInterface
	{
		// Token: 0x0600098A RID: 2442 RVA: 0x000378B9 File Offset: 0x00035AB9
		public ResearchClientBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
			base.SendMessage(new ResearchClientSyncMessage());
		}

		// Token: 0x0600098B RID: 2443 RVA: 0x000378CE File Offset: 0x00035ACE
		protected override void Open()
		{
			base.Open();
			this._menu = new ResearchClientServerSelectionMenu(this);
			this._menu.OnClose += base.Close;
			this._menu.OpenCentered();
		}

		// Token: 0x0600098C RID: 2444 RVA: 0x00037904 File Offset: 0x00035B04
		public void SelectServer(int serverId)
		{
			base.SendMessage(new ResearchClientServerSelectedMessage(serverId));
		}

		// Token: 0x0600098D RID: 2445 RVA: 0x00037912 File Offset: 0x00035B12
		public void DeselectServer()
		{
			base.SendMessage(new ResearchClientServerDeselectedMessage());
		}

		// Token: 0x0600098E RID: 2446 RVA: 0x00037920 File Offset: 0x00035B20
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			ResearchClientBoundInterfaceState researchClientBoundInterfaceState = state as ResearchClientBoundInterfaceState;
			if (researchClientBoundInterfaceState == null)
			{
				return;
			}
			ResearchClientServerSelectionMenu menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.Populate(researchClientBoundInterfaceState.ServerCount, researchClientBoundInterfaceState.ServerNames, researchClientBoundInterfaceState.ServerIds, researchClientBoundInterfaceState.SelectedServerId);
		}

		// Token: 0x0600098F RID: 2447 RVA: 0x00037967 File Offset: 0x00035B67
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			ResearchClientServerSelectionMenu menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.Dispose();
		}

		// Token: 0x040004C5 RID: 1221
		[Nullable(2)]
		private ResearchClientServerSelectionMenu _menu;
	}
}
