using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.Weather;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Content.Server.Weather
{
	// Token: 0x020000AB RID: 171
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class WeatherSystem : SharedWeatherSystem
	{
		// Token: 0x060002AA RID: 682 RVA: 0x0000DE54 File Offset: 0x0000C054
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<WeatherComponent, ComponentGetState>(new ComponentEventRefHandler<WeatherComponent, ComponentGetState>(this.OnWeatherGetState), null, null);
			this._console.RegisterCommand("weather", Loc.GetString("cmd-weather-desc"), Loc.GetString("cmd-weather-help"), new ConCommandCallback(this.WeatherTwo), new ConCommandCompletionCallback(this.WeatherCompletion), false);
		}

		// Token: 0x060002AB RID: 683 RVA: 0x0000DEB8 File Offset: 0x0000C0B8
		private void OnWeatherGetState(EntityUid uid, WeatherComponent component, ref ComponentGetState args)
		{
			args.State = new SharedWeatherSystem.WeatherComponentState
			{
				Weather = component.Weather,
				EndTime = component.EndTime,
				StartTime = component.StartTime
			};
		}

		// Token: 0x060002AC RID: 684 RVA: 0x0000DEEC File Offset: 0x0000C0EC
		[AdminCommand(AdminFlags.Fun)]
		private void WeatherTwo(IConsoleShell shell, string argstr, string[] args)
		{
			if (args.Length != 2)
			{
				return;
			}
			int mapInt;
			if (!int.TryParse(args[0], out mapInt))
			{
				return;
			}
			MapId mapId;
			mapId..ctor(mapInt);
			if (!this.MapManager.MapExists(mapId))
			{
				return;
			}
			if (args[1].Equals("null"))
			{
				base.SetWeather(mapId, null);
				return;
			}
			WeatherPrototype weatherProto;
			if (this.ProtoMan.TryIndex<WeatherPrototype>(args[1], ref weatherProto))
			{
				base.SetWeather(mapId, weatherProto);
				return;
			}
			shell.WriteError("Unable to parse weather prototype");
		}

		// Token: 0x060002AD RID: 685 RVA: 0x0000DF64 File Offset: 0x0000C164
		private CompletionResult WeatherCompletion(IConsoleShell shell, string[] args)
		{
			List<CompletionOption> options = new List<CompletionOption>();
			if (args.Length == 1)
			{
				options.AddRange(from o in base.EntityQuery<MapComponent>(true)
				select new CompletionOption(o.WorldMap.ToString(), null, 0));
				return CompletionResult.FromHintOptions(options, "Map Id");
			}
			return CompletionResult.FromHintOptions(CompletionHelper.PrototypeIDs<WeatherPrototype>(true, this.ProtoMan), Loc.GetString("cmd-weather-hint"));
		}

		// Token: 0x040001DB RID: 475
		[Dependency]
		private readonly IConsoleHost _console;
	}
}
