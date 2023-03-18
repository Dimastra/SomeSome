using System;
using System.Runtime.CompilerServices;
using Content.Client.Credits;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Client.Commands
{
	// Token: 0x020003A3 RID: 931
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	public sealed class CreditsCommand : IConsoleCommand
	{
		// Token: 0x170004AA RID: 1194
		// (get) Token: 0x06001731 RID: 5937 RVA: 0x00086597 File Offset: 0x00084797
		public string Command
		{
			get
			{
				return "credits";
			}
		}

		// Token: 0x170004AB RID: 1195
		// (get) Token: 0x06001732 RID: 5938 RVA: 0x0008659E File Offset: 0x0008479E
		public string Description
		{
			get
			{
				return "Opens the credits window";
			}
		}

		// Token: 0x170004AC RID: 1196
		// (get) Token: 0x06001733 RID: 5939 RVA: 0x00086597 File Offset: 0x00084797
		public string Help
		{
			get
			{
				return "credits";
			}
		}

		// Token: 0x06001734 RID: 5940 RVA: 0x00068779 File Offset: 0x00066979
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			new CreditsWindow().Open();
		}
	}
}
