using System;
using System.Runtime.CompilerServices;
using Content.Client.Administration.Managers;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.NodeContainer
{
	// Token: 0x02000218 RID: 536
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class NodeVisCommand : IConsoleCommand
	{
		// Token: 0x170002FD RID: 765
		// (get) Token: 0x06000E04 RID: 3588 RVA: 0x00054CB7 File Offset: 0x00052EB7
		public string Command
		{
			get
			{
				return "nodevis";
			}
		}

		// Token: 0x170002FE RID: 766
		// (get) Token: 0x06000E05 RID: 3589 RVA: 0x00054CBE File Offset: 0x00052EBE
		public string Description
		{
			get
			{
				return "Toggles node group visualization";
			}
		}

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x06000E06 RID: 3590 RVA: 0x00054CC5 File Offset: 0x00052EC5
		public string Help
		{
			get
			{
				return "";
			}
		}

		// Token: 0x06000E07 RID: 3591 RVA: 0x00054CCC File Offset: 0x00052ECC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (!IoCManager.Resolve<IClientAdminManager>().HasFlag(AdminFlags.Debug))
			{
				shell.WriteError("You need +DEBUG for this command");
				return;
			}
			NodeGroupSystem nodeGroupSystem = EntitySystem.Get<NodeGroupSystem>();
			nodeGroupSystem.SetVisEnabled(!nodeGroupSystem.VisEnabled);
		}
	}
}
