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

namespace Content.Server.White.Stalin.Commands
{
	// Token: 0x02000090 RID: 144
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class EnableStalinBunker : IConsoleCommand
	{
		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000233 RID: 563 RVA: 0x0000C37F File Offset: 0x0000A57F
		public string Command
		{
			get
			{
				return "stalinbunker";
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000234 RID: 564 RVA: 0x0000C386 File Offset: 0x0000A586
		public string Description
		{
			get
			{
				return "Enables the stalin bunker, like PaNIk bunker, but better";
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000235 RID: 565 RVA: 0x0000C38D File Offset: 0x0000A58D
		public string Help
		{
			get
			{
				return "stalinBunker <bool>";
			}
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0000C394 File Offset: 0x0000A594
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length > 1)
			{
				shell.WriteError(Loc.GetString("shell-need-between-arguments", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("lower", 0),
					new ValueTuple<string, object>("upper", 1)
				}));
				return;
			}
			bool enabled = this._cfg.GetCVar<bool>(CCVars.PanicBunkerEnabled);
			if (args.Length == 0)
			{
				enabled = !enabled;
			}
			if (args.Length == 1 && !bool.TryParse(args[0], out enabled))
			{
				shell.WriteError(Loc.GetString("shell-argument-must-be-boolean"));
				return;
			}
			IoCManager.InjectDependencies<EnableStalinBunker>(this);
			this._cfg.SetCVar<bool>(CCVars.StalinEnabled, enabled, false);
			string text = "stalin-panic-bunker";
			ValueTuple<string, object>[] array = new ValueTuple<string, object>[1];
			int num = 0;
			string item = "enabled";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted<bool>(enabled);
			array[num] = new ValueTuple<string, object>(item, defaultInterpolatedStringHandler.ToStringAndClear());
			string announce = Loc.GetString(text, array);
			IoCManager.Resolve<IChatManager>().DispatchServerAnnouncement(announce, new Color?(Color.Red));
		}

		// Token: 0x04000192 RID: 402
		[Dependency]
		private readonly IConfigurationManager _cfg;
	}
}
