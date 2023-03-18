using System;
using System.Runtime.CompilerServices;

namespace Content.Shared.Atmos
{
	// Token: 0x02000697 RID: 1687
	public static class PipeShapeHelpers
	{
		// Token: 0x06001499 RID: 5273 RVA: 0x000449A0 File Offset: 0x00042BA0
		public static PipeDirection ToBaseDirection(this PipeShape shape)
		{
			PipeDirection result;
			switch (shape)
			{
			case PipeShape.Half:
				result = PipeDirection.South;
				break;
			case PipeShape.Straight:
				result = PipeDirection.Longitudinal;
				break;
			case PipeShape.Bend:
				result = PipeDirection.SWBend;
				break;
			case PipeShape.TJunction:
				result = PipeDirection.TSouth;
				break;
			case PipeShape.Fourway:
				result = PipeDirection.Fourway;
				break;
			default:
			{
				string paramName = "shape";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 2);
				defaultInterpolatedStringHandler.AppendFormatted<PipeShape>(shape);
				defaultInterpolatedStringHandler.AppendLiteral(" does not have an associated ");
				defaultInterpolatedStringHandler.AppendFormatted("PipeDirection");
				defaultInterpolatedStringHandler.AppendLiteral(".");
				throw new ArgumentOutOfRangeException(paramName, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			}
			return result;
		}
	}
}
