using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000833 RID: 2099
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Ban)]
	public sealed class BanCommand : IConsoleCommand
	{
		// Token: 0x17000725 RID: 1829
		// (get) Token: 0x06002DF4 RID: 11764 RVA: 0x000F0636 File Offset: 0x000EE836
		public string Command
		{
			get
			{
				return "ban";
			}
		}

		// Token: 0x17000726 RID: 1830
		// (get) Token: 0x06002DF5 RID: 11765 RVA: 0x000F063D File Offset: 0x000EE83D
		public string Description
		{
			get
			{
				return Loc.GetString("cmd-ban-desc");
			}
		}

		// Token: 0x17000727 RID: 1831
		// (get) Token: 0x06002DF6 RID: 11766 RVA: 0x000F0649 File Offset: 0x000EE849
		public string Help
		{
			get
			{
				return Loc.GetString("cmd-ban-help", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("Command", this.Command)
				});
			}
		}

		// Token: 0x06002DF7 RID: 11767 RVA: 0x000F0674 File Offset: 0x000EE874
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			BanCommand.<Execute>d__6 <Execute>d__;
			<Execute>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Execute>d__.<>4__this = this;
			<Execute>d__.shell = shell;
			<Execute>d__.args = args;
			<Execute>d__.<>1__state = -1;
			<Execute>d__.<>t__builder.Start<BanCommand.<Execute>d__6>(ref <Execute>d__);
		}

		// Token: 0x06002DF8 RID: 11768 RVA: 0x000F06BC File Offset: 0x000EE8BC
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			if (args.Length == 1)
			{
				return CompletionResult.FromHintOptions((from c in IoCManager.Resolve<IPlayerManager>().ServerSessions
				select c.Name into c
				orderby c
				select c).ToArray<string>(), Loc.GetString("cmd-ban-hint"));
			}
			if (args.Length == 2)
			{
				return CompletionResult.FromHint(Loc.GetString("cmd-ban-hint-reason"));
			}
			if (args.Length == 3)
			{
				return CompletionResult.FromHintOptions(new CompletionOption[]
				{
					new CompletionOption("0", Loc.GetString("cmd-ban-hint-duration-1"), 0),
					new CompletionOption("1440", Loc.GetString("cmd-ban-hint-duration-2"), 0),
					new CompletionOption("4320", Loc.GetString("cmd-ban-hint-duration-3"), 0),
					new CompletionOption("10080", Loc.GetString("cmd-ban-hint-duration-4"), 0),
					new CompletionOption("20160", Loc.GetString("cmd-ban-hint-duration-5"), 0),
					new CompletionOption("43800", Loc.GetString("cmd-ban-hint-duration-6"), 0)
				}, Loc.GetString("cmd-ban-hint-duration"));
			}
			if (args.Length == 4)
			{
				return CompletionResult.FromHintOptions(new CompletionOption[]
				{
					new CompletionOption("true", "Kick", 0),
					new CompletionOption("false", "Don't kick", 0)
				}, "[kick]");
			}
			return CompletionResult.Empty;
		}

		// Token: 0x06002DF9 RID: 11769 RVA: 0x000F085D File Offset: 0x000EEA5D
		private bool ParseMinutes(IConsoleShell shell, string arg, out uint minutes)
		{
			if (!uint.TryParse(arg, out minutes))
			{
				shell.WriteLine(arg + " is not a valid amount of minutes.\n" + this.Help);
				return false;
			}
			return true;
		}
	}
}
