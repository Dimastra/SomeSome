using System;
using System.Runtime.CompilerServices;
using Content.Client.Actions.UI;
using Content.Client.Cooldown;
using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.Utility;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.Systems.Actions.Controls
{
	// Token: 0x020000C9 RID: 201
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ActionButton : Control
	{
		// Token: 0x170000DB RID: 219
		// (get) Token: 0x0600058A RID: 1418 RVA: 0x0001E765 File Offset: 0x0001C965
		private ActionUIController Controller
		{
			get
			{
				return base.UserInterfaceManager.GetUIController<ActionUIController>();
			}
		}

		// Token: 0x170000DC RID: 220
		// (set) Token: 0x0600058B RID: 1419 RVA: 0x0001E772 File Offset: 0x0001C972
		public BoundKeyFunction? KeyBind
		{
			set
			{
				this._keybind = value;
				if (this._keybind != null)
				{
					this.Label.Text = BoundKeyHelper.ShortKeyName(this._keybind.Value);
				}
			}
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x0600058C RID: 1420 RVA: 0x0001E7A3 File Offset: 0x0001C9A3
		// (set) Token: 0x0600058D RID: 1421 RVA: 0x0001E7AB File Offset: 0x0001C9AB
		[Nullable(2)]
		public ActionType Action { [NullableContext(2)] get; [NullableContext(2)] private set; }

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x0600058E RID: 1422 RVA: 0x0001E7B4 File Offset: 0x0001C9B4
		// (set) Token: 0x0600058F RID: 1423 RVA: 0x0001E7BC File Offset: 0x0001C9BC
		public bool Locked { get; set; }

		// Token: 0x14000027 RID: 39
		// (add) Token: 0x06000590 RID: 1424 RVA: 0x0001E7C8 File Offset: 0x0001C9C8
		// (remove) Token: 0x06000591 RID: 1425 RVA: 0x0001E800 File Offset: 0x0001CA00
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		public event Action<GUIBoundKeyEventArgs, ActionButton> ActionPressed;

		// Token: 0x14000028 RID: 40
		// (add) Token: 0x06000592 RID: 1426 RVA: 0x0001E838 File Offset: 0x0001CA38
		// (remove) Token: 0x06000593 RID: 1427 RVA: 0x0001E870 File Offset: 0x0001CA70
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		public event Action<GUIBoundKeyEventArgs, ActionButton> ActionUnpressed;

		// Token: 0x14000029 RID: 41
		// (add) Token: 0x06000594 RID: 1428 RVA: 0x0001E8A8 File Offset: 0x0001CAA8
		// (remove) Token: 0x06000595 RID: 1429 RVA: 0x0001E8E0 File Offset: 0x0001CAE0
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<ActionButton> ActionFocusExited;

		// Token: 0x06000596 RID: 1430 RVA: 0x0001E918 File Offset: 0x0001CB18
		public ActionButton()
		{
			base.MouseFilter = 1;
			this.Button = new TextureRect
			{
				Name = "Button",
				TextureScale = new Vector2(2f, 2f)
			};
			this.HighlightRect = new PanelContainer
			{
				StyleClasses = 
				{
					"HandSlotHighlight"
				},
				MinSize = new ValueTuple<float, float>(32f, 32f),
				Visible = false
			};
			this._bigActionIcon = new TextureRect
			{
				HorizontalExpand = true,
				VerticalExpand = true,
				Stretch = 1,
				Visible = false
			};
			this._smallActionIcon = new TextureRect
			{
				HorizontalAlignment = 3,
				VerticalAlignment = 3,
				Stretch = 1,
				Visible = false
			};
			this.Label = new Label
			{
				Name = "Label",
				HorizontalAlignment = 1,
				VerticalAlignment = 1,
				Margin = new Thickness(5f, 0f, 0f, 0f)
			};
			this._bigItemSpriteView = new SpriteView
			{
				Name = "Big Sprite",
				HorizontalExpand = true,
				VerticalExpand = true,
				Scale = new ValueTuple<float, float>(2f, 2f),
				Visible = false,
				OverrideDirection = new Direction?(0)
			};
			this._smallItemSpriteView = new SpriteView
			{
				Name = "Small Sprite",
				HorizontalAlignment = 3,
				VerticalAlignment = 3,
				Visible = false,
				OverrideDirection = new Direction?(0)
			};
			BoxContainer boxContainer = new BoxContainer
			{
				Orientation = 0,
				HorizontalExpand = true,
				VerticalExpand = true,
				MinSize = new ValueTuple<float, float>(64f, 64f)
			};
			boxContainer.AddChild(new Control
			{
				MinSize = new ValueTuple<float, float>(32f, 32f)
			});
			Control control = boxContainer;
			Control control2 = new Control();
			control2.Children.Add(this._smallActionIcon);
			control2.Children.Add(this._smallItemSpriteView);
			control.AddChild(control2);
			this.Cooldown = new CooldownGraphic
			{
				Visible = false
			};
			base.AddChild(this._bigActionIcon);
			base.AddChild(this._bigItemSpriteView);
			base.AddChild(this.Button);
			base.AddChild(this.HighlightRect);
			base.AddChild(this.Label);
			base.AddChild(this.Cooldown);
			base.AddChild(boxContainer);
			this.Button.Modulate = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, 150);
			this.OnThemeUpdated();
			base.OnKeyBindDown += delegate(GUIBoundKeyEventArgs args)
			{
				this.Depress(args, true);
				this.OnPressed(args);
			};
			base.OnKeyBindUp += delegate(GUIBoundKeyEventArgs args)
			{
				this.Depress(args, false);
				this.OnUnpressed(args);
			};
			base.TooltipDelay = new float?(0.5f);
			base.TooltipSupplier = new TooltipSupplier(this.SupplyTooltip);
		}

		// Token: 0x06000597 RID: 1431 RVA: 0x0001EC0C File Offset: 0x0001CE0C
		protected override void OnThemeUpdated()
		{
			this.Button.Texture = base.Theme.ResolveTexture("SlotBackground");
			this.Label.FontColorOverride = new Color?(base.Theme.ResolveColorOrSpecified("whiteText", default(Color)));
		}

		// Token: 0x06000598 RID: 1432 RVA: 0x0001EC5D File Offset: 0x0001CE5D
		private void OnPressed(GUIBoundKeyEventArgs args)
		{
			Action<GUIBoundKeyEventArgs, ActionButton> actionPressed = this.ActionPressed;
			if (actionPressed == null)
			{
				return;
			}
			actionPressed(args, this);
		}

		// Token: 0x06000599 RID: 1433 RVA: 0x0001EC71 File Offset: 0x0001CE71
		private void OnUnpressed(GUIBoundKeyEventArgs args)
		{
			Action<GUIBoundKeyEventArgs, ActionButton> actionUnpressed = this.ActionUnpressed;
			if (actionUnpressed == null)
			{
				return;
			}
			actionUnpressed(args, this);
		}

		// Token: 0x0600059A RID: 1434 RVA: 0x0001EC88 File Offset: 0x0001CE88
		[return: Nullable(2)]
		private Control SupplyTooltip(Control sender)
		{
			if (this.Action == null)
			{
				return null;
			}
			FormattedMessage name = FormattedMessage.FromMarkupPermissive(Loc.GetString(this.Action.DisplayName));
			FormattedMessage desc = FormattedMessage.FromMarkupPermissive(Loc.GetString(this.Action.Description));
			return new ActionAlertTooltip(name, desc, null);
		}

		// Token: 0x0600059B RID: 1435 RVA: 0x0001ECD1 File Offset: 0x0001CED1
		protected override void ControlFocusExited()
		{
			Action<ActionButton> actionFocusExited = this.ActionFocusExited;
			if (actionFocusExited == null)
			{
				return;
			}
			actionFocusExited(this);
		}

		// Token: 0x0600059C RID: 1436 RVA: 0x0001ECE4 File Offset: 0x0001CEE4
		private void UpdateItemIcon()
		{
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			ActionType action = this.Action;
			if (action != null && action.EntityIcon != null && !entityManager.EntityExists(this.Action.EntityIcon))
			{
				this._spriteViewDirty = true;
				return;
			}
			ActionType action2 = this.Action;
			SpriteComponent sprite;
			if (action2 == null || action2.EntityIcon == null || !entityManager.TryGetComponent<SpriteComponent>(this.Action.EntityIcon.Value, ref sprite))
			{
				this._bigItemSpriteView.Visible = false;
				this._bigItemSpriteView.Sprite = null;
				this._smallItemSpriteView.Visible = false;
				this._smallItemSpriteView.Sprite = null;
				return;
			}
			switch (this.Action.ItemIconStyle)
			{
			case ItemActionIconStyle.BigItem:
				this._bigItemSpriteView.Visible = true;
				this._bigItemSpriteView.Sprite = sprite;
				this._smallItemSpriteView.Visible = false;
				this._smallItemSpriteView.Sprite = null;
				return;
			case ItemActionIconStyle.BigAction:
				this._bigItemSpriteView.Visible = false;
				this._bigItemSpriteView.Sprite = null;
				this._smallItemSpriteView.Visible = true;
				this._smallItemSpriteView.Sprite = sprite;
				return;
			case ItemActionIconStyle.NoItem:
				this._bigItemSpriteView.Visible = false;
				this._bigItemSpriteView.Sprite = null;
				this._smallItemSpriteView.Visible = false;
				this._smallItemSpriteView.Sprite = null;
				return;
			default:
				return;
			}
		}

		// Token: 0x0600059D RID: 1437 RVA: 0x0001EE4C File Offset: 0x0001D04C
		[NullableContext(2)]
		private void SetActionIcon(Texture texture)
		{
			if (texture == null || this.Action == null)
			{
				this._bigActionIcon.Texture = null;
				this._bigActionIcon.Visible = false;
				this._smallActionIcon.Texture = null;
				this._smallActionIcon.Visible = false;
				return;
			}
			if (this.Action.EntityIcon != null && this.Action.ItemIconStyle == ItemActionIconStyle.BigItem)
			{
				this._smallActionIcon.Texture = texture;
				this._smallActionIcon.Modulate = this.Action.IconColor;
				this._smallActionIcon.Visible = true;
				this._bigActionIcon.Texture = null;
				this._bigActionIcon.Visible = false;
				return;
			}
			this._bigActionIcon.Texture = texture;
			this._bigActionIcon.Modulate = this.Action.IconColor;
			this._bigActionIcon.Visible = true;
			this._smallActionIcon.Texture = null;
			this._smallActionIcon.Visible = false;
		}

		// Token: 0x0600059E RID: 1438 RVA: 0x0001EF44 File Offset: 0x0001D144
		public void UpdateIcons()
		{
			this.UpdateItemIcon();
			if (this.Action == null)
			{
				this.SetActionIcon(null);
				return;
			}
			if ((this.Controller.SelectingTargetFor == this.Action || this.Action.Toggled) && this.Action.IconOn != null)
			{
				this.SetActionIcon(SpriteSpecifierExt.Frame0(this.Action.IconOn));
				return;
			}
			SpriteSpecifier icon = this.Action.Icon;
			this.SetActionIcon((icon != null) ? SpriteSpecifierExt.Frame0(icon) : null);
		}

		// Token: 0x0600059F RID: 1439 RVA: 0x0001EFC8 File Offset: 0x0001D1C8
		public bool TryReplaceWith(ActionType action)
		{
			if (this.Locked)
			{
				return false;
			}
			this.UpdateData(action);
			return true;
		}

		// Token: 0x060005A0 RID: 1440 RVA: 0x0001EFDC File Offset: 0x0001D1DC
		public void UpdateData(ActionType action)
		{
			this.Action = action;
			this.Label.Visible = true;
			this.UpdateIcons();
		}

		// Token: 0x060005A1 RID: 1441 RVA: 0x0001EFF7 File Offset: 0x0001D1F7
		public void ClearData()
		{
			this.Action = null;
			this.Cooldown.Visible = false;
			this.Cooldown.Progress = 1f;
			this.Label.Visible = false;
			this.UpdateIcons();
		}

		// Token: 0x060005A2 RID: 1442 RVA: 0x0001F030 File Offset: 0x0001D230
		protected override void FrameUpdate(FrameEventArgs args)
		{
			base.FrameUpdate(args);
			if (this._spriteViewDirty)
			{
				this._spriteViewDirty = false;
				this.UpdateIcons();
			}
			ActionType action = this.Action;
			if (action != null && action.Cooldown != null)
			{
				this.Cooldown.FromTime(this.Action.Cooldown.Value.Item1, this.Action.Cooldown.Value.Item2);
			}
			if (this.Action != null && this._toggled != this.Action.Toggled)
			{
				this._toggled = this.Action.Toggled;
			}
		}

		// Token: 0x060005A3 RID: 1443 RVA: 0x0001F0D3 File Offset: 0x0001D2D3
		protected override void MouseEntered()
		{
			base.MouseEntered();
			this._beingHovered = true;
			this.DrawModeChanged();
		}

		// Token: 0x060005A4 RID: 1444 RVA: 0x0001F0E8 File Offset: 0x0001D2E8
		protected override void MouseExited()
		{
			base.MouseExited();
			this._beingHovered = false;
			this.DrawModeChanged();
		}

		// Token: 0x060005A5 RID: 1445 RVA: 0x0001F100 File Offset: 0x0001D300
		public void Depress(GUIBoundKeyEventArgs args, bool depress)
		{
			ActionType action = this.Action;
			if (action == null || !action.Enabled)
			{
				return;
			}
			if (this._depressed && !depress)
			{
				this.OnUnpressed(args);
			}
			this._depressed = depress;
			this.DrawModeChanged();
		}

		// Token: 0x060005A6 RID: 1446 RVA: 0x0001F140 File Offset: 0x0001D340
		public void DrawModeChanged()
		{
			this.HighlightRect.Visible = this._beingHovered;
			if (this.Action == null)
			{
				base.SetOnlyStylePseudoClass("normal");
				return;
			}
			if (this._beingHovered && (this.Controller.IsDragging || this.Action.Enabled))
			{
				base.SetOnlyStylePseudoClass("hover");
			}
			if (this._depressed)
			{
				this.HighlightRect.Visible = false;
				base.SetOnlyStylePseudoClass("pressed");
				return;
			}
			if (this.Action.Toggled || this.Controller.SelectingTargetFor == this.Action)
			{
				base.SetOnlyStylePseudoClass((this.Action.IconOn != null) ? "normal" : "pressed");
				return;
			}
			if (!this.Action.Enabled)
			{
				base.SetOnlyStylePseudoClass("disabled");
				return;
			}
			base.SetOnlyStylePseudoClass("normal");
		}

		// Token: 0x04000288 RID: 648
		private bool _beingHovered;

		// Token: 0x04000289 RID: 649
		private bool _depressed;

		// Token: 0x0400028A RID: 650
		private bool _toggled;

		// Token: 0x0400028B RID: 651
		private bool _spriteViewDirty;

		// Token: 0x0400028C RID: 652
		private BoundKeyFunction? _keybind;

		// Token: 0x0400028D RID: 653
		public readonly TextureRect Button;

		// Token: 0x0400028E RID: 654
		public readonly PanelContainer HighlightRect;

		// Token: 0x0400028F RID: 655
		private readonly TextureRect _bigActionIcon;

		// Token: 0x04000290 RID: 656
		private readonly TextureRect _smallActionIcon;

		// Token: 0x04000291 RID: 657
		public readonly Label Label;

		// Token: 0x04000292 RID: 658
		public readonly CooldownGraphic Cooldown;

		// Token: 0x04000293 RID: 659
		private readonly SpriteView _smallItemSpriteView;

		// Token: 0x04000294 RID: 660
		private readonly SpriteView _bigItemSpriteView;
	}
}
