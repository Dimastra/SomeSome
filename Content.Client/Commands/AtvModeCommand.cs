using System;
using System.Runtime.CompilerServices;
using Content.Client.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;

namespace Content.Client.Commands
{
	// Token: 0x020003A1 RID: 929
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class AtvModeCommand : IConsoleCommand
	{
		// Token: 0x170004A4 RID: 1188
		// (get) Token: 0x06001727 RID: 5927 RVA: 0x00086470 File Offset: 0x00084670
		public string Command
		{
			get
			{
				return "atvmode";
			}
		}

		// Token: 0x170004A5 RID: 1189
		// (get) Token: 0x06001728 RID: 5928 RVA: 0x00086477 File Offset: 0x00084677
		public string Description
		{
			get
			{
				return "Sets the atmos debug mode. This will automatically reset the scale.";
			}
		}

		// Token: 0x170004A6 RID: 1190
		// (get) Token: 0x06001729 RID: 5929 RVA: 0x0008647E File Offset: 0x0008467E
		public string Help
		{
			get
			{
				return "atvmode <TotalMoles/GasMoles/Temperature> [<gas ID (for GasMoles)>]";
			}
		}

		// Token: 0x0600172A RID: 5930 RVA: 0x00086488 File Offset: 0x00084688
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length < 1)
			{
				shell.WriteLine(this.Help);
				return;
			}
			AtmosDebugOverlayMode atmosDebugOverlayMode;
			if (!Enum.TryParse<AtmosDebugOverlayMode>(args[0], out atmosDebugOverlayMode))
			{
				shell.WriteLine("Invalid mode");
				return;
			}
			int cfgSpecificGas = 0;
			float cfgBase = 0f;
			float cfgScale = 207.85599f;
			if (atmosDebugOverlayMode == AtmosDebugOverlayMode.GasMoles)
			{
				if (args.Length != 2)
				{
					shell.WriteLine("A target gas must be provided for this mode.");
					return;
				}
				if (!AtmosCommandUtils.TryParseGasID(args[1], out cfgSpecificGas))
				{
					shell.WriteLine("Gas ID not parsable or out of range.");
					return;
				}
			}
			else
			{
				if (args.Length != 1)
				{
					shell.WriteLine("No further information is required for this mode.");
					return;
				}
				if (atmosDebugOverlayMode == AtmosDebugOverlayMode.Temperature)
				{
					cfgBase = 373.15f;
					cfgScale = -160f;
				}
			}
			AtmosDebugOverlaySystem atmosDebugOverlaySystem = EntitySystem.Get<AtmosDebugOverlaySystem>();
			atmosDebugOverlaySystem.CfgMode = atmosDebugOverlayMode;
			atmosDebugOverlaySystem.CfgSpecificGas = cfgSpecificGas;
			atmosDebugOverlaySystem.CfgBase = cfgBase;
			atmosDebugOverlaySystem.CfgScale = cfgScale;
		}
	}
}
