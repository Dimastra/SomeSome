using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.Verbs;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Players;

namespace Content.Server.Verbs.Commands
{
	// Token: 0x020000D1 RID: 209
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class ListVerbsCommand : IConsoleCommand
	{
		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060003A3 RID: 931 RVA: 0x000135C8 File Offset: 0x000117C8
		public string Command
		{
			get
			{
				return "listverbs";
			}
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060003A4 RID: 932 RVA: 0x000135CF File Offset: 0x000117CF
		public string Description
		{
			get
			{
				return Loc.GetString("list-verbs-command-description");
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060003A5 RID: 933 RVA: 0x000135DB File Offset: 0x000117DB
		public string Help
		{
			get
			{
				return Loc.GetString("list-verbs-command-help");
			}
		}

		// Token: 0x060003A6 RID: 934 RVA: 0x000135E8 File Offset: 0x000117E8
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 2)
			{
				shell.WriteLine(Loc.GetString("list-verbs-command-invalid-args"));
				return;
			}
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			SharedVerbSystem verbSystem = EntitySystem.Get<SharedVerbSystem>();
			EntityUid? playerEntity = null;
			int intPlayerUid;
			if (!int.TryParse(args[0], out intPlayerUid))
			{
				if (args[0] == "self")
				{
					ICommonSession player = shell.Player;
					if (player != null && player.AttachedEntity != null)
					{
						playerEntity = shell.Player.AttachedEntity;
						goto IL_8F;
					}
				}
				shell.WriteError(Loc.GetString("list-verbs-command-invalid-player-uid"));
				return;
			}
			entityManager.EntityExists(new EntityUid(intPlayerUid));
			IL_8F:
			int intUid;
			if (!int.TryParse(args[1], out intUid))
			{
				shell.WriteError(Loc.GetString("list-verbs-command-invalid-target-uid"));
				return;
			}
			if (playerEntity == null)
			{
				shell.WriteError(Loc.GetString("list-verbs-command-invalid-player-entity"));
				return;
			}
			EntityUid target;
			target..ctor(intUid);
			if (!entityManager.EntityExists(target))
			{
				shell.WriteError(Loc.GetString("list-verbs-command-invalid-target-entity"));
				return;
			}
			foreach (Verb verb in verbSystem.GetLocalVerbs(target, playerEntity.Value, Verb.VerbTypes, false))
			{
				shell.WriteLine(Loc.GetString("list-verbs-verb-listing", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("type", verb.GetType().Name),
					new ValueTuple<string, object>("verb", verb.Text)
				}));
			}
		}
	}
}
