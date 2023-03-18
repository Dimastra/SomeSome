using System;
using Content.Shared.FixedPoint;

namespace Content.Shared.Chemistry
{
	// Token: 0x020005D2 RID: 1490
	public static class ChemMasterReagentAmountToFixedPoint
	{
		// Token: 0x06001203 RID: 4611 RVA: 0x0003B362 File Offset: 0x00039562
		public static FixedPoint2 GetFixedPoint(this ChemMasterReagentAmount amount)
		{
			if (amount == ChemMasterReagentAmount.All)
			{
				return FixedPoint2.MaxValue;
			}
			return FixedPoint2.New((int)amount);
		}
	}
}
