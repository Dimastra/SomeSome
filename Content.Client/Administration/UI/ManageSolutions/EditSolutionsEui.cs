using System;
using System.Runtime.CompilerServices;
using Content.Client.Eui;
using Content.Shared.Administration;
using Content.Shared.Eui;

namespace Content.Client.Administration.UI.ManageSolutions
{
	// Token: 0x020004BD RID: 1213
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EditSolutionsEui : BaseEui
	{
		// Token: 0x06001E98 RID: 7832 RVA: 0x000B33FA File Offset: 0x000B15FA
		public EditSolutionsEui()
		{
			this._window = new EditSolutionsWindow();
			this._window.OnClose += delegate()
			{
				base.SendMessage(new EditSolutionsEuiMsg.Close());
			};
		}

		// Token: 0x06001E99 RID: 7833 RVA: 0x000B3424 File Offset: 0x000B1624
		public override void Opened()
		{
			base.Opened();
			this._window.OpenCentered();
		}

		// Token: 0x06001E9A RID: 7834 RVA: 0x000B3437 File Offset: 0x000B1637
		public override void Closed()
		{
			base.Closed();
			this._window.OnClose -= delegate()
			{
				base.SendMessage(new EditSolutionsEuiMsg.Close());
			};
			this._window.Close();
		}

		// Token: 0x06001E9B RID: 7835 RVA: 0x000B3464 File Offset: 0x000B1664
		public override void HandleState(EuiStateBase baseState)
		{
			EditSolutionsEuiState editSolutionsEuiState = (EditSolutionsEuiState)baseState;
			this._window.SetTargetEntity(editSolutionsEuiState.Target);
			this._window.UpdateSolutions(editSolutionsEuiState.Solutions);
			this._window.UpdateReagents();
		}

		// Token: 0x04000EDE RID: 3806
		private readonly EditSolutionsWindow _window;
	}
}
