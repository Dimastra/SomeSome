using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Chat.Managers;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Players;

namespace Content.Server.Shuttles.Commands
{
	// Token: 0x02000210 RID: 528
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class EnableShuttleCallCommand : IConsoleCommand
	{
		// Token: 0x17000192 RID: 402
		// (get) Token: 0x06000A77 RID: 2679 RVA: 0x00037214 File Offset: 0x00035414
		public string Command
		{
			get
			{
				return "enableShuttleCall";
			}
		}

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x06000A78 RID: 2680 RVA: 0x0003721B File Offset: 0x0003541B
		public string Description
		{
			get
			{
				return Loc.GetString("Toggles the shuttle call.");
			}
		}

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x06000A79 RID: 2681 RVA: 0x00037227 File Offset: 0x00035427
		public string Help
		{
			get
			{
				return this.Command + " <bool>";
			}
		}

		// Token: 0x06000A7A RID: 2682 RVA: 0x0003723C File Offset: 0x0003543C
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			bool value;
			if (args.Length != 1 || !bool.TryParse(args[0], out value))
			{
				shell.WriteError(args[0] + " is not a valid boolean.");
				return;
			}
			bool shuttleEnabled = this._cfg.GetCVar<bool>(CCVars.EmergencyShuttleCallEnabled);
			if (value == shuttleEnabled)
			{
				shell.WriteError("enableShuttleCall is already " + args[0]);
				return;
			}
			this._cfg.SetCVar<bool>(CCVars.EmergencyShuttleCallEnabled, value, false);
			string text = "emergency_shuttle-announce-toggle";
			ValueTuple<string, object>[] array = new ValueTuple<string, object>[2];
			int num = 0;
			string item = "admin";
			ICommonSession player = shell.Player;
			array[num] = new ValueTuple<string, object>(item, ((player != null) ? player.Name : null) ?? "");
			int num2 = 1;
			string item2 = "value";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<bool>(value);
			array[num2] = new ValueTuple<string, object>(item2, defaultInterpolatedStringHandler.ToStringAndClear());
			string announce = Loc.GetString(text, array);
			IoCManager.Resolve<IChatManager>().SendAdminAnnouncement(announce);
		}

		// Token: 0x04000668 RID: 1640
		[Dependency]
		private readonly IConfigurationManager _cfg;
	}
}
