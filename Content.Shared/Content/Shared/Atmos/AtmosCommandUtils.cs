using System;
using System.Runtime.CompilerServices;

namespace Content.Shared.Atmos
{
	// Token: 0x0200068B RID: 1675
	public sealed class AtmosCommandUtils
	{
		// Token: 0x06001483 RID: 5251 RVA: 0x00044408 File Offset: 0x00042608
		[NullableContext(1)]
		public static bool TryParseGasID(string str, out int x)
		{
			x = -1;
			Gas gas;
			if (Enum.TryParse<Gas>(str, true, out gas))
			{
				x = (int)gas;
			}
			else if (!int.TryParse(str, out x))
			{
				return false;
			}
			return x >= 0 && x < 9;
		}
	}
}
