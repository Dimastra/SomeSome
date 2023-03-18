using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.Maps;
using Content.Shared.Tag;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Content.Server.Construction.Commands
{
	// Token: 0x0200062B RID: 1579
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Mapping)]
	internal sealed class TileWallsCommand : IConsoleCommand
	{
		// Token: 0x17000523 RID: 1315
		// (get) Token: 0x060021A2 RID: 8610 RVA: 0x000AF38C File Offset: 0x000AD58C
		public string Command
		{
			get
			{
				return "tilewalls";
			}
		}

		// Token: 0x17000524 RID: 1316
		// (get) Token: 0x060021A3 RID: 8611 RVA: 0x000AF393 File Offset: 0x000AD593
		public string Description
		{
			get
			{
				return "Puts an underplating tile below every wall on a grid.";
			}
		}

		// Token: 0x17000525 RID: 1317
		// (get) Token: 0x060021A4 RID: 8612 RVA: 0x000AF39A File Offset: 0x000AD59A
		public string Help
		{
			get
			{
				return "Usage: " + this.Command + " <gridId> | " + this.Command;
			}
		}

		// Token: 0x060021A5 RID: 8613 RVA: 0x000AF3B8 File Offset: 0x000AD5B8
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			int num = args.Length;
			EntityUid? gridId;
			if (num == 0)
			{
				EntityUid? entityUid = (player != null) ? player.AttachedEntity : null;
				if (entityUid != null)
				{
					EntityUid playerEntity = entityUid.GetValueOrDefault();
					if (playerEntity.Valid)
					{
						gridId = entityManager.GetComponent<TransformComponent>(playerEntity).GridUid;
						goto IL_AD;
					}
				}
				shell.WriteLine("Only a player can run this command.");
				return;
			}
			if (num != 1)
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
			IL_AD:
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
			ITileDefinitionManager tileDefinitionManager = IoCManager.Resolve<ITileDefinitionManager>();
			TagSystem tagSystem = entityManager.EntitySysManager.GetEntitySystem<TagSystem>();
			ITileDefinition underplating = tileDefinitionManager["Plating"];
			Tile underplatingTile;
			underplatingTile..ctor(underplating.TileId, 0, 0);
			int changed = 0;
			foreach (EntityUid child in entityManager.GetComponent<TransformComponent>(grid.Owner).ChildEntities)
			{
				if (entityManager.EntityExists(child) && tagSystem.HasTag(child, "Wall"))
				{
					TransformComponent childTransform = entityManager.GetComponent<TransformComponent>(child);
					if (childTransform.Anchored)
					{
						TileRef tile = grid.GetTileRef(childTransform.Coordinates);
						if (!(((ContentTileDefinition)tileDefinitionManager[(int)tile.Tile.TypeId]).ID == "Plating"))
						{
							grid.SetTile(childTransform.Coordinates, underplatingTile);
							changed++;
						}
					}
				}
			}
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Changed ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(changed);
			defaultInterpolatedStringHandler.AppendLiteral(" tiles.");
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x0400149A RID: 5274
		public const string TilePrototypeId = "Plating";

		// Token: 0x0400149B RID: 5275
		public const string WallTag = "Wall";
	}
}
