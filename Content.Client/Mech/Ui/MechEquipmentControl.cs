﻿using System;
using System.Runtime.CompilerServices;
using CompiledRobustXaml;
using Content.Client.Administration.UI.CustomControls;
using Robust.Client.AutoGenerated;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client.Mech.Ui
{
	// Token: 0x0200023F RID: 575
	[GenerateTypedNameReferences]
	public sealed class MechEquipmentControl : Control
	{
		// Token: 0x14000051 RID: 81
		// (add) Token: 0x06000E94 RID: 3732 RVA: 0x00057E40 File Offset: 0x00056040
		// (remove) Token: 0x06000E95 RID: 3733 RVA: 0x00057E78 File Offset: 0x00056078
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action OnRemoveButtonPressed;

		// Token: 0x06000E96 RID: 3734 RVA: 0x00057EB0 File Offset: 0x000560B0
		[NullableContext(2)]
		public MechEquipmentControl([Nullable(1)] string itemName, SpriteComponent sprite, Control fragment)
		{
			MechEquipmentControl.!XamlIlPopulateTrampoline(this);
			this.EquipmentName.SetMessage(itemName);
			this.EquipmentView.Sprite = sprite;
			this.RemoveButton.TexturePath = "/Textures/Interface/Nano/cross.svg.png";
			if (fragment != null)
			{
				this.Separator.Visible = true;
				this.CustomControlContainer.AddChild(fragment);
			}
			this.RemoveButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				Action onRemoveButtonPressed = this.OnRemoveButtonPressed;
				if (onRemoveButtonPressed == null)
				{
					return;
				}
				onRemoveButtonPressed();
			};
		}

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x06000E97 RID: 3735 RVA: 0x00057F23 File Offset: 0x00056123
		private SpriteView EquipmentView
		{
			get
			{
				return base.FindControl<SpriteView>("EquipmentView");
			}
		}

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x06000E98 RID: 3736 RVA: 0x00057F30 File Offset: 0x00056130
		private RichTextLabel EquipmentName
		{
			get
			{
				return base.FindControl<RichTextLabel>("EquipmentName");
			}
		}

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x06000E99 RID: 3737 RVA: 0x00057F3D File Offset: 0x0005613D
		private TextureButton RemoveButton
		{
			get
			{
				return base.FindControl<TextureButton>("RemoveButton");
			}
		}

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x06000E9A RID: 3738 RVA: 0x00057F4A File Offset: 0x0005614A
		private HSeparator Separator
		{
			get
			{
				return base.FindControl<HSeparator>("Separator");
			}
		}

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x06000E9B RID: 3739 RVA: 0x00057F57 File Offset: 0x00056157
		private BoxContainer CustomControlContainer
		{
			get
			{
				return base.FindControl<BoxContainer>("CustomControlContainer");
			}
		}

		// Token: 0x06000E9D RID: 3741 RVA: 0x00057F78 File Offset: 0x00056178
		static void xaml(IServiceProvider A_0, Control A_1)
		{
			XamlIlContext.Context<Control> context = new XamlIlContext.Context<Control>(A_0, null, "resm:Content.Client.Mech.Ui.MechEquipmentControl.xaml?assembly=Content.Client");
			context.RootObject = A_1;
			context.IntermediateRoot = A_1;
			Button button = new Button();
			button.Disabled = true;
			button.Margin = new Thickness(0f, 0f, 0f, 0f);
			button.HorizontalExpand = true;
			button.VerticalExpand = true;
			string item = "ButtonSquare";
			button.StyleClasses.Add(item);
			BoxContainer boxContainer = new BoxContainer();
			boxContainer.Orientation = 1;
			boxContainer.HorizontalExpand = true;
			boxContainer.VerticalExpand = true;
			boxContainer.VerticalAlignment = 1;
			BoxContainer boxContainer2 = new BoxContainer();
			boxContainer2.HorizontalExpand = true;
			boxContainer2.VerticalExpand = true;
			boxContainer2.Orientation = 0;
			SpriteView spriteView = new SpriteView();
			spriteView.Name = "EquipmentView";
			Control control = spriteView;
			context.RobustNameScope.Register("EquipmentView", control);
			spriteView.OverrideDirection = new Direction?(0);
			spriteView.MinSize = new Vector2(32f, 32f);
			spriteView.SetSize = new Vector2(32f, 32f);
			spriteView.Scale = new Vector2(1f, 1f);
			spriteView.RectClipContent = true;
			control = spriteView;
			boxContainer2.XamlChildren.Add(control);
			RichTextLabel richTextLabel = new RichTextLabel();
			richTextLabel.Name = "EquipmentName";
			control = richTextLabel;
			context.RobustNameScope.Register("EquipmentName", control);
			richTextLabel.VerticalAlignment = 2;
			control = richTextLabel;
			boxContainer2.XamlChildren.Add(control);
			BoxContainer boxContainer3 = new BoxContainer();
			boxContainer3.Orientation = 1;
			boxContainer3.HorizontalExpand = true;
			boxContainer3.HorizontalAlignment = 3;
			boxContainer3.VerticalAlignment = 2;
			TextureButton textureButton = new TextureButton();
			textureButton.Name = "RemoveButton";
			control = textureButton;
			context.RobustNameScope.Register("RemoveButton", control);
			textureButton.Scale = new Vector2(0.5f, 0.5f);
			control = textureButton;
			boxContainer3.XamlChildren.Add(control);
			control = boxContainer3;
			boxContainer2.XamlChildren.Add(control);
			control = boxContainer2;
			boxContainer.XamlChildren.Add(control);
			HSeparator hseparator = new HSeparator();
			hseparator.Name = "Separator";
			control = hseparator;
			context.RobustNameScope.Register("Separator", control);
			item = "LowDivider";
			hseparator.StyleClasses.Add(item);
			hseparator.Visible = false;
			control = hseparator;
			boxContainer.XamlChildren.Add(control);
			BoxContainer boxContainer4 = new BoxContainer();
			boxContainer4.Name = "CustomControlContainer";
			control = boxContainer4;
			context.RobustNameScope.Register("CustomControlContainer", control);
			boxContainer4.Margin = new Thickness(0f, 10f, 0f, 0f);
			boxContainer4.HorizontalExpand = true;
			boxContainer4.VerticalExpand = true;
			control = boxContainer4;
			boxContainer.XamlChildren.Add(control);
			control = boxContainer;
			button.XamlChildren.Add(control);
			control = button;
			A_1.XamlChildren.Add(control);
			if ((control = (A_1 as Control)) != null)
			{
				context.RobustNameScope.Absorb(control.NameScope);
				control.NameScope = context.RobustNameScope;
			}
			context.RobustNameScope.Complete();
		}

		// Token: 0x06000E9E RID: 3742 RVA: 0x0005832E File Offset: 0x0005652E
		private static void !XamlIlPopulateTrampoline(MechEquipmentControl A_0)
		{
			MechEquipmentControl.Populate:Content.Client.Mech.Ui.MechEquipmentControl.xaml(null, A_0);
		}
	}
}
