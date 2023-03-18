using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.SubFloor
{
	// Token: 0x02000108 RID: 264
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class TrayScannerComponent : Component
	{
		// Token: 0x1700008F RID: 143
		// (get) Token: 0x060002EB RID: 747 RVA: 0x0000D1EF File Offset: 0x0000B3EF
		// (set) Token: 0x060002EC RID: 748 RVA: 0x0000D1F7 File Offset: 0x0000B3F7
		[ViewVariables]
		public bool Enabled { get; set; }

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060002ED RID: 749 RVA: 0x0000D200 File Offset: 0x0000B400
		// (set) Token: 0x060002EE RID: 750 RVA: 0x0000D208 File Offset: 0x0000B408
		[ViewVariables]
		public Vector2i? LastLocation { get; set; }

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060002EF RID: 751 RVA: 0x0000D211 File Offset: 0x0000B411
		// (set) Token: 0x060002F0 RID: 752 RVA: 0x0000D219 File Offset: 0x0000B419
		[DataField("range", false, 1, false, false, null)]
		public float Range { get; set; } = 2.5f;

		// Token: 0x0400033B RID: 827
		[Nullable(1)]
		[ViewVariables]
		public HashSet<EntityUid> RevealedSubfloors = new HashSet<EntityUid>();
	}
}
