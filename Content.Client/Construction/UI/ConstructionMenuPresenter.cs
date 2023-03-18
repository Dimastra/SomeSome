using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Shared.Construction;
using Content.Shared.Construction.Prototypes;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Placement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.Utility;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Client.Construction.UI
{
	// Token: 0x02000393 RID: 915
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class ConstructionMenuPresenter : IDisposable
	{
		// Token: 0x1700048E RID: 1166
		// (get) Token: 0x060016C0 RID: 5824 RVA: 0x00084B75 File Offset: 0x00082D75
		// (set) Token: 0x060016C1 RID: 5825 RVA: 0x00084B8C File Offset: 0x00082D8C
		private bool CraftingAvailable
		{
			get
			{
				return this._uiManager.GetActiveUIWidget<GameTopMenuBar>().CraftingButton.Visible;
			}
			set
			{
				this._uiManager.GetActiveUIWidget<GameTopMenuBar>().CraftingButton.Visible = value;
				if (!value)
				{
					this._constructionView.Close();
				}
			}
		}

		// Token: 0x1700048F RID: 1167
		// (get) Token: 0x060016C2 RID: 5826 RVA: 0x00084BB2 File Offset: 0x00082DB2
		private bool IsAtFront
		{
			get
			{
				return this._constructionView.IsOpen && this._constructionView.IsAtFront();
			}
		}

		// Token: 0x17000490 RID: 1168
		// (get) Token: 0x060016C3 RID: 5827 RVA: 0x00084BCE File Offset: 0x00082DCE
		// (set) Token: 0x060016C4 RID: 5828 RVA: 0x00084BDC File Offset: 0x00082DDC
		private bool WindowOpen
		{
			get
			{
				return this._constructionView.IsOpen;
			}
			set
			{
				if (value && this.CraftingAvailable)
				{
					if (this._constructionView.IsOpen)
					{
						this._constructionView.MoveToFront();
					}
					else
					{
						this._constructionView.OpenCentered();
					}
					if (this._selected != null)
					{
						this.PopulateInfo(this._selected);
						return;
					}
				}
				else
				{
					this._constructionView.Close();
				}
			}
		}

		// Token: 0x060016C5 RID: 5829 RVA: 0x00084C3C File Offset: 0x00082E3C
		public ConstructionMenuPresenter()
		{
			IoCManager.InjectDependencies<ConstructionMenuPresenter>(this);
			this._constructionView = new ConstructionMenu();
			ConstructionSystem newSystem;
			if (this._systemManager.TryGetEntitySystem<ConstructionSystem>(ref newSystem))
			{
				this.SystemBindingChanged(newSystem);
			}
			this._systemManager.SystemLoaded += this.OnSystemLoaded;
			this._systemManager.SystemUnloaded += this.OnSystemUnloaded;
			this._placementManager.PlacementChanged += this.OnPlacementChanged;
			this._constructionView.OnClose += delegate()
			{
				this._uiManager.GetActiveUIWidget<GameTopMenuBar>().CraftingButton.Pressed = false;
			};
			this._constructionView.ClearAllGhosts += delegate([Nullable(2)] object _, EventArgs _)
			{
				ConstructionSystem constructionSystem = this._constructionSystem;
				if (constructionSystem == null)
				{
					return;
				}
				constructionSystem.ClearAllGhosts();
			};
			this._constructionView.PopulateRecipes += this.OnViewPopulateRecipes;
			this._constructionView.RecipeSelected += this.OnViewRecipeSelected;
			this._constructionView.BuildButtonToggled += delegate(object _, bool b)
			{
				this.BuildButtonToggled(b);
			};
			this._constructionView.EraseButtonToggled += delegate(object _, bool b)
			{
				if (this._constructionSystem == null)
				{
					return;
				}
				if (b)
				{
					this._placementManager.Clear();
				}
				this._placementManager.ToggleEraserHijacked(new ConstructionPlacementHijack(this._constructionSystem, null));
				this._constructionView.EraseButtonPressed = b;
			};
			this.PopulateCategories();
			this.OnViewPopulateRecipes(this._constructionView, new ValueTuple<string, string>(string.Empty, string.Empty));
		}

		// Token: 0x060016C6 RID: 5830 RVA: 0x00084D67 File Offset: 0x00082F67
		public void OnHudCraftingButtonToggled(BaseButton.ButtonToggledEventArgs args)
		{
			this.WindowOpen = args.Pressed;
		}

		// Token: 0x060016C7 RID: 5831 RVA: 0x00084D78 File Offset: 0x00082F78
		public void Dispose()
		{
			this._constructionView.Dispose();
			this.SystemBindingChanged(null);
			this._systemManager.SystemLoaded -= this.OnSystemLoaded;
			this._systemManager.SystemUnloaded -= this.OnSystemUnloaded;
			this._placementManager.PlacementChanged -= this.OnPlacementChanged;
		}

		// Token: 0x060016C8 RID: 5832 RVA: 0x00084DDC File Offset: 0x00082FDC
		private void OnPlacementChanged([Nullable(2)] object sender, EventArgs e)
		{
			this._constructionView.ResetPlacement();
		}

		// Token: 0x060016C9 RID: 5833 RVA: 0x00084DEC File Offset: 0x00082FEC
		[NullableContext(2)]
		private void OnViewRecipeSelected(object sender, ItemList.Item item)
		{
			if (item == null)
			{
				this._selected = null;
				this._constructionView.ClearRecipeInfo();
				return;
			}
			this._selected = (ConstructionPrototype)item.Metadata;
			if (this._placementManager.IsActive && !this._placementManager.Eraser)
			{
				this.UpdateGhostPlacement();
			}
			this.PopulateInfo(this._selected);
		}

		// Token: 0x060016CA RID: 5834 RVA: 0x00084E4C File Offset: 0x0008304C
		[NullableContext(2)]
		private void OnViewPopulateRecipes(object sender, [TupleElementNames(new string[]
		{
			"search",
			"catagory"
		})] [Nullable(new byte[]
		{
			0,
			1,
			1
		})] ValueTuple<string, string> args)
		{
			string item = args.Item1;
			string item2 = args.Item2;
			ItemList recipes = this._constructionView.Recipes;
			recipes.Clear();
			List<ConstructionPrototype> list = new List<ConstructionPrototype>();
			foreach (ConstructionPrototype constructionPrototype in this._prototypeManager.EnumeratePrototypes<ConstructionPrototype>())
			{
				if ((string.IsNullOrEmpty(item) || constructionPrototype.Name.ToLowerInvariant().Contains(item.Trim().ToLowerInvariant())) && (string.IsNullOrEmpty(item2) || !(item2 != "construction-category-all") || !(constructionPrototype.Category != item2)))
				{
					list.Add(constructionPrototype);
				}
			}
			list.Sort((ConstructionPrototype a, ConstructionPrototype b) => string.Compare(a.Name, b.Name, StringComparison.InvariantCulture));
			foreach (ConstructionPrototype recipe in list)
			{
				recipes.Add(ConstructionMenuPresenter.GetItem(recipe, recipes));
			}
		}

		// Token: 0x060016CB RID: 5835 RVA: 0x00084F80 File Offset: 0x00083180
		private void PopulateCategories()
		{
			HashSet<string> hashSet = new HashSet<string>();
			hashSet.Add("construction-category-all");
			foreach (ConstructionPrototype constructionPrototype in this._prototypeManager.EnumeratePrototypes<ConstructionPrototype>())
			{
				string category = constructionPrototype.Category;
				if (!string.IsNullOrEmpty(category))
				{
					hashSet.Add(category);
				}
			}
			this._constructionView.Category.Clear();
			IEnumerable<string> source = hashSet;
			Func<string, string> keySelector;
			if ((keySelector = ConstructionMenuPresenter.<>O.<0>__GetString) == null)
			{
				keySelector = (ConstructionMenuPresenter.<>O.<0>__GetString = new Func<string, string>(Loc.GetString));
			}
			string[] array = source.OrderBy(keySelector).ToArray<string>();
			Array.Sort<string>(array);
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				this._constructionView.Category.AddItem(Loc.GetString(text), new int?(i));
			}
			this._constructionView.Categories = array;
		}

		// Token: 0x060016CC RID: 5836 RVA: 0x00085074 File Offset: 0x00083274
		private void PopulateInfo(ConstructionPrototype prototype)
		{
			SpriteSystem entitySystem = this._systemManager.GetEntitySystem<SpriteSystem>();
			this._constructionView.ClearRecipeInfo();
			this._constructionView.SetRecipeInfo(prototype.Name, prototype.Description, entitySystem.Frame0(prototype.Icon), prototype.Type != ConstructionType.Item);
			ItemList recipeStepList = this._constructionView.RecipeStepList;
			this.GenerateStepList(prototype, recipeStepList);
		}

		// Token: 0x060016CD RID: 5837 RVA: 0x000850DC File Offset: 0x000832DC
		private void GenerateStepList(ConstructionPrototype prototype, ItemList stepList)
		{
			ConstructionSystem constructionSystem = this._constructionSystem;
			ConstructionGuide constructionGuide = (constructionSystem != null) ? constructionSystem.GetGuide(prototype) : null;
			if (constructionGuide == null)
			{
				return;
			}
			SpriteSystem entitySystem = this._systemManager.GetEntitySystem<SpriteSystem>();
			foreach (ConstructionGuideEntry constructionGuideEntry in constructionGuide.Entries)
			{
				string text = (constructionGuideEntry.Arguments != null) ? Loc.GetString(constructionGuideEntry.Localization, constructionGuideEntry.Arguments) : Loc.GetString(constructionGuideEntry.Localization);
				int? entryNumber = constructionGuideEntry.EntryNumber;
				if (entryNumber != null)
				{
					int valueOrDefault = entryNumber.GetValueOrDefault();
					text = Loc.GetString("construction-presenter-step-wrapper", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("step-number", valueOrDefault),
						new ValueTuple<string, object>("text", text)
					});
				}
				text = text.PadLeft(text.Length + constructionGuideEntry.Padding);
				Texture texture = (constructionGuideEntry.Icon != null) ? entitySystem.Frame0(constructionGuideEntry.Icon) : Texture.Transparent;
				stepList.AddItem(text, texture, false);
			}
		}

		// Token: 0x060016CE RID: 5838 RVA: 0x000851F4 File Offset: 0x000833F4
		private static ItemList.Item GetItem(ConstructionPrototype recipe, ItemList itemList)
		{
			return new ItemList.Item(itemList)
			{
				Metadata = recipe,
				Text = recipe.Name,
				Icon = SpriteSpecifierExt.Frame0(recipe.Icon),
				TooltipEnabled = true,
				TooltipText = recipe.Description
			};
		}

		// Token: 0x060016CF RID: 5839 RVA: 0x00085234 File Offset: 0x00083434
		private void BuildButtonToggled(bool pressed)
		{
			if (pressed)
			{
				if (this._selected == null)
				{
					return;
				}
				if (this._constructionSystem == null)
				{
					this._constructionView.BuildButtonPressed = false;
					return;
				}
				if (this._selected.Type == ConstructionType.Item)
				{
					this._constructionSystem.TryStartItemConstruction(this._selected.ID);
					this._constructionView.BuildButtonPressed = false;
					return;
				}
				this._placementManager.BeginPlacing(new PlacementInformation
				{
					IsTile = false,
					PlacementOption = this._selected.PlacementMode
				}, new ConstructionPlacementHijack(this._constructionSystem, this._selected));
				this.UpdateGhostPlacement();
			}
			else
			{
				this._placementManager.Clear();
			}
			this._constructionView.BuildButtonPressed = pressed;
		}

		// Token: 0x060016D0 RID: 5840 RVA: 0x000852F0 File Offset: 0x000834F0
		private void UpdateGhostPlacement()
		{
			if (this._selected == null || this._selected.Type != ConstructionType.Structure)
			{
				return;
			}
			ConstructionSystem entitySystem = this._systemManager.GetEntitySystem<ConstructionSystem>();
			this._placementManager.BeginPlacing(new PlacementInformation
			{
				IsTile = false,
				PlacementOption = this._selected.PlacementMode
			}, new ConstructionPlacementHijack(entitySystem, this._selected));
			this._constructionView.BuildButtonPressed = true;
		}

		// Token: 0x060016D1 RID: 5841 RVA: 0x00085360 File Offset: 0x00083560
		private void OnSystemLoaded([Nullable(2)] object sender, SystemChangedArgs args)
		{
			ConstructionSystem constructionSystem = args.System as ConstructionSystem;
			if (constructionSystem != null)
			{
				this.SystemBindingChanged(constructionSystem);
			}
		}

		// Token: 0x060016D2 RID: 5842 RVA: 0x00085383 File Offset: 0x00083583
		private void OnSystemUnloaded([Nullable(2)] object sender, SystemChangedArgs args)
		{
			if (args.System is ConstructionSystem)
			{
				this.SystemBindingChanged(null);
			}
		}

		// Token: 0x060016D3 RID: 5843 RVA: 0x00085399 File Offset: 0x00083599
		[NullableContext(2)]
		private void SystemBindingChanged(ConstructionSystem newSystem)
		{
			if (newSystem == null)
			{
				if (this._constructionSystem == null)
				{
					return;
				}
				this.UnbindFromSystem();
				return;
			}
			else
			{
				if (this._constructionSystem == null)
				{
					this.BindToSystem(newSystem);
					return;
				}
				this.UnbindFromSystem();
				this.BindToSystem(newSystem);
				return;
			}
		}

		// Token: 0x060016D4 RID: 5844 RVA: 0x000853CC File Offset: 0x000835CC
		private void BindToSystem(ConstructionSystem system)
		{
			this._constructionSystem = system;
			system.ToggleCraftingWindow += this.SystemOnToggleMenu;
			system.CraftingAvailabilityChanged += this.SystemCraftingAvailabilityChanged;
			system.ConstructionGuideAvailable += this.SystemGuideAvailable;
			if (this._uiManager.GetActiveUIWidgetOrNull<GameTopMenuBar>() != null)
			{
				this.CraftingAvailable = system.CraftingEnabled;
			}
		}

		// Token: 0x060016D5 RID: 5845 RVA: 0x00085430 File Offset: 0x00083630
		private void UnbindFromSystem()
		{
			ConstructionSystem constructionSystem = this._constructionSystem;
			if (constructionSystem == null)
			{
				throw new InvalidOperationException();
			}
			constructionSystem.ToggleCraftingWindow -= this.SystemOnToggleMenu;
			constructionSystem.CraftingAvailabilityChanged -= this.SystemCraftingAvailabilityChanged;
			constructionSystem.ConstructionGuideAvailable -= this.SystemGuideAvailable;
			this._constructionSystem = null;
		}

		// Token: 0x060016D6 RID: 5846 RVA: 0x00085488 File Offset: 0x00083688
		private void SystemCraftingAvailabilityChanged([Nullable(2)] object sender, CraftingAvailabilityChangedArgs e)
		{
			if (this._uiManager.ActiveScreen == null)
			{
				return;
			}
			this.CraftingAvailable = e.Available;
		}

		// Token: 0x060016D7 RID: 5847 RVA: 0x000854A4 File Offset: 0x000836A4
		private void SystemOnToggleMenu([Nullable(2)] object sender, EventArgs eventArgs)
		{
			if (!this.CraftingAvailable)
			{
				return;
			}
			if (!this.WindowOpen)
			{
				this.WindowOpen = true;
				this._uiManager.GetActiveUIWidget<GameTopMenuBar>().CraftingButton.Pressed = true;
				return;
			}
			if (this.IsAtFront)
			{
				this.WindowOpen = false;
				this._uiManager.GetActiveUIWidget<GameTopMenuBar>().CraftingButton.Pressed = false;
				return;
			}
			this._constructionView.MoveToFront();
		}

		// Token: 0x060016D8 RID: 5848 RVA: 0x00085511 File Offset: 0x00083711
		private void SystemGuideAvailable([Nullable(2)] object sender, string e)
		{
			if (!this.CraftingAvailable)
			{
				return;
			}
			if (!this.WindowOpen)
			{
				return;
			}
			if (this._selected == null)
			{
				return;
			}
			this.PopulateInfo(this._selected);
		}

		// Token: 0x04000BDC RID: 3036
		[Dependency]
		private readonly IEntitySystemManager _systemManager;

		// Token: 0x04000BDD RID: 3037
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000BDE RID: 3038
		[Dependency]
		private readonly IPlacementManager _placementManager;

		// Token: 0x04000BDF RID: 3039
		[Dependency]
		private readonly IUserInterfaceManager _uiManager;

		// Token: 0x04000BE0 RID: 3040
		private readonly IConstructionMenuView _constructionView;

		// Token: 0x04000BE1 RID: 3041
		[Nullable(2)]
		private ConstructionSystem _constructionSystem;

		// Token: 0x04000BE2 RID: 3042
		[Nullable(2)]
		private ConstructionPrototype _selected;

		// Token: 0x02000394 RID: 916
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04000BE3 RID: 3043
			[Nullable(0)]
			public static Func<string, string> <0>__GetString;
		}
	}
}
