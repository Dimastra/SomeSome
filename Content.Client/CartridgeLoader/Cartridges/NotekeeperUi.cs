using System;
using System.Runtime.CompilerServices;
using Content.Client.UserInterface.Fragments;
using Content.Shared.CartridgeLoader;
using Content.Shared.CartridgeLoader.Cartridges;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client.CartridgeLoader.Cartridges
{
	// Token: 0x020003FE RID: 1022
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NotekeeperUi : UIFragment
	{
		// Token: 0x06001918 RID: 6424 RVA: 0x0009007B File Offset: 0x0008E27B
		public override Control GetUIFragmentRoot()
		{
			return this._fragment;
		}

		// Token: 0x06001919 RID: 6425 RVA: 0x00090084 File Offset: 0x0008E284
		public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
		{
			this._fragment = new NotekeeperUiFragment();
			this._fragment.OnNoteRemoved += delegate(string note)
			{
				this.SendNotekeeperMessage(NotekeeperUiAction.Remove, note, userInterface);
			};
			this._fragment.OnNoteAdded += delegate(string note)
			{
				this.SendNotekeeperMessage(NotekeeperUiAction.Add, note, userInterface);
			};
		}

		// Token: 0x0600191A RID: 6426 RVA: 0x000900E0 File Offset: 0x0008E2E0
		public override void UpdateState(BoundUserInterfaceState state)
		{
			NotekeeperUiState notekeeperUiState = state as NotekeeperUiState;
			if (notekeeperUiState == null)
			{
				return;
			}
			NotekeeperUiFragment fragment = this._fragment;
			if (fragment == null)
			{
				return;
			}
			fragment.UpdateState(notekeeperUiState.Notes);
		}

		// Token: 0x0600191B RID: 6427 RVA: 0x00090110 File Offset: 0x0008E310
		private void SendNotekeeperMessage(NotekeeperUiAction action, string note, BoundUserInterface userInterface)
		{
			CartridgeUiMessage cartridgeUiMessage = new CartridgeUiMessage(new NotekeeperUiMessageEvent(action, note));
			userInterface.SendMessage(cartridgeUiMessage);
		}

		// Token: 0x04000CD1 RID: 3281
		[Nullable(2)]
		private NotekeeperUiFragment _fragment;
	}
}
