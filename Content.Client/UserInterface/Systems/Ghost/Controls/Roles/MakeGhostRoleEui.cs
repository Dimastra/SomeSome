using System;
using System.Runtime.CompilerServices;
using Content.Client.Eui;
using Content.Shared.Eui;
using Content.Shared.Ghost.Roles;
using Robust.Client.Console;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.Systems.Ghost.Controls.Roles
{
	// Token: 0x02000092 RID: 146
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MakeGhostRoleEui : BaseEui
	{
		// Token: 0x06000362 RID: 866 RVA: 0x00014A18 File Offset: 0x00012C18
		public MakeGhostRoleEui()
		{
			this._window = new MakeGhostRoleWindow();
			this._window.OnClose += this.OnClose;
			this._window.OnMake += this.OnMake;
		}

		// Token: 0x06000363 RID: 867 RVA: 0x00014A64 File Offset: 0x00012C64
		public override void HandleState(EuiStateBase state)
		{
			MakeGhostRoleEuiState makeGhostRoleEuiState = state as MakeGhostRoleEuiState;
			if (makeGhostRoleEuiState == null)
			{
				return;
			}
			this._window.SetEntity(makeGhostRoleEuiState.EntityUid);
		}

		// Token: 0x06000364 RID: 868 RVA: 0x00014A8D File Offset: 0x00012C8D
		public override void Opened()
		{
			base.Opened();
			this._window.OpenCentered();
		}

		// Token: 0x06000365 RID: 869 RVA: 0x00014AA0 File Offset: 0x00012CA0
		private void OnMake(EntityUid uid, string name, string description, string rules, bool makeSentient)
		{
			LocalPlayer localPlayer = this._playerManager.LocalPlayer;
			if (localPlayer == null)
			{
				return;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 4);
			defaultInterpolatedStringHandler.AppendLiteral("makeghostrole ");
			defaultInterpolatedStringHandler.AppendLiteral("\"");
			defaultInterpolatedStringHandler.AppendFormatted(CommandParsing.Escape(uid.ToString()));
			defaultInterpolatedStringHandler.AppendLiteral("\" ");
			defaultInterpolatedStringHandler.AppendLiteral("\"");
			defaultInterpolatedStringHandler.AppendFormatted(CommandParsing.Escape(name));
			defaultInterpolatedStringHandler.AppendLiteral("\" ");
			defaultInterpolatedStringHandler.AppendLiteral("\"");
			defaultInterpolatedStringHandler.AppendFormatted(CommandParsing.Escape(description));
			defaultInterpolatedStringHandler.AppendLiteral("\" ");
			defaultInterpolatedStringHandler.AppendLiteral("\"");
			defaultInterpolatedStringHandler.AppendFormatted(CommandParsing.Escape(rules));
			defaultInterpolatedStringHandler.AppendLiteral("\"");
			string text = defaultInterpolatedStringHandler.ToStringAndClear();
			this._consoleHost.ExecuteCommand(localPlayer.Session, text);
			if (makeSentient)
			{
				string text2 = "makesentient \"" + CommandParsing.Escape(uid.ToString()) + "\"";
				this._consoleHost.ExecuteCommand(localPlayer.Session, text2);
			}
			this._window.Close();
		}

		// Token: 0x06000366 RID: 870 RVA: 0x00014BD1 File Offset: 0x00012DD1
		private void OnClose()
		{
			base.Closed();
			base.SendMessage(new MakeGhostRoleWindowClosedMessage());
		}

		// Token: 0x040001A5 RID: 421
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x040001A6 RID: 422
		[Dependency]
		private readonly IClientConsoleHost _consoleHost;

		// Token: 0x040001A7 RID: 423
		private readonly MakeGhostRoleWindow _window;
	}
}
