using System;
using System.Runtime.CompilerServices;
using Content.Server.Database;
using Content.Server.Preferences.Managers;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Network;

namespace Content.Server.Administration.Commands
{
	// Token: 0x0200085F RID: 2143
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	internal sealed class SetAdminOOC : IConsoleCommand
	{
		// Token: 0x170007A5 RID: 1957
		// (get) Token: 0x06002EDD RID: 11997 RVA: 0x000F3113 File Offset: 0x000F1313
		public string Command
		{
			get
			{
				return "setadminooc";
			}
		}

		// Token: 0x170007A6 RID: 1958
		// (get) Token: 0x06002EDE RID: 11998 RVA: 0x000F311A File Offset: 0x000F131A
		public string Description
		{
			get
			{
				return Loc.GetString("set-admin-ooc-command-description", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("command", this.Command)
				});
			}
		}

		// Token: 0x170007A7 RID: 1959
		// (get) Token: 0x06002EDF RID: 11999 RVA: 0x000F3143 File Offset: 0x000F1343
		public string Help
		{
			get
			{
				return Loc.GetString("set-admin-ooc-command-help-text", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("command", this.Command)
				});
			}
		}

		// Token: 0x06002EE0 RID: 12000 RVA: 0x000F316C File Offset: 0x000F136C
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (!(shell.Player is IPlayerSession))
			{
				shell.WriteError(Loc.GetString("shell-only-players-can-run-this-command"));
				return;
			}
			if (args.Length < 1)
			{
				return;
			}
			string colorArg = string.Join(" ", args).Trim();
			if (string.IsNullOrEmpty(colorArg))
			{
				return;
			}
			Color? color = Color.TryFromHex(colorArg);
			if (color == null)
			{
				shell.WriteError(Loc.GetString("shell-invalid-color-hex"));
				return;
			}
			NetUserId userId = shell.Player.UserId;
			IoCManager.Resolve<IServerDbManager>().SaveAdminOOCColorAsync(userId, color.Value);
			IoCManager.Resolve<IServerPreferencesManager>().GetPreferences(userId).AdminOOCColor = color.Value;
		}
	}
}
