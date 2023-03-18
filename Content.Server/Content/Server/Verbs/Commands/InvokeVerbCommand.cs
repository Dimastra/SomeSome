using System;
using System.Collections.Generic;
using System.Linq;
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
	// Token: 0x020000D0 RID: 208
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class InvokeVerbCommand : IConsoleCommand
	{
		// Token: 0x17000085 RID: 133
		// (get) Token: 0x0600039E RID: 926 RVA: 0x000132A2 File Offset: 0x000114A2
		public string Command
		{
			get
			{
				return "invokeverb";
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x0600039F RID: 927 RVA: 0x000132A9 File Offset: 0x000114A9
		public string Description
		{
			get
			{
				return Loc.GetString("invoke-verb-command-description");
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x060003A0 RID: 928 RVA: 0x000132B5 File Offset: 0x000114B5
		public string Help
		{
			get
			{
				return Loc.GetString("invoke-verb-command-help");
			}
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x000132C4 File Offset: 0x000114C4
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 3)
			{
				shell.WriteLine(Loc.GetString("invoke-verb-command-invalid-args"));
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
						playerEntity = new EntityUid?(shell.Player.AttachedEntity.Value);
						goto IL_A5;
					}
				}
				shell.WriteError(Loc.GetString("invoke-verb-command-invalid-player-uid"));
				return;
			}
			entityManager.EntityExists(new EntityUid(intPlayerUid));
			IL_A5:
			int intUid;
			if (!int.TryParse(args[1], out intUid))
			{
				shell.WriteError(Loc.GetString("invoke-verb-command-invalid-target-uid"));
				return;
			}
			if (playerEntity == null)
			{
				shell.WriteError(Loc.GetString("invoke-verb-command-invalid-player-entity"));
				return;
			}
			EntityUid target;
			target..ctor(intUid);
			if (!entityManager.EntityExists(target))
			{
				shell.WriteError(Loc.GetString("invoke-verb-command-invalid-target-entity"));
				return;
			}
			string verbName = args[2].ToLowerInvariant();
			SortedSet<Verb> verbs = verbSystem.GetLocalVerbs(target, playerEntity.Value, Verb.VerbTypes, true);
			Type verbType = Verb.VerbTypes.FirstOrDefault((Type x) => x.Name == verbName);
			if (verbType != null)
			{
				Verb verb = verbs.FirstOrDefault((Verb v) => v.GetType() == verbType);
				if (verb != null)
				{
					verbSystem.ExecuteVerb(verb, playerEntity.Value, target, true);
					shell.WriteLine(Loc.GetString("invoke-verb-command-success", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("verb", verbName),
						new ValueTuple<string, object>("target", target),
						new ValueTuple<string, object>("player", playerEntity)
					}));
					return;
				}
			}
			foreach (Verb verb2 in verbs)
			{
				if (verb2.Text.ToLowerInvariant() == verbName)
				{
					verbSystem.ExecuteVerb(verb2, playerEntity.Value, target, true);
					shell.WriteLine(Loc.GetString("invoke-verb-command-success", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("verb", verb2.Text),
						new ValueTuple<string, object>("target", target),
						new ValueTuple<string, object>("player", playerEntity)
					}));
					return;
				}
			}
			shell.WriteError(Loc.GetString("invoke-verb-command-verb-not-found", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("verb", verbName),
				new ValueTuple<string, object>("target", target)
			}));
		}
	}
}
