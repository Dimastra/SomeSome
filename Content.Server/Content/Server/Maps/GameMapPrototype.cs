using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Content.Server.Station;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Server.Maps
{
	// Token: 0x020003D5 RID: 981
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("gameMap", 1)]
	[DebuggerDisplay("GameMapPrototype [{ID} - {MapName}]")]
	public sealed class GameMapPrototype : IPrototype
	{
		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x06001423 RID: 5155 RVA: 0x00068991 File Offset: 0x00066B91
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x06001424 RID: 5156 RVA: 0x00068999 File Offset: 0x00066B99
		[DataField("mapName", false, 1, true, false, null)]
		public string MapName { get; }

		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x06001425 RID: 5157 RVA: 0x000689A1 File Offset: 0x00066BA1
		[DataField("mapPath", false, 1, true, false, null)]
		public ResourcePath MapPath { get; }

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x06001426 RID: 5158 RVA: 0x000689A9 File Offset: 0x00066BA9
		public IReadOnlyDictionary<string, StationConfig> Stations
		{
			get
			{
				return this._stations;
			}
		}

		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x06001427 RID: 5159 RVA: 0x000689B1 File Offset: 0x00066BB1
		[DataField("fallback", false, 1, false, false, null)]
		public bool Fallback { get; }

		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x06001428 RID: 5160 RVA: 0x000689B9 File Offset: 0x00066BB9
		[DataField("minPlayers", false, 1, true, false, null)]
		public uint MinPlayers { get; }

		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x06001429 RID: 5161 RVA: 0x000689C1 File Offset: 0x00066BC1
		[DataField("maxPlayers", false, 1, false, false, null)]
		public uint MaxPlayers { get; } = -1;

		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x0600142A RID: 5162 RVA: 0x000689C9 File Offset: 0x00066BC9
		public IReadOnlyList<GameMapCondition> Conditions
		{
			get
			{
				return this._conditions;
			}
		}

		// Token: 0x04000C7C RID: 3196
		[DataField("stations", false, 1, true, false, null)]
		private Dictionary<string, StationConfig> _stations = new Dictionary<string, StationConfig>();

		// Token: 0x04000C80 RID: 3200
		[DataField("conditions", false, 1, false, false, null)]
		private readonly List<GameMapCondition> _conditions = new List<GameMapCondition>();
	}
}
