using System;
using System.Runtime.CompilerServices;
using Content.Client.Atmos.EntitySystems;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;

namespace Content.Client.Commands
{
	// Token: 0x020003A0 RID: 928
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class AtvRangeCommand : IConsoleCommand
	{
		// Token: 0x170004A1 RID: 1185
		// (get) Token: 0x06001722 RID: 5922 RVA: 0x000863E4 File Offset: 0x000845E4
		public string Command
		{
			get
			{
				return "atvrange";
			}
		}

		// Token: 0x170004A2 RID: 1186
		// (get) Token: 0x06001723 RID: 5923 RVA: 0x000863EB File Offset: 0x000845EB
		public string Description
		{
			get
			{
				return "Sets the atmos debug range (as two floats, start [red] and end [blue])";
			}
		}

		// Token: 0x170004A3 RID: 1187
		// (get) Token: 0x06001724 RID: 5924 RVA: 0x000863F2 File Offset: 0x000845F2
		public string Help
		{
			get
			{
				return "atvrange <start> <end>";
			}
		}

		// Token: 0x06001725 RID: 5925 RVA: 0x000863FC File Offset: 0x000845FC
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 2)
			{
				shell.WriteLine(this.Help);
				return;
			}
			float num;
			if (!float.TryParse(args[0], out num))
			{
				shell.WriteLine("Bad float START");
				return;
			}
			float num2;
			if (!float.TryParse(args[1], out num2))
			{
				shell.WriteLine("Bad float END");
				return;
			}
			if (num == num2)
			{
				shell.WriteLine("Scale cannot be zero, as this would cause a division by zero in AtmosDebugOverlay.");
				return;
			}
			AtmosDebugOverlaySystem atmosDebugOverlaySystem = EntitySystem.Get<AtmosDebugOverlaySystem>();
			atmosDebugOverlaySystem.CfgBase = num;
			atmosDebugOverlaySystem.CfgScale = num2 - num;
		}
	}
}
