using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Administration;
using Content.Shared.Atmos;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Content.Server.Atmos.Commands
{
	// Token: 0x020007B4 RID: 1972
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class FillGas : IConsoleCommand
	{
		// Token: 0x17000693 RID: 1683
		// (get) Token: 0x06002AC3 RID: 10947 RVA: 0x000E06E0 File Offset: 0x000DE8E0
		public string Command
		{
			get
			{
				return "fillgas";
			}
		}

		// Token: 0x17000694 RID: 1684
		// (get) Token: 0x06002AC4 RID: 10948 RVA: 0x000E06E7 File Offset: 0x000DE8E7
		public string Description
		{
			get
			{
				return "Adds gas to all tiles in a grid.";
			}
		}

		// Token: 0x17000695 RID: 1685
		// (get) Token: 0x06002AC5 RID: 10949 RVA: 0x000E06EE File Offset: 0x000DE8EE
		public string Help
		{
			get
			{
				return "fillgas <GridEid> <Gas> <moles>";
			}
		}

		// Token: 0x06002AC6 RID: 10950 RVA: 0x000E06F8 File Offset: 0x000DE8F8
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length < 3)
			{
				return;
			}
			EntityUid gridId;
			int gasId;
			float moles;
			if (!EntityUid.TryParse(args[0], ref gridId) || !AtmosCommandUtils.TryParseGasID(args[1], out gasId) || !float.TryParse(args[2], out moles))
			{
				return;
			}
			MapGridComponent grid;
			if (!IoCManager.Resolve<IMapManager>().TryGetGrid(new EntityUid?(gridId), ref grid))
			{
				shell.WriteLine("Invalid grid ID.");
				return;
			}
			foreach (GasMixture gasMixture in EntitySystem.Get<AtmosphereSystem>().GetAllMixtures(grid.Owner, true))
			{
				gasMixture.AdjustMoles(gasId, moles);
			}
		}
	}
}
