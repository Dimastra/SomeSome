using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.CCVar;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.ViewVariables;

namespace Content.Server.Maps
{
	// Token: 0x020003D3 RID: 979
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GameMapManager : IGameMapManager
	{
		// Token: 0x0600140C RID: 5132 RVA: 0x00068468 File Offset: 0x00066668
		public void Initialize()
		{
			this._configurationManager.OnValueChanged<string>(CCVars.GameMap, delegate(string value)
			{
				GameMapPrototype map2;
				if (this.TryLookupMap(value, out map2))
				{
					this._configSelectedMap = map2;
					return;
				}
				if (string.IsNullOrEmpty(value))
				{
					this._configSelectedMap = null;
					return;
				}
				Logger.ErrorS("mapsel", "Unknown map prototype " + value + " was selected!");
			}, true);
			this._configurationManager.OnValueChanged<bool>(CCVars.GameMapRotation, delegate(bool value)
			{
				this._mapRotationEnabled = value;
			}, true);
			this._configurationManager.OnValueChanged<int>(CCVars.GameMapMemoryDepth, delegate(int value)
			{
				this._mapQueueDepth = value;
				while (this._previousMaps.Count > this._mapQueueDepth)
				{
					this._previousMaps.Dequeue();
				}
			}, true);
			GameMapPrototype[] maps = this.AllVotableMaps().ToArray<GameMapPrototype>();
			this._random.Shuffle<GameMapPrototype>(maps);
			foreach (GameMapPrototype map in maps)
			{
				if (this._previousMaps.Count >= this._mapQueueDepth)
				{
					break;
				}
				this._previousMaps.Enqueue(map.ID);
			}
		}

		// Token: 0x0600140D RID: 5133 RVA: 0x0006851C File Offset: 0x0006671C
		public IEnumerable<GameMapPrototype> CurrentlyEligibleMaps()
		{
			GameMapPrototype[] maps = this.AllVotableMaps().Where(new Func<GameMapPrototype, bool>(this.IsMapEligible)).ToArray<GameMapPrototype>();
			if (maps.Length != 0)
			{
				return maps;
			}
			return from x in this.AllMaps()
			where x.Fallback
			select x;
		}

		// Token: 0x0600140E RID: 5134 RVA: 0x00068578 File Offset: 0x00066778
		public IEnumerable<GameMapPrototype> AllVotableMaps()
		{
			GameMapPoolPrototype pool;
			if (this._prototypeManager.TryIndex<GameMapPoolPrototype>(this._configurationManager.GetCVar<string>(CCVars.GameMapPool), ref pool))
			{
				foreach (string map in pool.Maps)
				{
					GameMapPrototype mapProto;
					if (!this._prototypeManager.TryIndex<GameMapPrototype>(map, ref mapProto))
					{
						Logger.Error("Couldn't index map " + map + " in pool " + pool.ID);
					}
					else
					{
						yield return mapProto;
					}
				}
				HashSet<string>.Enumerator enumerator = default(HashSet<string>.Enumerator);
				yield break;
			}
			throw new Exception("Could not index map pool prototype " + this._configurationManager.GetCVar<string>(CCVars.GameMapPool) + "!");
			yield break;
		}

		// Token: 0x0600140F RID: 5135 RVA: 0x00068588 File Offset: 0x00066788
		public IEnumerable<GameMapPrototype> AllMaps()
		{
			return this._prototypeManager.EnumeratePrototypes<GameMapPrototype>();
		}

		// Token: 0x06001410 RID: 5136 RVA: 0x00068595 File Offset: 0x00066795
		[NullableContext(2)]
		public GameMapPrototype GetSelectedMap()
		{
			return this._configSelectedMap ?? this._selectedMap;
		}

		// Token: 0x06001411 RID: 5137 RVA: 0x000685A7 File Offset: 0x000667A7
		public void ClearSelectedMap()
		{
			this._selectedMap = null;
		}

		// Token: 0x06001412 RID: 5138 RVA: 0x000685B0 File Offset: 0x000667B0
		public bool TrySelectMapIfEligible(string gameMap)
		{
			GameMapPrototype map;
			if (!this.TryLookupMap(gameMap, out map) || !this.IsMapEligible(map))
			{
				return false;
			}
			this._selectedMap = map;
			return true;
		}

		// Token: 0x06001413 RID: 5139 RVA: 0x000685DC File Offset: 0x000667DC
		public void SelectMap(string gameMap)
		{
			GameMapPrototype map;
			if (!this.TryLookupMap(gameMap, out map))
			{
				throw new ArgumentException("The map \"" + gameMap + "\" is invalid!");
			}
			this._selectedMap = map;
		}

		// Token: 0x06001414 RID: 5140 RVA: 0x00068614 File Offset: 0x00066814
		public void SelectMapRandom()
		{
			List<GameMapPrototype> maps = this.CurrentlyEligibleMaps().ToList<GameMapPrototype>();
			this._selectedMap = RandomExtensions.Pick<GameMapPrototype>(this._random, maps);
		}

		// Token: 0x06001415 RID: 5141 RVA: 0x00068640 File Offset: 0x00066840
		public void SelectMapFromRotationQueue(bool markAsPlayed = false)
		{
			GameMapPrototype map = this.GetFirstInRotationQueue();
			this._selectedMap = map;
			if (markAsPlayed)
			{
				this.EnqueueMap(map.ID);
			}
		}

		// Token: 0x06001416 RID: 5142 RVA: 0x0006866A File Offset: 0x0006686A
		public void SelectMapByConfigRules()
		{
			if (this._mapRotationEnabled)
			{
				Logger.InfoS("mapsel", "selecting the next map from the rotation queue");
				this.SelectMapFromRotationQueue(true);
				return;
			}
			Logger.InfoS("mapsel", "selecting a random map");
			this.SelectMapRandom();
		}

		// Token: 0x06001417 RID: 5143 RVA: 0x000686A0 File Offset: 0x000668A0
		public bool CheckMapExists(string gameMap)
		{
			GameMapPrototype gameMapPrototype;
			return this.TryLookupMap(gameMap, out gameMapPrototype);
		}

		// Token: 0x06001418 RID: 5144 RVA: 0x000686B8 File Offset: 0x000668B8
		private bool IsMapEligible(GameMapPrototype map)
		{
			return (ulong)map.MaxPlayers >= (ulong)((long)this._playerManager.PlayerCount) && (ulong)map.MinPlayers <= (ulong)((long)this._playerManager.PlayerCount) && map.Conditions.All((GameMapCondition x) => x.Check(map));
		}

		// Token: 0x06001419 RID: 5145 RVA: 0x00068724 File Offset: 0x00066924
		private bool TryLookupMap(string gameMap, [Nullable(2)] [NotNullWhen(true)] out GameMapPrototype map)
		{
			return this._prototypeManager.TryIndex<GameMapPrototype>(gameMap, ref map);
		}

		// Token: 0x0600141A RID: 5146 RVA: 0x00068734 File Offset: 0x00066934
		private int GetMapRotationQueuePriority(string gameMapProtoName)
		{
			int i = 0;
			using (IEnumerator<string> enumerator = this._previousMaps.Reverse<string>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == gameMapProtoName)
					{
						return i;
					}
					i++;
				}
			}
			return this._mapQueueDepth;
		}

		// Token: 0x0600141B RID: 5147 RVA: 0x00068798 File Offset: 0x00066998
		private GameMapPrototype GetFirstInRotationQueue()
		{
			Logger.InfoS("mapsel", "map queue: " + string.Join(", ", this._previousMaps));
			ValueTuple<GameMapPrototype, int>[] eligible = (from x in this.CurrentlyEligibleMaps()
			select new ValueTuple<GameMapPrototype, int>(x, this.GetMapRotationQueuePriority(x.ID)) into x
			orderby x.Item2 descending
			select x).ToArray<ValueTuple<GameMapPrototype, int>>();
			Logger.InfoS("mapsel", "eligible queue: " + string.Join<ValueTuple<string, int>>(", ", from x in eligible
			select new ValueTuple<string, int>(x.Item1.ID, x.Item2)));
			int weight = eligible[0].Item2;
			return (from x in eligible
			where x.Item2 == weight
			orderby x.Item1.ID
			select x).First<ValueTuple<GameMapPrototype, int>>().Item1;
		}

		// Token: 0x0600141C RID: 5148 RVA: 0x000688AC File Offset: 0x00066AAC
		private void EnqueueMap(string mapProtoName)
		{
			this._previousMaps.Enqueue(mapProtoName);
			while (this._previousMaps.Count > this._mapQueueDepth)
			{
				this._previousMaps.Dequeue();
			}
		}

		// Token: 0x04000C6E RID: 3182
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04000C6F RID: 3183
		[Dependency]
		private readonly IConfigurationManager _configurationManager;

		// Token: 0x04000C70 RID: 3184
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000C71 RID: 3185
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000C72 RID: 3186
		[ViewVariables]
		private readonly Queue<string> _previousMaps = new Queue<string>();

		// Token: 0x04000C73 RID: 3187
		[Nullable(2)]
		[ViewVariables]
		private GameMapPrototype _configSelectedMap;

		// Token: 0x04000C74 RID: 3188
		[Nullable(2)]
		[ViewVariables]
		private GameMapPrototype _selectedMap;

		// Token: 0x04000C75 RID: 3189
		[ViewVariables]
		private bool _mapRotationEnabled;

		// Token: 0x04000C76 RID: 3190
		[ViewVariables]
		private int _mapQueueDepth = 1;
	}
}
