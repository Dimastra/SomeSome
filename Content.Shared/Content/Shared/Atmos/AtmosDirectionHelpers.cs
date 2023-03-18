using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;

namespace Content.Shared.Atmos
{
	// Token: 0x0200068D RID: 1677
	public static class AtmosDirectionHelpers
	{
		// Token: 0x06001485 RID: 5253 RVA: 0x00044448 File Offset: 0x00042648
		public static AtmosDirection GetOpposite(this AtmosDirection direction)
		{
			switch (direction)
			{
			case AtmosDirection.North:
				return AtmosDirection.South;
			case AtmosDirection.South:
				return AtmosDirection.North;
			case AtmosDirection.East:
				return AtmosDirection.West;
			case AtmosDirection.NorthEast:
				return AtmosDirection.SouthWest;
			case AtmosDirection.SouthEast:
				return AtmosDirection.NorthWest;
			case AtmosDirection.West:
				return AtmosDirection.East;
			case AtmosDirection.NorthWest:
				return AtmosDirection.SouthEast;
			case AtmosDirection.SouthWest:
				return AtmosDirection.NorthEast;
			}
			throw new ArgumentOutOfRangeException("direction");
		}

		// Token: 0x06001486 RID: 5254 RVA: 0x000444B8 File Offset: 0x000426B8
		public static Direction ToDirection(this AtmosDirection direction)
		{
			switch (direction)
			{
			case AtmosDirection.Invalid:
				return -1;
			case AtmosDirection.North:
				return 4;
			case AtmosDirection.South:
				return 0;
			case AtmosDirection.East:
				return 2;
			case AtmosDirection.NorthEast:
				return 3;
			case AtmosDirection.SouthEast:
				return 1;
			case AtmosDirection.West:
				return 6;
			case AtmosDirection.NorthWest:
				return 5;
			case AtmosDirection.SouthWest:
				return 7;
			}
			throw new ArgumentOutOfRangeException("direction");
		}

		// Token: 0x06001487 RID: 5255 RVA: 0x0004452C File Offset: 0x0004272C
		public static AtmosDirection ToAtmosDirection(this Direction direction)
		{
			AtmosDirection result;
			switch (direction)
			{
			case -1:
				result = AtmosDirection.Invalid;
				break;
			case 0:
				result = AtmosDirection.South;
				break;
			case 1:
				result = AtmosDirection.SouthEast;
				break;
			case 2:
				result = AtmosDirection.East;
				break;
			case 3:
				result = AtmosDirection.NorthEast;
				break;
			case 4:
				result = AtmosDirection.North;
				break;
			case 5:
				result = AtmosDirection.NorthWest;
				break;
			case 6:
				result = AtmosDirection.West;
				break;
			case 7:
				result = AtmosDirection.SouthWest;
				break;
			default:
				throw new ArgumentOutOfRangeException("direction");
			}
			return result;
		}

		// Token: 0x06001488 RID: 5256 RVA: 0x0004459C File Offset: 0x0004279C
		public static Angle ToAngle(this AtmosDirection direction)
		{
			switch (direction)
			{
			case AtmosDirection.North:
				return Angle.FromDegrees(180.0);
			case AtmosDirection.South:
				return Angle.FromDegrees(0.0);
			case AtmosDirection.East:
				return Angle.FromDegrees(90.0);
			case AtmosDirection.NorthEast:
				return Angle.FromDegrees(135.0);
			case AtmosDirection.SouthEast:
				return Angle.FromDegrees(45.0);
			case AtmosDirection.West:
				return Angle.FromDegrees(270.0);
			case AtmosDirection.NorthWest:
				return Angle.FromDegrees(205.0);
			case AtmosDirection.SouthWest:
				return Angle.FromDegrees(315.0);
			}
			string paramName = "direction";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 1);
			defaultInterpolatedStringHandler.AppendLiteral("It was ");
			defaultInterpolatedStringHandler.AppendFormatted<AtmosDirection>(direction);
			defaultInterpolatedStringHandler.AppendLiteral(".");
			throw new ArgumentOutOfRangeException(paramName, defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x06001489 RID: 5257 RVA: 0x000446AB File Offset: 0x000428AB
		public static AtmosDirection ToAtmosDirectionCardinal(this Angle angle)
		{
			return angle.GetCardinalDir().ToAtmosDirection();
		}

		// Token: 0x0600148A RID: 5258 RVA: 0x000446B9 File Offset: 0x000428B9
		public static AtmosDirection ToAtmosDirection(this Angle angle)
		{
			return angle.GetDir().ToAtmosDirection();
		}

		// Token: 0x0600148B RID: 5259 RVA: 0x000446C7 File Offset: 0x000428C7
		public static int ToIndex(this AtmosDirection direction)
		{
			return (int)Math.Log2((double)direction);
		}

		// Token: 0x0600148C RID: 5260 RVA: 0x000446D1 File Offset: 0x000428D1
		public static AtmosDirection WithFlag(this AtmosDirection direction, AtmosDirection other)
		{
			return direction | other;
		}

		// Token: 0x0600148D RID: 5261 RVA: 0x000446D6 File Offset: 0x000428D6
		public static AtmosDirection WithoutFlag(this AtmosDirection direction, AtmosDirection other)
		{
			return direction & ~other;
		}

		// Token: 0x0600148E RID: 5262 RVA: 0x000446DC File Offset: 0x000428DC
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsFlagSet(this AtmosDirection direction, AtmosDirection other)
		{
			return (direction & other) == other;
		}

		// Token: 0x0600148F RID: 5263 RVA: 0x000446E4 File Offset: 0x000428E4
		public static Vector2i CardinalToIntVec(this AtmosDirection dir)
		{
			switch (dir)
			{
			case AtmosDirection.North:
				return new Vector2i(0, 1);
			case AtmosDirection.South:
				return new Vector2i(0, -1);
			case AtmosDirection.North | AtmosDirection.South:
				break;
			case AtmosDirection.East:
				return new Vector2i(1, 0);
			default:
				if (dir == AtmosDirection.West)
				{
					return new Vector2i(-1, 0);
				}
				break;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(42, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Direction dir ");
			defaultInterpolatedStringHandler.AppendFormatted<AtmosDirection>(dir);
			defaultInterpolatedStringHandler.AppendLiteral(" is not a cardinal direction");
			throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), "dir");
		}

		// Token: 0x06001490 RID: 5264 RVA: 0x0004476A File Offset: 0x0004296A
		public static Vector2i Offset(this Vector2i pos, AtmosDirection dir)
		{
			return pos + dir.CardinalToIntVec();
		}
	}
}
