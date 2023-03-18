using System;
using System.Runtime.CompilerServices;
using Content.Client.Atmos.EntitySystems;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;

namespace Content.Client.Commands
{
	// Token: 0x020003A2 RID: 930
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class AtvCBMCommand : IConsoleCommand
	{
		// Token: 0x170004A7 RID: 1191
		// (get) Token: 0x0600172C RID: 5932 RVA: 0x0008653E File Offset: 0x0008473E
		public string Command
		{
			get
			{
				return "atvcbm";
			}
		}

		// Token: 0x170004A8 RID: 1192
		// (get) Token: 0x0600172D RID: 5933 RVA: 0x00086545 File Offset: 0x00084745
		public string Description
		{
			get
			{
				return "Changes from red/green/blue to greyscale";
			}
		}

		// Token: 0x170004A9 RID: 1193
		// (get) Token: 0x0600172E RID: 5934 RVA: 0x0008654C File Offset: 0x0008474C
		public string Help
		{
			get
			{
				return "atvcbm <true/false>";
			}
		}

		// Token: 0x0600172F RID: 5935 RVA: 0x00086554 File Offset: 0x00084754
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteLine(this.Help);
				return;
			}
			bool cfgCBM;
			if (!bool.TryParse(args[0], out cfgCBM))
			{
				shell.WriteLine("Invalid flag");
				return;
			}
			EntitySystem.Get<AtmosDebugOverlaySystem>().CfgCBM = cfgCBM;
		}
	}
}
