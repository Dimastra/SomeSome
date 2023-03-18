using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Humanoid.Markings
{
	// Token: 0x02000421 RID: 1057
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class MarkingsComponent : Component
	{
		// Token: 0x04000C8B RID: 3211
		public Dictionary<HumanoidVisualLayers, List<Marking>> ActiveMarkings = new Dictionary<HumanoidVisualLayers, List<Marking>>();

		// Token: 0x04000C8C RID: 3212
		[DataField("layerPoints", false, 1, false, false, null)]
		public Dictionary<MarkingCategories, MarkingPoints> LayerPoints = new Dictionary<MarkingCategories, MarkingPoints>();
	}
}
