using System;
using System.Runtime.CompilerServices;
using Content.Client.Actions;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;

namespace Content.Client.Commands
{
	// Token: 0x0200039E RID: 926
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	public sealed class LoadActionsCommand : IConsoleCommand
	{
		// Token: 0x1700049B RID: 1179
		// (get) Token: 0x06001718 RID: 5912 RVA: 0x00086315 File Offset: 0x00084515
		public string Command
		{
			get
			{
				return "loadacts";
			}
		}

		// Token: 0x1700049C RID: 1180
		// (get) Token: 0x06001719 RID: 5913 RVA: 0x0008631C File Offset: 0x0008451C
		public string Description
		{
			get
			{
				return "Loads action toolbar assignments from a user-file.";
			}
		}

		// Token: 0x1700049D RID: 1181
		// (get) Token: 0x0600171A RID: 5914 RVA: 0x00086323 File Offset: 0x00084523
		public string Help
		{
			get
			{
				return "Usage: " + this.Command + " <user resource path>";
			}
		}

		// Token: 0x0600171B RID: 5915 RVA: 0x0008633C File Offset: 0x0008453C
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteLine(this.Help);
				return;
			}
			try
			{
				EntitySystem.Get<ActionsSystem>().LoadActionAssignments(args[0], true);
			}
			catch
			{
				shell.WriteLine("Failed to load action assignments");
			}
		}
	}
}
