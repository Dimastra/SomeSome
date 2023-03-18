using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Server.Player;

namespace Content.Server.Voting
{
	// Token: 0x020000BE RID: 190
	[NullableContext(1)]
	public interface IVoteHandle
	{
		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000321 RID: 801
		int Id { get; }

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000322 RID: 802
		string Title { get; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000323 RID: 803
		string InitiatorText { get; }

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000324 RID: 804
		bool Finished { get; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000325 RID: 805
		bool Cancelled { get; }

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000326 RID: 806
		IReadOnlyDictionary<object, int> VotesPerOption { get; }

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000327 RID: 807
		// (remove) Token: 0x06000328 RID: 808
		event VoteFinishedEventHandler OnFinished;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000329 RID: 809
		// (remove) Token: 0x0600032A RID: 810
		event VoteCancelledEventHandler OnCancelled;

		// Token: 0x0600032B RID: 811
		bool IsValidOption(int optionId);

		// Token: 0x0600032C RID: 812
		void CastVote(IPlayerSession session, int? optionId);

		// Token: 0x0600032D RID: 813
		void Cancel();
	}
}
