using System;
using System.Runtime.CompilerServices;
using Content.Shared.Wires;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.Wires
{
	// Token: 0x02000075 RID: 117
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class Wire
	{
		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060001A7 RID: 423 RVA: 0x00009D4E File Offset: 0x00007F4E
		public EntityUid Owner { get; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060001A8 RID: 424 RVA: 0x00009D56 File Offset: 0x00007F56
		// (set) Token: 0x060001A9 RID: 425 RVA: 0x00009D5E File Offset: 0x00007F5E
		public bool IsCut { get; set; }

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060001AA RID: 426 RVA: 0x00009D67 File Offset: 0x00007F67
		// (set) Token: 0x060001AB RID: 427 RVA: 0x00009D6F File Offset: 0x00007F6F
		[ViewVariables]
		public int Id { get; set; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060001AC RID: 428 RVA: 0x00009D78 File Offset: 0x00007F78
		// (set) Token: 0x060001AD RID: 429 RVA: 0x00009D80 File Offset: 0x00007F80
		[ViewVariables]
		public int OriginalPosition { get; set; }

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060001AE RID: 430 RVA: 0x00009D89 File Offset: 0x00007F89
		[ViewVariables]
		public WireColor Color { get; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060001AF RID: 431 RVA: 0x00009D91 File Offset: 0x00007F91
		[ViewVariables]
		public WireLetter Letter { get; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060001B0 RID: 432 RVA: 0x00009D99 File Offset: 0x00007F99
		// (set) Token: 0x060001B1 RID: 433 RVA: 0x00009DA1 File Offset: 0x00007FA1
		public IWireAction Action { get; set; }

		// Token: 0x060001B2 RID: 434 RVA: 0x00009DAA File Offset: 0x00007FAA
		public Wire(EntityUid owner, bool isCut, WireColor color, WireLetter letter, int position, IWireAction action)
		{
			this.Owner = owner;
			this.IsCut = isCut;
			this.Color = color;
			this.OriginalPosition = position;
			this.Letter = letter;
			this.Action = action;
		}
	}
}
