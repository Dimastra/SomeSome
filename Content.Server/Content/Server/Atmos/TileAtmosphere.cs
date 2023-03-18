using System;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Maps;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos
{
	// Token: 0x0200073A RID: 1850
	[NullableContext(2)]
	[Nullable(0)]
	[Access(new Type[]
	{
		typeof(AtmosphereSystem),
		typeof(GasTileOverlaySystem),
		typeof(AtmosDebugOverlaySystem)
	})]
	public sealed class TileAtmosphere : IGasMixtureHolder
	{
		// Token: 0x170005C5 RID: 1477
		// (get) Token: 0x060026CD RID: 9933 RVA: 0x000CC914 File Offset: 0x000CAB14
		// (set) Token: 0x060026CE RID: 9934 RVA: 0x000CC91C File Offset: 0x000CAB1C
		[ViewVariables]
		public float Temperature { get; set; } = 293.15f;

		// Token: 0x170005C6 RID: 1478
		// (get) Token: 0x060026CF RID: 9935 RVA: 0x000CC925 File Offset: 0x000CAB25
		// (set) Token: 0x060026D0 RID: 9936 RVA: 0x000CC92D File Offset: 0x000CAB2D
		[ViewVariables]
		public float TemperatureArchived { get; set; } = 293.15f;

		// Token: 0x170005C7 RID: 1479
		// (get) Token: 0x060026D1 RID: 9937 RVA: 0x000CC936 File Offset: 0x000CAB36
		// (set) Token: 0x060026D2 RID: 9938 RVA: 0x000CC93E File Offset: 0x000CAB3E
		[ViewVariables]
		public TileAtmosphere PressureSpecificTarget { get; set; }

		// Token: 0x170005C8 RID: 1480
		// (get) Token: 0x060026D3 RID: 9939 RVA: 0x000CC947 File Offset: 0x000CAB47
		// (set) Token: 0x060026D4 RID: 9940 RVA: 0x000CC94F File Offset: 0x000CAB4F
		[ViewVariables]
		public float PressureDifference { get; set; }

		// Token: 0x170005C9 RID: 1481
		// (get) Token: 0x060026D5 RID: 9941 RVA: 0x000CC958 File Offset: 0x000CAB58
		// (set) Token: 0x060026D6 RID: 9942 RVA: 0x000CC960 File Offset: 0x000CAB60
		[ViewVariables]
		public float HeatCapacity { get; set; } = 0.0003f;

		// Token: 0x170005CA RID: 1482
		// (get) Token: 0x060026D7 RID: 9943 RVA: 0x000CC969 File Offset: 0x000CAB69
		// (set) Token: 0x060026D8 RID: 9944 RVA: 0x000CC971 File Offset: 0x000CAB71
		[ViewVariables]
		public float ThermalConductivity { get; set; } = 0.05f;

		// Token: 0x170005CB RID: 1483
		// (get) Token: 0x060026D9 RID: 9945 RVA: 0x000CC97A File Offset: 0x000CAB7A
		// (set) Token: 0x060026DA RID: 9946 RVA: 0x000CC982 File Offset: 0x000CAB82
		[ViewVariables]
		public bool Excited { get; set; }

		// Token: 0x170005CC RID: 1484
		// (get) Token: 0x060026DB RID: 9947 RVA: 0x000CC98B File Offset: 0x000CAB8B
		// (set) Token: 0x060026DC RID: 9948 RVA: 0x000CC993 File Offset: 0x000CAB93
		[ViewVariables]
		public bool Space { get; set; }

		// Token: 0x170005CD RID: 1485
		// (get) Token: 0x060026DD RID: 9949 RVA: 0x000CC99C File Offset: 0x000CAB9C
		// (set) Token: 0x060026DE RID: 9950 RVA: 0x000CC9A4 File Offset: 0x000CABA4
		[ViewVariables]
		[Access(new Type[]
		{
			typeof(AtmosphereSystem)
		})]
		public EntityUid GridIndex { get; set; }

		// Token: 0x170005CE RID: 1486
		// (get) Token: 0x060026DF RID: 9951 RVA: 0x000CC9AD File Offset: 0x000CABAD
		[ViewVariables]
		public TileRef? Tile
		{
			get
			{
				return new TileRef?(this.GridIndices.GetTileRef(this.GridIndex, null));
			}
		}

		// Token: 0x170005CF RID: 1487
		// (get) Token: 0x060026E0 RID: 9952 RVA: 0x000CC9C6 File Offset: 0x000CABC6
		[ViewVariables]
		public Vector2i GridIndices { get; }

		// Token: 0x170005D0 RID: 1488
		// (get) Token: 0x060026E1 RID: 9953 RVA: 0x000CC9CE File Offset: 0x000CABCE
		// (set) Token: 0x060026E2 RID: 9954 RVA: 0x000CC9D6 File Offset: 0x000CABD6
		[ViewVariables]
		public ExcitedGroup ExcitedGroup { get; set; }

		// Token: 0x170005D1 RID: 1489
		// (get) Token: 0x060026E3 RID: 9955 RVA: 0x000CC9DF File Offset: 0x000CABDF
		// (set) Token: 0x060026E4 RID: 9956 RVA: 0x000CC9E7 File Offset: 0x000CABE7
		[ViewVariables]
		[Access]
		public GasMixture Air { get; set; }

		// Token: 0x170005D2 RID: 1490
		// (get) Token: 0x060026E5 RID: 9957 RVA: 0x000CC9F0 File Offset: 0x000CABF0
		// (set) Token: 0x060026E6 RID: 9958 RVA: 0x000CCA12 File Offset: 0x000CAC12
		[Nullable(1)]
		GasMixture IGasMixtureHolder.Air
		{
			[NullableContext(1)]
			get
			{
				GasMixture result;
				if ((result = this.Air) == null)
				{
					(result = new GasMixture(2500f)).Temperature = this.Temperature;
				}
				return result;
			}
			[NullableContext(1)]
			set
			{
				this.Air = value;
			}
		}

		// Token: 0x170005D3 RID: 1491
		// (get) Token: 0x060026E7 RID: 9959 RVA: 0x000CCA1B File Offset: 0x000CAC1B
		// (set) Token: 0x060026E8 RID: 9960 RVA: 0x000CCA23 File Offset: 0x000CAC23
		[ViewVariables]
		public float MaxFireTemperatureSustained { get; set; }

		// Token: 0x170005D4 RID: 1492
		// (get) Token: 0x060026E9 RID: 9961 RVA: 0x000CCA2C File Offset: 0x000CAC2C
		// (set) Token: 0x060026EA RID: 9962 RVA: 0x000CCA34 File Offset: 0x000CAC34
		[ViewVariables]
		public AtmosDirection BlockedAirflow { get; set; }

		// Token: 0x060026EB RID: 9963 RVA: 0x000CCA40 File Offset: 0x000CAC40
		public TileAtmosphere(EntityUid gridIndex, Vector2i gridIndices, GasMixture mixture = null, bool immutable = false, bool space = false)
		{
			this.GridIndex = gridIndex;
			this.GridIndices = gridIndices;
			this.Air = mixture;
			this.Space = space;
			this.MolesArchived = ((this.Air != null) ? new float[Atmospherics.AdjustedNumberOfGases] : null);
			if (immutable)
			{
				GasMixture air = this.Air;
				if (air == null)
				{
					return;
				}
				air.MarkImmutable();
			}
		}

		// Token: 0x0400181C RID: 6172
		[ViewVariables]
		public int ArchivedCycle;

		// Token: 0x0400181D RID: 6173
		[ViewVariables]
		public int CurrentCycle;

		// Token: 0x04001826 RID: 6182
		[Nullable(new byte[]
		{
			1,
			2
		})]
		[ViewVariables]
		public readonly TileAtmosphere[] AdjacentTiles = new TileAtmosphere[4];

		// Token: 0x04001827 RID: 6183
		[ViewVariables]
		public AtmosDirection AdjacentBits;

		// Token: 0x04001828 RID: 6184
		[ViewVariables]
		[Access]
		public MonstermosInfo MonstermosInfo;

		// Token: 0x04001829 RID: 6185
		[ViewVariables]
		public Hotspot Hotspot;

		// Token: 0x0400182A RID: 6186
		[ViewVariables]
		public AtmosDirection PressureDirection;

		// Token: 0x0400182B RID: 6187
		[ViewVariables]
		public AtmosDirection LastPressureDirection;

		// Token: 0x04001830 RID: 6192
		[DataField("lastShare", false, 1, false, false, null)]
		public float LastShare;

		// Token: 0x04001831 RID: 6193
		[ViewVariables]
		public float[] MolesArchived;
	}
}
