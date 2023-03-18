using System;
using System.Runtime.CompilerServices;
using Content.Server.Ghost;
using Content.Server.Revenant.EntitySystems;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000865 RID: 2149
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class ShowGhostsCommand : IConsoleCommand
	{
		// Token: 0x170007B7 RID: 1975
		// (get) Token: 0x06002EFC RID: 12028 RVA: 0x000F3A52 File Offset: 0x000F1C52
		public string Command
		{
			get
			{
				return "showghosts";
			}
		}

		// Token: 0x170007B8 RID: 1976
		// (get) Token: 0x06002EFD RID: 12029 RVA: 0x000F3A59 File Offset: 0x000F1C59
		public string Description
		{
			get
			{
				return "makes all of the currently present ghosts visible. Cannot be reversed.";
			}
		}

		// Token: 0x170007B9 RID: 1977
		// (get) Token: 0x06002EFE RID: 12030 RVA: 0x000F3A60 File Offset: 0x000F1C60
		public string Help
		{
			get
			{
				return "showghosts <visible>";
			}
		}

		// Token: 0x06002EFF RID: 12031 RVA: 0x000F3A68 File Offset: 0x000F1C68
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
				return;
			}
			bool visible;
			if (!bool.TryParse(args[0], out visible))
			{
				shell.WriteError(Loc.GetString("shell-invalid-bool"));
				return;
			}
			GhostSystem ghostSys = this._entities.EntitySysManager.GetEntitySystem<GhostSystem>();
			RevenantSystem entitySystem = this._entities.EntitySysManager.GetEntitySystem<RevenantSystem>();
			ghostSys.MakeVisible(visible);
			entitySystem.MakeVisible(visible);
		}

		// Token: 0x04001C62 RID: 7266
		[Dependency]
		private readonly IEntityManager _entities;
	}
}
