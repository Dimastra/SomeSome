using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Shared.Construction.Conditions
{
	// Token: 0x0200057F RID: 1407
	[NullableContext(2)]
	public interface IConstructionCondition
	{
		// Token: 0x0600114A RID: 4426
		ConstructionGuideEntry GenerateGuideEntry();

		// Token: 0x0600114B RID: 4427
		bool Condition(EntityUid user, EntityCoordinates location, Direction direction);
	}
}
