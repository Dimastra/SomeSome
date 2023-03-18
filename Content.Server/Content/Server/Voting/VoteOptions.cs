using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Server.Player;
using Robust.Shared.Localization;

namespace Content.Server.Voting
{
	// Token: 0x020000C7 RID: 199
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class VoteOptions
	{
		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000354 RID: 852 RVA: 0x000115C6 File Offset: 0x0000F7C6
		// (set) Token: 0x06000355 RID: 853 RVA: 0x000115CE File Offset: 0x0000F7CE
		public string InitiatorText { get; set; } = "<placeholder>";

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000356 RID: 854 RVA: 0x000115D7 File Offset: 0x0000F7D7
		// (set) Token: 0x06000357 RID: 855 RVA: 0x000115DF File Offset: 0x0000F7DF
		[Nullable(2)]
		public IPlayerSession InitiatorPlayer { [NullableContext(2)] get; [NullableContext(2)] set; }

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000358 RID: 856 RVA: 0x000115E8 File Offset: 0x0000F7E8
		// (set) Token: 0x06000359 RID: 857 RVA: 0x000115F0 File Offset: 0x0000F7F0
		public string Title { get; set; } = "<somebody forgot to fill this in lol>";

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600035A RID: 858 RVA: 0x000115F9 File Offset: 0x0000F7F9
		// (set) Token: 0x0600035B RID: 859 RVA: 0x00011601 File Offset: 0x0000F801
		public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(1.0);

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x0600035C RID: 860 RVA: 0x0001160A File Offset: 0x0000F80A
		// (set) Token: 0x0600035D RID: 861 RVA: 0x00011612 File Offset: 0x0000F812
		public TimeSpan? InitiatorTimeout { get; set; }

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600035E RID: 862 RVA: 0x0001161B File Offset: 0x0000F81B
		// (set) Token: 0x0600035F RID: 863 RVA: 0x00011623 File Offset: 0x0000F823
		[TupleElementNames(new string[]
		{
			"text",
			"data"
		})]
		[Nullable(new byte[]
		{
			1,
			0,
			1,
			1
		})]
		public List<ValueTuple<string, object>> Options { [return: TupleElementNames(new string[]
		{
			"text",
			"data"
		})] [return: Nullable(new byte[]
		{
			1,
			0,
			1,
			1
		})] get; [param: TupleElementNames(new string[]
		{
			"text",
			"data"
		})] [param: Nullable(new byte[]
		{
			1,
			0,
			1,
			1
		})] set; } = new List<ValueTuple<string, object>>();

		// Token: 0x06000360 RID: 864 RVA: 0x0001162C File Offset: 0x0000F82C
		public void SetInitiator(IPlayerSession player)
		{
			this.InitiatorPlayer = player;
			this.InitiatorText = player.Name;
		}

		// Token: 0x06000361 RID: 865 RVA: 0x00011641 File Offset: 0x0000F841
		[NullableContext(2)]
		public void SetInitiatorOrServer(IPlayerSession player)
		{
			if (player != null)
			{
				this.SetInitiator(player);
				return;
			}
			this.InitiatorText = Loc.GetString("vote-options-server-initiator-text");
		}
	}
}
