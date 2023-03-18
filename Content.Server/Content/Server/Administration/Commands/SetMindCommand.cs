using System;
using System.Runtime.CompilerServices;
using Content.Server.Mind;
using Content.Server.Mind.Components;
using Content.Server.Players;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000860 RID: 2144
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	internal sealed class SetMindCommand : IConsoleCommand
	{
		// Token: 0x170007A8 RID: 1960
		// (get) Token: 0x06002EE2 RID: 12002 RVA: 0x000F321C File Offset: 0x000F141C
		public string Command
		{
			get
			{
				return "setmind";
			}
		}

		// Token: 0x170007A9 RID: 1961
		// (get) Token: 0x06002EE3 RID: 12003 RVA: 0x000F3223 File Offset: 0x000F1423
		public string Description
		{
			get
			{
				return Loc.GetString("set-mind-command-description", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("requiredComponent", "MindComponent")
				});
			}
		}

		// Token: 0x170007AA RID: 1962
		// (get) Token: 0x06002EE4 RID: 12004 RVA: 0x000F324B File Offset: 0x000F144B
		public string Help
		{
			get
			{
				return Loc.GetString("set-mind-command-help-text", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("command", this.Command)
				});
			}
		}

		// Token: 0x06002EE5 RID: 12005 RVA: 0x000F3274 File Offset: 0x000F1474
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 2)
			{
				shell.WriteLine(Loc.GetString("shell-wrong-arguments-number"));
				return;
			}
			int entityUid;
			if (!int.TryParse(args[0], out entityUid))
			{
				shell.WriteLine(Loc.GetString("shell-entity-uid-must-be-number"));
				return;
			}
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			EntityUid eUid;
			eUid..ctor(entityUid);
			if (!eUid.IsValid() || !entityManager.EntityExists(eUid))
			{
				shell.WriteLine(Loc.GetString("shell-invalid-entity-id"));
				return;
			}
			if (!entityManager.HasComponent<MindComponent>(eUid))
			{
				shell.WriteLine(Loc.GetString("set-mind-command-target-has-no-mind-message"));
				return;
			}
			IPlayerSession session;
			if (!IoCManager.Resolve<IPlayerManager>().TryGetSessionByUsername(args[1], ref session))
			{
				shell.WriteLine(Loc.GetString("shell-target-player-does-not-exist"));
				return;
			}
			PlayerData playerCData = session.ContentData();
			if (playerCData == null)
			{
				shell.WriteLine(Loc.GetString("set-mind-command-target-has-no-content-data-message"));
				return;
			}
			Mind mind = playerCData.Mind;
			if (mind == null)
			{
				mind = new Mind(session.UserId)
				{
					CharacterName = entityManager.GetComponent<MetaDataComponent>(eUid).EntityName
				};
				mind.ChangeOwningPlayer(new NetUserId?(session.UserId));
			}
			mind.TransferTo(new EntityUid?(eUid), false, false);
		}
	}
}
