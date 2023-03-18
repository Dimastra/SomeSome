using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Content.Server.Voting
{
	// Token: 0x020000C4 RID: 196
	[NullableContext(2)]
	[Nullable(0)]
	public sealed class VoteFinishedEventArgs : EventArgs
	{
		// Token: 0x0600034B RID: 843 RVA: 0x000115B0 File Offset: 0x0000F7B0
		public VoteFinishedEventArgs(object winner, [Nullable(new byte[]
		{
			0,
			1
		})] ImmutableArray<object> winners)
		{
			this.Winner = winner;
			this.Winners = winners;
		}

		// Token: 0x0400021C RID: 540
		public readonly object Winner;

		// Token: 0x0400021D RID: 541
		[Nullable(new byte[]
		{
			0,
			1
		})]
		public readonly ImmutableArray<object> Winners;
	}
}
