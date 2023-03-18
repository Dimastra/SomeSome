using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Robust.Shared.Console;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000846 RID: 2118
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.ViewNotes)]
	public sealed class OpenAdminNotesCommand : IConsoleCommand
	{
		// Token: 0x1700075C RID: 1884
		// (get) Token: 0x06002E57 RID: 11863 RVA: 0x000F1C6E File Offset: 0x000EFE6E
		public string Command
		{
			get
			{
				return "adminnotes";
			}
		}

		// Token: 0x1700075D RID: 1885
		// (get) Token: 0x06002E58 RID: 11864 RVA: 0x000F1C75 File Offset: 0x000EFE75
		public string Description
		{
			get
			{
				return "Opens the admin notes panel.";
			}
		}

		// Token: 0x1700075E RID: 1886
		// (get) Token: 0x06002E59 RID: 11865 RVA: 0x000F1C7C File Offset: 0x000EFE7C
		public string Help
		{
			get
			{
				return "Usage: " + this.Command + " <notedPlayerUserId OR notedPlayerUsername>";
			}
		}

		// Token: 0x06002E5A RID: 11866 RVA: 0x000F1C94 File Offset: 0x000EFE94
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			OpenAdminNotesCommand.<Execute>d__7 <Execute>d__;
			<Execute>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Execute>d__.<>4__this = this;
			<Execute>d__.shell = shell;
			<Execute>d__.args = args;
			<Execute>d__.<>1__state = -1;
			<Execute>d__.<>t__builder.Start<OpenAdminNotesCommand.<Execute>d__7>(ref <Execute>d__);
		}

		// Token: 0x04001C50 RID: 7248
		public const string CommandName = "adminnotes";
	}
}
