using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Console;

namespace Content.Client.Administration.Commands
{
	// Token: 0x020004EA RID: 1258
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class LoadPrototypeCommand : IConsoleCommand
	{
		// Token: 0x170006F1 RID: 1777
		// (get) Token: 0x06001FFC RID: 8188 RVA: 0x000B9F5F File Offset: 0x000B815F
		public string Command { get; } = "loadprototype";

		// Token: 0x170006F2 RID: 1778
		// (get) Token: 0x06001FFD RID: 8189 RVA: 0x000B9F67 File Offset: 0x000B8167
		public string Description { get; } = "Load a prototype file into the server.";

		// Token: 0x170006F3 RID: 1779
		// (get) Token: 0x06001FFE RID: 8190 RVA: 0x000B9F6F File Offset: 0x000B816F
		public string Help
		{
			get
			{
				return this.Command;
			}
		}

		// Token: 0x06001FFF RID: 8191 RVA: 0x000AED3F File Offset: 0x000ACF3F
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			LoadPrototypeCommand.LoadPrototype();
		}

		// Token: 0x06002000 RID: 8192 RVA: 0x000B9F78 File Offset: 0x000B8178
		public static void LoadPrototype()
		{
			LoadPrototypeCommand.<LoadPrototype>d__9 <LoadPrototype>d__;
			<LoadPrototype>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<LoadPrototype>d__.<>1__state = -1;
			<LoadPrototype>d__.<>t__builder.Start<LoadPrototypeCommand.<LoadPrototype>d__9>(ref <LoadPrototype>d__);
		}
	}
}
