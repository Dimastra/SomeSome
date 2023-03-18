using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;

namespace Content.Server.Administration.Commands
{
	// Token: 0x0200083D RID: 2109
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class DirtyCommand : IConsoleCommand
	{
		// Token: 0x17000741 RID: 1857
		// (get) Token: 0x06002E28 RID: 11816 RVA: 0x000F1184 File Offset: 0x000EF384
		public string Command
		{
			get
			{
				return "dirty";
			}
		}

		// Token: 0x17000742 RID: 1858
		// (get) Token: 0x06002E29 RID: 11817 RVA: 0x000F118B File Offset: 0x000EF38B
		public string Description
		{
			get
			{
				return "Marks all components on an entity as dirty, if not specified, dirties everything";
			}
		}

		// Token: 0x17000743 RID: 1859
		// (get) Token: 0x06002E2A RID: 11818 RVA: 0x000F1192 File Offset: 0x000EF392
		public string Help
		{
			get
			{
				return "Usage: " + this.Command + " [entityUid]";
			}
		}

		// Token: 0x06002E2B RID: 11819 RVA: 0x000F11AC File Offset: 0x000EF3AC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			DirtyCommand.<Execute>d__6 <Execute>d__;
			<Execute>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Execute>d__.shell = shell;
			<Execute>d__.args = args;
			<Execute>d__.<>1__state = -1;
			<Execute>d__.<>t__builder.Start<DirtyCommand.<Execute>d__6>(ref <Execute>d__);
		}

		// Token: 0x06002E2C RID: 11820 RVA: 0x000F11EC File Offset: 0x000EF3EC
		private static void DirtyAll(IEntityManager manager, EntityUid entityUid)
		{
			foreach (IComponent component in manager.GetComponents(entityUid))
			{
				manager.Dirty((Component)component, null);
			}
		}
	}
}
