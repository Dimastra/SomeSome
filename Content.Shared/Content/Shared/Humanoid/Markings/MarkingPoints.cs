using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Shared.Humanoid.Markings
{
	// Token: 0x0200041E RID: 1054
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	[NetSerializable]
	[Serializable]
	public sealed class MarkingPoints
	{
		// Token: 0x06000C8A RID: 3210 RVA: 0x00029338 File Offset: 0x00027538
		public static Dictionary<MarkingCategories, MarkingPoints> CloneMarkingPointDictionary(Dictionary<MarkingCategories, MarkingPoints> self)
		{
			Dictionary<MarkingCategories, MarkingPoints> clone = new Dictionary<MarkingCategories, MarkingPoints>();
			foreach (KeyValuePair<MarkingCategories, MarkingPoints> keyValuePair in self)
			{
				MarkingCategories markingCategories;
				MarkingPoints markingPoints;
				keyValuePair.Deconstruct(out markingCategories, out markingPoints);
				MarkingCategories category = markingCategories;
				MarkingPoints points = markingPoints;
				clone[category] = new MarkingPoints
				{
					Points = points.Points,
					Required = points.Required,
					DefaultMarkings = points.DefaultMarkings
				};
			}
			return clone;
		}

		// Token: 0x04000C7D RID: 3197
		[DataField("points", false, 1, true, false, null)]
		public int Points;

		// Token: 0x04000C7E RID: 3198
		[DataField("required", false, 1, true, false, null)]
		public bool Required;

		// Token: 0x04000C7F RID: 3199
		[DataField("defaultMarkings", false, 1, false, false, typeof(PrototypeIdListSerializer<MarkingPrototype>))]
		public List<string> DefaultMarkings = new List<string>();
	}
}
