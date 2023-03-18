using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Explosion.EntitySystems;
using Content.Shared.Administration;
using Content.Shared.Explosion;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Players;
using Robust.Shared.Prototypes;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000840 RID: 2112
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class ExplosionCommand : IConsoleCommand
	{
		// Token: 0x1700074A RID: 1866
		// (get) Token: 0x06002E38 RID: 11832 RVA: 0x000F136A File Offset: 0x000EF56A
		public string Command
		{
			get
			{
				return "explosion";
			}
		}

		// Token: 0x1700074B RID: 1867
		// (get) Token: 0x06002E39 RID: 11833 RVA: 0x000F1371 File Offset: 0x000EF571
		public string Description
		{
			get
			{
				return "Train go boom";
			}
		}

		// Token: 0x1700074C RID: 1868
		// (get) Token: 0x06002E3A RID: 11834 RVA: 0x000F1378 File Offset: 0x000EF578
		public string Help
		{
			get
			{
				return "Usage: explosion [intensity] [slope] [maxIntensity] [x y] [mapId] [prototypeId]";
			}
		}

		// Token: 0x06002E3B RID: 11835 RVA: 0x000F1380 File Offset: 0x000EF580
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length == 0 || args.Length == 4 || args.Length > 7)
			{
				shell.WriteError("Wrong number of arguments.");
				return;
			}
			float intensity;
			if (!float.TryParse(args[0], out intensity))
			{
				shell.WriteError("Failed to parse intensity: " + args[0]);
				return;
			}
			float slope = 5f;
			if (args.Length > 1 && !float.TryParse(args[1], out slope))
			{
				shell.WriteError("Failed to parse float: " + args[1]);
				return;
			}
			float maxIntensity = 100f;
			if (args.Length > 2 && !float.TryParse(args[2], out maxIntensity))
			{
				shell.WriteError("Failed to parse float: " + args[2]);
				return;
			}
			float x = 0f;
			float y = 0f;
			if (args.Length > 4 && (!float.TryParse(args[3], out x) || !float.TryParse(args[4], out y)))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Failed to parse coordinates: ");
				defaultInterpolatedStringHandler.AppendFormatted<ValueTuple<string, string>>(new ValueTuple<string, string>(args[3], args[4]));
				shell.WriteError(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			MapCoordinates coords;
			if (args.Length > 5)
			{
				int parsed;
				if (!int.TryParse(args[5], out parsed))
				{
					shell.WriteError("Failed to parse map ID: " + args[5]);
					return;
				}
				coords..ctor(new ValueTuple<float, float>(x, y), new MapId(parsed));
			}
			else
			{
				IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
				ICommonSession player = shell.Player;
				TransformComponent xform;
				if (!entityManager.TryGetComponent<TransformComponent>((player != null) ? player.AttachedEntity : null, ref xform))
				{
					shell.WriteError("Failed get default coordinates/map via player's transform. Need to specify explicitly.");
					return;
				}
				if (args.Length > 4)
				{
					coords..ctor(new ValueTuple<float, float>(x, y), xform.MapID);
				}
				else
				{
					coords = xform.MapPosition;
				}
			}
			IPrototypeManager protoMan = IoCManager.Resolve<IPrototypeManager>();
			ExplosionPrototype type;
			if (args.Length > 6)
			{
				if (!protoMan.TryIndex<ExplosionPrototype>(args[6], ref type))
				{
					shell.WriteError("Unknown explosion prototype: " + args[6]);
					return;
				}
			}
			else
			{
				type = protoMan.EnumeratePrototypes<ExplosionPrototype>().FirstOrDefault<ExplosionPrototype>();
				if (type == null)
				{
					shell.WriteError("Prototype manager has no explosion prototypes?");
					return;
				}
			}
			IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<ExplosionSystem>().QueueExplosion(coords, type.ID, intensity, slope, maxIntensity, 1f, int.MaxValue, true, false, false);
		}
	}
}
