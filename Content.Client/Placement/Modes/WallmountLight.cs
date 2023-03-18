using System;
using System.Runtime.CompilerServices;
using Robust.Client.Placement;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Client.Placement.Modes
{
	// Token: 0x020001B7 RID: 439
	public sealed class WallmountLight : PlacementMode
	{
		// Token: 0x06000B52 RID: 2898 RVA: 0x00041C13 File Offset: 0x0003FE13
		[NullableContext(1)]
		public WallmountLight(PlacementManager pMan) : base(pMan)
		{
		}

		// Token: 0x06000B53 RID: 2899 RVA: 0x00041C1C File Offset: 0x0003FE1C
		public override void AlignPlacementMode(ScreenCoordinates mouseScreen)
		{
			base.MouseCoords = base.ScreenToCursorGrid(mouseScreen);
			base.CurrentTile = base.GetTileRef(base.MouseCoords);
			if (this.pManager.CurrentPermission.IsTile)
			{
				return;
			}
			EntityCoordinates mouseCoords;
			mouseCoords..ctor(base.MouseCoords.EntityId, base.CurrentTile.GridIndices);
			Vector2 vector;
			switch (this.pManager.Direction)
			{
			case 0:
				vector..ctor(0.5f, 0f);
				break;
			case 1:
			case 3:
			case 5:
				return;
			case 2:
				vector..ctor(1f, 0.5f);
				break;
			case 4:
				vector..ctor(0.5f, 1f);
				break;
			case 6:
				vector..ctor(0f, 0.5f);
				break;
			default:
				return;
			}
			mouseCoords = mouseCoords.Offset(vector);
			base.MouseCoords = mouseCoords;
		}

		// Token: 0x06000B54 RID: 2900 RVA: 0x00041D09 File Offset: 0x0003FF09
		public override bool IsValidPosition(EntityCoordinates position)
		{
			return !this.pManager.CurrentPermission.IsTile && base.RangeCheck(position);
		}
	}
}
