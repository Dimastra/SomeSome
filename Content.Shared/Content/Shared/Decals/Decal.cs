using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Decals
{
	// Token: 0x02000522 RID: 1314
	[NullableContext(1)]
	[Nullable(0)]
	[NetSerializable]
	[DataDefinition]
	[Serializable]
	public sealed class Decal
	{
		// Token: 0x06000FE4 RID: 4068 RVA: 0x00033178 File Offset: 0x00031378
		public Decal()
		{
		}

		// Token: 0x06000FE5 RID: 4069 RVA: 0x000331A4 File Offset: 0x000313A4
		public Decal(Vector2 coordinates, string id, Color? color, Angle angle, int zIndex, bool cleanable)
		{
			this.Coordinates = coordinates;
			this.Id = id;
			this.Color = color;
			this.Angle = angle;
			this.ZIndex = zIndex;
			this.Cleanable = cleanable;
		}

		// Token: 0x06000FE6 RID: 4070 RVA: 0x00033205 File Offset: 0x00031405
		public Decal WithCoordinates(Vector2 coordinates)
		{
			return new Decal(coordinates, this.Id, this.Color, this.Angle, this.ZIndex, this.Cleanable);
		}

		// Token: 0x06000FE7 RID: 4071 RVA: 0x0003322B File Offset: 0x0003142B
		public Decal WithId(string id)
		{
			return new Decal(this.Coordinates, id, this.Color, this.Angle, this.ZIndex, this.Cleanable);
		}

		// Token: 0x06000FE8 RID: 4072 RVA: 0x00033251 File Offset: 0x00031451
		public Decal WithColor(Color? color)
		{
			return new Decal(this.Coordinates, this.Id, color, this.Angle, this.ZIndex, this.Cleanable);
		}

		// Token: 0x06000FE9 RID: 4073 RVA: 0x00033277 File Offset: 0x00031477
		public Decal WithRotation(Angle angle)
		{
			return new Decal(this.Coordinates, this.Id, this.Color, angle, this.ZIndex, this.Cleanable);
		}

		// Token: 0x06000FEA RID: 4074 RVA: 0x0003329D File Offset: 0x0003149D
		public Decal WithZIndex(int zIndex)
		{
			return new Decal(this.Coordinates, this.Id, this.Color, this.Angle, zIndex, this.Cleanable);
		}

		// Token: 0x06000FEB RID: 4075 RVA: 0x000332C3 File Offset: 0x000314C3
		public Decal WithCleanable(bool cleanable)
		{
			return new Decal(this.Coordinates, this.Id, this.Color, this.Angle, this.ZIndex, cleanable);
		}

		// Token: 0x04000F10 RID: 3856
		[DataField("coordinates", false, 1, false, false, null)]
		public readonly Vector2 Coordinates = Vector2.Zero;

		// Token: 0x04000F11 RID: 3857
		[DataField("id", false, 1, false, false, null)]
		public readonly string Id = string.Empty;

		// Token: 0x04000F12 RID: 3858
		[DataField("color", false, 1, false, false, null)]
		public readonly Color? Color;

		// Token: 0x04000F13 RID: 3859
		[DataField("angle", false, 1, false, false, null)]
		public readonly Angle Angle = Angle.Zero;

		// Token: 0x04000F14 RID: 3860
		[DataField("zIndex", false, 1, false, false, null)]
		public readonly int ZIndex;

		// Token: 0x04000F15 RID: 3861
		[DataField("cleanable", false, 1, false, false, null)]
		public readonly bool Cleanable;
	}
}
