using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Robust.Server.GameObjects;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server.Mapping
{
	// Token: 0x020003DF RID: 991
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MappingSystem : EntitySystem
	{
		// Token: 0x0600145F RID: 5215 RVA: 0x000696F0 File Offset: 0x000678F0
		public override void Initialize()
		{
			base.Initialize();
			this._conHost.RegisterCommand("toggleautosave", "Toggles autosaving for a map.", "autosave <map> <path if enabling>", new ConCommandCallback(this.ToggleAutosaveCommand), false);
			this._sawmill = Logger.GetSawmill("autosave");
			this._cfg.OnValueChanged<bool>(CCVars.AutosaveEnabled, new Action<bool>(this.SetAutosaveEnabled), true);
		}

		// Token: 0x06001460 RID: 5216 RVA: 0x00069757 File Offset: 0x00067957
		public override void Shutdown()
		{
			base.Shutdown();
			this._cfg.UnsubValueChanged<bool>(CCVars.AutosaveEnabled, new Action<bool>(this.SetAutosaveEnabled));
		}

		// Token: 0x06001461 RID: 5217 RVA: 0x0006977B File Offset: 0x0006797B
		private void SetAutosaveEnabled(bool b)
		{
			if (!b)
			{
				this._currentlyAutosaving.Clear();
			}
			this._autosaveEnabled = b;
		}

		// Token: 0x06001462 RID: 5218 RVA: 0x00069794 File Offset: 0x00067994
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			if (!this._autosaveEnabled)
			{
				return;
			}
			foreach (KeyValuePair<MapId, ValueTuple<TimeSpan, string>> keyValuePair in Extensions.ToArray<MapId, ValueTuple<TimeSpan, string>>(this._currentlyAutosaving))
			{
				MapId mapId;
				ValueTuple<TimeSpan, string> valueTuple;
				keyValuePair.Deconstruct(out mapId, out valueTuple);
				ValueTuple<TimeSpan, string> valueTuple2 = valueTuple;
				MapId map = mapId;
				TimeSpan time = valueTuple2.Item1;
				string name = valueTuple2.Item2;
				if (!(this._timing.RealTime <= time))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
					if (!this._mapManager.MapExists(map) || this._mapManager.IsMapInitialized(map))
					{
						ISawmill sawmill = this._sawmill;
						defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(81, 1);
						defaultInterpolatedStringHandler.AppendLiteral("Can't autosave map ");
						defaultInterpolatedStringHandler.AppendFormatted<MapId>(map);
						defaultInterpolatedStringHandler.AppendLiteral("; it doesn't exist, or is initialized. Removing from autosave.");
						sawmill.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
						this._currentlyAutosaving.Remove(map);
						return;
					}
					string saveDir = Path.Combine(this._cfg.GetCVar<string>(CCVars.AutosaveDirectory), name);
					this._resMan.UserData.CreateDir(new ResourcePath(saveDir, "/").ToRootedPath());
					string path = Path.Combine(saveDir, DateTime.Now.ToString("yyyy-M-dd_HH.mm.ss") + "-AUTO.yml");
					this._currentlyAutosaving[map] = new ValueTuple<TimeSpan, string>(this.CalculateNextTime(), name);
					ISawmill sawmill2 = this._sawmill;
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 4);
					defaultInterpolatedStringHandler.AppendLiteral("Autosaving map ");
					defaultInterpolatedStringHandler.AppendFormatted(name);
					defaultInterpolatedStringHandler.AppendLiteral(" (");
					defaultInterpolatedStringHandler.AppendFormatted<MapId>(map);
					defaultInterpolatedStringHandler.AppendLiteral(") to ");
					defaultInterpolatedStringHandler.AppendFormatted(path);
					defaultInterpolatedStringHandler.AppendLiteral(". Next save in ");
					defaultInterpolatedStringHandler.AppendFormatted<double>(this.ReadableTimeLeft(map));
					defaultInterpolatedStringHandler.AppendLiteral(" seconds.");
					sawmill2.Info(defaultInterpolatedStringHandler.ToStringAndClear());
					this._map.SaveMap(map, path);
				}
			}
		}

		// Token: 0x06001463 RID: 5219 RVA: 0x0006997B File Offset: 0x00067B7B
		private TimeSpan CalculateNextTime()
		{
			return this._timing.RealTime + TimeSpan.FromSeconds((double)this._cfg.GetCVar<float>(CCVars.AutosaveInterval));
		}

		// Token: 0x06001464 RID: 5220 RVA: 0x000699A4 File Offset: 0x00067BA4
		private double ReadableTimeLeft(MapId map)
		{
			return Math.Round(this._currentlyAutosaving[map].Item1.TotalSeconds - this._timing.RealTime.TotalSeconds);
		}

		// Token: 0x06001465 RID: 5221 RVA: 0x000699E4 File Offset: 0x00067BE4
		[NullableContext(2)]
		public void ToggleAutosave(MapId map, string path = null)
		{
			if (!this._autosaveEnabled)
			{
				return;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			if (path == null || !this._currentlyAutosaving.TryAdd(map, new ValueTuple<TimeSpan, string>(this.CalculateNextTime(), Path.GetFileName(path))))
			{
				this._currentlyAutosaving.Remove(map);
				ISawmill sawmill = this._sawmill;
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Stopped autosaving on map ");
				defaultInterpolatedStringHandler.AppendFormatted<MapId>(map);
				sawmill.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			if (!this._mapManager.MapExists(map) || this._mapManager.IsMapInitialized(map))
			{
				this._sawmill.Warning("Tried to enable autosaving on non-existant or already initialized map!");
				this._currentlyAutosaving.Remove(map);
				return;
			}
			ISawmill sawmill2 = this._sawmill;
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(50, 3);
			defaultInterpolatedStringHandler.AppendLiteral("Started autosaving map ");
			defaultInterpolatedStringHandler.AppendFormatted(path);
			defaultInterpolatedStringHandler.AppendLiteral(" (");
			defaultInterpolatedStringHandler.AppendFormatted<MapId>(map);
			defaultInterpolatedStringHandler.AppendLiteral("). Next save in ");
			defaultInterpolatedStringHandler.AppendFormatted<double>(this.ReadableTimeLeft(map));
			defaultInterpolatedStringHandler.AppendLiteral(" seconds.");
			sawmill2.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x06001466 RID: 5222 RVA: 0x00069B04 File Offset: 0x00067D04
		[AdminCommand(AdminFlags.Server | AdminFlags.Mapping)]
		private void ToggleAutosaveCommand(IConsoleShell shell, string argstr, string[] args)
		{
			if (args.Length != 1 && args.Length != 2)
			{
				shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
				return;
			}
			int intMapId;
			if (!int.TryParse(args[0], out intMapId))
			{
				shell.WriteError(Loc.GetString("cmd-mapping-failure-integer", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("arg", args[0])
				}));
				return;
			}
			string path = null;
			if (args.Length == 2)
			{
				path = args[1];
			}
			MapId mapId;
			mapId..ctor(intMapId);
			this.ToggleAutosave(mapId, path);
		}

		// Token: 0x04000C8F RID: 3215
		[Dependency]
		private readonly IConsoleHost _conHost;

		// Token: 0x04000C90 RID: 3216
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000C91 RID: 3217
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000C92 RID: 3218
		[Dependency]
		private readonly IMapManager _mapManager;

		// Token: 0x04000C93 RID: 3219
		[Dependency]
		private readonly IResourceManager _resMan;

		// Token: 0x04000C94 RID: 3220
		[Dependency]
		private readonly MapLoaderSystem _map;

		// Token: 0x04000C95 RID: 3221
		[TupleElementNames(new string[]
		{
			"next",
			"fileName"
		})]
		[Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		private Dictionary<MapId, ValueTuple<TimeSpan, string>> _currentlyAutosaving = new Dictionary<MapId, ValueTuple<TimeSpan, string>>();

		// Token: 0x04000C96 RID: 3222
		private ISawmill _sawmill;

		// Token: 0x04000C97 RID: 3223
		private bool _autosaveEnabled;
	}
}
