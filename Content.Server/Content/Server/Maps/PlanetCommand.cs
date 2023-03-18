using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Atmos;
using Content.Server.Atmos.Components;
using Content.Shared.Administration;
using Content.Shared.Atmos;
using Content.Shared.Gravity;
using Content.Shared.Parallax.Biomes;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Maps
{
	// Token: 0x020003D9 RID: 985
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Mapping)]
	public sealed class PlanetCommand : IConsoleCommand
	{
		// Token: 0x170002DC RID: 732
		// (get) Token: 0x06001443 RID: 5187 RVA: 0x00068C18 File Offset: 0x00066E18
		public string Command
		{
			get
			{
				return "planet";
			}
		}

		// Token: 0x170002DD RID: 733
		// (get) Token: 0x06001444 RID: 5188 RVA: 0x00068C1F File Offset: 0x00066E1F
		public string Description
		{
			get
			{
				return Loc.GetString("cmd-planet-desc");
			}
		}

		// Token: 0x170002DE RID: 734
		// (get) Token: 0x06001445 RID: 5189 RVA: 0x00068C2B File Offset: 0x00066E2B
		public string Help
		{
			get
			{
				return Loc.GetString("cmd-planet-help", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("command", this.Command)
				});
			}
		}

		// Token: 0x06001446 RID: 5190 RVA: 0x00068C54 File Offset: 0x00066E54
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 2)
			{
				shell.WriteError(Loc.GetString("cmd-planet-args"));
				return;
			}
			int mapInt;
			if (!int.TryParse(args[0], out mapInt))
			{
				shell.WriteError(Loc.GetString("cmd-planet-map", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("map", mapInt)
				}));
				return;
			}
			MapId mapId;
			mapId..ctor(mapInt);
			if (!this._mapManager.MapExists(mapId))
			{
				shell.WriteError(Loc.GetString("cmd-planet-map", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("map", mapId)
				}));
				return;
			}
			if (!this._protoManager.HasIndex<BiomePrototype>(args[1]))
			{
				shell.WriteError(Loc.GetString("cmd-planet-map-prototype", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("prototype", args[1])
				}));
				return;
			}
			EntityUid mapUid = this._mapManager.GetMapEntityId(mapId);
			MetaDataComponent metadata = null;
			BiomeComponent biome = this._entManager.EnsureComponent<BiomeComponent>(mapUid);
			biome.BiomePrototype = args[1];
			biome.Seed = this._random.Next();
			this._entManager.Dirty(biome, null);
			GravityComponent gravity = this._entManager.EnsureComponent<GravityComponent>(mapUid);
			gravity.Enabled = true;
			this._entManager.Dirty(gravity, metadata);
			MapLightComponent light = this._entManager.EnsureComponent<MapLightComponent>(mapUid);
			light.AmbientLightColor = Color.FromHex("#D8B059", null);
			this._entManager.Dirty(light, metadata);
			MapAtmosphereComponent mapAtmosphereComponent = this._entManager.EnsureComponent<MapAtmosphereComponent>(mapUid);
			mapAtmosphereComponent.Space = false;
			float[] moles = new float[Atmospherics.AdjustedNumberOfGases];
			moles[0] = 21.82478f;
			moles[1] = 82.10312f;
			mapAtmosphereComponent.Mixture = new GasMixture(2500f)
			{
				Temperature = 293.15f,
				Moles = moles
			};
			this._entManager.EnsureComponent<MapGridComponent>(mapUid);
			shell.WriteLine(Loc.GetString("cmd-planet-success", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("mapId", mapId)
			}));
		}

		// Token: 0x06001447 RID: 5191 RVA: 0x00068E68 File Offset: 0x00067068
		public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
		{
			if (args.Length == 1)
			{
				return CompletionResult.FromOptions(from o in this._entManager.EntityQuery<MapComponent>(true)
				select new CompletionOption(o.WorldMap.ToString(), "MapId", 0));
			}
			if (args.Length == 2)
			{
				return CompletionResult.FromOptions(from o in this._protoManager.EnumeratePrototypes<BiomePrototype>()
				select new CompletionOption(o.ID, "Biome", 0));
			}
			return CompletionResult.Empty;
		}

		// Token: 0x04000C84 RID: 3204
		[Dependency]
		private readonly IEntityManager _entManager;

		// Token: 0x04000C85 RID: 3205
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000C86 RID: 3206
		[Dependency]
		private readonly IPrototypeManager _protoManager;

		// Token: 0x04000C87 RID: 3207
		[Dependency]
		private readonly IRobustRandom _random;
	}
}
