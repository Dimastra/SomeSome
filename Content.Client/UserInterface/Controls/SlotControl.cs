using System;
using System.Runtime.CompilerServices;
using Content.Client.Cooldown;
using Content.Client.UserInterface.Systems.Inventory.Controls;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;

namespace Content.Client.UserInterface.Controls
{
	// Token: 0x020000E4 RID: 228
	[NullableContext(1)]
	[Nullable(0)]
	[Virtual]
	public abstract class SlotControl : Control
	{
		// Token: 0x17000112 RID: 274
		// (get) Token: 0x0600065B RID: 1627 RVA: 0x00021EBA File Offset: 0x000200BA
		public TextureRect ButtonRect { get; }

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x0600065C RID: 1628 RVA: 0x00021EC2 File Offset: 0x000200C2
		public TextureRect BlockedRect { get; }

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x0600065D RID: 1629 RVA: 0x00021ECA File Offset: 0x000200CA
		public TextureRect HighlightRect { get; }

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x0600065E RID: 1630 RVA: 0x00021ED2 File Offset: 0x000200D2
		public SpriteView SpriteView { get; }

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x0600065F RID: 1631 RVA: 0x00021EDA File Offset: 0x000200DA
		public SpriteView HoverSpriteView { get; }

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x06000660 RID: 1632 RVA: 0x00021EE2 File Offset: 0x000200E2
		public TextureButton StorageButton { get; }

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x06000661 RID: 1633 RVA: 0x00021EEA File Offset: 0x000200EA
		public CooldownGraphic CooldownDisplay { get; }

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x06000662 RID: 1634 RVA: 0x00021EF4 File Offset: 0x000200F4
		public EntityUid? Entity
		{
			get
			{
				SpriteComponent sprite = this.SpriteView.Sprite;
				if (sprite == null)
				{
					return null;
				}
				return new EntityUid?(sprite.Owner);
			}
		}

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x06000663 RID: 1635 RVA: 0x00021F24 File Offset: 0x00020124
		// (set) Token: 0x06000664 RID: 1636 RVA: 0x00021F2C File Offset: 0x0002012C
		public string SlotName
		{
			get
			{
				return this._slotName;
			}
			set
			{
				if (this._slotNameSet)
				{
					Logger.Warning("Tried to set slotName after init for:" + base.Name);
					return;
				}
				this._slotNameSet = true;
				IItemslotUIContainer itemslotUIContainer = base.Parent as IItemslotUIContainer;
				if (itemslotUIContainer != null)
				{
					itemslotUIContainer.TryRegisterButton(this, value);
				}
				base.Name = "SlotButton_" + value;
				this._slotName = value;
			}
		}

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x06000665 RID: 1637 RVA: 0x00021F8E File Offset: 0x0002018E
		// (set) Token: 0x06000666 RID: 1638 RVA: 0x00021F9B File Offset: 0x0002019B
		public bool Highlight
		{
			get
			{
				return this.HighlightRect.Visible;
			}
			set
			{
				this.HighlightRect.Visible = value;
			}
		}

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x06000667 RID: 1639 RVA: 0x00021FA9 File Offset: 0x000201A9
		// (set) Token: 0x06000668 RID: 1640 RVA: 0x00021FB6 File Offset: 0x000201B6
		public bool Blocked
		{
			get
			{
				return this.BlockedRect.Visible;
			}
			set
			{
				this.BlockedRect.Visible = value;
			}
		}

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x06000669 RID: 1641 RVA: 0x00021FC4 File Offset: 0x000201C4
		public Texture BlockedTexture
		{
			get
			{
				return base.Theme.ResolveTexture(this.BlockedTexturePath);
			}
		}

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x0600066A RID: 1642 RVA: 0x00021FD7 File Offset: 0x000201D7
		// (set) Token: 0x0600066B RID: 1643 RVA: 0x00021FDF File Offset: 0x000201DF
		public string BlockedTexturePath
		{
			get
			{
				return this._blockedTexturePath;
			}
			set
			{
				this._blockedTexturePath = value;
				this.BlockedRect.Texture = base.Theme.ResolveTexture(this._blockedTexturePath);
			}
		}

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x0600066C RID: 1644 RVA: 0x00022004 File Offset: 0x00020204
		public Texture ButtonTexture
		{
			get
			{
				return base.Theme.ResolveTexture(this.ButtonTexturePath);
			}
		}

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x0600066D RID: 1645 RVA: 0x00022017 File Offset: 0x00020217
		// (set) Token: 0x0600066E RID: 1646 RVA: 0x0002201F File Offset: 0x0002021F
		public string ButtonTexturePath
		{
			get
			{
				return this._buttonTexturePath;
			}
			set
			{
				this._buttonTexturePath = value;
				this.ButtonRect.Texture = base.Theme.ResolveTexture(this._buttonTexturePath);
			}
		}

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x0600066F RID: 1647 RVA: 0x00022044 File Offset: 0x00020244
		public Texture StorageTexture
		{
			get
			{
				return base.Theme.ResolveTexture(this.StorageTexturePath);
			}
		}

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x06000670 RID: 1648 RVA: 0x00022017 File Offset: 0x00020217
		// (set) Token: 0x06000671 RID: 1649 RVA: 0x00022057 File Offset: 0x00020257
		public string StorageTexturePath
		{
			get
			{
				return this._buttonTexturePath;
			}
			set
			{
				this._storageTexturePath = value;
				this.StorageButton.TextureNormal = base.Theme.ResolveTexture(this._storageTexturePath);
			}
		}

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x06000672 RID: 1650 RVA: 0x0002207C File Offset: 0x0002027C
		// (set) Token: 0x06000673 RID: 1651 RVA: 0x00022084 File Offset: 0x00020284
		public string HighlightTexturePath
		{
			get
			{
				return this._highlightTexturePath;
			}
			set
			{
				this._highlightTexturePath = value;
				this.HighlightRect.Texture = base.Theme.ResolveTexture(this._highlightTexturePath);
			}
		}

		// Token: 0x1400002D RID: 45
		// (add) Token: 0x06000674 RID: 1652 RVA: 0x000220AC File Offset: 0x000202AC
		// (remove) Token: 0x06000675 RID: 1653 RVA: 0x000220E4 File Offset: 0x000202E4
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
		public event Action<GUIBoundKeyEventArgs, SlotControl> Pressed;

		// Token: 0x1400002E RID: 46
		// (add) Token: 0x06000676 RID: 1654 RVA: 0x0002211C File Offset: 0x0002031C
		// (remove) Token: 0x06000677 RID: 1655 RVA: 0x00022154 File Offset: 0x00020354
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
		public event Action<GUIBoundKeyEventArgs, SlotControl> Unpressed;

		// Token: 0x1400002F RID: 47
		// (add) Token: 0x06000678 RID: 1656 RVA: 0x0002218C File Offset: 0x0002038C
		// (remove) Token: 0x06000679 RID: 1657 RVA: 0x000221C4 File Offset: 0x000203C4
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
		public event Action<GUIBoundKeyEventArgs, SlotControl> StoragePressed;

		// Token: 0x14000030 RID: 48
		// (add) Token: 0x0600067A RID: 1658 RVA: 0x000221FC File Offset: 0x000203FC
		// (remove) Token: 0x0600067B RID: 1659 RVA: 0x00022234 File Offset: 0x00020434
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
		public event Action<GUIMouseHoverEventArgs, SlotControl> Hover;

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x0600067C RID: 1660 RVA: 0x00022269 File Offset: 0x00020469
		public bool EntityHover
		{
			get
			{
				return this.HoverSpriteView.Sprite != null;
			}
		}

		// Token: 0x0600067D RID: 1661 RVA: 0x0002227C File Offset: 0x0002047C
		public SlotControl()
		{
			IoCManager.InjectDependencies<SlotControl>(this);
			base.Name = "SlotButton_null";
			base.MinSize = new ValueTuple<float, float>((float)SlotControl.DefaultButtonSize, (float)SlotControl.DefaultButtonSize);
			TextureRect textureRect = new TextureRect();
			textureRect.TextureScale = new ValueTuple<float, float>(2f, 2f);
			textureRect.MouseFilter = 0;
			TextureRect textureRect2 = textureRect;
			this.ButtonRect = textureRect;
			base.AddChild(textureRect2);
			TextureRect textureRect3 = new TextureRect();
			textureRect3.Visible = false;
			textureRect3.TextureScale = new ValueTuple<float, float>(2f, 2f);
			textureRect3.MouseFilter = 2;
			textureRect2 = textureRect3;
			this.HighlightRect = textureRect3;
			base.AddChild(textureRect2);
			this.ButtonRect.OnKeyBindDown += this.OnButtonPressed;
			this.ButtonRect.OnKeyBindUp += this.OnButtonUnpressed;
			SpriteView spriteView = new SpriteView();
			spriteView.Scale = new ValueTuple<float, float>(2f, 2f);
			spriteView.OverrideDirection = new Direction?(0);
			SpriteView spriteView2 = spriteView;
			this.SpriteView = spriteView;
			base.AddChild(spriteView2);
			SpriteView spriteView3 = new SpriteView();
			spriteView3.Scale = new ValueTuple<float, float>(2f, 2f);
			spriteView3.OverrideDirection = new Direction?(0);
			spriteView2 = spriteView3;
			this.HoverSpriteView = spriteView3;
			base.AddChild(spriteView2);
			TextureButton textureButton = new TextureButton();
			textureButton.Scale = new ValueTuple<float, float>(0.75f, 0.75f);
			textureButton.HorizontalAlignment = 3;
			textureButton.VerticalAlignment = 3;
			textureButton.Visible = false;
			TextureButton textureButton2 = textureButton;
			this.StorageButton = textureButton;
			base.AddChild(textureButton2);
			this.StorageButton.OnKeyBindDown += delegate(GUIBoundKeyEventArgs args)
			{
				if (args.Function != EngineKeyFunctions.UIClick)
				{
					this.OnButtonPressed(args);
				}
			};
			this.StorageButton.OnPressed += this.OnStorageButtonPressed;
			this.ButtonRect.OnMouseEntered += delegate(GUIMouseHoverEventArgs _)
			{
				this.MouseIsHovering = true;
			};
			this.ButtonRect.OnMouseEntered += this.OnButtonHover;
			this.ButtonRect.OnMouseExited += delegate(GUIMouseHoverEventArgs _)
			{
				this.MouseIsHovering = false;
				this.ClearHover();
			};
			CooldownGraphic cooldownGraphic = new CooldownGraphic();
			cooldownGraphic.Visible = false;
			CooldownGraphic cooldownGraphic2 = cooldownGraphic;
			this.CooldownDisplay = cooldownGraphic;
			base.AddChild(cooldownGraphic2);
			TextureRect textureRect4 = new TextureRect();
			textureRect4.TextureScale = new ValueTuple<float, float>(2f, 2f);
			textureRect4.MouseFilter = 0;
			textureRect4.Visible = false;
			textureRect2 = textureRect4;
			this.BlockedRect = textureRect4;
			base.AddChild(textureRect2);
			this.HighlightTexturePath = "slot_highlight";
			this.BlockedTexturePath = "blocked";
		}

		// Token: 0x0600067E RID: 1662 RVA: 0x0002252C File Offset: 0x0002072C
		public void ClearHover()
		{
			if (!this.EntityHover)
			{
				return;
			}
			SpriteComponent sprite = this.HoverSpriteView.Sprite;
			if (sprite != null)
			{
				IoCManager.Resolve<IEntityManager>().DeleteEntity(sprite.Owner);
			}
			this.HoverSpriteView.Sprite = null;
		}

		// Token: 0x0600067F RID: 1663 RVA: 0x0002256D File Offset: 0x0002076D
		private void OnButtonPressed(GUIBoundKeyEventArgs args)
		{
			Action<GUIBoundKeyEventArgs, SlotControl> pressed = this.Pressed;
			if (pressed == null)
			{
				return;
			}
			pressed(args, this);
		}

		// Token: 0x06000680 RID: 1664 RVA: 0x00022581 File Offset: 0x00020781
		private void OnButtonUnpressed(GUIBoundKeyEventArgs args)
		{
			Action<GUIBoundKeyEventArgs, SlotControl> unpressed = this.Unpressed;
			if (unpressed == null)
			{
				return;
			}
			unpressed(args, this);
		}

		// Token: 0x06000681 RID: 1665 RVA: 0x00022598 File Offset: 0x00020798
		private void OnStorageButtonPressed(BaseButton.ButtonEventArgs args)
		{
			if (args.Event.Function == EngineKeyFunctions.UIClick)
			{
				Action<GUIBoundKeyEventArgs, SlotControl> storagePressed = this.StoragePressed;
				if (storagePressed == null)
				{
					return;
				}
				storagePressed(args.Event, this);
				return;
			}
			else
			{
				Action<GUIBoundKeyEventArgs, SlotControl> pressed = this.Pressed;
				if (pressed == null)
				{
					return;
				}
				pressed(args.Event, this);
				return;
			}
		}

		// Token: 0x06000682 RID: 1666 RVA: 0x000225EB File Offset: 0x000207EB
		private void OnButtonHover(GUIMouseHoverEventArgs args)
		{
			Action<GUIMouseHoverEventArgs, SlotControl> hover = this.Hover;
			if (hover == null)
			{
				return;
			}
			hover(args, this);
		}

		// Token: 0x06000683 RID: 1667 RVA: 0x00022600 File Offset: 0x00020800
		protected override void OnThemeUpdated()
		{
			this.StorageButton.TextureNormal = base.Theme.ResolveTexture(this._storageTexturePath);
			this.ButtonRect.Texture = base.Theme.ResolveTexture(this._buttonTexturePath);
			this.HighlightRect.Texture = base.Theme.ResolveTexture(this._highlightTexturePath);
		}

		// Token: 0x040002DC RID: 732
		private const string HighlightShader = "SelectionOutlineInrange";

		// Token: 0x040002DD RID: 733
		public static int DefaultButtonSize = 64;

		// Token: 0x040002E5 RID: 741
		private bool _slotNameSet;

		// Token: 0x040002E6 RID: 742
		private string _slotName = "";

		// Token: 0x040002E7 RID: 743
		private string _blockedTexturePath = "";

		// Token: 0x040002E8 RID: 744
		private string _buttonTexturePath = "";

		// Token: 0x040002E9 RID: 745
		private string _storageTexturePath = "";

		// Token: 0x040002EA RID: 746
		private string _highlightTexturePath = "";

		// Token: 0x040002EF RID: 751
		public bool MouseIsHovering;
	}
}
