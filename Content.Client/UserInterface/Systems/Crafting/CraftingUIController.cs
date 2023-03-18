using System;
using System.Runtime.CompilerServices;
using Content.Client.Construction.UI;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Systems.Crafting
{
	// Token: 0x0200009F RID: 159
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class CraftingUIController : UIController, IOnStateChanged<GameplayState>, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
	{
		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x060003C9 RID: 969 RVA: 0x00016922 File Offset: 0x00014B22
		private MenuButton CraftingButton
		{
			get
			{
				GameTopMenuBar activeUIWidgetOrNull = this.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>();
				if (activeUIWidgetOrNull == null)
				{
					return null;
				}
				return activeUIWidgetOrNull.CraftingButton;
			}
		}

		// Token: 0x060003CA RID: 970 RVA: 0x0001693A File Offset: 0x00014B3A
		[NullableContext(1)]
		public void OnStateEntered(GameplayState state)
		{
			this._presenter = new ConstructionMenuPresenter();
		}

		// Token: 0x060003CB RID: 971 RVA: 0x00016947 File Offset: 0x00014B47
		[NullableContext(1)]
		public void OnStateExited(GameplayState state)
		{
			if (this._presenter == null)
			{
				return;
			}
			this.UnloadButton(this._presenter);
			this._presenter.Dispose();
			this._presenter = null;
		}

		// Token: 0x060003CC RID: 972 RVA: 0x00016970 File Offset: 0x00014B70
		internal void UnloadButton(ConstructionMenuPresenter presenter = null)
		{
			if (this.CraftingButton == null)
			{
				return;
			}
			if (presenter == null)
			{
				if (presenter == null)
				{
					presenter = this._presenter;
				}
				if (presenter == null)
				{
					return;
				}
			}
			this.CraftingButton.Pressed = false;
			this.CraftingButton.OnToggled -= presenter.OnHudCraftingButtonToggled;
		}

		// Token: 0x060003CD RID: 973 RVA: 0x000169B0 File Offset: 0x00014BB0
		public void LoadButton()
		{
			if (this.CraftingButton == null)
			{
				return;
			}
			this.CraftingButton.OnToggled += this.ButtonToggled;
		}

		// Token: 0x060003CE RID: 974 RVA: 0x000169D2 File Offset: 0x00014BD2
		[NullableContext(1)]
		private void ButtonToggled(BaseButton.ButtonToggledEventArgs obj)
		{
			ConstructionMenuPresenter presenter = this._presenter;
			if (presenter == null)
			{
				return;
			}
			presenter.OnHudCraftingButtonToggled(obj);
		}

		// Token: 0x040001D6 RID: 470
		private ConstructionMenuPresenter _presenter;
	}
}
