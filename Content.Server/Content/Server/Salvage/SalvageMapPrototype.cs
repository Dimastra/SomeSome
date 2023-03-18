using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Server.Salvage
{
	// Token: 0x0200021D RID: 541
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("salvageMap", 1)]
	public sealed class SalvageMapPrototype : IPrototype
	{
		// Token: 0x1700019E RID: 414
		// (get) Token: 0x06000AB9 RID: 2745 RVA: 0x00038174 File Offset: 0x00036374
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x06000ABA RID: 2746 RVA: 0x0003817C File Offset: 0x0003637C
		[DataField("mapPath", false, 1, true, false, null)]
		public ResourcePath MapPath { get; }

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x06000ABB RID: 2747 RVA: 0x00038184 File Offset: 0x00036384
		[DataField("bounds", false, 1, true, false, null)]
		public Box2 Bounds { get; } = Box2.UnitCentered;

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x06000ABC RID: 2748 RVA: 0x0003818C File Offset: 0x0003638C
		[DataField("name", false, 1, false, false, null)]
		public string Name { get; } = "";
	}
}
