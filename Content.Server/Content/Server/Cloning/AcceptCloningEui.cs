using System;
using System.Runtime.CompilerServices;
using Content.Server.EUI;
using Content.Server.Mind;
using Content.Shared.Cloning;
using Content.Shared.Eui;

namespace Content.Server.Cloning
{
	// Token: 0x0200063E RID: 1598
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AcceptCloningEui : BaseEui
	{
		// Token: 0x060021F0 RID: 8688 RVA: 0x000B0E43 File Offset: 0x000AF043
		public AcceptCloningEui(Mind mind, CloningSystem cloningSys)
		{
			this._mind = mind;
			this._cloningSystem = cloningSys;
		}

		// Token: 0x060021F1 RID: 8689 RVA: 0x000B0E5C File Offset: 0x000AF05C
		public override void HandleMessage(EuiMessageBase msg)
		{
			base.HandleMessage(msg);
			AcceptCloningChoiceMessage choice = msg as AcceptCloningChoiceMessage;
			if (choice == null || choice.Button == AcceptCloningUiButton.Deny)
			{
				base.Close();
				return;
			}
			this._cloningSystem.TransferMindToClone(this._mind);
			base.Close();
		}

		// Token: 0x040014CE RID: 5326
		private readonly CloningSystem _cloningSystem;

		// Token: 0x040014CF RID: 5327
		private readonly Mind _mind;
	}
}
