using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

namespace Content.Shared.Humanoid.Prototypes
{
	// Token: 0x02000417 RID: 1047
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("randomHumanoidSettings", 1)]
	public sealed class RandomHumanoidSettingsPrototype : IPrototype, IInheritingPrototype
	{
		// Token: 0x1700027D RID: 637
		// (get) Token: 0x06000C5B RID: 3163 RVA: 0x00028BC7 File Offset: 0x00026DC7
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x1700027E RID: 638
		// (get) Token: 0x06000C5C RID: 3164 RVA: 0x00028BCF File Offset: 0x00026DCF
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[ParentDataField(typeof(PrototypeIdArraySerializer<RandomHumanoidSettingsPrototype>), 1)]
		public string[] Parents { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; }

		// Token: 0x1700027F RID: 639
		// (get) Token: 0x06000C5D RID: 3165 RVA: 0x00028BD7 File Offset: 0x00026DD7
		[AbstractDataField(1)]
		public bool Abstract { get; }

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x06000C5E RID: 3166 RVA: 0x00028BDF File Offset: 0x00026DDF
		[DataField("randomizeName", false, 1, false, false, null)]
		public bool RandomizeName { get; } = 1;

		// Token: 0x17000281 RID: 641
		// (get) Token: 0x06000C5F RID: 3167 RVA: 0x00028BE7 File Offset: 0x00026DE7
		[DataField("speciesBlacklist", false, 1, false, false, null)]
		public HashSet<string> SpeciesBlacklist { get; } = new HashSet<string>();

		// Token: 0x17000282 RID: 642
		// (get) Token: 0x06000C60 RID: 3168 RVA: 0x00028BEF File Offset: 0x00026DEF
		[Nullable(2)]
		[DataField("components", false, 1, false, false, null)]
		public EntityPrototype.ComponentRegistry Components { [NullableContext(2)] get; }
	}
}
