using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Instruments
{
	// Token: 0x020003E2 RID: 994
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedInstrumentSystem)
	})]
	public abstract class SharedInstrumentComponent : Component
	{
		// Token: 0x1700024B RID: 587
		// (get) Token: 0x06000BA3 RID: 2979 RVA: 0x00026440 File Offset: 0x00024640
		// (set) Token: 0x06000BA4 RID: 2980 RVA: 0x00026448 File Offset: 0x00024648
		[ViewVariables]
		public bool Playing { get; set; }

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x06000BA5 RID: 2981 RVA: 0x00026451 File Offset: 0x00024651
		// (set) Token: 0x06000BA6 RID: 2982 RVA: 0x00026459 File Offset: 0x00024659
		[DataField("program", false, 1, false, false, null)]
		[ViewVariables]
		public byte InstrumentProgram { get; set; }

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x06000BA7 RID: 2983 RVA: 0x00026462 File Offset: 0x00024662
		// (set) Token: 0x06000BA8 RID: 2984 RVA: 0x0002646A File Offset: 0x0002466A
		[DataField("bank", false, 1, false, false, null)]
		[ViewVariables]
		public byte InstrumentBank { get; set; }

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x06000BA9 RID: 2985 RVA: 0x00026473 File Offset: 0x00024673
		// (set) Token: 0x06000BAA RID: 2986 RVA: 0x0002647B File Offset: 0x0002467B
		[DataField("allowPercussion", false, 1, false, false, null)]
		[ViewVariables]
		public bool AllowPercussion { get; set; }

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x06000BAB RID: 2987 RVA: 0x00026484 File Offset: 0x00024684
		// (set) Token: 0x06000BAC RID: 2988 RVA: 0x0002648C File Offset: 0x0002468C
		[DataField("allowProgramChange", false, 1, false, false, null)]
		[ViewVariables]
		public bool AllowProgramChange { get; set; }

		// Token: 0x17000250 RID: 592
		// (get) Token: 0x06000BAD RID: 2989 RVA: 0x00026495 File Offset: 0x00024695
		// (set) Token: 0x06000BAE RID: 2990 RVA: 0x0002649D File Offset: 0x0002469D
		[DataField("respectMidiLimits", false, 1, false, false, null)]
		[ViewVariables]
		public bool RespectMidiLimits { get; set; } = true;

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x06000BAF RID: 2991 RVA: 0x000264A6 File Offset: 0x000246A6
		// (set) Token: 0x06000BB0 RID: 2992 RVA: 0x000264AE File Offset: 0x000246AE
		[ViewVariables]
		[Access]
		public bool DirtyRenderer { get; set; }
	}
}
