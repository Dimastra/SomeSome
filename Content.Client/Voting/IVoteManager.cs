using System;
using System.Runtime.CompilerServices;
using Content.Shared.Voting;
using Robust.Client.UserInterface;

namespace Content.Client.Voting
{
	// Token: 0x02000043 RID: 67
	[NullableContext(1)]
	public interface IVoteManager
	{
		// Token: 0x0600011F RID: 287
		void Initialize();

		// Token: 0x06000120 RID: 288
		void SendCastVote(int voteId, int option);

		// Token: 0x06000121 RID: 289
		void ClearPopupContainer();

		// Token: 0x06000122 RID: 290
		void SetPopupContainer(Control container);

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000123 RID: 291
		bool CanCallVote { get; }

		// Token: 0x06000124 RID: 292
		bool CanCallStandardVote(StandardVoteType type, out TimeSpan whenCan);

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06000125 RID: 293
		// (remove) Token: 0x06000126 RID: 294
		event Action<bool> CanCallVoteChanged;

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06000127 RID: 295
		// (remove) Token: 0x06000128 RID: 296
		event Action CanCallStandardVotesChanged;
	}
}
