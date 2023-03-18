using System;
using System.Runtime.CompilerServices;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Storage.Components
{
	// Token: 0x02000135 RID: 309
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SharedItemCounterSystem)
	})]
	public sealed class ItemCounterComponent : Component
	{
		// Token: 0x170000AE RID: 174
		// (get) Token: 0x0600038B RID: 907 RVA: 0x0000F31E File Offset: 0x0000D51E
		// (set) Token: 0x0600038C RID: 908 RVA: 0x0000F326 File Offset: 0x0000D526
		[DataField("count", false, 1, true, false, null)]
		public EntityWhitelist Count { get; set; }

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x0600038D RID: 909 RVA: 0x0000F32F File Offset: 0x0000D52F
		// (set) Token: 0x0600038E RID: 910 RVA: 0x0000F337 File Offset: 0x0000D537
		[DataField("amount", false, 1, false, false, null)]
		public int? MaxAmount { get; set; }
	}
}
