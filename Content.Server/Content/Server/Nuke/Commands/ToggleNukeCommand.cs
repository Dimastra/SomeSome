using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Nuke.Commands
{
	// Token: 0x0200032D RID: 813
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class ToggleNukeCommand : IConsoleCommand
	{
		// Token: 0x17000273 RID: 627
		// (get) Token: 0x060010CB RID: 4299 RVA: 0x0005678B File Offset: 0x0005498B
		public string Command
		{
			get
			{
				return "nukearm";
			}
		}

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x060010CC RID: 4300 RVA: 0x00056792 File Offset: 0x00054992
		public string Description
		{
			get
			{
				return "Toggle nuclear bomb timer. You can set timer directly. Uid is optional.";
			}
		}

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x060010CD RID: 4301 RVA: 0x00056799 File Offset: 0x00054999
		public string Help
		{
			get
			{
				return "nukearm <timer> <uid>";
			}
		}

		// Token: 0x060010CE RID: 4302 RVA: 0x000567A0 File Offset: 0x000549A0
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			NukeComponent bomb = null;
			EntityUid bombUid;
			if (args.Length >= 2)
			{
				if (!EntityUid.TryParse(args[1], ref bombUid))
				{
					shell.WriteError(Loc.GetString("shell-entity-uid-must-be-number"));
					return;
				}
			}
			else
			{
				bomb = IoCManager.Resolve<IEntityManager>().EntityQuery<NukeComponent>(false).FirstOrDefault<NukeComponent>();
				if (bomb == null)
				{
					shell.WriteError("Can't find any entity with a NukeComponent");
					return;
				}
				bombUid = bomb.Owner;
			}
			NukeSystem nukeSys = EntitySystem.Get<NukeSystem>();
			if (args.Length >= 1)
			{
				float timer;
				if (!float.TryParse(args[0], out timer))
				{
					shell.WriteError("shell-argument-must-be-number");
					return;
				}
				nukeSys.SetRemainingTime(bombUid, timer, bomb);
			}
			nukeSys.ToggleBomb(bombUid, bomb);
		}
	}
}
