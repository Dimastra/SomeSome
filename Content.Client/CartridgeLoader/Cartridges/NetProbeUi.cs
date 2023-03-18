using System;
using System.Runtime.CompilerServices;
using Content.Client.UserInterface.Fragments;
using Content.Shared.CartridgeLoader.Cartridges;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client.CartridgeLoader.Cartridges
{
	// Token: 0x020003FC RID: 1020
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NetProbeUi : UIFragment
	{
		// Token: 0x0600190C RID: 6412 RVA: 0x0008FA51 File Offset: 0x0008DC51
		public override Control GetUIFragmentRoot()
		{
			return this._fragment;
		}

		// Token: 0x0600190D RID: 6413 RVA: 0x0008FA59 File Offset: 0x0008DC59
		public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
		{
			this._fragment = new NetProbeUiFragment();
		}

		// Token: 0x0600190E RID: 6414 RVA: 0x0008FA68 File Offset: 0x0008DC68
		public override void UpdateState(BoundUserInterfaceState state)
		{
			NetProbeUiState netProbeUiState = state as NetProbeUiState;
			if (netProbeUiState == null)
			{
				return;
			}
			NetProbeUiFragment fragment = this._fragment;
			if (fragment == null)
			{
				return;
			}
			fragment.UpdateState(netProbeUiState.ProbedDevices);
		}

		// Token: 0x04000CCF RID: 3279
		[Nullable(2)]
		private NetProbeUiFragment _fragment;
	}
}
