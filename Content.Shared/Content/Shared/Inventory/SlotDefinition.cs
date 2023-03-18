using System;
using System.Runtime.CompilerServices;
using Content.Shared.Whitelist;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Inventory
{
	// Token: 0x020003B0 RID: 944
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class SlotDefinition
	{
		// Token: 0x17000213 RID: 531
		// (get) Token: 0x06000AEB RID: 2795 RVA: 0x0002432B File Offset: 0x0002252B
		[DataField("name", false, 1, true, false, null)]
		public string Name { get; } = string.Empty;

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x06000AEC RID: 2796 RVA: 0x00024333 File Offset: 0x00022533
		[DataField("slotTexture", false, 1, false, false, null)]
		public string TextureName { get; } = "pocket";

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x06000AED RID: 2797 RVA: 0x0002433B File Offset: 0x0002253B
		[DataField("slotFlags", false, 1, false, false, null)]
		public SlotFlags SlotFlags { get; } = 1;

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x06000AEE RID: 2798 RVA: 0x00024343 File Offset: 0x00022543
		[DataField("showInWindow", false, 1, false, false, null)]
		public bool ShowInWindow { get; } = 1;

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x06000AEF RID: 2799 RVA: 0x0002434B File Offset: 0x0002254B
		[DataField("slotGroup", false, 1, false, false, null)]
		public string SlotGroup { get; } = "Default";

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x06000AF0 RID: 2800 RVA: 0x00024353 File Offset: 0x00022553
		[DataField("stripTime", false, 1, false, false, null)]
		public float StripTime { get; } = 4f;

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x06000AF1 RID: 2801 RVA: 0x0002435B File Offset: 0x0002255B
		[DataField("uiWindowPos", false, 1, true, false, null)]
		public Vector2i UIWindowPosition { get; }

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x06000AF2 RID: 2802 RVA: 0x00024363 File Offset: 0x00022563
		[DataField("strippingWindowPos", false, 1, true, false, null)]
		public Vector2i StrippingWindowPos { get; }

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x06000AF3 RID: 2803 RVA: 0x0002436B File Offset: 0x0002256B
		[Nullable(2)]
		[DataField("dependsOn", false, 1, false, false, null)]
		public string DependsOn { [NullableContext(2)] get; }

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x06000AF4 RID: 2804 RVA: 0x00024373 File Offset: 0x00022573
		[DataField("displayName", false, 1, true, false, null)]
		public string DisplayName { get; } = string.Empty;

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x06000AF5 RID: 2805 RVA: 0x0002437B File Offset: 0x0002257B
		[DataField("stripHidden", false, 1, false, false, null)]
		public bool StripHidden { get; }

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x06000AF6 RID: 2806 RVA: 0x00024383 File Offset: 0x00022583
		[DataField("offset", false, 1, false, false, null)]
		public Vector2 Offset { get; } = Vector2.Zero;

		// Token: 0x04000ACC RID: 2764
		[Nullable(2)]
		[DataField("whitelist", false, 1, false, false, null)]
		public EntityWhitelist Whitelist;

		// Token: 0x04000ACD RID: 2765
		[Nullable(2)]
		[DataField("blacklist", false, 1, false, false, null)]
		public EntityWhitelist Blacklist;
	}
}
