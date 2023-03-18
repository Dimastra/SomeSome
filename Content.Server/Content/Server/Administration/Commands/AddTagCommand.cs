using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Content.Shared.Tag;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000868 RID: 2152
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class AddTagCommand : LocalizedCommands
	{
		// Token: 0x170007C0 RID: 1984
		// (get) Token: 0x06002F0B RID: 12043 RVA: 0x000F3C4B File Offset: 0x000F1E4B
		public override string Command
		{
			get
			{
				return "addtag";
			}
		}

		// Token: 0x170007C1 RID: 1985
		// (get) Token: 0x06002F0C RID: 12044 RVA: 0x000F3C52 File Offset: 0x000F1E52
		public override string Description
		{
			get
			{
				return Loc.GetString("addtag-command-description");
			}
		}

		// Token: 0x170007C2 RID: 1986
		// (get) Token: 0x06002F0D RID: 12045 RVA: 0x000F3C5E File Offset: 0x000F1E5E
		public override string Help
		{
			get
			{
				return Loc.GetString("addtag-command-help");
			}
		}

		// Token: 0x06002F0E RID: 12046 RVA: 0x000F3C6C File Offset: 0x000F1E6C
		public override void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 2)
			{
				shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
				return;
			}
			EntityUid entityUid;
			if (!EntityUid.TryParse(args[0], ref entityUid))
			{
				shell.WriteError(Loc.GetString("shell-entity-uid-must-be-number"));
				return;
			}
			TagSystem tagSystem;
			if (!this._entityManager.TrySystem<TagSystem>(ref tagSystem))
			{
				return;
			}
			this._entityManager.EnsureComponent<TagComponent>(entityUid);
			if (tagSystem.TryAddTag(entityUid, args[1]))
			{
				shell.WriteLine(Loc.GetString("addtag-command-success", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("tag", args[1]),
					new ValueTuple<string, object>("target", entityUid)
				}));
				return;
			}
			shell.WriteError(Loc.GetString("addtag-command-fail", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("tag", args[1]),
				new ValueTuple<string, object>("target", entityUid)
			}));
		}

		// Token: 0x06002F0F RID: 12047 RVA: 0x000F3D5E File Offset: 0x000F1F5E
		public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			if (args.Length == 1)
			{
				return CompletionResult.FromHint(Loc.GetString("shell-argument-uid"));
			}
			if (args.Length == 2)
			{
				return CompletionResult.FromHintOptions(CompletionHelper.PrototypeIDs<TagPrototype>(true, null), Loc.GetString("tag-command-arg-tag"));
			}
			return CompletionResult.Empty;
		}

		// Token: 0x04001C63 RID: 7267
		[Dependency]
		private readonly IEntityManager _entityManager;
	}
}
