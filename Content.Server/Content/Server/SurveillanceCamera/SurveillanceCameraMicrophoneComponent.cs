using System;
using System.Runtime.CompilerServices;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.SurveillanceCamera
{
	// Token: 0x0200013D RID: 317
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class SurveillanceCameraMicrophoneComponent : Component
	{
		// Token: 0x17000113 RID: 275
		// (get) Token: 0x060005D1 RID: 1489 RVA: 0x0001C3A0 File Offset: 0x0001A5A0
		// (set) Token: 0x060005D2 RID: 1490 RVA: 0x0001C3A8 File Offset: 0x0001A5A8
		[DataField("enabled", false, 1, false, false, null)]
		public bool Enabled { get; set; } = true;

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x060005D3 RID: 1491 RVA: 0x0001C3B1 File Offset: 0x0001A5B1
		[DataField("blacklist", false, 1, false, false, null)]
		public EntityWhitelist Blacklist { get; } = new EntityWhitelist();

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x060005D4 RID: 1492 RVA: 0x0001C3B9 File Offset: 0x0001A5B9
		[ViewVariables]
		[DataField("range", false, 1, false, false, null)]
		public int Range { get; } = 10;
	}
}
