using System;
using Robust.Shared.Maths;

namespace Content.Shared.Atmos
{
	// Token: 0x02000698 RID: 1688
	public static class PipeDirectionHelpers
	{
		// Token: 0x0600149A RID: 5274 RVA: 0x00044A28 File Offset: 0x00042C28
		public static bool HasDirection(this PipeDirection pipeDirection, PipeDirection other)
		{
			return (pipeDirection & other) == other;
		}

		// Token: 0x0600149B RID: 5275 RVA: 0x00044A30 File Offset: 0x00042C30
		public static Angle ToAngle(this PipeDirection pipeDirection)
		{
			return DirectionExtensions.ToAngle(pipeDirection.ToDirection());
		}

		// Token: 0x0600149C RID: 5276 RVA: 0x00044A40 File Offset: 0x00042C40
		public static PipeDirection ToPipeDirection(this Direction direction)
		{
			switch (direction)
			{
			case 0:
				return PipeDirection.South;
			case 2:
				return PipeDirection.East;
			case 4:
				return PipeDirection.North;
			case 6:
				return PipeDirection.West;
			}
			throw new ArgumentOutOfRangeException("direction");
		}

		// Token: 0x0600149D RID: 5277 RVA: 0x00044A90 File Offset: 0x00042C90
		public static Direction ToDirection(this PipeDirection pipeDirection)
		{
			switch (pipeDirection)
			{
			case PipeDirection.North:
				return 4;
			case PipeDirection.South:
				return 0;
			case PipeDirection.Longitudinal:
				break;
			case PipeDirection.West:
				return 6;
			default:
				if (pipeDirection == PipeDirection.East)
				{
					return 2;
				}
				break;
			}
			throw new ArgumentOutOfRangeException("pipeDirection");
		}

		// Token: 0x0600149E RID: 5278 RVA: 0x00044AD8 File Offset: 0x00042CD8
		public static PipeDirection GetOpposite(this PipeDirection pipeDirection)
		{
			switch (pipeDirection)
			{
			case PipeDirection.North:
				return PipeDirection.South;
			case PipeDirection.South:
				return PipeDirection.North;
			case PipeDirection.Longitudinal:
				break;
			case PipeDirection.West:
				return PipeDirection.East;
			default:
				if (pipeDirection == PipeDirection.East)
				{
					return PipeDirection.West;
				}
				break;
			}
			throw new ArgumentOutOfRangeException("pipeDirection");
		}

		// Token: 0x0600149F RID: 5279 RVA: 0x00044B20 File Offset: 0x00042D20
		public static PipeShape PipeDirectionToPipeShape(this PipeDirection pipeDirection)
		{
			PipeShape result;
			switch (pipeDirection)
			{
			case PipeDirection.North:
				result = PipeShape.Half;
				break;
			case PipeDirection.South:
				result = PipeShape.Half;
				break;
			case PipeDirection.Longitudinal:
				result = PipeShape.Straight;
				break;
			case PipeDirection.West:
				result = PipeShape.Half;
				break;
			case PipeDirection.NWBend:
				result = PipeShape.Bend;
				break;
			case PipeDirection.SWBend:
				result = PipeShape.Bend;
				break;
			case PipeDirection.TWest:
				result = PipeShape.TJunction;
				break;
			case PipeDirection.East:
				result = PipeShape.Half;
				break;
			case PipeDirection.NEBend:
				result = PipeShape.Bend;
				break;
			case PipeDirection.SEBend:
				result = PipeShape.Bend;
				break;
			case PipeDirection.TEast:
				result = PipeShape.TJunction;
				break;
			case PipeDirection.Lateral:
				result = PipeShape.Straight;
				break;
			case PipeDirection.TNorth:
				result = PipeShape.TJunction;
				break;
			case PipeDirection.TSouth:
				result = PipeShape.TJunction;
				break;
			case PipeDirection.Fourway:
				result = PipeShape.Fourway;
				break;
			default:
				throw new ArgumentOutOfRangeException("pipeDirection");
			}
			return result;
		}

		// Token: 0x060014A0 RID: 5280 RVA: 0x00044BBC File Offset: 0x00042DBC
		public static PipeDirection RotatePipeDirection(this PipeDirection pipeDirection, double diff)
		{
			PipeDirection newPipeDir = PipeDirection.None;
			for (int i = 0; i < 4; i++)
			{
				PipeDirection currentPipeDirection = (PipeDirection)(1 << i);
				if (pipeDirection.HasFlag(currentPipeDirection))
				{
					Angle angle = currentPipeDirection.ToAngle();
					newPipeDir |= (angle + diff).GetCardinalDir().ToPipeDirection();
				}
			}
			return newPipeDir;
		}

		// Token: 0x04001497 RID: 5271
		public const int PipeDirections = 4;

		// Token: 0x04001498 RID: 5272
		public const int AllPipeDirections = 6;
	}
}
