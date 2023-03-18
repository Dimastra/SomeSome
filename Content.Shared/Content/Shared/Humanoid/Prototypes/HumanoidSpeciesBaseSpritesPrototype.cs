using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Humanoid.Prototypes
{
	// Token: 0x02000415 RID: 1045
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("speciesBaseSprites", 1)]
	public sealed class HumanoidSpeciesBaseSpritesPrototype : IPrototype
	{
		// Token: 0x17000276 RID: 630
		// (get) Token: 0x06000C52 RID: 3154 RVA: 0x00028B5B File Offset: 0x00026D5B
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x04000C43 RID: 3139
		[DataField("sprites", false, 1, true, false, null)]
		public Dictionary<HumanoidVisualLayers, string> Sprites = new Dictionary<HumanoidVisualLayers, string>();
	}
}
