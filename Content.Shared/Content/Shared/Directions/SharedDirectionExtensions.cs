using System;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Shared.Directions
{
	// Token: 0x0200050A RID: 1290
	public static class SharedDirectionExtensions
	{
		// Token: 0x06000FAF RID: 4015 RVA: 0x00032B2C File Offset: 0x00030D2C
		public static EntityCoordinates Offset(this EntityCoordinates coordinates, Direction direction)
		{
			return coordinates.Offset(DirectionExtensions.ToVec(direction));
		}
	}
}
