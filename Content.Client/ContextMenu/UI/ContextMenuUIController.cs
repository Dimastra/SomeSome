using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Client.Gameplay;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Content.Client.ContextMenu.UI
{
	// Token: 0x0200037F RID: 895
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ContextMenuUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
	{
		// Token: 0x17000472 RID: 1138
		// (get) Token: 0x06001601 RID: 5633 RVA: 0x00081EE7 File Offset: 0x000800E7
		public Stack<ContextMenuPopup> Menus { get; } = new Stack<ContextMenuPopup>();

		// Token: 0x06001602 RID: 5634 RVA: 0x00081EEF File Offset: 0x000800EF
		public void OnStateEntered(GameplayState state)
		{
			this.RootMenu = new ContextMenuPopup(this, null);
			this.RootMenu.OnPopupHide += this.Close;
			this.Menus.Push(this.RootMenu);
		}

		// Token: 0x06001603 RID: 5635 RVA: 0x00081F26 File Offset: 0x00080126
		public void OnStateExited(GameplayState state)
		{
			this.Close();
			this.RootMenu.OnPopupHide -= this.Close;
			this.RootMenu.Dispose();
		}

		// Token: 0x06001604 RID: 5636 RVA: 0x00081F50 File Offset: 0x00080150
		public void Close()
		{
			this.RootMenu.MenuBody.DisposeAllChildren();
			CancellationTokenSource cancelOpen = this.CancelOpen;
			if (cancelOpen != null)
			{
				cancelOpen.Cancel();
			}
			CancellationTokenSource cancelClose = this.CancelClose;
			if (cancelClose != null)
			{
				cancelClose.Cancel();
			}
			Action onContextClosed = this.OnContextClosed;
			if (onContextClosed != null)
			{
				onContextClosed();
			}
			this.RootMenu.Close();
		}

		// Token: 0x06001605 RID: 5637 RVA: 0x00081FAC File Offset: 0x000801AC
		[NullableContext(2)]
		public void CloseSubMenus(ContextMenuPopup menu)
		{
			if (menu == null || !menu.Visible)
			{
				return;
			}
			ContextMenuPopup contextMenuPopup;
			while (this.Menus.TryPeek(out contextMenuPopup) && contextMenuPopup != menu)
			{
				this.Menus.Pop().Close();
			}
			CancellationTokenSource cancelClose = this.CancelClose;
			if (cancelClose != null)
			{
				cancelClose.Cancel();
			}
			this.CancelClose = null;
		}

		// Token: 0x06001606 RID: 5638 RVA: 0x00082004 File Offset: 0x00080204
		private void OnMouseEntered(ContextMenuElement element)
		{
			ContextMenuPopup contextMenuPopup;
			if (!this.Menus.TryPeek(out contextMenuPopup))
			{
				Logger.Error("Context Menu: Mouse entered menu without any open menus?");
				return;
			}
			if (element.ParentMenu == contextMenuPopup || element.SubMenu == contextMenuPopup)
			{
				CancellationTokenSource cancelClose = this.CancelClose;
				if (cancelClose != null)
				{
					cancelClose.Cancel();
				}
			}
			if (element.SubMenu == contextMenuPopup)
			{
				return;
			}
			CancellationTokenSource cancelOpen = this.CancelOpen;
			if (cancelOpen != null)
			{
				cancelOpen.Cancel();
			}
			this.CancelOpen = new CancellationTokenSource();
			Timer.Spawn(ContextMenuUIController.HoverDelay, delegate()
			{
				this.OpenSubMenu(element);
			}, this.CancelOpen.Token);
		}

		// Token: 0x06001607 RID: 5639 RVA: 0x000820B8 File Offset: 0x000802B8
		private void OnMouseExited(ContextMenuElement element)
		{
			CancellationTokenSource cancelOpen = this.CancelOpen;
			if (cancelOpen != null)
			{
				cancelOpen.Cancel();
			}
			if (element.SubMenu == null)
			{
				return;
			}
			CancellationTokenSource cancelClose = this.CancelClose;
			if (cancelClose != null)
			{
				cancelClose.Cancel();
			}
			this.CancelClose = new CancellationTokenSource();
			Timer.Spawn(ContextMenuUIController.HoverDelay, delegate()
			{
				this.CloseSubMenus(element.ParentMenu);
			}, this.CancelClose.Token);
			Action<ContextMenuElement> onContextMouseExited = this.OnContextMouseExited;
			if (onContextMouseExited == null)
			{
				return;
			}
			onContextMouseExited(element);
		}

		// Token: 0x06001608 RID: 5640 RVA: 0x0008214B File Offset: 0x0008034B
		private void OnKeyBindDown(ContextMenuElement element, GUIBoundKeyEventArgs args)
		{
			Action<ContextMenuElement, GUIBoundKeyEventArgs> onContextKeyEvent = this.OnContextKeyEvent;
			if (onContextKeyEvent == null)
			{
				return;
			}
			onContextKeyEvent(element, args);
		}

		// Token: 0x06001609 RID: 5641 RVA: 0x00082160 File Offset: 0x00080360
		public void OpenSubMenu(ContextMenuElement element)
		{
			ContextMenuPopup contextMenuPopup;
			if (!this.Menus.TryPeek(out contextMenuPopup))
			{
				Logger.Error("Context Menu: Attempting to open sub menu without any open menus?");
				return;
			}
			if (element.SubMenu == contextMenuPopup)
			{
				return;
			}
			if (element.Disposed || element.ParentMenu == null || !element.ParentMenu.Visible)
			{
				return;
			}
			this.CloseSubMenus(element.ParentMenu);
			CancellationTokenSource cancelOpen = this.CancelOpen;
			if (cancelOpen != null)
			{
				cancelOpen.Cancel();
			}
			this.CancelOpen = null;
			if (element.SubMenu == null)
			{
				return;
			}
			Vector2 globalPosition = element.GlobalPosition;
			Vector2 vector = globalPosition + new ValueTuple<float, float>(element.Width + 4f, -4f);
			element.SubMenu.Open(new UIBox2?(UIBox2.FromDimensions(vector, new ValueTuple<float, float>(1f, 1f))), new Vector2?(globalPosition));
			element.SubMenu.SetPositionLast();
			this.Menus.Push(element.SubMenu);
			Action<ContextMenuElement> onSubMenuOpened = this.OnSubMenuOpened;
			if (onSubMenuOpened == null)
			{
				return;
			}
			onSubMenuOpened(element);
		}

		// Token: 0x0600160A RID: 5642 RVA: 0x00082264 File Offset: 0x00080464
		public void AddElement(ContextMenuPopup menu, ContextMenuElement element)
		{
			element.OnMouseEntered += delegate(GUIMouseHoverEventArgs _)
			{
				this.OnMouseEntered(element);
			};
			element.OnMouseExited += delegate(GUIMouseHoverEventArgs _)
			{
				this.OnMouseExited(element);
			};
			element.OnKeyBindDown += delegate(GUIBoundKeyEventArgs args)
			{
				this.OnKeyBindDown(element, args);
			};
			element.ParentMenu = menu;
			menu.MenuBody.AddChild(element);
			menu.InvalidateMeasure();
		}

		// Token: 0x0600160B RID: 5643 RVA: 0x000822F0 File Offset: 0x000804F0
		public void OnRemoveElement(ContextMenuPopup menu, Control control)
		{
			ContextMenuElement element = control as ContextMenuElement;
			if (element == null)
			{
				return;
			}
			element.OnMouseEntered -= delegate(GUIMouseHoverEventArgs _)
			{
				this.OnMouseEntered(element);
			};
			element.OnMouseExited -= delegate(GUIMouseHoverEventArgs _)
			{
				this.OnMouseExited(element);
			};
			element.OnKeyBindDown -= delegate(GUIBoundKeyEventArgs args)
			{
				this.OnKeyBindDown(element, args);
			};
			menu.InvalidateMeasure();
		}

		// Token: 0x04000B80 RID: 2944
		public static readonly TimeSpan HoverDelay = TimeSpan.FromSeconds(0.2);

		// Token: 0x04000B81 RID: 2945
		public ContextMenuPopup RootMenu;

		// Token: 0x04000B83 RID: 2947
		[Nullable(2)]
		public CancellationTokenSource CancelOpen;

		// Token: 0x04000B84 RID: 2948
		[Nullable(2)]
		public CancellationTokenSource CancelClose;

		// Token: 0x04000B85 RID: 2949
		[Nullable(2)]
		public Action OnContextClosed;

		// Token: 0x04000B86 RID: 2950
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public Action<ContextMenuElement> OnContextMouseEntered;

		// Token: 0x04000B87 RID: 2951
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public Action<ContextMenuElement> OnContextMouseExited;

		// Token: 0x04000B88 RID: 2952
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public Action<ContextMenuElement> OnSubMenuOpened;

		// Token: 0x04000B89 RID: 2953
		[Nullable(new byte[]
		{
			2,
			1,
			1
		})]
		public Action<ContextMenuElement, GUIBoundKeyEventArgs> OnContextKeyEvent;
	}
}
