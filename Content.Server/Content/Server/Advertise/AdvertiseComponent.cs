using System;
using System.Runtime.CompilerServices;
using Content.Server.Advertisements;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Advertise
{
	// Token: 0x020007F9 RID: 2041
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(AdvertiseSystem)
	})]
	public sealed class AdvertiseComponent : Component
	{
		// Token: 0x170006D9 RID: 1753
		// (get) Token: 0x06002C1D RID: 11293 RVA: 0x000E6D65 File Offset: 0x000E4F65
		[ViewVariables]
		[DataField("minWait", false, 1, false, false, null)]
		public int MinimumWait { get; } = 480;

		// Token: 0x170006DA RID: 1754
		// (get) Token: 0x06002C1E RID: 11294 RVA: 0x000E6D6D File Offset: 0x000E4F6D
		[ViewVariables]
		[DataField("maxWait", false, 1, false, false, null)]
		public int MaximumWait { get; } = 600;

		// Token: 0x170006DB RID: 1755
		// (get) Token: 0x06002C1F RID: 11295 RVA: 0x000E6D75 File Offset: 0x000E4F75
		[DataField("pack", false, 1, false, false, typeof(PrototypeIdSerializer<AdvertisementsPackPrototype>))]
		public string PackPrototypeId { get; } = string.Empty;

		// Token: 0x170006DC RID: 1756
		// (get) Token: 0x06002C20 RID: 11296 RVA: 0x000E6D7D File Offset: 0x000E4F7D
		// (set) Token: 0x06002C21 RID: 11297 RVA: 0x000E6D85 File Offset: 0x000E4F85
		[ViewVariables]
		public TimeSpan NextAdvertisementTime { get; set; } = TimeSpan.Zero;

		// Token: 0x170006DD RID: 1757
		// (get) Token: 0x06002C22 RID: 11298 RVA: 0x000E6D8E File Offset: 0x000E4F8E
		// (set) Token: 0x06002C23 RID: 11299 RVA: 0x000E6D96 File Offset: 0x000E4F96
		[ViewVariables]
		public bool Enabled { get; set; } = true;
	}
}
