using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Content.Shared.FixedPoint;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;

namespace Content.Server.Traitor.Uplink.Commands
{
	// Token: 0x02000110 RID: 272
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class AddUplinkCommand : IConsoleCommand
	{
		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x060004DC RID: 1244 RVA: 0x0001738C File Offset: 0x0001558C
		public string Command
		{
			get
			{
				return "adduplink";
			}
		}

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x060004DD RID: 1245 RVA: 0x00017393 File Offset: 0x00015593
		public string Description
		{
			get
			{
				return Loc.GetString("add-uplink-command-description");
			}
		}

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x060004DE RID: 1246 RVA: 0x0001739F File Offset: 0x0001559F
		public string Help
		{
			get
			{
				return Loc.GetString("add-uplink-command-help");
			}
		}

		// Token: 0x060004DF RID: 1247 RVA: 0x000173AC File Offset: 0x000155AC
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			int num = args.Length;
			CompletionResult result;
			if (num != 1)
			{
				if (num != 2)
				{
					result = CompletionResult.Empty;
				}
				else
				{
					result = CompletionResult.FromHint(Loc.GetString("add-uplink-command-completion-2"));
				}
			}
			else
			{
				result = CompletionResult.FromHintOptions(CompletionHelper.SessionNames(true, null), Loc.GetString("add-uplink-command-completion-1"));
			}
			return result;
		}

		// Token: 0x060004E0 RID: 1248 RVA: 0x000173FC File Offset: 0x000155FC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length > 2)
			{
				shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
				return;
			}
			IPlayerSession session;
			if (args.Length != 0)
			{
				if (!IoCManager.Resolve<IPlayerManager>().TryGetSessionByUsername(args[0], ref session))
				{
					shell.WriteLine(Loc.GetString("shell-target-player-does-not-exist"));
					return;
				}
			}
			else
			{
				session = (IPlayerSession)shell.Player;
			}
			EntityUid? entityUid = (session != null) ? session.AttachedEntity : null;
			if (entityUid != null)
			{
				EntityUid user = entityUid.GetValueOrDefault();
				EntityUid? uplinkEntity = null;
				IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
				if (args.Length >= 2)
				{
					int itemID;
					if (!int.TryParse(args[1], out itemID))
					{
						shell.WriteLine(Loc.GetString("shell-entity-uid-must-be-number"));
						return;
					}
					EntityUid eUid;
					eUid..ctor(itemID);
					if (!eUid.IsValid() || !entityManager.EntityExists(eUid))
					{
						shell.WriteLine(Loc.GetString("shell-invalid-entity-id"));
						return;
					}
					uplinkEntity = new EntityUid?(eUid);
				}
				int tcCount = IoCManager.Resolve<IConfigurationManager>().GetCVar<int>(CCVars.TraitorStartingBalance);
				Logger.Debug(entityManager.ToPrettyString(user));
				if (!entityManager.EntitySysManager.GetEntitySystem<UplinkSystem>().AddUplink(user, new FixedPoint2?(FixedPoint2.New(tcCount)), "StorePresetUplink", uplinkEntity))
				{
					shell.WriteLine(Loc.GetString("add-uplink-command-error-2"));
				}
				return;
			}
			shell.WriteLine(Loc.GetString("add-uplink-command-error-1"));
		}
	}
}
