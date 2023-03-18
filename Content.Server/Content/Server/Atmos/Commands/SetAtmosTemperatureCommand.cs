using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Content.Server.Atmos.Commands
{
	// Token: 0x020007B7 RID: 1975
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class SetAtmosTemperatureCommand : IConsoleCommand
	{
		// Token: 0x1700069C RID: 1692
		// (get) Token: 0x06002AD2 RID: 10962 RVA: 0x000E0905 File Offset: 0x000DEB05
		public string Command
		{
			get
			{
				return "setatmostemp";
			}
		}

		// Token: 0x1700069D RID: 1693
		// (get) Token: 0x06002AD3 RID: 10963 RVA: 0x000E090C File Offset: 0x000DEB0C
		public string Description
		{
			get
			{
				return "Sets a grid's temperature (in kelvin).";
			}
		}

		// Token: 0x1700069E RID: 1694
		// (get) Token: 0x06002AD4 RID: 10964 RVA: 0x000E0913 File Offset: 0x000DEB13
		public string Help
		{
			get
			{
				return "Usage: setatmostemp <GridId> <Temperature>";
			}
		}

		// Token: 0x06002AD5 RID: 10965 RVA: 0x000E091C File Offset: 0x000DEB1C
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length < 2)
			{
				return;
			}
			EntityUid gridId;
			float temperature;
			if (!EntityUid.TryParse(args[0], ref gridId) || !float.TryParse(args[1], out temperature))
			{
				return;
			}
			IMapManager mapMan = IoCManager.Resolve<IMapManager>();
			if (temperature < 2.7f)
			{
				shell.WriteLine("Invalid temperature.");
				return;
			}
			MapGridComponent gridComp;
			if (!gridId.IsValid() || !mapMan.TryGetGrid(new EntityUid?(gridId), ref gridComp))
			{
				shell.WriteLine("Invalid grid ID.");
				return;
			}
			AtmosphereSystem atmosphereSystem = EntitySystem.Get<AtmosphereSystem>();
			int tiles = 0;
			foreach (GasMixture gasMixture in atmosphereSystem.GetAllMixtures(gridComp.Owner, true))
			{
				tiles++;
				gasMixture.Temperature = temperature;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Changed the temperature of ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(tiles);
			defaultInterpolatedStringHandler.AppendLiteral(" tiles.");
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
