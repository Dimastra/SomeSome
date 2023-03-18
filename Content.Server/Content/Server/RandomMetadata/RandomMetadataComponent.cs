using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.RandomMetadata
{
	// Token: 0x02000254 RID: 596
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class RandomMetadataComponent : Component
	{
		// Token: 0x04000762 RID: 1890
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("descriptionSegments", false, 1, false, false, null)]
		public List<string> DescriptionSegments;

		// Token: 0x04000763 RID: 1891
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("nameSegments", false, 1, false, false, null)]
		public List<string> NameSegments;

		// Token: 0x04000764 RID: 1892
		[DataField("nameSeparator", false, 1, false, false, null)]
		public string NameSeparator = " ";

		// Token: 0x04000765 RID: 1893
		[DataField("descriptionSeparator", false, 1, false, false, null)]
		public string DescriptionSeparator = " ";
	}
}
