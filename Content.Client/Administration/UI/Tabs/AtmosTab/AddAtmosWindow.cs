﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Robust.Client.AutoGenerated;
using Robust.Client.Console;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Client.Administration.UI.Tabs.AtmosTab
{
	// Token: 0x0200049E RID: 1182
	[GenerateTypedNameReferences]
	public sealed class AddAtmosWindow : DefaultWindow
	{
		// Token: 0x06001D18 RID: 7448 RVA: 0x000A9428 File Offset: 0x000A7628
		protected override void EnteredTree()
		{
			this._data = from g in IoCManager.Resolve<IMapManager>().GetAllGrids()
			where (int)g.Owner != 0
			select g;
			foreach (MapGridComponent mapGridComponent in this._data)
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
			this.SubmitButton.OnPressed += this.SubmitButtonOnOnPressed;
		}

		// Token: 0x06001D19 RID: 7449 RVA: 0x000A959C File Offset: 0x000A779C
		[NullableContext(1)]
		private void SubmitButtonOnOnPressed(BaseButton.ButtonEventArgs obj)
		{
			if (this._data == null)
			{
				return;
			}
			EntityUid owner = this._data.ToList<MapGridComponent>()[this.GridOptions.SelectedId].Owner;
			IConsoleHost consoleHost = IoCManager.Resolve<IClientConsoleHost>();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 1);
			defaultInterpolatedStringHandler.AppendLiteral("addatmos ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(owner);
			consoleHost.ExecuteCommand(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x1700060C RID: 1548
		// (get) Token: 0x06001D1A RID: 7450 RVA: 0x000A9602 File Offset: 0x000A7802
		private OptionButton GridOptions
		{
			get
			{
				return base.FindControl<OptionButton>("GridOptions");
			}
		}

		// Token: 0x1700060D RID: 1549
		// (get) Token: 0x06001D1B RID: 7451 RVA: 0x00091514 File Offset: 0x0008F714
		private Button SubmitButton
		{
			get
			{
				return base.FindControl<Button>("SubmitButton");
			}
		}

		// Token: 0x06001D1C RID: 7452 RVA: 0x000A960F File Offset: 0x000A780F
		public AddAtmosWindow()
		{
			AddAtmosWindow.!XamlIlPopulateTrampoline(this);
		}

		// Token: 0x06001D1E RID: 7454 RVA: 0x000A9630 File Offset: 0x000A7830
		static void xaml(IServiceProvider A_0, DefaultWindow A_1)
		{
			XamlIlContext.Context<DefaultWindow> context = new XamlIlContext.Context<DefaultWindow>(A_0, null, "resm:Content.Client.Administration.UI.Tabs.AtmosTab.AddAtmosWindow.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Title = (string)new LocExtension("Add Atmos").ProvideValue();
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
			Button button = new Button();
			button.Name = "SubmitButton";
			control = button;
			context.RobustNameScope.Register("SubmitButton", control);
			button.Text = (string)new LocExtension("Add Atmos").ProvideValue();
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

		// Token: 0x06001D1F RID: 7455 RVA: 0x000A983A File Offset: 0x000A7A3A
		private static void !XamlIlPopulateTrampoline(AddAtmosWindow A_0)
		{
			AddAtmosWindow.Populate:Content.Client.Administration.UI.Tabs.AtmosTab.AddAtmosWindow.xaml(null, A_0);
		}

		// Token: 0x04000E8E RID: 3726
		[Nullable(new byte[]
		{
			2,
			1
		})]
		private IEnumerable<MapGridComponent> _data;
	}
}
