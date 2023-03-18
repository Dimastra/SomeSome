using System;
using System.Runtime.CompilerServices;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Controls
{
	// Token: 0x020000DF RID: 223
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MenuButton : ContainerButton
	{
		// Token: 0x1700010A RID: 266
		// (get) Token: 0x06000640 RID: 1600 RVA: 0x0002192B File Offset: 0x0001FB2B
		private Color NormalColor
		{
			get
			{
				if (!base.HasStyleClass("topButtonLabel"))
				{
					return MenuButton.ColorNormal;
				}
				return MenuButton.ColorRedNormal;
			}
		}

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x06000641 RID: 1601 RVA: 0x00021945 File Offset: 0x0001FB45
		private Color HoveredColor
		{
			get
			{
				if (!base.HasStyleClass("topButtonLabel"))
				{
					return MenuButton.ColorHovered;
				}
				return MenuButton.ColorRedHovered;
			}
		}

		// Token: 0x1700010C RID: 268
		// (set) Token: 0x06000642 RID: 1602 RVA: 0x0002195F File Offset: 0x0001FB5F
		public string AppendStyleClass
		{
			set
			{
				base.AddStyleClass(value);
			}
		}

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x06000643 RID: 1603 RVA: 0x00021968 File Offset: 0x0001FB68
		// (set) Token: 0x06000644 RID: 1604 RVA: 0x00021975 File Offset: 0x0001FB75
		[Nullable(2)]
		public Texture Icon
		{
			[NullableContext(2)]
			get
			{
				return this._buttonIcon.Texture;
			}
			[NullableContext(2)]
			set
			{
				this._buttonIcon.Texture = value;
			}
		}

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x06000645 RID: 1605 RVA: 0x00021983 File Offset: 0x0001FB83
		// (set) Token: 0x06000646 RID: 1606 RVA: 0x0002198B File Offset: 0x0001FB8B
		public BoundKeyFunction BoundKey
		{
			get
			{
				return this._function;
			}
			set
			{
				this._function = value;
				this._buttonLabel.Text = BoundKeyHelper.ShortKeyName(value);
			}
		}

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06000647 RID: 1607 RVA: 0x000219A5 File Offset: 0x0001FBA5
		public BoxContainer ButtonRoot
		{
			get
			{
				return this._root;
			}
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x000219B0 File Offset: 0x0001FBB0
		public MenuButton()
		{
			IoCManager.InjectDependencies<MenuButton>(this);
			base.TooltipDelay = new float?(0.4f);
			this._buttonIcon = new TextureRect
			{
				TextureScale = new ValueTuple<float, float>(0.5f, 0.5f),
				HorizontalAlignment = 2,
				VerticalAlignment = 2,
				VerticalExpand = true,
				Margin = new Thickness(0f, 8f),
				ModulateSelfOverride = new Color?(this.NormalColor),
				Stretch = 4
			};
			this._buttonLabel = new Label
			{
				Text = "",
				HorizontalAlignment = 2,
				ModulateSelfOverride = new Color?(this.NormalColor),
				StyleClasses = 
				{
					"topButtonLabel"
				}
			};
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			boxContainer.Children.Add(this._buttonIcon);
			boxContainer.Children.Add(this._buttonLabel);
			this._root = boxContainer;
			base.AddChild(this._root);
			base.ToggleMode = true;
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x00021AC8 File Offset: 0x0001FCC8
		protected override void EnteredTree()
		{
			this._inputManager.OnKeyBindingAdded += this.OnKeyBindingChanged;
			this._inputManager.OnKeyBindingRemoved += this.OnKeyBindingChanged;
			this._inputManager.OnInputModeChanged += this.OnKeyBindingChanged;
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x00021B1C File Offset: 0x0001FD1C
		protected override void ExitedTree()
		{
			this._inputManager.OnKeyBindingAdded -= this.OnKeyBindingChanged;
			this._inputManager.OnKeyBindingRemoved -= this.OnKeyBindingChanged;
			this._inputManager.OnInputModeChanged -= this.OnKeyBindingChanged;
		}

		// Token: 0x0600064B RID: 1611 RVA: 0x00021B6E File Offset: 0x0001FD6E
		private void OnKeyBindingChanged(IKeyBinding obj)
		{
			this._buttonLabel.Text = BoundKeyHelper.ShortKeyName(this._function);
		}

		// Token: 0x0600064C RID: 1612 RVA: 0x00021B6E File Offset: 0x0001FD6E
		private void OnKeyBindingChanged()
		{
			this._buttonLabel.Text = BoundKeyHelper.ShortKeyName(this._function);
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x00021B86 File Offset: 0x0001FD86
		protected override void StylePropertiesChanged()
		{
			base.StylePropertiesChanged();
			this.UpdateChildColors();
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x00021B94 File Offset: 0x0001FD94
		private void UpdateChildColors()
		{
			if (this._buttonIcon == null || this._buttonLabel == null)
			{
				return;
			}
			switch (base.DrawMode)
			{
			case 0:
				this._buttonIcon.ModulateSelfOverride = new Color?(this.NormalColor);
				this._buttonLabel.ModulateSelfOverride = new Color?(this.NormalColor);
				return;
			case 1:
				this._buttonIcon.ModulateSelfOverride = new Color?(MenuButton.ColorPressed);
				this._buttonLabel.ModulateSelfOverride = new Color?(MenuButton.ColorPressed);
				return;
			case 2:
				this._buttonIcon.ModulateSelfOverride = new Color?(this.HoveredColor);
				this._buttonLabel.ModulateSelfOverride = new Color?(this.HoveredColor);
				break;
			case 3:
				break;
			default:
				return;
			}
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x00021C54 File Offset: 0x0001FE54
		protected override void DrawModeChanged()
		{
			base.DrawModeChanged();
			this.UpdateChildColors();
		}

		// Token: 0x040002C9 RID: 713
		[Dependency]
		private readonly IInputManager _inputManager;

		// Token: 0x040002CA RID: 714
		public const string StyleClassLabelTopButton = "topButtonLabel";

		// Token: 0x040002CB RID: 715
		public const string StyleClassRedTopButton = "topButtonLabel";

		// Token: 0x040002CC RID: 716
		private const float CustomTooltipDelay = 0.4f;

		// Token: 0x040002CD RID: 717
		private static readonly Color ColorNormal = Color.FromHex("#7b7e7e", null);

		// Token: 0x040002CE RID: 718
		private static readonly Color ColorRedNormal = Color.FromHex("#FEFEFE", null);

		// Token: 0x040002CF RID: 719
		private static readonly Color ColorHovered = Color.FromHex("#969999", null);

		// Token: 0x040002D0 RID: 720
		private static readonly Color ColorRedHovered = Color.FromHex("#FFFFFF", null);

		// Token: 0x040002D1 RID: 721
		private static readonly Color ColorPressed = Color.FromHex("#789B8C", null);

		// Token: 0x040002D2 RID: 722
		private const float VertPad = 8f;

		// Token: 0x040002D3 RID: 723
		private BoundKeyFunction _function;

		// Token: 0x040002D4 RID: 724
		private readonly BoxContainer _root;

		// Token: 0x040002D5 RID: 725
		[Nullable(2)]
		private readonly TextureRect _buttonIcon;

		// Token: 0x040002D6 RID: 726
		[Nullable(2)]
		private readonly Label _buttonLabel;
	}
}
