using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Content.Shared.Hands.Components
{
	// Token: 0x0200043D RID: 1085
	[NullableContext(1)]
	[Nullable(0)]
	public static class HandHelpers
	{
		// Token: 0x06000D23 RID: 3363 RVA: 0x0002BA64 File Offset: 0x00029C64
		public static bool IsAnyHandFree(this SharedHandsComponent component)
		{
			return component.Hands.Values.Any((Hand hand) => hand.IsEmpty);
		}

		// Token: 0x06000D24 RID: 3364 RVA: 0x0002BA95 File Offset: 0x00029C95
		public static int CountFreeHands(this SharedHandsComponent component)
		{
			return component.Hands.Values.Count((Hand hand) => hand.IsEmpty);
		}

		// Token: 0x06000D25 RID: 3365 RVA: 0x0002BAC6 File Offset: 0x00029CC6
		public static IEnumerable<Hand> GetFreeHands(this SharedHandsComponent component)
		{
			return from hand in component.Hands.Values
			where !hand.IsEmpty
			select hand;
		}

		// Token: 0x06000D26 RID: 3366 RVA: 0x0002BAF7 File Offset: 0x00029CF7
		public static IEnumerable<string> GetFreeHandNames(this SharedHandsComponent component)
		{
			return from hand in component.GetFreeHands()
			select hand.Name;
		}
	}
}
