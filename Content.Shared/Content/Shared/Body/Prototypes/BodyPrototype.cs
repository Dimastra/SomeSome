using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Body.Prototypes
{
	// Token: 0x02000653 RID: 1619
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("body", 1)]
	public sealed class BodyPrototype : IPrototype
	{
		// Token: 0x170003F1 RID: 1009
		// (get) Token: 0x060013B4 RID: 5044 RVA: 0x00042108 File Offset: 0x00040308
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x170003F2 RID: 1010
		// (get) Token: 0x060013B5 RID: 5045 RVA: 0x00042110 File Offset: 0x00040310
		// (set) Token: 0x060013B6 RID: 5046 RVA: 0x00042118 File Offset: 0x00040318
		[DataField("name", false, 1, false, false, null)]
		public string Name { get; private set; } = "";

		// Token: 0x170003F3 RID: 1011
		// (get) Token: 0x060013B7 RID: 5047 RVA: 0x00042121 File Offset: 0x00040321
		[DataField("root", false, 1, false, false, null)]
		public string Root { get; } = string.Empty;

		// Token: 0x170003F4 RID: 1012
		// (get) Token: 0x060013B8 RID: 5048 RVA: 0x00042129 File Offset: 0x00040329
		[DataField("slots", false, 1, false, false, null)]
		public Dictionary<string, BodyPrototypeSlot> Slots { get; } = new Dictionary<string, BodyPrototypeSlot>();

		// Token: 0x060013B9 RID: 5049 RVA: 0x00042131 File Offset: 0x00040331
		private BodyPrototype()
		{
		}

		// Token: 0x060013BA RID: 5050 RVA: 0x0004215C File Offset: 0x0004035C
		public BodyPrototype(string id, string name, string root, Dictionary<string, BodyPrototypeSlot> slots)
		{
			this.ID = id;
			this.Name = name;
			this.Root = root;
			this.Slots = slots;
		}
	}
}
