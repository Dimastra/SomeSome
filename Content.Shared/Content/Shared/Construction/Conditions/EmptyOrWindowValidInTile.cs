using System;
using System.Runtime.CompilerServices;
using Content.Shared.Maps;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Construction.Conditions
{
	// Token: 0x0200057E RID: 1406
	[DataDefinition]
	public sealed class EmptyOrWindowValidInTile : IConstructionCondition
	{
		// Token: 0x06001147 RID: 4423 RVA: 0x00038CE8 File Offset: 0x00036EE8
		public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
		{
			bool result = false;
			foreach (EntityUid entity in location.GetEntitiesInTile(5, null))
			{
				if (IoCManager.Resolve<IEntityManager>().HasComponent<SharedCanBuildWindowOnTopComponent>(entity))
				{
					result = true;
				}
			}
			if (!result)
			{
				result = this._tileNotBlocked.Condition(user, location, direction);
			}
			return result;
		}

		// Token: 0x06001148 RID: 4424 RVA: 0x00038D54 File Offset: 0x00036F54
		[NullableContext(1)]
		public ConstructionGuideEntry GenerateGuideEntry()
		{
			return new ConstructionGuideEntry
			{
				Localization = "construction-guide-condition-empty-or-window-valid-in-tile"
			};
		}

		// Token: 0x04001001 RID: 4097
		[Nullable(1)]
		[DataField("tileNotBlocked", false, 1, false, false, null)]
		private readonly TileNotBlocked _tileNotBlocked = new TileNotBlocked();
	}
}
