using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.Decals;
using Content.Shared.Maps;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Server.Decals.Commands
{
	// Token: 0x020005B2 RID: 1458
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Mapping)]
	public sealed class AddDecalCommand : IConsoleCommand
	{
		// Token: 0x17000478 RID: 1144
		// (get) Token: 0x06001E4A RID: 7754 RVA: 0x000A0C35 File Offset: 0x0009EE35
		public string Command
		{
			get
			{
				return "adddecal";
			}
		}

		// Token: 0x17000479 RID: 1145
		// (get) Token: 0x06001E4B RID: 7755 RVA: 0x000A0C3C File Offset: 0x0009EE3C
		public string Description
		{
			get
			{
				return "Creates a decal on the map";
			}
		}

		// Token: 0x1700047A RID: 1146
		// (get) Token: 0x06001E4C RID: 7756 RVA: 0x000A0C43 File Offset: 0x0009EE43
		public string Help
		{
			get
			{
				return this.Command + " <id> <x position> <y position> <gridId> [angle=<angle> zIndex=<zIndex> color=<color>]";
			}
		}

		// Token: 0x06001E4D RID: 7757 RVA: 0x000A0C58 File Offset: 0x0009EE58
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length < 4 || args.Length > 7)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(78, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Received invalid amount of arguments arguments. Expected 4 to 7, got ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(args.Length);
				defaultInterpolatedStringHandler.AppendLiteral(".\nUsage: ");
				defaultInterpolatedStringHandler.AppendFormatted(this.Help);
				shell.WriteError(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			if (!IoCManager.Resolve<IPrototypeManager>().HasIndex<DecalPrototype>(args[0]))
			{
				shell.WriteError("Cannot find decalprototype '" + args[0] + "'.");
			}
			float x;
			if (!float.TryParse(args[1], NumberStyles.Any, CultureInfo.InvariantCulture, out x))
			{
				shell.WriteError("Failed parsing x-coordinate '" + args[1] + "'.");
				return;
			}
			float y;
			if (!float.TryParse(args[2], NumberStyles.Any, CultureInfo.InvariantCulture, out y))
			{
				shell.WriteError("Failed parsing y-coordinate'" + args[2] + "'.");
				return;
			}
			IMapManager mapManager = IoCManager.Resolve<IMapManager>();
			EntityUid gridIdRaw;
			MapGridComponent grid;
			if (!EntityUid.TryParse(args[3], ref gridIdRaw) || !mapManager.TryGetGrid(new EntityUid?(gridIdRaw), ref grid))
			{
				shell.WriteError("Failed parsing gridId '" + args[3] + "'.");
				return;
			}
			EntityCoordinates coordinates;
			coordinates..ctor(grid.Owner, new Vector2(x, y));
			if (grid.GetTileRef(coordinates).IsSpace(null))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Cannot create decal on space tile at ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityCoordinates>(coordinates);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				shell.WriteError(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			Color? color = null;
			int zIndex = 0;
			Angle? rotation = null;
			if (args.Length > 4)
			{
				for (int i = 4; i < args.Length; i++)
				{
					string[] rawValue = args[i].Split('=', StringSplitOptions.None);
					if (rawValue.Length != 2)
					{
						shell.WriteError("Failed parsing parameter: '" + args[i] + "'");
						return;
					}
					string a = rawValue[0];
					if (!(a == "angle"))
					{
						if (!(a == "zIndex"))
						{
							if (!(a == "color"))
							{
								shell.WriteError("Unknown parameter key '" + rawValue[0] + "'.");
								return;
							}
							Color colorRaw;
							if (!Color.TryFromName(rawValue[1], ref colorRaw))
							{
								shell.WriteError("Failed parsing color '" + rawValue[1] + "'.");
								return;
							}
							color = new Color?(colorRaw);
						}
						else if (!int.TryParse(rawValue[1], NumberStyles.Any, CultureInfo.InvariantCulture, out zIndex))
						{
							shell.WriteError("Failed parsing zIndex '" + rawValue[1] + "'.");
							return;
						}
					}
					else
					{
						double degrees;
						if (!double.TryParse(rawValue[1], NumberStyles.Any, CultureInfo.InvariantCulture, out degrees))
						{
							shell.WriteError("Failed parsing angle '" + rawValue[1] + "'.");
							return;
						}
						rotation = new Angle?(Angle.FromDegrees(degrees));
					}
				}
			}
			uint uid;
			if (EntitySystem.Get<DecalSystem>().TryAddDecal(args[0], coordinates, out uid, color, rotation, zIndex, false))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Successfully created decal ");
				defaultInterpolatedStringHandler.AppendFormatted<uint>(uid);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			shell.WriteError("Failed adding decal.");
		}
	}
}
