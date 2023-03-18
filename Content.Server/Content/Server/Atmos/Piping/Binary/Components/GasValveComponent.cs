using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Atmos.Piping.Binary.Components
{
	// Token: 0x02000771 RID: 1905
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class GasValveComponent : Component
	{
		// Token: 0x17000628 RID: 1576
		// (get) Token: 0x06002866 RID: 10342 RVA: 0x000D340D File Offset: 0x000D160D
		// (set) Token: 0x06002867 RID: 10343 RVA: 0x000D3415 File Offset: 0x000D1615
		[DataField("open", false, 1, false, false, null)]
		public bool Open { get; set; } = true;

		// Token: 0x17000629 RID: 1577
		// (get) Token: 0x06002868 RID: 10344 RVA: 0x000D341E File Offset: 0x000D161E
		// (set) Token: 0x06002869 RID: 10345 RVA: 0x000D3426 File Offset: 0x000D1626
		[DataField("inlet", false, 1, false, false, null)]
		public string InletName { get; set; } = "inlet";

		// Token: 0x1700062A RID: 1578
		// (get) Token: 0x0600286A RID: 10346 RVA: 0x000D342F File Offset: 0x000D162F
		// (set) Token: 0x0600286B RID: 10347 RVA: 0x000D3437 File Offset: 0x000D1637
		[DataField("outlet", false, 1, false, false, null)]
		public string OutletName { get; set; } = "outlet";

		// Token: 0x1700062B RID: 1579
		// (get) Token: 0x0600286C RID: 10348 RVA: 0x000D3440 File Offset: 0x000D1640
		[DataField("valveSound", false, 1, false, false, null)]
		public SoundSpecifier ValveSound { get; } = new SoundCollectionSpecifier("valveSqueak", null);
	}
}
