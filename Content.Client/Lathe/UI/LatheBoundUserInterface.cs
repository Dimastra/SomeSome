using System;
using System.Runtime.CompilerServices;
using Content.Shared.Lathe;
using Content.Shared.Research.Components;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Lathe.UI
{
	// Token: 0x0200027F RID: 639
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LatheBoundUserInterface : BoundUserInterface
	{
		// Token: 0x0600104A RID: 4170 RVA: 0x000613D5 File Offset: 0x0005F5D5
		public LatheBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey)
		{
			this.Lathe = owner.Owner;
		}

		// Token: 0x0600104B RID: 4171 RVA: 0x000613EC File Offset: 0x0005F5EC
		protected override void Open()
		{
			base.Open();
			this._menu = new LatheMenu(this);
			this._queueMenu = new LatheQueueMenu();
			this._menu.OnClose += base.Close;
			this._menu.OnQueueButtonPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this._queueMenu.OpenCenteredLeft();
			};
			this._menu.OnServerListButtonPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ConsoleServerSelectionMessage());
			};
			this._menu.OnServerSyncButtonPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				base.SendMessage(new ConsoleServerSyncMessage());
			};
			this._menu.RecipeQueueAction += delegate(string recipe, int amount)
			{
				base.SendMessage(new LatheQueueRecipeMessage(recipe, amount));
			};
			this._menu.OpenCentered();
		}

		// Token: 0x0600104C RID: 4172 RVA: 0x00061494 File Offset: 0x0005F694
		protected override void UpdateState(BoundUserInterfaceState state)
		{
			base.UpdateState(state);
			LatheUpdateState latheUpdateState = state as LatheUpdateState;
			if (latheUpdateState != null)
			{
				if (this._menu != null)
				{
					this._menu.Recipes = latheUpdateState.Recipes;
				}
				LatheMenu menu = this._menu;
				if (menu != null)
				{
					menu.PopulateRecipes(base.Owner.Owner);
				}
				LatheMenu menu2 = this._menu;
				if (menu2 != null)
				{
					menu2.PopulateMaterials(this.Lathe);
				}
				LatheQueueMenu queueMenu = this._queueMenu;
				if (queueMenu != null)
				{
					queueMenu.PopulateList(latheUpdateState.Queue);
				}
				LatheQueueMenu queueMenu2 = this._queueMenu;
				if (queueMenu2 == null)
				{
					return;
				}
				queueMenu2.SetInfo(latheUpdateState.CurrentlyProducing);
			}
		}

		// Token: 0x0600104D RID: 4173 RVA: 0x0006152B File Offset: 0x0005F72B
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposing)
			{
				return;
			}
			LatheMenu menu = this._menu;
			if (menu != null)
			{
				menu.Dispose();
			}
			LatheQueueMenu queueMenu = this._queueMenu;
			if (queueMenu == null)
			{
				return;
			}
			queueMenu.Dispose();
		}

		// Token: 0x04000809 RID: 2057
		[Nullable(2)]
		[ViewVariables]
		private LatheMenu _menu;

		// Token: 0x0400080A RID: 2058
		[Nullable(2)]
		[ViewVariables]
		private LatheQueueMenu _queueMenu;

		// Token: 0x0400080B RID: 2059
		public EntityUid Lathe;
	}
}
