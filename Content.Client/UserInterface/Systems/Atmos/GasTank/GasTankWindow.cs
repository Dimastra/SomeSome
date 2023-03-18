using System;
using System.Runtime.CompilerServices;
using Content.Client.Message;
using Content.Client.Resources;
using Content.Client.Stylesheets;
using Content.Shared.Atmos.Components;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Systems.Atmos.GasTank
{
	// Token: 0x020000B7 RID: 183
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GasTankWindow : BaseWindow
	{
		// Token: 0x060004F2 RID: 1266 RVA: 0x0001B2D4 File Offset: 0x000194D4
		public GasTankWindow(GasTankBoundUserInterface owner)
		{
			this._resourceCache = IoCManager.Resolve<IResourceCache>();
			this._owner = owner;
			LayoutContainer layoutContainer = new LayoutContainer
			{
				Name = "GasTankRoot"
			};
			base.AddChild(layoutContainer);
			base.MouseFilter = 0;
			Texture texture = this._resourceCache.GetTexture("/Textures/Interface/Nano/button.svg.96dpi.png");
			StyleBoxTexture styleBoxTexture = new StyleBoxTexture
			{
				Texture = texture,
				Modulate = Color.FromHex("#252525", null)
			};
			styleBoxTexture.SetPatchMargin(15, 10f);
			PanelContainer panelContainer = new PanelContainer
			{
				PanelOverride = styleBoxTexture,
				MouseFilter = 1
			};
			LayoutContainer layoutContainer2 = new LayoutContainer
			{
				Name = "BottomWrap"
			};
			layoutContainer.AddChild(panelContainer);
			layoutContainer.AddChild(layoutContainer2);
			LayoutContainer.SetAnchorPreset(panelContainer, 15, false);
			LayoutContainer.SetMarginBottom(panelContainer, -85f);
			LayoutContainer.SetAnchorPreset(layoutContainer2, 13, false);
			LayoutContainer.SetGrowHorizontal(layoutContainer2, 2);
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			Control.OrderedChildCollection children = boxContainer.Children;
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.Orientation = 1;
			BoxContainer boxContainer3 = boxContainer2;
			this._topContainer = boxContainer2;
			children.Add(boxContainer3);
			boxContainer.Children.Add(new Control
			{
				MinSize = new ValueTuple<float, float>(0f, 110f)
			});
			BoxContainer boxContainer4 = boxContainer;
			layoutContainer.AddChild(boxContainer4);
			LayoutContainer.SetAnchorPreset(boxContainer4, 15, false);
			Font font = this._resourceCache.GetFont("/Fonts/Boxfont-round/Boxfont Round.ttf", 13);
			BoxContainer boxContainer5 = new BoxContainer();
			boxContainer5.Orientation = 0;
			boxContainer5.Margin = new Thickness(4f, 2f, 12f, 2f);
			Control.OrderedChildCollection children2 = boxContainer5.Children;
			Label label = new Label();
			label.Text = Loc.GetString("gas-tank-window-label");
			label.FontOverride = font;
			label.FontColorOverride = new Color?(StyleNano.NanoGold);
			label.VerticalAlignment = 2;
			label.HorizontalExpand = true;
			label.HorizontalAlignment = 1;
			label.Margin = new Thickness(0f, 0f, 20f, 0f);
			Label label2 = label;
			this._lblName = label;
			children2.Add(label2);
			Control.OrderedChildCollection children3 = boxContainer5.Children;
			TextureButton textureButton = new TextureButton();
			textureButton.StyleClasses.Add("windowCloseButton");
			textureButton.VerticalAlignment = 2;
			TextureButton textureButton2 = textureButton;
			children3.Add(textureButton);
			BoxContainer boxContainer6 = boxContainer5;
			PanelContainer panelContainer2 = new PanelContainer();
			panelContainer2.PanelOverride = new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex("#202020", null)
			};
			Control.OrderedChildCollection children4 = panelContainer2.Children;
			BoxContainer boxContainer7 = new BoxContainer();
			boxContainer7.Orientation = 1;
			boxContainer7.Margin = new Thickness(8f, 4f);
			Control control = boxContainer7;
			this._contentContainer = boxContainer7;
			children4.Add(control);
			PanelContainer panelContainer3 = panelContainer2;
			this._topContainer.AddChild(boxContainer6);
			this._topContainer.AddChild(new PanelContainer
			{
				MinSize = new ValueTuple<float, float>(0f, 2f),
				PanelOverride = new StyleBoxFlat
				{
					BackgroundColor = Color.FromHex("#525252ff", null)
				}
			});
			this._topContainer.AddChild(panelContainer3);
			this._topContainer.AddChild(new PanelContainer
			{
				MinSize = new ValueTuple<float, float>(0f, 2f),
				PanelOverride = new StyleBoxFlat
				{
					BackgroundColor = Color.FromHex("#525252ff", null)
				}
			});
			this._lblPressure = new RichTextLabel();
			this._contentContainer.AddChild(this._lblPressure);
			this._lblInternals = new RichTextLabel
			{
				MinSize = new ValueTuple<float, float>(200f, 0f),
				VerticalAlignment = 2
			};
			this._btnInternals = new Button
			{
				Text = Loc.GetString("gas-tank-window-internals-toggle-button")
			};
			Control contentContainer = this._contentContainer;
			BoxContainer boxContainer8 = new BoxContainer();
			boxContainer8.Orientation = 0;
			boxContainer8.Margin = new Thickness(0f, 7f, 0f, 0f);
			boxContainer8.Children.Add(this._lblInternals);
			boxContainer8.Children.Add(this._btnInternals);
			contentContainer.AddChild(boxContainer8);
			this._contentContainer.AddChild(new Control
			{
				MinSize = new Vector2(0f, 10f)
			});
			this._contentContainer.AddChild(new Label
			{
				Text = Loc.GetString("gas-tank-window-output-pressure-label"),
				Align = 1
			});
			FloatSpinBox floatSpinBox = new FloatSpinBox();
			floatSpinBox.IsValid = ((float f) => f >= 0f || f <= 3000f);
			floatSpinBox.Margin = new Thickness(25f, 0f, 25f, 7f);
			this._spbPressure = floatSpinBox;
			this._contentContainer.AddChild(this._spbPressure);
			this._spbPressure.OnValueChanged += delegate(FloatSpinBox.FloatSpinBoxEventArgs args)
			{
				GasTankBoundUserInterface owner2 = this._owner;
				float value = args.Value;
				owner2.SetOutputPressure(value);
			};
			this._btnInternals.OnPressed += delegate(BaseButton.ButtonEventArgs args)
			{
				this._owner.ToggleInternals();
			};
			textureButton2.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.Close();
			};
		}

		// Token: 0x060004F3 RID: 1267 RVA: 0x0001B7E0 File Offset: 0x000199E0
		public void UpdateState(GasTankBoundUserInterfaceState state)
		{
			RichTextLabel lblPressure = this._lblPressure;
			string text = "gas-tank-window-tank-pressure-text";
			ValueTuple<string, object>[] array = new ValueTuple<string, object>[1];
			int num = 0;
			string item = "tankPressure";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<float>(state.TankPressure, "0.##");
			array[num] = new ValueTuple<string, object>(item, defaultInterpolatedStringHandler.ToStringAndClear());
			lblPressure.SetMarkup(Loc.GetString(text, array));
			this._btnInternals.Disabled = !state.CanConnectInternals;
			this._lblInternals.SetMarkup(Loc.GetString("gas-tank-window-internal-text", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("status", Loc.GetString(state.InternalsConnected ? "gas-tank-window-internal-connected" : "gas-tank-window-internal-disconnected"))
			}));
			if (state.OutputPressure != null)
			{
				this._spbPressure.Value = state.OutputPressure.Value;
			}
		}

		// Token: 0x060004F4 RID: 1268 RVA: 0x00003C56 File Offset: 0x00001E56
		protected override BaseWindow.DragMode GetDragModeFor(Vector2 relativeMousePos)
		{
			return 1;
		}

		// Token: 0x060004F5 RID: 1269 RVA: 0x00003C59 File Offset: 0x00001E59
		protected override bool HasPoint(Vector2 point)
		{
			return false;
		}

		// Token: 0x04000248 RID: 584
		private GasTankBoundUserInterface _owner;

		// Token: 0x04000249 RID: 585
		private readonly Label _lblName;

		// Token: 0x0400024A RID: 586
		private readonly BoxContainer _topContainer;

		// Token: 0x0400024B RID: 587
		private readonly Control _contentContainer;

		// Token: 0x0400024C RID: 588
		private readonly IResourceCache _resourceCache;

		// Token: 0x0400024D RID: 589
		private readonly RichTextLabel _lblPressure;

		// Token: 0x0400024E RID: 590
		private readonly FloatSpinBox _spbPressure;

		// Token: 0x0400024F RID: 591
		private readonly RichTextLabel _lblInternals;

		// Token: 0x04000250 RID: 592
		private readonly Button _btnInternals;
	}
}
