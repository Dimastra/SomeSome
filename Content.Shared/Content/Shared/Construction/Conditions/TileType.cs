using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Maps;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Construction.Conditions
{
	// Token: 0x02000582 RID: 1410
	[NullableContext(1)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class TileType : IConstructionCondition
	{
		// Token: 0x17000379 RID: 889
		// (get) Token: 0x06001152 RID: 4434 RVA: 0x00038E70 File Offset: 0x00037070
		[DataField("targets", false, 1, false, false, null)]
		public List<string> TargetTiles { get; } = new List<string>();

		// Token: 0x06001153 RID: 4435 RVA: 0x00038E78 File Offset: 0x00037078
		public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
		{
			TileRef? tileFound = location.GetTileRef(null, null);
			if (tileFound == null)
			{
				return false;
			}
			ContentTileDefinition tile = tileFound.Value.Tile.GetContentTileDefinition(null);
			foreach (string targetTile in this.TargetTiles)
			{
				if (tile.ID == targetTile)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001154 RID: 4436 RVA: 0x00038F04 File Offset: 0x00037104
		[NullableContext(2)]
		public ConstructionGuideEntry GenerateGuideEntry()
		{
			if (this.GuideText == null)
			{
				return null;
			}
			return new ConstructionGuideEntry
			{
				Localization = this.GuideText,
				Icon = this.GuideIcon
			};
		}

		// Token: 0x04001005 RID: 4101
		[Nullable(2)]
		[DataField("guideText", false, 1, false, false, null)]
		public string GuideText;

		// Token: 0x04001006 RID: 4102
		[Nullable(2)]
		[DataField("guideIcon", false, 1, false, false, null)]
		public SpriteSpecifier GuideIcon;
	}
}
