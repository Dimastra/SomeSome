using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.Localization;

namespace Content.Server.Utility.Commands
{
	// Token: 0x020000F1 RID: 241
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	internal sealed class EchoCommand : IConsoleCommand
	{
		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x06000469 RID: 1129 RVA: 0x000158C0 File Offset: 0x00013AC0
		public string Command
		{
			get
			{
				return "echo";
			}
		}

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x0600046A RID: 1130 RVA: 0x000158C7 File Offset: 0x00013AC7
		public string Description
		{
			get
			{
				return Loc.GetString("echo-command-description");
			}
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x0600046B RID: 1131 RVA: 0x000158D3 File Offset: 0x00013AD3
		public string Help
		{
			get
			{
				return Loc.GetString("echo-command-help-text", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("command", this.Command)
				});
			}
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x000158FC File Offset: 0x00013AFC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (argStr.Length > this.Command.Length)
			{
				shell.WriteLine(argStr.Substring(this.Command.Length + 1));
			}
		}
	}
}
