using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Crayon;
using Content.Shared.Decals;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client.Crayon.UI
{
	// Token: 0x02000379 RID: 889
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class CrayonBoundUserInterface : BoundUserInterface
	{
		// Token: 0x060015D1 RID: 5585 RVA: 0x000021BC File Offset: 0x000003BC
		public CrayonBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
		}

		// Token: 0x060015D2 RID: 5586 RVA: 0x00081178 File Offset: 0x0007F378
		protected override void Open()
		{
			base.Open();
			this._menu = new CrayonWindow(this);
			this._menu.OnClose += base.Close;
			IEnumerable<DecalPrototype> prototypes = from x in IoCManager.Resolve<IPrototypeManager>().EnumeratePrototypes<DecalPrototype>()
			where x.Tags.Contains("crayon")
			select x;
			this._menu.Populate(prototypes);
			this._menu.OpenCentered();
		}

		// Token: 0x060015D3 RID: 5587 RVA: 0x000811F4 File Offset: 0x0007F3F4
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			CrayonWindow menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.UpdateState((CrayonBoundUserInterfaceState)state);
		}

		// Token: 0x060015D4 RID: 5588 RVA: 0x00081213 File Offset: 0x0007F413
		public void Select(string state)
		{
			base.SendMessage(new CrayonSelectMessage(state));
		}

		// Token: 0x060015D5 RID: 5589 RVA: 0x00081221 File Offset: 0x0007F421
		public void SelectColor(Color color)
		{
			base.SendMessage(new CrayonColorMessage(color));
		}

		// Token: 0x060015D6 RID: 5590 RVA: 0x0008122F File Offset: 0x0007F42F
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				CrayonWindow menu = this._menu;
				if (menu != null)
				{
					menu.Close();
				}
				this._menu = null;
			}
		}

		// Token: 0x04000B6A RID: 2922
		[Nullable(2)]
		private CrayonWindow _menu;
	}
}
