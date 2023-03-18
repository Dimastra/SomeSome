using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Content.Server.Construction.Commands
{
	// Token: 0x0200062A RID: 1578
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Mapping)]
	internal sealed class TileReplaceCommand : IConsoleCommand
	{
		// Token: 0x17000520 RID: 1312
		// (get) Token: 0x0600219D RID: 8605 RVA: 0x000AF140 File Offset: 0x000AD340
		public string Command
		{
			get
			{
				return "tilereplace";
			}
		}

		// Token: 0x17000521 RID: 1313
		// (get) Token: 0x0600219E RID: 8606 RVA: 0x000AF147 File Offset: 0x000AD347
		public string Description
		{
			get
			{
				return "Replaces one tile with another.";
			}
		}

		// Token: 0x17000522 RID: 1314
		// (get) Token: 0x0600219F RID: 8607 RVA: 0x000AF14E File Offset: 0x000AD34E
		public string Help
		{
			get
			{
				return "Usage: " + this.Command + " [<gridId>] <src> <dst>";
			}
		}

		// Token: 0x060021A0 RID: 8608 RVA: 0x000AF168 File Offset: 0x000AD368
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			int num = args.Length;
			EntityUid? gridId;
			string tileIdA;
			string tileIdB;
			if (num == 2)
			{
				EntityUid? entityUid = (player != null) ? player.AttachedEntity : null;
				if (entityUid != null)
				{
					EntityUid playerEntity = entityUid.GetValueOrDefault();
					if (playerEntity.Valid)
					{
						gridId = entityManager.GetComponent<TransformComponent>(playerEntity).GridUid;
						tileIdA = args[0];
						tileIdB = args[1];
						goto IL_C3;
					}
				}
				shell.WriteLine("Only a player can run this command without a grid ID.");
				return;
			}
			if (num != 3)
			{
				shell.WriteLine(this.Help);
				return;
			}
			EntityUid id;
			if (!EntityUid.TryParse(args[0], ref id))
			{
				shell.WriteLine(args[0] + " is not a valid entity.");
				return;
			}
			gridId = new EntityUid?(id);
			tileIdA = args[1];
			tileIdB = args[2];
			IL_C3:
			ITileDefinitionManager tileDefinitionManager = IoCManager.Resolve<ITileDefinitionManager>();
			ITileDefinition tileA = tileDefinitionManager[tileIdA];
			ITileDefinition tileB = tileDefinitionManager[tileIdB];
			MapGridComponent grid;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			if (!IoCManager.Resolve<IMapManager>().TryGetGrid(gridId, ref grid))
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 1);
				defaultInterpolatedStringHandler.AppendLiteral("No grid exists with id ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid?>(gridId);
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			if (!entityManager.EntityExists(grid.Owner))
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Grid ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid?>(gridId);
				defaultInterpolatedStringHandler.AppendLiteral(" doesn't have an associated grid entity.");
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			int changed = 0;
			foreach (TileRef tile in grid.GetAllTiles(true))
			{
				if (tile.Tile.TypeId == tileA.TileId)
				{
					grid.SetTile(tile.GridIndices, new Tile(tileB.TileId, 0, 0));
					changed++;
				}
			}
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Changed ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(changed);
			defaultInterpolatedStringHandler.AppendLiteral(" tiles.");
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
