using System;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Content.Shared.Maps;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Random;

namespace Content.Server.Administration.Commands
{
	// Token: 0x0200086B RID: 2155
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Mapping)]
	public sealed class VariantizeCommand : IConsoleCommand
	{
		// Token: 0x170007C9 RID: 1993
		// (get) Token: 0x06002F1C RID: 12060 RVA: 0x000F3F5E File Offset: 0x000F215E
		public string Command
		{
			get
			{
				return "variantize";
			}
		}

		// Token: 0x170007CA RID: 1994
		// (get) Token: 0x06002F1D RID: 12061 RVA: 0x000F3F65 File Offset: 0x000F2165
		public string Description
		{
			get
			{
				return Loc.GetString("variantize-command-description");
			}
		}

		// Token: 0x170007CB RID: 1995
		// (get) Token: 0x06002F1E RID: 12062 RVA: 0x000F3F71 File Offset: 0x000F2171
		public string Help
		{
			get
			{
				return Loc.GetString("variantize-command-help-text");
			}
		}

		// Token: 0x06002F1F RID: 12063 RVA: 0x000F3F80 File Offset: 0x000F2180
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
				return;
			}
			IEntityManager entMan = IoCManager.Resolve<IEntityManager>();
			IRobustRandom random = IoCManager.Resolve<IRobustRandom>();
			EntityUid euid;
			if (!EntityUid.TryParse(args[0], ref euid))
			{
				shell.WriteError("Failed to parse euid '" + args[0] + "'.");
				return;
			}
			MapGridComponent gridComp;
			if (!entMan.TryGetComponent<MapGridComponent>(euid, ref gridComp))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Euid '");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(euid);
				defaultInterpolatedStringHandler.AppendLiteral("' does not exist or is not a grid.");
				shell.WriteError(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			foreach (TileRef tile in gridComp.GetAllTiles(true))
			{
				ContentTileDefinition def = tile.GetContentTileDefinition(null);
				Tile newTile;
				newTile..ctor(tile.Tile.TypeId, tile.Tile.Flags, RandomExtensions.Pick<byte>(random, def.PlacementVariants));
				gridComp.SetTile(tile.GridIndices, newTile);
			}
		}
	}
}
