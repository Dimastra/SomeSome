using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Atmos.Serialization;
using Content.Server.NodeContainer.NodeGroups;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Components
{
	// Token: 0x020007A9 RID: 1961
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(AtmosphereSystem),
		typeof(GasTileOverlaySystem),
		typeof(AtmosDebugOverlaySystem)
	})]
	[Serializable]
	public sealed class GridAtmosphereComponent : Component
	{
		// Token: 0x17000674 RID: 1652
		// (get) Token: 0x06002A8A RID: 10890 RVA: 0x000DFD96 File Offset: 0x000DDF96
		// (set) Token: 0x06002A8B RID: 10891 RVA: 0x000DFD9E File Offset: 0x000DDF9E
		[ViewVariables]
		public bool Simulated { get; set; } = true;

		// Token: 0x17000675 RID: 1653
		// (get) Token: 0x06002A8C RID: 10892 RVA: 0x000DFDA7 File Offset: 0x000DDFA7
		// (set) Token: 0x06002A8D RID: 10893 RVA: 0x000DFDAF File Offset: 0x000DDFAF
		[ViewVariables]
		public bool ProcessingPaused { get; set; }

		// Token: 0x17000676 RID: 1654
		// (get) Token: 0x06002A8E RID: 10894 RVA: 0x000DFDB8 File Offset: 0x000DDFB8
		// (set) Token: 0x06002A8F RID: 10895 RVA: 0x000DFDC0 File Offset: 0x000DDFC0
		[ViewVariables]
		public float Timer { get; set; }

		// Token: 0x17000677 RID: 1655
		// (get) Token: 0x06002A90 RID: 10896 RVA: 0x000DFDC9 File Offset: 0x000DDFC9
		// (set) Token: 0x06002A91 RID: 10897 RVA: 0x000DFDD1 File Offset: 0x000DDFD1
		[ViewVariables]
		public int UpdateCounter { get; set; } = 1;

		// Token: 0x17000678 RID: 1656
		// (get) Token: 0x06002A92 RID: 10898 RVA: 0x000DFDDA File Offset: 0x000DDFDA
		[ViewVariables]
		public int ActiveTilesCount
		{
			get
			{
				return this.ActiveTiles.Count;
			}
		}

		// Token: 0x17000679 RID: 1657
		// (get) Token: 0x06002A93 RID: 10899 RVA: 0x000DFDE7 File Offset: 0x000DDFE7
		[ViewVariables]
		public int ExcitedGroupCount
		{
			get
			{
				return this.ExcitedGroups.Count;
			}
		}

		// Token: 0x1700067A RID: 1658
		// (get) Token: 0x06002A94 RID: 10900 RVA: 0x000DFDF4 File Offset: 0x000DDFF4
		[ViewVariables]
		public int HotspotTilesCount
		{
			get
			{
				return this.HotspotTiles.Count;
			}
		}

		// Token: 0x1700067B RID: 1659
		// (get) Token: 0x06002A95 RID: 10901 RVA: 0x000DFE01 File Offset: 0x000DE001
		[ViewVariables]
		public int SuperconductivityTilesCount
		{
			get
			{
				return this.SuperconductivityTiles.Count;
			}
		}

		// Token: 0x1700067C RID: 1660
		// (get) Token: 0x06002A96 RID: 10902 RVA: 0x000DFE0E File Offset: 0x000DE00E
		[ViewVariables]
		public int HighPressureDeltaCount
		{
			get
			{
				return this.HighPressureDelta.Count;
			}
		}

		// Token: 0x1700067D RID: 1661
		// (get) Token: 0x06002A97 RID: 10903 RVA: 0x000DFE1B File Offset: 0x000DE01B
		[ViewVariables]
		public int InvalidatedCoordsCount
		{
			get
			{
				return this.InvalidatedCoords.Count;
			}
		}

		// Token: 0x1700067E RID: 1662
		// (get) Token: 0x06002A98 RID: 10904 RVA: 0x000DFE28 File Offset: 0x000DE028
		// (set) Token: 0x06002A99 RID: 10905 RVA: 0x000DFE30 File Offset: 0x000DE030
		[ViewVariables]
		public long EqualizationQueueCycleControl { get; set; }

		// Token: 0x1700067F RID: 1663
		// (get) Token: 0x06002A9A RID: 10906 RVA: 0x000DFE39 File Offset: 0x000DE039
		// (set) Token: 0x06002A9B RID: 10907 RVA: 0x000DFE41 File Offset: 0x000DE041
		[ViewVariables]
		public AtmosphereProcessingState State { get; set; }

		// Token: 0x04001A60 RID: 6752
		[ViewVariables]
		[IncludeDataField(false, 1, false, typeof(TileAtmosCollectionSerializer))]
		public readonly Dictionary<Vector2i, TileAtmosphere> Tiles = new Dictionary<Vector2i, TileAtmosphere>(1000);

		// Token: 0x04001A61 RID: 6753
		[ViewVariables]
		public readonly HashSet<TileAtmosphere> ActiveTiles = new HashSet<TileAtmosphere>(1000);

		// Token: 0x04001A62 RID: 6754
		[ViewVariables]
		public readonly HashSet<ExcitedGroup> ExcitedGroups = new HashSet<ExcitedGroup>(1000);

		// Token: 0x04001A63 RID: 6755
		[ViewVariables]
		public readonly HashSet<TileAtmosphere> HotspotTiles = new HashSet<TileAtmosphere>(1000);

		// Token: 0x04001A64 RID: 6756
		[ViewVariables]
		public readonly HashSet<TileAtmosphere> SuperconductivityTiles = new HashSet<TileAtmosphere>(1000);

		// Token: 0x04001A65 RID: 6757
		[ViewVariables]
		public HashSet<TileAtmosphere> HighPressureDelta = new HashSet<TileAtmosphere>(1000);

		// Token: 0x04001A66 RID: 6758
		[ViewVariables]
		public readonly HashSet<IPipeNet> PipeNets = new HashSet<IPipeNet>();

		// Token: 0x04001A67 RID: 6759
		[ViewVariables]
		public readonly HashSet<AtmosDeviceComponent> AtmosDevices = new HashSet<AtmosDeviceComponent>();

		// Token: 0x04001A68 RID: 6760
		[ViewVariables]
		public Queue<TileAtmosphere> CurrentRunTiles = new Queue<TileAtmosphere>();

		// Token: 0x04001A69 RID: 6761
		[ViewVariables]
		public Queue<ExcitedGroup> CurrentRunExcitedGroups = new Queue<ExcitedGroup>();

		// Token: 0x04001A6A RID: 6762
		[ViewVariables]
		public Queue<IPipeNet> CurrentRunPipeNet = new Queue<IPipeNet>();

		// Token: 0x04001A6B RID: 6763
		[ViewVariables]
		public Queue<AtmosDeviceComponent> CurrentRunAtmosDevices = new Queue<AtmosDeviceComponent>();

		// Token: 0x04001A6C RID: 6764
		[ViewVariables]
		public readonly HashSet<Vector2i> InvalidatedCoords = new HashSet<Vector2i>(1000);

		// Token: 0x04001A6D RID: 6765
		[ViewVariables]
		public Queue<Vector2i> CurrentRunInvalidatedCoordinates = new Queue<Vector2i>();
	}
}
