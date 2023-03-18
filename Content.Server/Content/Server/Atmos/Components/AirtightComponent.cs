using System;
using System.Runtime.CompilerServices;
using Content.Shared.Atmos;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Server.Atmos.Components
{
	// Token: 0x0200079E RID: 1950
	[RegisterComponent]
	public sealed class AirtightComponent : Component
	{
		// Token: 0x1700065C RID: 1628
		// (get) Token: 0x06002A52 RID: 10834 RVA: 0x000DFA3F File Offset: 0x000DDC3F
		// (set) Token: 0x06002A53 RID: 10835 RVA: 0x000DFA47 File Offset: 0x000DDC47
		[TupleElementNames(new string[]
		{
			"Grid",
			"Tile"
		})]
		public ValueTuple<EntityUid, Vector2i> LastPosition { [return: TupleElementNames(new string[]
		{
			"Grid",
			"Tile"
		})] get; [param: TupleElementNames(new string[]
		{
			"Grid",
			"Tile"
		})] set; }

		// Token: 0x1700065D RID: 1629
		// (get) Token: 0x06002A54 RID: 10836 RVA: 0x000DFA50 File Offset: 0x000DDC50
		// (set) Token: 0x06002A55 RID: 10837 RVA: 0x000DFA58 File Offset: 0x000DDC58
		[DataField("airBlockedDirection", false, 1, false, false, typeof(FlagSerializer<AtmosDirectionFlags>))]
		public int InitialAirBlockedDirection { get; set; } = 15;

		// Token: 0x1700065E RID: 1630
		// (get) Token: 0x06002A56 RID: 10838 RVA: 0x000DFA61 File Offset: 0x000DDC61
		// (set) Token: 0x06002A57 RID: 10839 RVA: 0x000DFA69 File Offset: 0x000DDC69
		[DataField("airBlocked", false, 1, false, false, null)]
		public bool AirBlocked { get; set; } = true;

		// Token: 0x1700065F RID: 1631
		// (get) Token: 0x06002A58 RID: 10840 RVA: 0x000DFA72 File Offset: 0x000DDC72
		// (set) Token: 0x06002A59 RID: 10841 RVA: 0x000DFA7A File Offset: 0x000DDC7A
		[DataField("fixVacuum", false, 1, false, false, null)]
		public bool FixVacuum { get; set; } = true;

		// Token: 0x17000660 RID: 1632
		// (get) Token: 0x06002A5A RID: 10842 RVA: 0x000DFA83 File Offset: 0x000DDC83
		// (set) Token: 0x06002A5B RID: 10843 RVA: 0x000DFA8B File Offset: 0x000DDC8B
		[DataField("rotateAirBlocked", false, 1, false, false, null)]
		public bool RotateAirBlocked { get; set; } = true;

		// Token: 0x17000661 RID: 1633
		// (get) Token: 0x06002A5C RID: 10844 RVA: 0x000DFA94 File Offset: 0x000DDC94
		// (set) Token: 0x06002A5D RID: 10845 RVA: 0x000DFA9C File Offset: 0x000DDC9C
		[DataField("fixAirBlockedDirectionInitialize", false, 1, false, false, null)]
		public bool FixAirBlockedDirectionInitialize { get; set; } = true;

		// Token: 0x17000662 RID: 1634
		// (get) Token: 0x06002A5E RID: 10846 RVA: 0x000DFAA5 File Offset: 0x000DDCA5
		// (set) Token: 0x06002A5F RID: 10847 RVA: 0x000DFAAD File Offset: 0x000DDCAD
		[DataField("noAirWhenFullyAirBlocked", false, 1, false, false, null)]
		public bool NoAirWhenFullyAirBlocked { get; set; } = true;

		// Token: 0x17000663 RID: 1635
		// (get) Token: 0x06002A60 RID: 10848 RVA: 0x000DFAB6 File Offset: 0x000DDCB6
		public AtmosDirection AirBlockedDirection
		{
			get
			{
				return (AtmosDirection)this.CurrentAirBlockedDirection;
			}
		}

		// Token: 0x04001A2B RID: 6699
		[ViewVariables]
		public int CurrentAirBlockedDirection;
	}
}
