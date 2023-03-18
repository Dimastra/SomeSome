using System;
using System.Runtime.CompilerServices;
using Content.Shared.Borgs;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client.Borgs
{
	// Token: 0x02000419 RID: 1049
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LawsBoundUserInterface : BoundUserInterface
	{
		// Token: 0x060019B5 RID: 6581 RVA: 0x0009380F File Offset: 0x00091A0F
		public LawsBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
			this.Machine = owner.Owner;
		}

		// Token: 0x060019B6 RID: 6582 RVA: 0x00093825 File Offset: 0x00091A25
		protected override void Open()
		{
			base.Open();
			this._lawsMenu = new LawsMenu(this);
			this._lawsMenu.OnClose += base.Close;
			this._lawsMenu.OpenCentered();
		}

		// Token: 0x060019B7 RID: 6583 RVA: 0x0009385B File Offset: 0x00091A5B
		public void StateLawsMessage()
		{
			base.SendMessage(new StateLawsMessage());
		}

		// Token: 0x060019B8 RID: 6584 RVA: 0x00093868 File Offset: 0x00091A68
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
		}

		// Token: 0x060019B9 RID: 6585 RVA: 0x00093871 File Offset: 0x00091A71
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			LawsMenu lawsMenu = this._lawsMenu;
			if (lawsMenu == null)
			{
				return;
			}
			lawsMenu.Dispose();
		}

		// Token: 0x04000D0F RID: 3343
		[Nullable(2)]
		private LawsMenu _lawsMenu;

		// Token: 0x04000D10 RID: 3344
		public EntityUid Machine;
	}
}
