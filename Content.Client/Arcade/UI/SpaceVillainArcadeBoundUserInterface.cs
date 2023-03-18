using System;
using System.Runtime.CompilerServices;
using Content.Shared.Arcade;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Arcade.UI
{
	// Token: 0x02000469 RID: 1129
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SpaceVillainArcadeBoundUserInterface : BoundUserInterface
	{
		// Token: 0x06001C04 RID: 7172 RVA: 0x000A295F File Offset: 0x000A0B5F
		public SpaceVillainArcadeBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
			this.SendAction(SharedSpaceVillainArcadeComponent.PlayerAction.RequestData);
		}

		// Token: 0x06001C05 RID: 7173 RVA: 0x000A2970 File Offset: 0x000A0B70
		public void SendAction(SharedSpaceVillainArcadeComponent.PlayerAction action)
		{
			base.SendMessage(new SharedSpaceVillainArcadeComponent.SpaceVillainArcadePlayerActionMessage(action));
		}

		// Token: 0x06001C06 RID: 7174 RVA: 0x000A297E File Offset: 0x000A0B7E
		protected override void Open()
		{
			base.Open();
			this._menu = new SpaceVillainArcadeMenu(this);
			this._menu.OnClose += base.Close;
			this._menu.OpenCentered();
		}

		// Token: 0x06001C07 RID: 7175 RVA: 0x000A29B4 File Offset: 0x000A0BB4
		protected override void ReceiveMessage(BoundUserInterfaceMessage message)
		{
			SharedSpaceVillainArcadeComponent.SpaceVillainArcadeDataUpdateMessage spaceVillainArcadeDataUpdateMessage = message as SharedSpaceVillainArcadeComponent.SpaceVillainArcadeDataUpdateMessage;
			if (spaceVillainArcadeDataUpdateMessage != null)
			{
				SpaceVillainArcadeMenu menu = this._menu;
				if (menu == null)
				{
					return;
				}
				menu.UpdateInfo(spaceVillainArcadeDataUpdateMessage);
			}
		}

		// Token: 0x06001C08 RID: 7176 RVA: 0x000A29DC File Offset: 0x000A0BDC
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				SpaceVillainArcadeMenu menu = this._menu;
				if (menu == null)
				{
					return;
				}
				menu.Dispose();
			}
		}

		// Token: 0x04000E19 RID: 3609
		[Nullable(2)]
		[ViewVariables]
		private SpaceVillainArcadeMenu _menu;
	}
}
