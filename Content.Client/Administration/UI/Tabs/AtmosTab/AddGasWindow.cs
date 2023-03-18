﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Client.Atmos.EntitySystems;
using Content.Shared.Atmos.Prototypes;
using Robust.Client.AutoGenerated;
using Robust.Client.Console;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Client.Administration.UI.Tabs.AtmosTab
{
	// Token: 0x020004A0 RID: 1184
	[GenerateTypedNameReferences]
	public sealed class AddGasWindow : DefaultWindow
	{
		// Token: 0x06001D23 RID: 7459 RVA: 0x000A9860 File Offset: 0x000A7A60
		protected override void EnteredTree()
		{
			this._gridData = from g in IoCManager.Resolve<IMapManager>().GetAllGrids()
			where (int)g.Owner != 0
			select g;
			foreach (MapGridComponent mapGridComponent in this._gridData)
			{
				LocalPlayer localPlayer = IoCManager.Resolve<IPlayerManager>().LocalPlayer;
				EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
				TransformComponent componentOrNull = EntityManagerExt.GetComponentOrNull<TransformComponent>(IoCManager.Resolve<IEntityManager>(), entityUid);
				EntityUid? entityUid2 = (componentOrNull != null) ? componentOrNull.GridUid : null;
				OptionButton gridOptions = this.GridOptions;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(mapGridComponent.Owner);
				defaultInterpolatedStringHandler.AppendLiteral(" ");
				EntityUid? entityUid3 = entityUid2;
				EntityUid owner = mapGridComponent.Owner;
				defaultInterpolatedStringHandler.AppendFormatted((entityUid3 != null && (entityUid3 == null || entityUid3.GetValueOrDefault() == owner)) ? " (Current)" : "");
				gridOptions.AddItem(defaultInterpolatedStringHandler.ToStringAndClear(), null);
			}
			this.GridOptions.OnItemSelected += delegate(OptionButton.ItemSelectedEventArgs eventArgs)
			{
				this.GridOptions.SelectId(eventArgs.Id);
			};
			this._gasData = EntitySystem.Get<AtmosphereSystem>().Gases;
			foreach (GasPrototype gasPrototype in this._gasData)
			{
				string @string = Loc.GetString(gasPrototype.Name);
				this.GasOptions.AddItem(@string + " (" + gasPrototype.ID + ")", null);
			}
			this.GasOptions.OnItemSelected += delegate(OptionButton.ItemSelectedEventArgs eventArgs)
			{
				this.GasOptions.SelectId(eventArgs.Id);
			};
			this.SubmitButton.OnPressed += this.SubmitButtonOnOnPressed;
		}

		// Token: 0x06001D24 RID: 7460 RVA: 0x000A9A70 File Offset: 0x000A7C70
		[NullableContext(1)]
		private void SubmitButtonOnOnPressed(BaseButton.ButtonEventArgs obj)
		{
			if (this._gridData == null || this._gasData == null)
			{
				return;
			}
			EntityUid owner = this._gridData.ToList<MapGridComponent>()[this.GridOptions.SelectedId].Owner;
			string id = this._gasData.ToList<GasPrototype>()[this.GasOptions.SelectedId].ID;
			IConsoleHost consoleHost = IoCManager.Resolve<IClientConsoleHost>();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 5);
			defaultInterpolatedStringHandler.AppendLiteral("addgas ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(this.TileXSpin.Value);
			defaultInterpolatedStringHandler.AppendLiteral(" ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(this.TileYSpin.Value);
			defaultInterpolatedStringHandler.AppendLiteral(" ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(owner);
			defaultInterpolatedStringHandler.AppendLiteral(" ");
			defaultInterpolatedStringHandler.AppendFormatted(id);
			defaultInterpolatedStringHandler.AppendLiteral(" ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(this.AmountSpin.Value);
			consoleHost.ExecuteCommand(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x1700060E RID: 1550
		// (get) Token: 0x06001D25 RID: 7461 RVA: 0x000A9602 File Offset: 0x000A7802
		private OptionButton GridOptions
		{
			get
			{
				return base.FindControl<OptionButton>("GridOptions");
			}
		}

		// Token: 0x1700060F RID: 1551
		// (get) Token: 0x06001D26 RID: 7462 RVA: 0x000A9B6D File Offset: 0x000A7D6D
		private SpinBox TileXSpin
		{
			get
			{
				return base.FindControl<SpinBox>("TileXSpin");
			}
		}

		// Token: 0x17000610 RID: 1552
		// (get) Token: 0x06001D27 RID: 7463 RVA: 0x000A9B7A File Offset: 0x000A7D7A
		private SpinBox TileYSpin
		{
			get
			{
				return base.FindControl<SpinBox>("TileYSpin");
			}
		}

		// Token: 0x17000611 RID: 1553
		// (get) Token: 0x06001D28 RID: 7464 RVA: 0x000A9B87 File Offset: 0x000A7D87
		private OptionButton GasOptions
		{
			get
			{
				return base.FindControl<OptionButton>("GasOptions");
			}
		}

		// Token: 0x17000612 RID: 1554
		// (get) Token: 0x06001D29 RID: 7465 RVA: 0x000A9B94 File Offset: 0x000A7D94
		private SpinBox AmountSpin
		{
			get
			{
				return base.FindControl<SpinBox>("AmountSpin");
			}
		}

		// Token: 0x17000613 RID: 1555
		// (get) Token: 0x06001D2A RID: 7466 RVA: 0x00091514 File Offset: 0x0008F714
		private Button SubmitButton
		{
			get
			{
				return base.FindControl<Button>("SubmitButton");
			}
		}

		// Token: 0x06001D2B RID: 7467 RVA: 0x000A9BA1 File Offset: 0x000A7DA1
		public AddGasWindow()
		{
			AddGasWindow.!XamlIlPopulateTrampoline(this);
		}

		// Token: 0x06001D2E RID: 7470 RVA: 0x000A9BD8 File Offset: 0x000A7DD8
		static void xaml(IServiceProvider A_0, DefaultWindow A_1)
		{
			XamlIlContext.Context<DefaultWindow> context = new XamlIlContext.Context<DefaultWindow>(A_0, null, "resm:Content.Client.Administration.UI.Tabs.AtmosTab.AddGasWindow.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Title = (string)new LocExtension("Add Gas").ProvideValue();
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 0;
			Control control = new Label
			{
				Text = (string)new LocExtension("Grid").ProvideValue(),
				MinSize = new Vector2(100f, 0f)
			};
			boxContainer2.XamlChildren.Add(control);
			control = new Control
			{
				MinSize = new Vector2(50f, 0f)
			};
			boxContainer2.XamlChildren.Add(control);
			OptionButton optionButton = new OptionButton();
			optionButton.Name = "GridOptions";
			control = optionButton;
			context.RobustNameScope.Register("GridOptions", control);
			optionButton.MinSize = new Vector2(100f, 0f);
			optionButton.HorizontalExpand = true;
			control = optionButton;
			boxContainer2.XamlChildren.Add(control);
			control = boxContainer2;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer3 = new BoxContainer();
			boxContainer3.Orientation = 0;
			control = new Label
			{
				Text = (string)new LocExtension("TileX").ProvideValue(),
				MinSize = new Vector2(100f, 0f)
			};
			boxContainer3.XamlChildren.Add(control);
			control = new Control
			{
				MinSize = new Vector2(50f, 0f)
			};
			boxContainer3.XamlChildren.Add(control);
			SpinBox spinBox = new SpinBox();
			spinBox.Name = "TileXSpin";
			control = spinBox;
			context.RobustNameScope.Register("TileXSpin", control);
			spinBox.MinSize = new Vector2(100f, 0f);
			spinBox.HorizontalExpand = true;
			control = spinBox;
			boxContainer3.XamlChildren.Add(control);
			control = boxContainer3;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer4 = new BoxContainer();
			boxContainer4.Orientation = 0;
			control = new Label
			{
				Text = (string)new LocExtension("TileY").ProvideValue(),
				MinSize = new Vector2(100f, 0f)
			};
			boxContainer4.XamlChildren.Add(control);
			control = new Control
			{
				MinSize = new Vector2(50f, 0f)
			};
			boxContainer4.XamlChildren.Add(control);
			SpinBox spinBox2 = new SpinBox();
			spinBox2.Name = "TileYSpin";
			control = spinBox2;
			context.RobustNameScope.Register("TileYSpin", control);
			spinBox2.MinSize = new Vector2(100f, 0f);
			spinBox2.HorizontalExpand = true;
			control = spinBox2;
			boxContainer4.XamlChildren.Add(control);
			control = boxContainer4;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer5 = new BoxContainer();
			boxContainer5.Orientation = 0;
			control = new Label
			{
				Text = (string)new LocExtension("Gas").ProvideValue(),
				MinSize = new Vector2(100f, 0f)
			};
			boxContainer5.XamlChildren.Add(control);
			control = new Control
			{
				MinSize = new Vector2(50f, 0f)
			};
			boxContainer5.XamlChildren.Add(control);
			OptionButton optionButton2 = new OptionButton();
			optionButton2.Name = "GasOptions";
			control = optionButton2;
			context.RobustNameScope.Register("GasOptions", control);
			optionButton2.MinSize = new Vector2(100f, 0f);
			optionButton2.HorizontalExpand = true;
			control = optionButton2;
			boxContainer5.XamlChildren.Add(control);
			control = boxContainer5;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer6 = new BoxContainer();
			boxContainer6.Orientation = 0;
			control = new Label
			{
				Text = (string)new LocExtension("Amount").ProvideValue(),
				MinSize = new Vector2(100f, 0f)
			};
			boxContainer6.XamlChildren.Add(control);
			control = new Control
			{
				MinSize = new Vector2(50f, 0f)
			};
			boxContainer6.XamlChildren.Add(control);
			SpinBox spinBox3 = new SpinBox();
			spinBox3.Name = "AmountSpin";
			control = spinBox3;
			context.RobustNameScope.Register("AmountSpin", control);
			spinBox3.MinSize = new Vector2(100f, 0f);
			spinBox3.HorizontalExpand = true;
			control = spinBox3;
			boxContainer6.XamlChildren.Add(control);
			control = boxContainer6;
			boxContainer.XamlChildren.Add(control);
			Button button = new Button();
			button.Name = "SubmitButton";
			control = button;
			context.RobustNameScope.Register("SubmitButton", control);
			button.Text = (string)new LocExtension("Add Gas").ProvideValue();
			control = button;
			boxContainer.XamlChildren.Add(control);
			control = boxContainer;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x06001D2F RID: 7471 RVA: 0x000AA1C2 File Offset: 0x000A83C2
		private static void !XamlIlPopulateTrampoline(AddGasWindow A_0)
		{
			AddGasWindow.Populate:Content.Client.Administration.UI.Tabs.AtmosTab.AddGasWindow.xaml(null, A_0);
		}

		// Token: 0x04000E91 RID: 3729
		[Nullable(new byte[]
		{
			2,
			1
		})]
		private IEnumerable<MapGridComponent> _gridData;

		// Token: 0x04000E92 RID: 3730
		[Nullable(new byte[]
		{
			2,
			1
		})]
		private IEnumerable<GasPrototype> _gasData;
	}
}