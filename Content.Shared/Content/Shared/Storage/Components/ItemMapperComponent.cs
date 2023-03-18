using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Storage.Components
{
	// Token: 0x02000136 RID: 310
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Access(new Type[]
	{
		typeof(SharedItemMapperSystem)
	})]
	public sealed class ItemMapperComponent : Component
	{
		// Token: 0x040003AD RID: 941
		[DataField("mapLayers", false, 1, false, false, null)]
		public readonly Dictionary<string, SharedMapLayerData> MapLayers = new Dictionary<string, SharedMapLayerData>();

		// Token: 0x040003AE RID: 942
		[Nullable(2)]
		[DataField("sprite", false, 1, false, false, null)]
		public ResourcePath RSIPath;

		// Token: 0x040003AF RID: 943
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("containerWhitelist", false, 1, false, false, null)]
		public HashSet<string> ContainerWhitelist;

		// Token: 0x040003B0 RID: 944
		public readonly List<string> SpriteLayers = new List<string>();
	}
}
