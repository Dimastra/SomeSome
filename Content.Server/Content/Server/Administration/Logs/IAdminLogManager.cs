using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Content.Server.Database;
using Content.Server.GameTicking;
using Content.Shared.Administration.Logs;

namespace Content.Server.Administration.Logs
{
	// Token: 0x0200081B RID: 2075
	[NullableContext(1)]
	public interface IAdminLogManager : ISharedAdminLogManager
	{
		// Token: 0x06002D89 RID: 11657
		void Initialize();

		// Token: 0x06002D8A RID: 11658
		Task Shutdown();

		// Token: 0x06002D8B RID: 11659
		void Update();

		// Token: 0x06002D8C RID: 11660
		void RoundStarting(int id);

		// Token: 0x06002D8D RID: 11661
		void RunLevelChanged(GameRunLevel level);

		// Token: 0x06002D8E RID: 11662
		Task<List<SharedAdminLog>> All([Nullable(2)] LogFilter filter = null, [Nullable(new byte[]
		{
			2,
			1
		})] Func<List<SharedAdminLog>> listProvider = null);

		// Token: 0x06002D8F RID: 11663
		IAsyncEnumerable<string> AllMessages([Nullable(2)] LogFilter filter = null);

		// Token: 0x06002D90 RID: 11664
		IAsyncEnumerable<JsonDocument> AllJson([Nullable(2)] LogFilter filter = null);

		// Token: 0x06002D91 RID: 11665
		Task<Round> Round(int roundId);

		// Token: 0x06002D92 RID: 11666
		Task<List<SharedAdminLog>> CurrentRoundLogs([Nullable(2)] LogFilter filter = null);

		// Token: 0x06002D93 RID: 11667
		IAsyncEnumerable<string> CurrentRoundMessages([Nullable(2)] LogFilter filter = null);

		// Token: 0x06002D94 RID: 11668
		IAsyncEnumerable<JsonDocument> CurrentRoundJson([Nullable(2)] LogFilter filter = null);

		// Token: 0x06002D95 RID: 11669
		Task<Round> CurrentRound();
	}
}
