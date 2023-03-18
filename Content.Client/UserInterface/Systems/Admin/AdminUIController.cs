using System;
using System.Runtime.CompilerServices;
using Content.Client.Administration.Managers;
using Content.Client.Administration.UI;
using Content.Client.Administration.UI.Tabs.ObjectsTab;
using Content.Client.Administration.UI.Tabs.PlayerTab;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Client.Verbs.UI;
using Content.Shared.Input;
using Robust.Client.Console;
using Robust.Client.Input;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Players;

namespace Content.Client.UserInterface.Systems.Admin
{
	// Token: 0x020000BC RID: 188
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AdminUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
	{
		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000518 RID: 1304 RVA: 0x0001C256 File Offset: 0x0001A456
		[Nullable(2)]
		private MenuButton AdminButton
		{
			[NullableContext(2)]
			get
			{
				GameTopMenuBar activeUIWidgetOrNull = this.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>();
				if (activeUIWidgetOrNull == null)
				{
					return null;
				}
				return activeUIWidgetOrNull.AdminButton;
			}
		}

		// Token: 0x06000519 RID: 1305 RVA: 0x0001C270 File Offset: 0x0001A470
		public void OnStateEntered(GameplayState state)
		{
			this._window = this.UIManager.CreateWindow<AdminMenuWindow>();
			LayoutContainer.SetAnchorPreset(this._window, 8, false);
			this._window.PlayerTabControl.OnEntryPressed += this.PlayerTabEntryPressed;
			this._window.ObjectsTabControl.OnEntryPressed += this.ObjectsTabEntryPressed;
			this._window.OnOpen += this.OnWindowOpen;
			this._window.OnClose += this.OnWindowClosed;
			this._admin.AdminStatusUpdated += this.AdminStatusUpdated;
			this._input.SetInputCommand(ContentKeyFunctions.OpenAdminMenu, InputCmdHandler.FromDelegate(delegate(ICommonSession _)
			{
				this.Toggle();
			}, null, true, true));
			this.AdminStatusUpdated();
		}

		// Token: 0x0600051A RID: 1306 RVA: 0x0001C342 File Offset: 0x0001A542
		public void UnloadButton()
		{
			if (this.AdminButton == null)
			{
				return;
			}
			this.AdminButton.OnPressed -= this.AdminButtonPressed;
		}

		// Token: 0x0600051B RID: 1307 RVA: 0x0001C364 File Offset: 0x0001A564
		public void LoadButton()
		{
			if (this.AdminButton == null)
			{
				return;
			}
			this.AdminButton.OnPressed += this.AdminButtonPressed;
		}

		// Token: 0x0600051C RID: 1308 RVA: 0x0001C386 File Offset: 0x0001A586
		private void OnWindowOpen()
		{
			if (this.AdminButton != null)
			{
				this.AdminButton.Pressed = true;
			}
		}

		// Token: 0x0600051D RID: 1309 RVA: 0x0001C39C File Offset: 0x0001A59C
		private void OnWindowClosed()
		{
			if (this.AdminButton != null)
			{
				this.AdminButton.Pressed = false;
			}
		}

		// Token: 0x0600051E RID: 1310 RVA: 0x0001C3B4 File Offset: 0x0001A5B4
		public void OnStateExited(GameplayState state)
		{
			if (this._window != null)
			{
				this._window.PlayerTabControl.OnEntryPressed -= this.PlayerTabEntryPressed;
				this._window.ObjectsTabControl.OnEntryPressed -= this.ObjectsTabEntryPressed;
				this._window.OnOpen -= this.OnWindowOpen;
				this._window.OnClose -= this.OnWindowClosed;
				this._window.Dispose();
				this._window = null;
			}
			this._admin.AdminStatusUpdated -= this.AdminStatusUpdated;
			CommandBinds.Unregister<AdminUIController>();
		}

		// Token: 0x0600051F RID: 1311 RVA: 0x0001C45D File Offset: 0x0001A65D
		private void AdminStatusUpdated()
		{
			this.AdminButton.Visible = this._conGroups.CanAdminMenu();
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x0001C475 File Offset: 0x0001A675
		private void AdminButtonPressed(BaseButton.ButtonEventArgs args)
		{
			this.Toggle();
		}

		// Token: 0x06000521 RID: 1313 RVA: 0x0001C480 File Offset: 0x0001A680
		private void Toggle()
		{
			AdminMenuWindow window = this._window;
			if (window != null && window.IsOpen)
			{
				this._window.Close();
				return;
			}
			if (this._conGroups.CanAdminMenu())
			{
				AdminMenuWindow window2 = this._window;
				if (window2 == null)
				{
					return;
				}
				window2.Open();
			}
		}

		// Token: 0x06000522 RID: 1314 RVA: 0x0001C4C8 File Offset: 0x0001A6C8
		private void PlayerTabEntryPressed(BaseButton.ButtonEventArgs args)
		{
			PlayerTabEntry playerTabEntry = args.Button as PlayerTabEntry;
			if (playerTabEntry == null || playerTabEntry.PlayerUid == null)
			{
				return;
			}
			EntityUid value = playerTabEntry.PlayerUid.Value;
			BoundKeyFunction function = args.Event.Function;
			if (function == EngineKeyFunctions.UIClick)
			{
				IConsoleHost conHost = this._conHost;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 1);
				defaultInterpolatedStringHandler.AppendLiteral("vv ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(value);
				conHost.ExecuteCommand(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else
			{
				if (!(function == EngineKeyFunctions.UseSecondary))
				{
					return;
				}
				this._verb.OpenVerbMenu(value, true, null);
			}
			args.Event.Handle();
		}

		// Token: 0x06000523 RID: 1315 RVA: 0x0001C574 File Offset: 0x0001A774
		private void ObjectsTabEntryPressed(BaseButton.ButtonEventArgs args)
		{
			ObjectsTabEntry objectsTabEntry = args.Button as ObjectsTabEntry;
			if (objectsTabEntry == null)
			{
				return;
			}
			EntityUid assocEntity = objectsTabEntry.AssocEntity;
			BoundKeyFunction function = args.Event.Function;
			if (function == EngineKeyFunctions.UIClick)
			{
				IConsoleHost conHost = this._conHost;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 1);
				defaultInterpolatedStringHandler.AppendLiteral("vv ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(assocEntity);
				conHost.ExecuteCommand(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else
			{
				if (!(function == EngineKeyFunctions.UseSecondary))
				{
					return;
				}
				this._verb.OpenVerbMenu(assocEntity, true, null);
			}
			args.Event.Handle();
		}

		// Token: 0x0400025D RID: 605
		[Dependency]
		private readonly IClientAdminManager _admin;

		// Token: 0x0400025E RID: 606
		[Dependency]
		private readonly IClientConGroupController _conGroups;

		// Token: 0x0400025F RID: 607
		[Dependency]
		private readonly IClientConsoleHost _conHost;

		// Token: 0x04000260 RID: 608
		[Dependency]
		private readonly IInputManager _input;

		// Token: 0x04000261 RID: 609
		[Dependency]
		private readonly VerbMenuUIController _verb;

		// Token: 0x04000262 RID: 610
		[Nullable(2)]
		private AdminMenuWindow _window;
	}
}
