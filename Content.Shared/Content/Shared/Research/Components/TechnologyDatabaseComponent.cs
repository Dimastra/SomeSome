using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Research.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Shared.Research.Components
{
	// Token: 0x02000214 RID: 532
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class TechnologyDatabaseComponent : Component
	{
		// Token: 0x040005F9 RID: 1529
		[DataField("technologyIds", false, 1, false, false, typeof(PrototypeIdListSerializer<TechnologyPrototype>))]
		public List<string> TechnologyIds = new List<string>();

		// Token: 0x040005FA RID: 1530
		[DataField("recipeIds", false, 1, false, false, typeof(PrototypeIdListSerializer<LatheRecipePrototype>))]
		public List<string> RecipeIds = new List<string>();
	}
}
