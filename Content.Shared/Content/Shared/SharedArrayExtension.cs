using System;
using System.Runtime.CompilerServices;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Shared
{
	// Token: 0x0200000F RID: 15
	public static class SharedArrayExtension
	{
		// Token: 0x06000015 RID: 21 RVA: 0x000021F8 File Offset: 0x000003F8
		[NullableContext(2)]
		public unsafe static void Shuffle<T>([Nullable(new byte[]
		{
			0,
			1
		})] this Span<T> array, IRobustRandom random = null)
		{
			int i = array.Length;
			if (i <= 1)
			{
				return;
			}
			IoCManager.Resolve<IRobustRandom>(ref random);
			while (i > 1)
			{
				i--;
				int j = random.Next(i + 1);
				ref T ptr = ref array[j];
				ref T ptr2 = ref array[i];
				T t = *array[i];
				T t2 = *array[j];
				ptr = t;
				ptr2 = t2;
			}
		}
	}
}
