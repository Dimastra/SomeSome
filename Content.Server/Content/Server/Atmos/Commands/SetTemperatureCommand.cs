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
using Robust.Shared.Maths;

namespace Content.Server.Atmos.Commands
{
	// Token: 0x020007B8 RID: 1976
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class SetTemperatureCommand : IConsoleCommand
	{
		// Token: 0x1700069F RID: 1695
		// (get) Token: 0x06002AD7 RID: 10967 RVA: 0x000E0A24 File Offset: 0x000DEC24
		public string Command
		{
			get
			{
				return "settemp";
			}
		}

		// Token: 0x170006A0 RID: 1696
		// (get) Token: 0x06002AD8 RID: 10968 RVA: 0x000E0A2B File Offset: 0x000DEC2B
		public string Description
		{
			get
			{
				return "Sets a tile's temperature (in kelvin).";
			}
		}

		// Token: 0x170006A1 RID: 1697
		// (get) Token: 0x06002AD9 RID: 10969 RVA: 0x000E0A32 File Offset: 0x000DEC32
		public string Help
		{
			get
			{
				return "Usage: settemp <X> <Y> <GridId> <Temperature>";
			}
		}

		// Token: 0x06002ADA RID: 10970 RVA: 0x000E0A3C File Offset: 0x000DEC3C
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length < 4)
			{
				return;
			}
			int x;
			int y;
			EntityUid gridId;
			float temperature;
			if (!int.TryParse(args[0], out x) || !int.TryParse(args[1], out y) || !EntityUid.TryParse(args[2], ref gridId) || !float.TryParse(args[3], out temperature))
			{
				return;
			}
			if (temperature < 2.7f)
			{
				shell.WriteLine("Invalid temperature.");
				return;
			}
			MapGridComponent grid;
			if (!this._mapManager.TryGetGrid(new EntityUid?(gridId), ref grid))
			{
				shell.WriteError("Invalid grid.");
				return;
			}
			AtmosphereSystem entitySystem = this._entities.EntitySysManager.GetEntitySystem<AtmosphereSystem>();
			Vector2i indices;
			indices..ctor(x, y);
			GasMixture tile = entitySystem.GetTileMixture(new EntityUid?(grid.Owner), null, indices, true);
			if (tile == null)
			{
				shell.WriteLine("Invalid coordinates or tile.");
				return;
			}
			tile.Temperature = temperature;
		}

		// Token: 0x04001A84 RID: 6788
		[Dependency]
		private readonly IEntityManager _entities;

		// Token: 0x04001A85 RID: 6789
		[Dependency]
		private readonly IMapManager _mapManager;
	}
}
