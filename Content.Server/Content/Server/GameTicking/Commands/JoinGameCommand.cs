using System;
using System.Runtime.CompilerServices;
using Content.Server.White.Stalin;
using Content.Shared.Administration;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Server.GameTicking.Commands
{
	// Token: 0x020004D7 RID: 1239
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	internal sealed class JoinGameCommand : IConsoleCommand
	{
		// Token: 0x170003C0 RID: 960
		// (get) Token: 0x06001991 RID: 6545 RVA: 0x00086615 File Offset: 0x00084815
		public string Command
		{
			get
			{
				return "joingame";
			}
		}

		// Token: 0x170003C1 RID: 961
		// (get) Token: 0x06001992 RID: 6546 RVA: 0x0008661C File Offset: 0x0008481C
		public string Description
		{
			get
			{
				return "";
			}
		}

		// Token: 0x170003C2 RID: 962
		// (get) Token: 0x06001993 RID: 6547 RVA: 0x00086623 File Offset: 0x00084823
		public string Help
		{
			get
			{
				return "";
			}
		}

		// Token: 0x06001994 RID: 6548 RVA: 0x0008662A File Offset: 0x0008482A
		public JoinGameCommand()
		{
			IoCManager.InjectDependencies<JoinGameCommand>(this);
		}

		// Token: 0x06001995 RID: 6549 RVA: 0x0008663C File Offset: 0x0008483C
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			JoinGameCommand.<Execute>d__10 <Execute>d__;
			<Execute>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Execute>d__.<>4__this = this;
			<Execute>d__.shell = shell;
			<Execute>d__.args = args;
			<Execute>d__.<>1__state = -1;
			<Execute>d__.<>t__builder.Start<JoinGameCommand.<Execute>d__10>(ref <Execute>d__);
		}

		// Token: 0x04001025 RID: 4133
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04001026 RID: 4134
		[Dependency]
		private readonly StalinManager _stalinManager;

		// Token: 0x04001027 RID: 4135
		[Dependency]
		private readonly IConfigurationManager _configurationManager;
	}
}
