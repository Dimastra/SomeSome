using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Materials
{
	// Token: 0x02000331 RID: 817
	[NullableContext(1)]
	[Nullable(0)]
	[Access(new Type[]
	{
		typeof(SharedMaterialStorageSystem)
	})]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class MaterialStorageComponent : Component
	{
		// Token: 0x170001BA RID: 442
		// (get) Token: 0x06000963 RID: 2403 RVA: 0x0001F749 File Offset: 0x0001D949
		// (set) Token: 0x06000964 RID: 2404 RVA: 0x0001F751 File Offset: 0x0001D951
		[DataField("storage", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<int, MaterialPrototype>))]
		public Dictionary<string, int> Storage { get; set; } = new Dictionary<string, int>();

		// Token: 0x04000951 RID: 2385
		[ViewVariables]
		[DataField("storageLimit", false, 1, false, false, null)]
		public int? StorageLimit;

		// Token: 0x04000952 RID: 2386
		[Nullable(2)]
		[DataField("whitelist", false, 1, false, false, null)]
		public EntityWhitelist EntityWhitelist;

		// Token: 0x04000953 RID: 2387
		[DataField("dropOnDeconstruct", false, 1, false, false, null)]
		public bool DropOnDeconstruct = true;

		// Token: 0x04000954 RID: 2388
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("materialWhiteList", false, 1, false, false, typeof(PrototypeIdListSerializer<MaterialPrototype>))]
		public List<string> MaterialWhiteList;

		// Token: 0x04000955 RID: 2389
		[DataField("ignoreColor", false, 1, false, false, null)]
		public bool IgnoreColor;

		// Token: 0x04000956 RID: 2390
		[Nullable(2)]
		[DataField("insertingSound", false, 1, false, false, null)]
		public SoundSpecifier InsertingSound;

		// Token: 0x04000957 RID: 2391
		[DataField("insertionTime", false, 1, false, false, typeof(TimeOffsetSerializer))]
		public TimeSpan InsertionTime = TimeSpan.FromSeconds(0.7900000214576721);
	}
}
