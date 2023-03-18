using System;
using System.Runtime.CompilerServices;
using Content.Client.Eui;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Robust.Shared.GameObjects;

namespace Content.Client.Administration.UI.SetOutfit
{
	// Token: 0x020004B4 RID: 1204
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SetOutfitEui : BaseEui
	{
		// Token: 0x06001E21 RID: 7713 RVA: 0x000B114B File Offset: 0x000AF34B
		public SetOutfitEui()
		{
			this._window = new SetOutfitMenu();
		}

		// Token: 0x06001E22 RID: 7714 RVA: 0x000B115E File Offset: 0x000AF35E
		public override void Opened()
		{
			this._window.OpenCentered();
		}

		// Token: 0x06001E23 RID: 7715 RVA: 0x000B116B File Offset: 0x000AF36B
		public override void Closed()
		{
			base.Closed();
			this._window.Close();
		}

		// Token: 0x06001E24 RID: 7716 RVA: 0x000B1180 File Offset: 0x000AF380
		public override void HandleState(EuiStateBase state)
		{
			SetOutfitEuiState setOutfitEuiState = (SetOutfitEuiState)state;
			this._window.TargetEntityId = new EntityUid?(setOutfitEuiState.TargetEntityId);
		}

		// Token: 0x04000EBE RID: 3774
		private readonly SetOutfitMenu _window;
	}
}
