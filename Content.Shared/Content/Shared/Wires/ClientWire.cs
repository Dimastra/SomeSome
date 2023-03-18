using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Wires
{
	// Token: 0x02000029 RID: 41
	[NetSerializable]
	[Serializable]
	public sealed class ClientWire
	{
		// Token: 0x06000035 RID: 53 RVA: 0x000025BF File Offset: 0x000007BF
		public ClientWire(int id, bool isCut, WireColor color, WireLetter letter)
		{
			this.Id = id;
			this.IsCut = isCut;
			this.Letter = letter;
			this.Color = color;
		}

		// Token: 0x0400007A RID: 122
		public int Id;

		// Token: 0x0400007B RID: 123
		public bool IsCut;

		// Token: 0x0400007C RID: 124
		public WireColor Color;

		// Token: 0x0400007D RID: 125
		public WireLetter Letter;
	}
}
