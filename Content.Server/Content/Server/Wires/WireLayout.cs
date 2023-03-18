using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Wires;
using Robust.Shared.ViewVariables;

namespace Content.Server.Wires
{
	// Token: 0x02000078 RID: 120
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class WireLayout
	{
		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060001BA RID: 442 RVA: 0x00009E05 File Offset: 0x00008005
		[ViewVariables]
		public IReadOnlyDictionary<int, WireLayout.WireData> Specifications { get; }

		// Token: 0x060001BB RID: 443 RVA: 0x00009E0D File Offset: 0x0000800D
		public WireLayout(IReadOnlyDictionary<int, WireLayout.WireData> specifications)
		{
			this.Specifications = specifications;
		}

		// Token: 0x02000899 RID: 2201
		[NullableContext(0)]
		public sealed class WireData
		{
			// Token: 0x170007E8 RID: 2024
			// (get) Token: 0x06002FD9 RID: 12249 RVA: 0x000F6D23 File Offset: 0x000F4F23
			public WireLetter Letter { get; }

			// Token: 0x170007E9 RID: 2025
			// (get) Token: 0x06002FDA RID: 12250 RVA: 0x000F6D2B File Offset: 0x000F4F2B
			public WireColor Color { get; }

			// Token: 0x170007EA RID: 2026
			// (get) Token: 0x06002FDB RID: 12251 RVA: 0x000F6D33 File Offset: 0x000F4F33
			public int Position { get; }

			// Token: 0x06002FDC RID: 12252 RVA: 0x000F6D3B File Offset: 0x000F4F3B
			public WireData(WireLetter letter, WireColor color, int position)
			{
				this.Letter = letter;
				this.Color = color;
				this.Position = position;
			}
		}
	}
}
