﻿using System;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Client.Administration.Commands;
using Content.Client.Administration.Managers;
using Content.Client.Administration.UI.CustomControls;
using Content.Client.UserInterface.Systems.DecalPlacer;
using Content.Shared.Administration;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers.Implementations;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.Administration.UI.Tabs.AdminbusTab
{
	// Token: 0x020004AF RID: 1199
	[GenerateTypedNameReferences]
	public sealed class AdminbusTab : Control
	{
		// Token: 0x06001DD5 RID: 7637 RVA: 0x000AEC58 File Offset: 0x000ACE58
		public AdminbusTab()
		{
			AdminbusTab.!XamlIlPopulateTrampoline(this);
			IoCManager.InjectDependencies<AdminbusTab>(this);
			this._entitySpawningController = base.UserInterfaceManager.GetUIController<EntitySpawningUIController>();
			this._tileSpawningController = base.UserInterfaceManager.GetUIController<TileSpawningUIController>();
			this._decalPlacerController = base.UserInterfaceManager.GetUIController<DecalPlacerUIController>();
			IClientAdminManager clientAdminManager = IoCManager.Resolve<IClientAdminManager>();
			this.SpawnEntitiesButton.OnPressed += this.SpawnEntitiesButtonOnPressed;
			this.SpawnTilesButton.OnPressed += this.SpawnTilesButtonOnOnPressed;
			this.SpawnDecalsButton.OnPressed += this.SpawnDecalsButtonOnPressed;
			this.LoadGamePrototypeButton.OnPressed += this.LoadGamePrototypeButtonOnPressed;
			this.LoadGamePrototypeButton.Disabled = !clientAdminManager.HasFlag(AdminFlags.Query);
			this.LoadBlueprintsButton.Disabled = !clientAdminManager.HasFlag(AdminFlags.Mapping);
		}

		// Token: 0x06001DD6 RID: 7638 RVA: 0x000AED3F File Offset: 0x000ACF3F
		[NullableContext(1)]
		private void LoadGamePrototypeButtonOnPressed(BaseButton.ButtonEventArgs obj)
		{
			LoadPrototypeCommand.LoadPrototype();
		}

		// Token: 0x06001DD7 RID: 7639 RVA: 0x000AED46 File Offset: 0x000ACF46
		[NullableContext(1)]
		private void SpawnEntitiesButtonOnPressed(BaseButton.ButtonEventArgs obj)
		{
			this._entitySpawningController.ToggleWindow();
		}

		// Token: 0x06001DD8 RID: 7640 RVA: 0x000AED53 File Offset: 0x000ACF53
		[NullableContext(1)]
		private void SpawnTilesButtonOnOnPressed(BaseButton.ButtonEventArgs obj)
		{
			this._tileSpawningController.ToggleWindow();
		}

		// Token: 0x06001DD9 RID: 7641 RVA: 0x000AED60 File Offset: 0x000ACF60
		[NullableContext(1)]
		private void SpawnDecalsButtonOnPressed(BaseButton.ButtonEventArgs obj)
		{
			this._decalPlacerController.ToggleWindow();
		}

		// Token: 0x17000664 RID: 1636
		// (get) Token: 0x06001DDA RID: 7642 RVA: 0x0000ED94 File Offset: 0x0000CF94
		private Button SpawnEntitiesButton
		{
			get
			{
				return base.FindControl<Button>("SpawnEntitiesButton");
			}
		}

		// Token: 0x17000665 RID: 1637
		// (get) Token: 0x06001DDB RID: 7643 RVA: 0x0000EDA1 File Offset: 0x0000CFA1
		private Button SpawnTilesButton
		{
			get
			{
				return base.FindControl<Button>("SpawnTilesButton");
			}
		}

		// Token: 0x17000666 RID: 1638
		// (get) Token: 0x06001DDC RID: 7644 RVA: 0x0000EDAE File Offset: 0x0000CFAE
		private Button SpawnDecalsButton
		{
			get
			{
				return base.FindControl<Button>("SpawnDecalsButton");
			}
		}

		// Token: 0x17000667 RID: 1639
		// (get) Token: 0x06001DDD RID: 7645 RVA: 0x000AED6D File Offset: 0x000ACF6D
		private Button LoadGamePrototypeButton
		{
			get
			{
				return base.FindControl<Button>("LoadGamePrototypeButton");
			}
		}

		// Token: 0x17000668 RID: 1640
		// (get) Token: 0x06001DDE RID: 7646 RVA: 0x000AED7A File Offset: 0x000ACF7A
		private UICommandButton LoadBlueprintsButton
		{
			get
			{
				return base.FindControl<UICommandButton>("LoadBlueprintsButton");
			}
		}

		// Token: 0x17000669 RID: 1641
		// (get) Token: 0x06001DDF RID: 7647 RVA: 0x000AED87 File Offset: 0x000ACF87
		private CommandButton DeleteSingulos
		{
			get
			{
				return base.FindControl<CommandButton>("DeleteSingulos");
			}
		}

		// Token: 0x06001DE0 RID: 7648 RVA: 0x000AED94 File Offset: 0x000ACF94
		static void xaml(IServiceProvider A_0, Control A_1)
		{
			XamlIlContext.Context<Control> context = new XamlIlContext.Context<Control>(A_0, null, "resm:Content.Client.Administration.UI.Tabs.AdminbusTab.AdminbusTab.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Margin = new Thickness(4f, 4f, 4f, 4f);
			A_1.MinSize = new Vector2(50f, 50f);
			GridContainer gridContainer = new GridContainer();
			gridContainer.Columns = 3;
			Button button = new Button();
			button.Name = "SpawnEntitiesButton";
			Control control = button;
			context.RobustNameScope.Register("SpawnEntitiesButton", control);
			button.Text = (string)new LocExtension("sandbox-window-spawn-entities-button").ProvideValue();
			control = button;
			gridContainer.XamlChildren.Add(control);
			Button button2 = new Button();
			button2.Name = "SpawnTilesButton";
			control = button2;
			context.RobustNameScope.Register("SpawnTilesButton", control);
			button2.Text = (string)new LocExtension("sandbox-window-spawn-tiles-button").ProvideValue();
			control = button2;
			gridContainer.XamlChildren.Add(control);
			Button button3 = new Button();
			button3.Name = "SpawnDecalsButton";
			control = button3;
			context.RobustNameScope.Register("SpawnDecalsButton", control);
			button3.Text = (string)new LocExtension("sandbox-window-spawn-decals-button").ProvideValue();
			control = button3;
			gridContainer.XamlChildren.Add(control);
			Button button4 = new Button();
			button4.Name = "LoadGamePrototypeButton";
			control = button4;
			context.RobustNameScope.Register("LoadGamePrototypeButton", control);
			button4.Text = (string)new LocExtension("load-game-prototype").ProvideValue();
			control = button4;
			gridContainer.XamlChildren.Add(control);
			UICommandButton uicommandButton = new UICommandButton();
			uicommandButton.Name = "LoadBlueprintsButton";
			control = uicommandButton;
			context.RobustNameScope.Register("LoadBlueprintsButton", control);
			uicommandButton.Command = "loadbp";
			uicommandButton.Text = (string)new LocExtension("load-blueprints").ProvideValue();
			uicommandButton.WindowType = typeof(LoadBlueprintsWindow);
			control = uicommandButton;
			gridContainer.XamlChildren.Add(control);
			CommandButton commandButton = new CommandButton();
			commandButton.Command = "deleteewc Singularity";
			commandButton.Name = "DeleteSingulos";
			control = commandButton;
			context.RobustNameScope.Register("DeleteSingulos", control);
			commandButton.Text = (string)new LocExtension("delete-singularities").ProvideValue();
			control = commandButton;
			gridContainer.XamlChildren.Add(control);
			control = gridContainer;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x06001DE1 RID: 7649 RVA: 0x000AF0AB File Offset: 0x000AD2AB
		private static void !XamlIlPopulateTrampoline(AdminbusTab A_0)
		{
			AdminbusTab.Populate:Content.Client.Administration.UI.Tabs.AdminbusTab.AdminbusTab.xaml(null, A_0);
		}

		// Token: 0x04000EA3 RID: 3747
		[Nullable(1)]
		private readonly EntitySpawningUIController _entitySpawningController;

		// Token: 0x04000EA4 RID: 3748
		[Nullable(1)]
		private readonly TileSpawningUIController _tileSpawningController;

		// Token: 0x04000EA5 RID: 3749
		[Nullable(1)]
		private readonly DecalPlacerUIController _decalPlacerController;
	}
}