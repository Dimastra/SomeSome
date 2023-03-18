using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Gatherable.Components
{
	// Token: 0x020004A3 RID: 1187
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class GatheringToolComponent : Component
	{
		// Token: 0x17000345 RID: 837
		// (get) Token: 0x060017D7 RID: 6103 RVA: 0x0007C6BF File Offset: 0x0007A8BF
		// (set) Token: 0x060017D8 RID: 6104 RVA: 0x0007C6C7 File Offset: 0x0007A8C7
		[ViewVariables]
		[DataField("sound", false, 1, false, false, null)]
		public SoundSpecifier GatheringSound { get; set; } = new SoundPathSpecifier("/Audio/Items/Mining/pickaxe.ogg", null);

		// Token: 0x17000346 RID: 838
		// (get) Token: 0x060017D9 RID: 6105 RVA: 0x0007C6D0 File Offset: 0x0007A8D0
		// (set) Token: 0x060017DA RID: 6106 RVA: 0x0007C6D8 File Offset: 0x0007A8D8
		[DataField("damage", false, 1, true, false, null)]
		public DamageSpecifier Damage { get; set; }

		// Token: 0x04000ECF RID: 3791
		[ViewVariables]
		[DataField("maxEntities", false, 1, false, false, null)]
		public int MaxGatheringEntities = 1;

		// Token: 0x04000ED0 RID: 3792
		[ViewVariables]
		public readonly List<EntityUid> GatheringEntities = new List<EntityUid>();
	}
}
