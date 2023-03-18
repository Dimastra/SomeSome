using System;
using System.Runtime.CompilerServices;
using Content.Shared.Maps;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Construction.Conditions
{
	// Token: 0x02000581 RID: 1409
	[DataDefinition]
	public sealed class TileNotBlocked : IConstructionCondition
	{
		// Token: 0x0600114F RID: 4431 RVA: 0x00038E00 File Offset: 0x00037000
		public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
		{
			TileRef? tileRef = location.GetTileRef(null, null);
			if (tileRef == null || tileRef.Value.IsSpace(null))
			{
				return !this._failIfSpace;
			}
			return !tileRef.Value.IsBlockedTurf(this._filterMobs, null, null);
		}

		// Token: 0x06001150 RID: 4432 RVA: 0x00038E4F File Offset: 0x0003704F
		[NullableContext(1)]
		public ConstructionGuideEntry GenerateGuideEntry()
		{
			return new ConstructionGuideEntry
			{
				Localization = "construction-step-condition-tile-not-blocked"
			};
		}

		// Token: 0x04001002 RID: 4098
		[DataField("filterMobs", false, 1, false, false, null)]
		private bool _filterMobs;

		// Token: 0x04001003 RID: 4099
		[DataField("failIfSpace", false, 1, false, false, null)]
		private bool _failIfSpace = true;
	}
}
