using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;

namespace Content.Server.White.StationGoal
{
	// Token: 0x02000088 RID: 136
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class StationGoalCommand : IConsoleCommand
	{
		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000207 RID: 519 RVA: 0x0000BC19 File Offset: 0x00009E19
		public string Command
		{
			get
			{
				return "sendstationgoal";
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000208 RID: 520 RVA: 0x0000BC20 File Offset: 0x00009E20
		public string Description
		{
			get
			{
				return Loc.GetString("send-station-goal-command-description");
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000209 RID: 521 RVA: 0x0000BC2C File Offset: 0x00009E2C
		public string Help
		{
			get
			{
				return Loc.GetString("send-station-goal-command-help-text", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("command", this.Command)
				});
			}
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000BC58 File Offset: 0x00009E58
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteError(Loc.GetString("shell-need-exactly-one-argument"));
				return;
			}
			string protoId = args[0];
			StationGoalPrototype proto;
			if (!IoCManager.Resolve<IPrototypeManager>().TryIndex<StationGoalPrototype>(protoId, ref proto))
			{
				shell.WriteError("No station goal found with ID " + protoId + "!");
				return;
			}
			if (!IoCManager.Resolve<IEntityManager>().System<StationGoalPaperSystem>().SendStationGoal(proto))
			{
				shell.WriteError("Station goal was not sent");
				return;
			}
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0000BCC4 File Offset: 0x00009EC4
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			if (args.Length == 1)
			{
				return CompletionResult.FromHintOptions(from p in IoCManager.Resolve<IPrototypeManager>().EnumeratePrototypes<StationGoalPrototype>()
				select new CompletionOption(p.ID, null, 0), Loc.GetString("send-station-goal-command-arg-id"));
			}
			return CompletionResult.Empty;
		}
	}
}
