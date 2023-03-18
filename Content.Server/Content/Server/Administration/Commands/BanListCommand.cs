using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Database;
using Content.Server.EUI;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000834 RID: 2100
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Ban)]
	public sealed class BanListCommand : LocalizedCommands
	{
		// Token: 0x17000728 RID: 1832
		// (get) Token: 0x06002DFB RID: 11771 RVA: 0x000F088A File Offset: 0x000EEA8A
		public override string Command
		{
			get
			{
				return "banlist";
			}
		}

		// Token: 0x06002DFC RID: 11772 RVA: 0x000F0894 File Offset: 0x000EEA94
		public override void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			BanListCommand.<Execute>d__5 <Execute>d__;
			<Execute>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Execute>d__.<>4__this = this;
			<Execute>d__.shell = shell;
			<Execute>d__.args = args;
			<Execute>d__.<>1__state = -1;
			<Execute>d__.<>t__builder.Start<BanListCommand.<Execute>d__5>(ref <Execute>d__);
		}

		// Token: 0x06002DFD RID: 11773 RVA: 0x000F08DC File Offset: 0x000EEADC
		public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			if (args.Length != 1)
			{
				return CompletionResult.Empty;
			}
			return CompletionResult.FromHintOptions((from c in IoCManager.Resolve<IPlayerManager>().ServerSessions
			select c.Name into c
			orderby c
			select c).ToArray<string>(), Loc.GetString("cmd-banlist-hint"));
		}

		// Token: 0x04001C4C RID: 7244
		[Dependency]
		private readonly IServerDbManager _dbManager;

		// Token: 0x04001C4D RID: 7245
		[Dependency]
		private readonly EuiManager _eui;

		// Token: 0x04001C4E RID: 7246
		[Dependency]
		private readonly IPlayerLocator _locator;
	}
}
