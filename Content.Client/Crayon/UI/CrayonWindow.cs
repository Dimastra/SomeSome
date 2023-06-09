﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Client.Stylesheets;
using Content.Shared.Crayon;
using Content.Shared.Decals;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.Utility;
using Robust.Shared.Maths;

namespace Content.Client.Crayon.UI
{
	// Token: 0x0200037B RID: 891
	[GenerateTypedNameReferences]
	public sealed class CrayonWindow : DefaultWindow
	{
		// Token: 0x17000466 RID: 1126
		// (get) Token: 0x060015DA RID: 5594 RVA: 0x00081271 File Offset: 0x0007F471
		[Nullable(1)]
		public CrayonBoundUserInterface Owner { [NullableContext(1)] get; }

		// Token: 0x060015DB RID: 5595 RVA: 0x0008127C File Offset: 0x0007F47C
		[NullableContext(1)]
		public CrayonWindow(CrayonBoundUserInterface owner)
		{
			CrayonWindow.!XamlIlPopulateTrampoline(this);
			this.Owner = owner;
			this.Search.OnTextChanged += delegate(LineEdit.LineEditEventArgs _)
			{
				this.RefreshList();
			};
			ColorSelectorSliders colorSelector = this.ColorSelector;
			colorSelector.OnColorChanged = (Action<Color>)Delegate.Combine(colorSelector.OnColorChanged, new Action<Color>(this.SelectColor));
		}

		// Token: 0x060015DC RID: 5596 RVA: 0x000812DA File Offset: 0x0007F4DA
		private void SelectColor(Color color)
		{
			this._color = color;
			this.Owner.SelectColor(color);
			this.RefreshList();
		}

		// Token: 0x060015DD RID: 5597 RVA: 0x000812F8 File Offset: 0x0007F4F8
		private void RefreshList()
		{
			this.Grid.RemoveAllChildren();
			if (this._decals == null)
			{
				return;
			}
			string text = this.Search.Text;
			foreach (KeyValuePair<string, Texture> keyValuePair in this._decals)
			{
				string text2;
				Texture texture;
				keyValuePair.Deconstruct(out text2, out texture);
				string text3 = text2;
				Texture textureNormal = texture;
				if (text3.Contains(text))
				{
					TextureButton textureButton = new TextureButton
					{
						TextureNormal = textureNormal,
						Name = text3,
						ToolTip = text3,
						Modulate = this._color
					};
					textureButton.OnPressed += this.ButtonOnPressed;
					if (this._selected == text3)
					{
						PanelContainer panelContainer = new PanelContainer();
						panelContainer.PanelOverride = new StyleBoxFlat
						{
							BackgroundColor = StyleNano.ButtonColorDefault
						};
						panelContainer.Children.Add(textureButton);
						PanelContainer panelContainer2 = panelContainer;
						this.Grid.AddChild(panelContainer2);
					}
					else
					{
						this.Grid.AddChild(textureButton);
					}
				}
			}
		}

		// Token: 0x060015DE RID: 5598 RVA: 0x00081418 File Offset: 0x0007F618
		[NullableContext(1)]
		private void ButtonOnPressed(BaseButton.ButtonEventArgs obj)
		{
			if (obj.Button.Name == null)
			{
				return;
			}
			this.Owner.Select(obj.Button.Name);
			this._selected = obj.Button.Name;
			this.RefreshList();
		}

		// Token: 0x060015DF RID: 5599 RVA: 0x00081458 File Offset: 0x0007F658
		[NullableContext(1)]
		public void UpdateState(CrayonBoundUserInterfaceState state)
		{
			this._selected = state.Selected;
			this.ColorSelector.Visible = state.SelectableColor;
			this._color = state.Color;
			if (this.ColorSelector.Visible)
			{
				this.ColorSelector.Color = state.Color;
			}
			this.RefreshList();
		}

		// Token: 0x060015E0 RID: 5600 RVA: 0x000814B4 File Offset: 0x0007F6B4
		[NullableContext(1)]
		public void Populate(IEnumerable<DecalPrototype> prototypes)
		{
			this._decals = new Dictionary<string, Texture>();
			foreach (DecalPrototype decalPrototype in prototypes)
			{
				this._decals.Add(decalPrototype.ID, SpriteSpecifierExt.Frame0(decalPrototype.Sprite));
			}
			this.RefreshList();
		}

		// Token: 0x17000467 RID: 1127
		// (get) Token: 0x060015E1 RID: 5601 RVA: 0x00081524 File Offset: 0x0007F724
		private ColorSelectorSliders ColorSelector
		{
			get
			{
				return base.FindControl<ColorSelectorSliders>("ColorSelector");
			}
		}

		// Token: 0x17000468 RID: 1128
		// (get) Token: 0x060015E2 RID: 5602 RVA: 0x0007D5D4 File Offset: 0x0007B7D4
		private LineEdit Search
		{
			get
			{
				return base.FindControl<LineEdit>("Search");
			}
		}

		// Token: 0x17000469 RID: 1129
		// (get) Token: 0x060015E3 RID: 5603 RVA: 0x0007D5E1 File Offset: 0x0007B7E1
		private GridContainer Grid
		{
			get
			{
				return base.FindControl<GridContainer>("Grid");
			}
		}

		// Token: 0x060015E5 RID: 5605 RVA: 0x0008153C File Offset: 0x0007F73C
		static void xaml(IServiceProvider A_0, DefaultWindow A_1)
		{
			XamlIlContext.Context<DefaultWindow> context = new XamlIlContext.Context<DefaultWindow>(A_0, null, "resm:Content.Client.Crayon.UI.CrayonWindow.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			A_1.Title = (string)new LocExtension("crayon-window-title").ProvideValue();
			A_1.MinSize = new Vector2(250f, 300f);
			A_1.SetSize = new Vector2(250f, 300f);
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			ColorSelectorSliders colorSelectorSliders = new ColorSelectorSliders();
			colorSelectorSliders.Name = "ColorSelector";
			Control control = colorSelectorSliders;
			context.RobustNameScope.Register("ColorSelector", control);
			colorSelectorSliders.Visible = false;
			control = colorSelectorSliders;
			boxContainer.XamlChildren.Add(control);
			LineEdit lineEdit = new LineEdit();
			lineEdit.Name = "Search";
			control = lineEdit;
			context.RobustNameScope.Register("Search", control);
			control = lineEdit;
			boxContainer.XamlChildren.Add(control);
			ScrollContainer scrollContainer = new ScrollContainer();
			scrollContainer.VerticalExpand = true;
			GridContainer gridContainer = new GridContainer();
			gridContainer.Name = "Grid";
			control = gridContainer;
			context.RobustNameScope.Register("Grid", control);
			gridContainer.Columns = 6;
			control = gridContainer;
			scrollContainer.XamlChildren.Add(control);
			control = scrollContainer;
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

		// Token: 0x060015E6 RID: 5606 RVA: 0x00081713 File Offset: 0x0007F913
		private static void !XamlIlPopulateTrampoline(CrayonWindow A_0)
		{
			CrayonWindow.Populate:Content.Client.Crayon.UI.CrayonWindow.xaml(null, A_0);
		}

		// Token: 0x04000B6E RID: 2926
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		private Dictionary<string, Texture> _decals;

		// Token: 0x04000B6F RID: 2927
		[Nullable(2)]
		private string _selected;

		// Token: 0x04000B70 RID: 2928
		private Color _color;
	}
}
