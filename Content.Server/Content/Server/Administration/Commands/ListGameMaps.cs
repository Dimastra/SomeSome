using System;
using System.Runtime.CompilerServices;
using Content.Server.GameTicking;
using Content.Server.Maps;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000844 RID: 2116
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Spawn | AdminFlags.Round)]
	public sealed class ListGameMaps : IConsoleCommand
	{
		// Token: 0x17000756 RID: 1878
		// (get) Token: 0x06002E4D RID: 11853 RVA: 0x000F1B5F File Offset: 0x000EFD5F
		public string Command
		{
			get
			{
				return "listgamemaps";
			}
		}

		// Token: 0x17000757 RID: 1879
		// (get) Token: 0x06002E4E RID: 11854 RVA: 0x000F1B66 File Offset: 0x000EFD66
		public string Description
		{
			get
			{
				return "Lists the game maps that can be used by loadgamemap";
			}
		}

		// Token: 0x17000758 RID: 1880
		// (get) Token: 0x06002E4F RID: 11855 RVA: 0x000F1B6D File Offset: 0x000EFD6D
		public string Help
		{
			get
			{
				return "listgamemaps";
			}
		}

		// Token: 0x06002E50 RID: 11856 RVA: 0x000F1B74 File Offset: 0x000EFD74
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPrototypeManager prototypeManager = IoCManager.Resolve<IPrototypeManager>();
			IoCManager.Resolve<IEntityManager>().EntitySysManager.GetEntitySystem<GameTicker>();
			if (args.Length != 0)
			{
				shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
				return;
			}
			foreach (GameMapPrototype prototype in prototypeManager.EnumeratePrototypes<GameMapPrototype>())
			{
				shell.WriteLine(prototype.ID + " - " + prototype.MapName);
			}
		}
	}
}
