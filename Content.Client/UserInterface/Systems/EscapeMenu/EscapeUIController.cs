using System;
using System.Runtime.CompilerServices;
using Content.Client.Gameplay;
using Content.Client.Guidebook;
using Content.Client.Options.UI;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Info;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Shared.CCVar;
using Robust.Client.Console;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Players;

namespace Content.Client.UserInterface.Systems.EscapeMenu
{
	// Token: 0x02000096 RID: 150
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class EscapeUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
	{
		// Token: 0x1700009C RID: 156
		// (get) Token: 0x06000383 RID: 899 RVA: 0x0001542C File Offset: 0x0001362C
		[Nullable(2)]
		private MenuButton EscapeButton
		{
			[NullableContext(2)]
			get
			{
				GameTopMenuBar activeUIWidgetOrNull = this.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>();
				if (activeUIWidgetOrNull == null)
				{
					return null;
				}
				return activeUIWidgetOrNull.EscapeButton;
			}
		}

		// Token: 0x06000384 RID: 900 RVA: 0x00015444 File Offset: 0x00013644
		public void UnloadButton()
		{
			if (this.EscapeButton == null)
			{
				return;
			}
			this.EscapeButton.Pressed = false;
			this.EscapeButton.OnPressed -= this.EscapeButtonOnOnPressed;
		}

		// Token: 0x06000385 RID: 901 RVA: 0x00015472 File Offset: 0x00013672
		public void LoadButton()
		{
			if (this.EscapeButton == null)
			{
				return;
			}
			this.EscapeButton.OnPressed += this.EscapeButtonOnOnPressed;
		}

		// Token: 0x06000386 RID: 902 RVA: 0x00015494 File Offset: 0x00013694
		private void ActivateButton()
		{
			this.EscapeButton.Pressed = true;
		}

		// Token: 0x06000387 RID: 903 RVA: 0x000154A2 File Offset: 0x000136A2
		private void DeactivateButton()
		{
			this.EscapeButton.Pressed = false;
		}

		// Token: 0x06000388 RID: 904 RVA: 0x000154B0 File Offset: 0x000136B0
		public void OnStateEntered(GameplayState state)
		{
			this._escapeWindow = this.UIManager.CreateWindow<EscapeMenu>();
			this._escapeWindow.OnClose += this.DeactivateButton;
			this._escapeWindow.OnOpen += this.ActivateButton;
			this._escapeWindow.ChangelogButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.CloseEscapeWindow();
				this._changelog.ToggleWindow();
			};
			this._escapeWindow.RulesButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.CloseEscapeWindow();
				this._info.OpenWindow();
			};
			this._escapeWindow.DisconnectButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.CloseEscapeWindow();
				this._console.ExecuteCommand("disconnect");
			};
			this._escapeWindow.OptionsButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.CloseEscapeWindow();
				this._options.OpenWindow();
			};
			this._escapeWindow.QuitButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this.CloseEscapeWindow();
				this._console.ExecuteCommand("quit");
			};
			this._escapeWindow.WikiButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this._uri.OpenUri(this._cfg.GetCVar<string>(CCVars.InfoLinksWiki));
			};
			this._escapeWindow.DiscordButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				this._uri.OpenUri(this._cfg.GetCVar<string>(CCVars.InfoLinksDiscord));
			};
			this._escapeWindow.GuidebookButton.OnPressed += delegate(BaseButton.ButtonEventArgs _)
			{
				GuidebookSystem guidebook = this._guidebook;
				if (guidebook == null)
				{
					return;
				}
				guidebook.OpenGuidebook(null, null, null, true, null);
			};
			this._escapeWindow.WikiButton.Visible = (this._cfg.GetCVar<string>(CCVars.InfoLinksWiki) != "");
			CommandBinds.Builder.Bind(EngineKeyFunctions.EscapeMenu, InputCmdHandler.FromDelegate(delegate(ICommonSession _)
			{
				this.ToggleWindow();
			}, null, true, true)).Register<EscapeUIController>();
		}

		// Token: 0x06000389 RID: 905 RVA: 0x0001562F File Offset: 0x0001382F
		public void OnStateExited(GameplayState state)
		{
			if (this._escapeWindow != null)
			{
				this._escapeWindow.Dispose();
				this._escapeWindow = null;
			}
			CommandBinds.Unregister<EscapeUIController>();
		}

		// Token: 0x0600038A RID: 906 RVA: 0x00015650 File Offset: 0x00013850
		private void EscapeButtonOnOnPressed(BaseButton.ButtonEventArgs obj)
		{
			this.ToggleWindow();
		}

		// Token: 0x0600038B RID: 907 RVA: 0x00015658 File Offset: 0x00013858
		private void CloseEscapeWindow()
		{
			EscapeMenu escapeWindow = this._escapeWindow;
			if (escapeWindow == null)
			{
				return;
			}
			escapeWindow.Close();
		}

		// Token: 0x0600038C RID: 908 RVA: 0x0001566C File Offset: 0x0001386C
		private void ToggleWindow()
		{
			if (this._escapeWindow == null)
			{
				return;
			}
			if (this._escapeWindow.IsOpen)
			{
				this.CloseEscapeWindow();
				this.EscapeButton.Pressed = false;
				return;
			}
			this._escapeWindow.OpenCentered();
			this.EscapeButton.Pressed = true;
		}

		// Token: 0x040001AB RID: 427
		[Dependency]
		private readonly IClientConsoleHost _console;

		// Token: 0x040001AC RID: 428
		[Dependency]
		private readonly IUriOpener _uri;

		// Token: 0x040001AD RID: 429
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x040001AE RID: 430
		[Dependency]
		private readonly ChangelogUIController _changelog;

		// Token: 0x040001AF RID: 431
		[Dependency]
		private readonly InfoUIController _info;

		// Token: 0x040001B0 RID: 432
		[Dependency]
		private readonly OptionsUIController _options;

		// Token: 0x040001B1 RID: 433
		[Nullable(2)]
		[UISystemDependency]
		private readonly GuidebookSystem _guidebook;

		// Token: 0x040001B2 RID: 434
		[Nullable(2)]
		private EscapeMenu _escapeWindow;
	}
}
