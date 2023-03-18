using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Content.Shared.Rejuvenate;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000856 RID: 2134
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class RejuvenateCommand : IConsoleCommand
	{
		// Token: 0x1700078A RID: 1930
		// (get) Token: 0x06002EAC RID: 11948 RVA: 0x000F27DD File Offset: 0x000F09DD
		public string Command
		{
			get
			{
				return "rejuvenate";
			}
		}

		// Token: 0x1700078B RID: 1931
		// (get) Token: 0x06002EAD RID: 11949 RVA: 0x000F27E4 File Offset: 0x000F09E4
		public string Description
		{
			get
			{
				return Loc.GetString("rejuvenate-command-description");
			}
		}

		// Token: 0x1700078C RID: 1932
		// (get) Token: 0x06002EAE RID: 11950 RVA: 0x000F27F0 File Offset: 0x000F09F0
		public string Help
		{
			get
			{
				return Loc.GetString("rejuvenate-command-help-text");
			}
		}

		// Token: 0x06002EAF RID: 11951 RVA: 0x000F27FC File Offset: 0x000F09FC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length < 1)
			{
				IPlayerSession player = shell.Player as IPlayerSession;
				if (player != null)
				{
					shell.WriteLine(Loc.GetString("rejuvenate-command-self-heal-message"));
					if (player.AttachedEntity == null)
					{
						shell.WriteLine(Loc.GetString("rejuvenate-command-no-entity-attached-message"));
						return;
					}
					RejuvenateCommand.PerformRejuvenate(player.AttachedEntity.Value);
				}
			}
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			foreach (string arg in args)
			{
				EntityUid entity;
				if (!EntityUid.TryParse(arg, ref entity) || !entityManager.EntityExists(entity))
				{
					shell.WriteLine(Loc.GetString("shell-could-not-find-entity", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("entity", arg)
					}));
				}
				else
				{
					RejuvenateCommand.PerformRejuvenate(entity);
				}
			}
		}

		// Token: 0x06002EB0 RID: 11952 RVA: 0x000F28CE File Offset: 0x000F0ACE
		public static void PerformRejuvenate(EntityUid target)
		{
			IoCManager.Resolve<IEntityManager>().EventBus.RaiseLocalEvent<RejuvenateEvent>(target, new RejuvenateEvent(), false);
		}
	}
}
