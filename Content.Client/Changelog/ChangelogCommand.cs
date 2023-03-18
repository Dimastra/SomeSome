using System;
using System.Runtime.CompilerServices;
using Content.Client.UserInterface.Systems.EscapeMenu;
using Content.Shared.Administration;
using Robust.Client.UserInterface;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Client.Changelog
{
	// Token: 0x020003F9 RID: 1017
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	public sealed class ChangelogCommand : IConsoleCommand
	{
		// Token: 0x17000522 RID: 1314
		// (get) Token: 0x060018F8 RID: 6392 RVA: 0x0008F77E File Offset: 0x0008D97E
		public string Command
		{
			get
			{
				return "changelog";
			}
		}

		// Token: 0x17000523 RID: 1315
		// (get) Token: 0x060018F9 RID: 6393 RVA: 0x0008F785 File Offset: 0x0008D985
		public string Description
		{
			get
			{
				return "Opens the changelog";
			}
		}

		// Token: 0x17000524 RID: 1316
		// (get) Token: 0x060018FA RID: 6394 RVA: 0x0008F78C File Offset: 0x0008D98C
		public string Help
		{
			get
			{
				return "Usage: changelog";
			}
		}

		// Token: 0x060018FB RID: 6395 RVA: 0x0008F793 File Offset: 0x0008D993
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IoCManager.Resolve<IUserInterfaceManager>().GetUIController<ChangelogUIController>().OpenWindow();
		}
	}
}
