using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Materials
{
	// Token: 0x02000336 RID: 822
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[Serializable]
	public sealed class MaterialStorageComponentState : ComponentState
	{
		// Token: 0x0600097F RID: 2431 RVA: 0x0001FA1B File Offset: 0x0001DC1B
		public MaterialStorageComponentState(Dictionary<string, int> storage, [Nullable(new byte[]
		{
			2,
			1
		})] List<string> materialWhitelist)
		{
			this.Storage = storage;
			this.MaterialWhitelist = materialWhitelist;
		}

		// Token: 0x0400095D RID: 2397
		public Dictionary<string, int> Storage;

		// Token: 0x0400095E RID: 2398
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public List<string> MaterialWhitelist;
	}
}
