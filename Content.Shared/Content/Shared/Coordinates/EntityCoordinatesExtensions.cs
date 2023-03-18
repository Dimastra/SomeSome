using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Shared.Coordinates
{
	// Token: 0x02000559 RID: 1369
	[NullableContext(1)]
	[Nullable(0)]
	public static class EntityCoordinatesExtensions
	{
		// Token: 0x0600109B RID: 4251 RVA: 0x000362B3 File Offset: 0x000344B3
		public static EntityCoordinates ToCoordinates(this EntityUid id)
		{
			return new EntityCoordinates(id, new Vector2(0f, 0f));
		}

		// Token: 0x0600109C RID: 4252 RVA: 0x000362CA File Offset: 0x000344CA
		public static EntityCoordinates ToCoordinates(this EntityUid id, Vector2 offset)
		{
			return new EntityCoordinates(id, offset);
		}

		// Token: 0x0600109D RID: 4253 RVA: 0x000362D3 File Offset: 0x000344D3
		public static EntityCoordinates ToCoordinates(this EntityUid id, float x, float y)
		{
			return new EntityCoordinates(id, x, y);
		}

		// Token: 0x0600109E RID: 4254 RVA: 0x000362DD File Offset: 0x000344DD
		public static EntityCoordinates ToCoordinates(this MapGridComponent grid, Vector2 offset)
		{
			return grid.Owner.ToCoordinates(offset);
		}

		// Token: 0x0600109F RID: 4255 RVA: 0x000362EB File Offset: 0x000344EB
		public static EntityCoordinates ToCoordinates(this MapGridComponent grid, float x, float y)
		{
			return grid.Owner.ToCoordinates(x, y);
		}

		// Token: 0x060010A0 RID: 4256 RVA: 0x000362FA File Offset: 0x000344FA
		public static EntityCoordinates ToCoordinates(this MapGridComponent grid)
		{
			return grid.Owner.ToCoordinates(Vector2.Zero);
		}
	}
}
