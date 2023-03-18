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
	// Token: 0x02000869 RID: 2153
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class RemoveTagCommand : LocalizedCommands
	{
		// Token: 0x170007C3 RID: 1987
		// (get) Token: 0x06002F11 RID: 12049 RVA: 0x000F3DA0 File Offset: 0x000F1FA0
		public override string Command
		{
			get
			{
				return "removetag";
			}
		}

		// Token: 0x170007C4 RID: 1988
		// (get) Token: 0x06002F12 RID: 12050 RVA: 0x000F3DA7 File Offset: 0x000F1FA7
		public override string Description
		{
			get
			{
				return Loc.GetString("removetag-command-description");
			}
		}

		// Token: 0x170007C5 RID: 1989
		// (get) Token: 0x06002F13 RID: 12051 RVA: 0x000F3DB3 File Offset: 0x000F1FB3
		public override string Help
		{
			get
			{
				return Loc.GetString("removetag-command-help");
			}
		}

		// Token: 0x06002F14 RID: 12052 RVA: 0x000F3DC0 File Offset: 0x000F1FC0
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
			if (tagSystem.RemoveTag(entityUid, args[1]))
			{
				shell.WriteLine(Loc.GetString("removetag-command-success", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("tag", args[1]),
					new ValueTuple<string, object>("target", entityUid)
				}));
				return;
			}
			shell.WriteError(Loc.GetString("removetag-command-fail", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("tag", args[1]),
				new ValueTuple<string, object>("target", entityUid)
			}));
		}

		// Token: 0x06002F15 RID: 12053 RVA: 0x000F3EA8 File Offset: 0x000F20A8
		public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			if (args.Length == 1)
			{
				return CompletionResult.FromHint(Loc.GetString("shell-argument-uid"));
			}
			EntityUid entityUid;
			TagComponent tagComponent;
			if (args.Length == 2 && EntityUid.TryParse(args[0], ref entityUid) && this._entityManager.TryGetComponent<TagComponent>(entityUid, ref tagComponent))
			{
				return CompletionResult.FromHintOptions(tagComponent.Tags, Loc.GetString("tag-command-arg-tag"));
			}
			return CompletionResult.Empty;
		}

		// Token: 0x04001C64 RID: 7268
		[Dependency]
		private readonly IEntityManager _entityManager;
	}
}
