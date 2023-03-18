using System;
using System.Runtime.CompilerServices;
using Content.Server.Holiday.Celebrate;
using Content.Server.Holiday.Greet;
using Content.Server.Holiday.Interfaces;
using Content.Server.Holiday.ShouldCelebrate;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Holiday
{
	// Token: 0x02000460 RID: 1120
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("holiday", 1)]
	public sealed class HolidayPrototype : IPrototype
	{
		// Token: 0x17000303 RID: 771
		// (get) Token: 0x06001696 RID: 5782 RVA: 0x00077444 File Offset: 0x00075644
		// (set) Token: 0x06001697 RID: 5783 RVA: 0x0007744C File Offset: 0x0007564C
		[DataField("name", false, 1, false, false, null)]
		public string Name { get; private set; } = string.Empty;

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x06001698 RID: 5784 RVA: 0x00077455 File Offset: 0x00075655
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x06001699 RID: 5785 RVA: 0x0007745D File Offset: 0x0007565D
		// (set) Token: 0x0600169A RID: 5786 RVA: 0x00077465 File Offset: 0x00075665
		[DataField("beginDay", false, 1, false, false, null)]
		public byte BeginDay { get; set; } = 1;

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x0600169B RID: 5787 RVA: 0x0007746E File Offset: 0x0007566E
		// (set) Token: 0x0600169C RID: 5788 RVA: 0x00077476 File Offset: 0x00075676
		[DataField("beginMonth", false, 1, false, false, null)]
		public Month BeginMonth { get; set; }

		// Token: 0x17000307 RID: 775
		// (get) Token: 0x0600169D RID: 5789 RVA: 0x0007747F File Offset: 0x0007567F
		// (set) Token: 0x0600169E RID: 5790 RVA: 0x00077487 File Offset: 0x00075687
		[DataField("endDay", false, 1, false, false, null)]
		public byte EndDay { get; set; }

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x0600169F RID: 5791 RVA: 0x00077490 File Offset: 0x00075690
		// (set) Token: 0x060016A0 RID: 5792 RVA: 0x00077498 File Offset: 0x00075698
		[DataField("endMonth", false, 1, false, false, null)]
		public Month EndMonth { get; set; }

		// Token: 0x060016A1 RID: 5793 RVA: 0x000774A1 File Offset: 0x000756A1
		public bool ShouldCelebrate(DateTime date)
		{
			return this._shouldCelebrate.ShouldCelebrate(date, this);
		}

		// Token: 0x060016A2 RID: 5794 RVA: 0x000774B0 File Offset: 0x000756B0
		public string Greet()
		{
			return this._greet.Greet(this);
		}

		// Token: 0x060016A3 RID: 5795 RVA: 0x000774BE File Offset: 0x000756BE
		public void Celebrate()
		{
			this._celebrate.Celebrate(this);
		}

		// Token: 0x04000E20 RID: 3616
		[DataField("shouldCelebrate", false, 1, false, false, null)]
		private readonly IHolidayShouldCelebrate _shouldCelebrate = new DefaultHolidayShouldCelebrate();

		// Token: 0x04000E21 RID: 3617
		[DataField("greet", false, 1, false, false, null)]
		private readonly IHolidayGreet _greet = new DefaultHolidayGreet();

		// Token: 0x04000E22 RID: 3618
		[DataField("celebrate", false, 1, false, false, null)]
		private readonly IHolidayCelebrate _celebrate = new DefaultHolidayCelebrate();
	}
}
