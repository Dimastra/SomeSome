using System;
using System.Runtime.CompilerServices;
using Content.Client.UserInterface.Systems.Actions;
using Content.Client.UserInterface.Systems.Admin;
using Content.Client.UserInterface.Systems.Bwoink;
using Content.Client.UserInterface.Systems.Character;
using Content.Client.UserInterface.Systems.Crafting;
using Content.Client.UserInterface.Systems.Emotions;
using Content.Client.UserInterface.Systems.EscapeMenu;
using Content.Client.UserInterface.Systems.Inventory;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Client.UserInterface.Systems.Sandbox;
using Content.Client.White.MeatyOre;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.IoC;

namespace Content.Client.UserInterface.Systems.MenuBar
{
	// Token: 0x02000075 RID: 117
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GameTopMenuBarUIController : UIController
	{
		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000250 RID: 592 RVA: 0x0000F85C File Offset: 0x0000DA5C
		[Nullable(2)]
		private GameTopMenuBar GameTopMenuBar
		{
			[NullableContext(2)]
			get
			{
				return this.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>();
			}
		}

		// Token: 0x06000251 RID: 593 RVA: 0x0000F86C File Offset: 0x0000DA6C
		public void UnloadButtons()
		{
			this._escape.UnloadButton();
			this._inventory.UnloadButton();
			this._admin.UnloadButton();
			this._character.UnloadButton();
			this._crafting.UnloadButton(null);
			this._ahelp.UnloadButton();
			this._action.UnloadButton();
			this._meatyOreUIController.UnloadButton();
			this._sandbox.UnloadButton();
			this._emotions.UnloadButton();
		}

		// Token: 0x06000252 RID: 594 RVA: 0x0000F8E8 File Offset: 0x0000DAE8
		public void LoadButtons()
		{
			this._escape.LoadButton();
			this._inventory.LoadButton();
			this._admin.LoadButton();
			this._character.LoadButton();
			this._crafting.LoadButton();
			this._ahelp.LoadButton();
			this._action.LoadButton();
			this._meatyOreUIController.LoadButton();
			this._sandbox.LoadButton();
			this._emotions.LoadButton();
		}

		// Token: 0x04000155 RID: 341
		[Dependency]
		private readonly EscapeUIController _escape;

		// Token: 0x04000156 RID: 342
		[Dependency]
		private readonly InventoryUIController _inventory;

		// Token: 0x04000157 RID: 343
		[Dependency]
		private readonly AdminUIController _admin;

		// Token: 0x04000158 RID: 344
		[Dependency]
		private readonly CharacterUIController _character;

		// Token: 0x04000159 RID: 345
		[Dependency]
		private readonly CraftingUIController _crafting;

		// Token: 0x0400015A RID: 346
		[Dependency]
		private readonly AHelpUIController _ahelp;

		// Token: 0x0400015B RID: 347
		[Dependency]
		private readonly ActionUIController _action;

		// Token: 0x0400015C RID: 348
		[Dependency]
		private readonly SandboxUIController _sandbox;

		// Token: 0x0400015D RID: 349
		[Dependency]
		private readonly MeatyOreUIController _meatyOreUIController;

		// Token: 0x0400015E RID: 350
		[Dependency]
		private readonly EmotionsUIController _emotions;
	}
}
