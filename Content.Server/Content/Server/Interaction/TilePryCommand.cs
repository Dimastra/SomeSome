using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.Maps;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Content.Server.Interaction
{
	// Token: 0x02000447 RID: 1095
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	internal sealed class TilePryCommand : IConsoleCommand
	{
		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x06001612 RID: 5650 RVA: 0x00074C18 File Offset: 0x00072E18
		public string Command
		{
			get
			{
				return "tilepry";
			}
		}

		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x06001613 RID: 5651 RVA: 0x00074C1F File Offset: 0x00072E1F
		public string Description
		{
			get
			{
				return "Pries up all tiles in a radius around the user.";
			}
		}

		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x06001614 RID: 5652 RVA: 0x00074C26 File Offset: 0x00072E26
		public string Help
		{
			get
			{
				return "Usage: " + this.Command + " <radius>";
			}
		}

		// Token: 0x06001615 RID: 5653 RVA: 0x00074C40 File Offset: 0x00072E40
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession playerSession = shell.Player as IPlayerSession;
			EntityUid? entityUid = (playerSession != null) ? playerSession.AttachedEntity : null;
			if (entityUid == null)
			{
				return;
			}
			EntityUid attached = entityUid.GetValueOrDefault();
			if (args.Length != 1)
			{
				shell.WriteLine(this.Help);
				return;
			}
			int radius;
			if (!int.TryParse(args[0], out radius))
			{
				shell.WriteLine(args[0] + " isn't a valid integer.");
				return;
			}
			if (radius < 0)
			{
				shell.WriteLine("Radius must be positive.");
				return;
			}
			IMapManager mapManager = IoCManager.Resolve<IMapManager>();
			TransformComponent xform = this._entities.GetComponent<TransformComponent>(attached);
			EntityUid? playerGrid = xform.GridUid;
			MapGridComponent mapGrid;
			if (!mapManager.TryGetGrid(playerGrid, ref mapGrid))
			{
				return;
			}
			EntityCoordinates playerPosition = xform.Coordinates;
			ITileDefinitionManager tileDefinitionManager = IoCManager.Resolve<ITileDefinitionManager>();
			for (int i = -radius; i <= radius; i++)
			{
				for (int j = -radius; j <= radius; j++)
				{
					TileRef tile = mapGrid.GetTileRef(playerPosition.Offset(new ValueTuple<float, float>((float)i, (float)j)));
					EntityCoordinates coordinates = mapGrid.GridTileToLocal(tile.GridIndices);
					if (((ContentTileDefinition)tileDefinitionManager[(int)tile.Tile.TypeId]).CanCrowbar)
					{
						ITileDefinition underplating = tileDefinitionManager["UnderPlating"];
						mapGrid.SetTile(coordinates, new Tile(underplating.TileId, 0, 0));
					}
				}
			}
		}

		// Token: 0x04000DD3 RID: 3539
		[Dependency]
		private readonly IEntityManager _entities;
	}
}
