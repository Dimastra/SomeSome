using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Client.Clothing.Systems;
using Content.Shared.Clothing.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Clothing.UI
{
	// Token: 0x020003B7 RID: 951
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ChameleonBoundUserInterface : BoundUserInterface
	{
		// Token: 0x0600179B RID: 6043 RVA: 0x00087967 File Offset: 0x00085B67
		public ChameleonBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
			IoCManager.InjectDependencies<ChameleonBoundUserInterface>(this);
			this._chameleon = this._entityManager.System<ChameleonClothingSystem>();
		}

		// Token: 0x0600179C RID: 6044 RVA: 0x0008798C File Offset: 0x00085B8C
		protected override void Open()
		{
			base.Open();
			this._menu = new ChameleonMenu();
			this._menu.OnClose += base.Close;
			this._menu.OnIdSelected += this.OnIdSelected;
			this._menu.OpenCentered();
		}

		// Token: 0x0600179D RID: 6045 RVA: 0x000879E4 File Offset: 0x00085BE4
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			ChameleonBoundUserInterfaceState chameleonBoundUserInterfaceState = state as ChameleonBoundUserInterfaceState;
			if (chameleonBoundUserInterfaceState == null)
			{
				return;
			}
			IEnumerable<string> validTargets = this._chameleon.GetValidTargets(chameleonBoundUserInterfaceState.Slot);
			ChameleonMenu menu = this._menu;
			if (menu == null)
			{
				return;
			}
			menu.UpdateState(validTargets, chameleonBoundUserInterfaceState.SelectedId);
		}

		// Token: 0x0600179E RID: 6046 RVA: 0x00087A2C File Offset: 0x00085C2C
		private void OnIdSelected(string selectedId)
		{
			base.SendMessage(new ChameleonPrototypeSelectedMessage(selectedId));
		}

		// Token: 0x0600179F RID: 6047 RVA: 0x00087A3A File Offset: 0x00085C3A
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				ChameleonMenu menu = this._menu;
				if (menu != null)
				{
					menu.Close();
				}
				this._menu = null;
			}
		}

		// Token: 0x04000C0A RID: 3082
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000C0B RID: 3083
		private readonly ChameleonClothingSystem _chameleon;

		// Token: 0x04000C0C RID: 3084
		[Nullable(2)]
		private ChameleonMenu _menu;
	}
}
