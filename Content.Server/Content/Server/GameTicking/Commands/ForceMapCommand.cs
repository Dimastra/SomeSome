using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Maps;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Server.GameTicking.Commands
{
	// Token: 0x020004D4 RID: 1236
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Round)]
	internal sealed class ForceMapCommand : IConsoleCommand
	{
		// Token: 0x170003B7 RID: 951
		// (get) Token: 0x06001980 RID: 6528 RVA: 0x000862E7 File Offset: 0x000844E7
		public string Command
		{
			get
			{
				return "forcemap";
			}
		}

		// Token: 0x170003B8 RID: 952
		// (get) Token: 0x06001981 RID: 6529 RVA: 0x000862EE File Offset: 0x000844EE
		public string Description
		{
			get
			{
				return Loc.GetString("forcemap-command-description");
			}
		}

		// Token: 0x170003B9 RID: 953
		// (get) Token: 0x06001982 RID: 6530 RVA: 0x000862FA File Offset: 0x000844FA
		public string Help
		{
			get
			{
				return Loc.GetString("forcemap-command-help");
			}
		}

		// Token: 0x06001983 RID: 6531 RVA: 0x00086308 File Offset: 0x00084508
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteLine(Loc.GetString("forcemap-command-need-one-argument"));
				return;
			}
			IoCManager.Resolve<IGameMapManager>();
			string name = args[0];
			this._configurationManager.SetCVar<string>(CCVars.GameMap, name, false);
			shell.WriteLine(Loc.GetString("forcemap-command-success", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("map", name)
			}));
		}

		// Token: 0x06001984 RID: 6532 RVA: 0x00086370 File Offset: 0x00084570
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			if (args.Length == 1)
			{
				return CompletionResult.FromHintOptions(from p in IoCManager.Resolve<IPrototypeManager>().EnumeratePrototypes<GameMapPrototype>()
				select new CompletionOption(p.ID, p.MapName, 0) into p
				orderby p.Value
				select p, Loc.GetString("forcemap-command-arg-map"));
			}
			return CompletionResult.Empty;
		}

		// Token: 0x04001024 RID: 4132
		[Dependency]
		private readonly IConfigurationManager _configurationManager;
	}
}
