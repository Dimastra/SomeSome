﻿using System;
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
using Robust.Shared.Maths;

namespace Content.Client.Administration.UI.Tabs.AdminbusTab
{
	// Token: 0x020004B0 RID: 1200
	[GenerateTypedNameReferences]
	public sealed class LoadBlueprintsWindow : DefaultWindow
	{
		// Token: 0x06001DE2 RID: 7650 RVA: 0x000AF0B4 File Offset: 0x000AD2B4
		public LoadBlueprintsWindow()
		{
			LoadBlueprintsWindow.!XamlIlPopulateTrampoline(this);
		}

		// Token: 0x06001DE3 RID: 7651 RVA: 0x000AF0C4 File Offset: 0x000AD2C4
		protected override void EnteredTree()
		{
			foreach (MapId mapId in IoCManager.Resolve<IMapManager>().GetAllMapIds())
			{
				this.MapOptions.AddItem(mapId.ToString(), new int?((int)mapId));
			}
			this.Reset();
			this.MapOptions.OnItemSelected += this.OnOptionSelect;
			this.RotationSpin.ValueChanged += this.OnRotate;
			this.SubmitButton.OnPressed += this.OnSubmitButtonPressed;
			this.TeleportButton.OnPressed += this.OnTeleportButtonPressed;
			this.ResetButton.OnPressed += this.OnResetButtonPressed;
		}

		// Token: 0x06001DE4 RID: 7652 RVA: 0x000AF1AC File Offset: 0x000AD3AC
		private void Reset()
		{
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			LocalPlayer localPlayer = IoCManager.Resolve<IPlayerManager>().LocalPlayer;
			EntityUid? entityUid = (localPlayer != null) ? localPlayer.ControlledEntity : null;
			MapId mapId = MapId.Nullspace;
			Vector2 vector = Vector2.Zero;
			Angle angle = Angle.Zero;
			TransformComponent transformComponent;
			if (entityManager.TryGetComponent<TransformComponent>(entityUid, ref transformComponent))
			{
				mapId = transformComponent.MapID;
				vector = transformComponent.WorldPosition;
				TransformComponent transformComponent2;
				if (entityManager.TryGetComponent<TransformComponent>(transformComponent.GridUid, ref transformComponent2))
				{
					angle = transformComponent2.WorldRotation;
				}
				else
				{
					angle = transformComponent.WorldRotation - transformComponent.LocalRotation;
				}
			}
			if (mapId != MapId.Nullspace)
			{
				this.MapOptions.Select((int)mapId);
			}
			this.XCoordinate.Value = (int)vector.X;
			this.YCoordinate.Value = (int)vector.Y;
			this.RotationSpin.OverrideValue(this.Wraparound((int)angle.Degrees));
		}

		// Token: 0x06001DE5 RID: 7653 RVA: 0x000AF299 File Offset: 0x000AD499
		[NullableContext(1)]
		private void OnResetButtonPressed(BaseButton.ButtonEventArgs obj)
		{
			this.Reset();
		}

		// Token: 0x06001DE6 RID: 7654 RVA: 0x000AF2A4 File Offset: 0x000AD4A4
		[NullableContext(1)]
		private void OnRotate([Nullable(2)] object sender, ValueChangedEventArgs e)
		{
			int num = this.Wraparound(e.Value);
			if (e.Value == num)
			{
				return;
			}
			this.RotationSpin.OverrideValue(num);
		}

		// Token: 0x06001DE7 RID: 7655 RVA: 0x000AF2D4 File Offset: 0x000AD4D4
		private int Wraparound(int value)
		{
			int num = value % 360;
			if (num < 0)
			{
				num += 360;
			}
			return num;
		}

		// Token: 0x06001DE8 RID: 7656 RVA: 0x000AF2F6 File Offset: 0x000AD4F6
		[NullableContext(1)]
		private void OnOptionSelect(OptionButton.ItemSelectedEventArgs obj)
		{
			this.MapOptions.SelectId(obj.Id);
		}

		// Token: 0x06001DE9 RID: 7657 RVA: 0x000AF30C File Offset: 0x000AD50C
		[NullableContext(1)]
		private void OnTeleportButtonPressed(BaseButton.ButtonEventArgs obj)
		{
			IConsoleHost consoleHost = IoCManager.Resolve<IClientConsoleHost>();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 3);
			defaultInterpolatedStringHandler.AppendLiteral("tp ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(this.XCoordinate.Value);
			defaultInterpolatedStringHandler.AppendLiteral(" ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(this.YCoordinate.Value);
			defaultInterpolatedStringHandler.AppendLiteral(" ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(this.MapOptions.SelectedId);
			consoleHost.ExecuteCommand(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x06001DEA RID: 7658 RVA: 0x000AF390 File Offset: 0x000AD590
		[NullableContext(1)]
		private void OnSubmitButtonPressed(BaseButton.ButtonEventArgs obj)
		{
			if (this.MapPath.Text.Length == 0)
			{
				return;
			}
			IConsoleHost consoleHost = IoCManager.Resolve<IClientConsoleHost>();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 5);
			defaultInterpolatedStringHandler.AppendLiteral("loadbp ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(this.MapOptions.SelectedId);
			defaultInterpolatedStringHandler.AppendLiteral(" \"");
			defaultInterpolatedStringHandler.AppendFormatted(this.MapPath.Text);
			defaultInterpolatedStringHandler.AppendLiteral("\" ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(this.XCoordinate.Value);
			defaultInterpolatedStringHandler.AppendLiteral(" ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(this.YCoordinate.Value);
			defaultInterpolatedStringHandler.AppendLiteral(" ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(this.RotationSpin.Value);
			consoleHost.ExecuteCommand(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x1700066A RID: 1642
		// (get) Token: 0x06001DEB RID: 7659 RVA: 0x000AF461 File Offset: 0x000AD661
		private OptionButton MapOptions
		{
			get
			{
				return base.FindControl<OptionButton>("MapOptions");
			}
		}

		// Token: 0x1700066B RID: 1643
		// (get) Token: 0x06001DEC RID: 7660 RVA: 0x000AF46E File Offset: 0x000AD66E
		private LineEdit MapPath
		{
			get
			{
				return base.FindControl<LineEdit>("MapPath");
			}
		}

		// Token: 0x1700066C RID: 1644
		// (get) Token: 0x06001DED RID: 7661 RVA: 0x000AF47B File Offset: 0x000AD67B
		private SpinBox XCoordinate
		{
			get
			{
				return base.FindControl<SpinBox>("XCoordinate");
			}
		}

		// Token: 0x1700066D RID: 1645
		// (get) Token: 0x06001DEE RID: 7662 RVA: 0x000AF488 File Offset: 0x000AD688
		private SpinBox YCoordinate
		{
			get
			{
				return base.FindControl<SpinBox>("YCoordinate");
			}
		}

		// Token: 0x1700066E RID: 1646
		// (get) Token: 0x06001DEF RID: 7663 RVA: 0x000AF495 File Offset: 0x000AD695
		private SpinBox RotationSpin
		{
			get
			{
				return base.FindControl<SpinBox>("RotationSpin");
			}
		}

		// Token: 0x1700066F RID: 1647
		// (get) Token: 0x06001DF0 RID: 7664 RVA: 0x00091514 File Offset: 0x0008F714
		private Button SubmitButton
		{
			get
			{
				return base.FindControl<Button>("SubmitButton");
			}
		}

		// Token: 0x17000670 RID: 1648
		// (get) Token: 0x06001DF1 RID: 7665 RVA: 0x000AF4A2 File Offset: 0x000AD6A2
		private Button TeleportButton
		{
			get
			{
				return base.FindControl<Button>("TeleportButton");
			}
		}

		// Token: 0x17000671 RID: 1649
		// (get) Token: 0x06001DF2 RID: 7666 RVA: 0x0004BA91 File Offset: 0x00049C91
		private Button ResetButton
		{
			get
			{
				return base.FindControl<Button>("ResetButton");
			}
		}

		// Token: 0x06001DF3 RID: 7667 RVA: 0x000AF4B0 File Offset: 0x000AD6B0
		static void xaml(IServiceProvider A_0, DefaultWindow A_1)
		{
			XamlIlContext.Context<DefaultWindow> context = new XamlIlContext.Context<DefaultWindow>(A_0, null, "resm:Content.Client.Administration.UI.Tabs.AdminbusTab.LoadBlueprintsWindow.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Title = (string)new LocExtension("Load Blueprint").ProvideValue();
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 0;
			Control control = new Label
			{
				Text = (string)new LocExtension("Map").ProvideValue(),
				MinSize = new Vector2(100f, 0f)
			};
			boxContainer2.XamlChildren.Add(control);
			control = new Control
			{
				MinSize = new Vector2(50f, 0f)
			};
			boxContainer2.XamlChildren.Add(control);
			OptionButton optionButton = new OptionButton();
			optionButton.Name = "MapOptions";
			control = optionButton;
			context.RobustNameScope.Register("MapOptions", control);
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
				Text = (string)new LocExtension("Path").ProvideValue(),
				MinSize = new Vector2(100f, 0f)
			};
			boxContainer3.XamlChildren.Add(control);
			control = new Control
			{
				MinSize = new Vector2(50f, 0f)
			};
			boxContainer3.XamlChildren.Add(control);
			LineEdit lineEdit = new LineEdit();
			lineEdit.Name = "MapPath";
			control = lineEdit;
			context.RobustNameScope.Register("MapPath", control);
			lineEdit.MinSize = new Vector2(200f, 0f);
			lineEdit.HorizontalExpand = true;
			lineEdit.Text = "/Maps/";
			control = lineEdit;
			boxContainer3.XamlChildren.Add(control);
			control = boxContainer3;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer4 = new BoxContainer();
			boxContainer4.Orientation = 0;
			control = new Label
			{
				Text = (string)new LocExtension("X").ProvideValue(),
				MinSize = new Vector2(100f, 0f)
			};
			boxContainer4.XamlChildren.Add(control);
			control = new Control
			{
				MinSize = new Vector2(50f, 0f)
			};
			boxContainer4.XamlChildren.Add(control);
			SpinBox spinBox = new SpinBox();
			spinBox.Name = "XCoordinate";
			control = spinBox;
			context.RobustNameScope.Register("XCoordinate", control);
			spinBox.MinSize = new Vector2(100f, 0f);
			spinBox.HorizontalExpand = true;
			control = spinBox;
			boxContainer4.XamlChildren.Add(control);
			control = boxContainer4;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer5 = new BoxContainer();
			boxContainer5.Orientation = 0;
			control = new Label
			{
				Text = (string)new LocExtension("Y").ProvideValue(),
				MinSize = new Vector2(100f, 0f)
			};
			boxContainer5.XamlChildren.Add(control);
			control = new Control
			{
				MinSize = new Vector2(50f, 0f)
			};
			boxContainer5.XamlChildren.Add(control);
			SpinBox spinBox2 = new SpinBox();
			spinBox2.Name = "YCoordinate";
			control = spinBox2;
			context.RobustNameScope.Register("YCoordinate", control);
			spinBox2.MinSize = new Vector2(100f, 0f);
			spinBox2.HorizontalExpand = true;
			control = spinBox2;
			boxContainer5.XamlChildren.Add(control);
			control = boxContainer5;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer6 = new BoxContainer();
			boxContainer6.Orientation = 0;
			control = new Label
			{
				Text = (string)new LocExtension("Rotation").ProvideValue(),
				MinSize = new Vector2(100f, 0f)
			};
			boxContainer6.XamlChildren.Add(control);
			control = new Control
			{
				MinSize = new Vector2(50f, 0f)
			};
			boxContainer6.XamlChildren.Add(control);
			SpinBox spinBox3 = new SpinBox();
			spinBox3.Name = "RotationSpin";
			control = spinBox3;
			context.RobustNameScope.Register("RotationSpin", control);
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
			button.Text = (string)new LocExtension("Load Blueprint").ProvideValue();
			control = button;
			boxContainer.XamlChildren.Add(control);
			Button button2 = new Button();
			button2.Name = "TeleportButton";
			control = button2;
			context.RobustNameScope.Register("TeleportButton", control);
			button2.Text = (string)new LocExtension("Teleport to").ProvideValue();
			control = button2;
			boxContainer.XamlChildren.Add(control);
			Button button3 = new Button();
			button3.Name = "ResetButton";
			control = button3;
			context.RobustNameScope.Register("ResetButton", control);
			button3.Text = (string)new LocExtension("Reset to default").ProvideValue();
			control = button3;
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

		// Token: 0x06001DF4 RID: 7668 RVA: 0x000AFB5B File Offset: 0x000ADD5B
		private static void !XamlIlPopulateTrampoline(LoadBlueprintsWindow A_0)
		{
			LoadBlueprintsWindow.Populate:Content.Client.Administration.UI.Tabs.AdminbusTab.LoadBlueprintsWindow.xaml(null, A_0);
		}
	}
}
