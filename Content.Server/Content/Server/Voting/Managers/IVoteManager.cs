using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Content.Shared.Voting;
using Robust.Server.Player;

namespace Content.Server.Voting.Managers
{
	// Token: 0x020000C8 RID: 200
	[NullableContext(1)]
	public interface IVoteManager
	{
		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000363 RID: 867
		IEnumerable<IVoteHandle> ActiveVotes { get; }

		// Token: 0x06000364 RID: 868
		[NullableContext(2)]
		bool TryGetVote(int voteId, [NotNullWhen(true)] out IVoteHandle vote);

		// Token: 0x06000365 RID: 869
		bool CanCallVote(IPlayerSession initiator, StandardVoteType? voteType = null);

		// Token: 0x06000366 RID: 870
		[NullableContext(2)]
		void CreateStandardVote(IPlayerSession initiator, StandardVoteType voteType);

		// Token: 0x06000367 RID: 871
		IVoteHandle CreateVote(VoteOptions options);

		// Token: 0x06000368 RID: 872
		void Initialize();

		// Token: 0x06000369 RID: 873
		void Update();
	}
}
