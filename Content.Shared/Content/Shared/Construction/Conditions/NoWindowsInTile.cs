using System;
using System.Runtime.CompilerServices;
using Content.Shared.Maps;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Construction.Conditions
{
	// Token: 0x02000580 RID: 1408
	[DataDefinition]
	public sealed class NoWindowsInTile : IConstructionCondition
	{
		// Token: 0x0600114C RID: 4428 RVA: 0x00038D7C File Offset: 0x00036F7C
		public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
		{
			TagSystem tagSystem = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<TagSystem>();
			foreach (EntityUid entity in location.GetEntitiesInTile(5, null))
			{
				if (tagSystem.HasTag(entity, "Window"))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600114D RID: 4429 RVA: 0x00038DE4 File Offset: 0x00036FE4
		[NullableContext(1)]
		public ConstructionGuideEntry GenerateGuideEntry()
		{
			return new ConstructionGuideEntry
			{
				Localization = "construction-step-condition-no-windows-in-tile"
			};
		}
	}
}
