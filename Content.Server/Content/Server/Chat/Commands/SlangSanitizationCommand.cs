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
using Robust.Shared.Maths;
using Robust.Shared.Players;

namespace Content.Server.Chat.Commands
{
	// Token: 0x020006CE RID: 1742
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class SlangSanitizationCommand : IConsoleCommand
	{
		// Token: 0x17000560 RID: 1376
		// (get) Token: 0x0600245E RID: 9310 RVA: 0x000BDE6A File Offset: 0x000BC06A
		public string Command
		{
			get
			{
				return "enableSlangSanitization";
			}
		}

		// Token: 0x17000561 RID: 1377
		// (get) Token: 0x0600245F RID: 9311 RVA: 0x000BDE71 File Offset: 0x000BC071
		public string Description
		{
			get
			{
				return "Toggles the slang sanitization.";
			}
		}

		// Token: 0x17000562 RID: 1378
		// (get) Token: 0x06002460 RID: 9312 RVA: 0x000BDE78 File Offset: 0x000BC078
		public string Help
		{
			get
			{
				return "enableSlangSanitization <bool>";
			}
		}

		// Token: 0x06002461 RID: 9313 RVA: 0x000BDE80 File Offset: 0x000BC080
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			bool value;
			if (args.Length != 1 || !bool.TryParse(args[0], out value))
			{
				shell.WriteError(args[0] + " is not a valid boolean.");
				return;
			}
			this._cfg.SetCVar<bool>(CCVars.ChatSlangFilter, value, false);
			string text = "chatsan-announce-slang-sanitization";
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
			IoCManager.Resolve<IChatManager>().DispatchServerAnnouncement(announce, new Color?(Color.Red));
		}

		// Token: 0x04001680 RID: 5760
		[Dependency]
		private readonly IConfigurationManager _cfg;
	}
}
