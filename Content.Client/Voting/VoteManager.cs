using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Client.Voting.UI;
using Content.Shared.Voting;
using Robust.Client;
using Robust.Client.Console;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client.Voting
{
	// Token: 0x02000044 RID: 68
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class VoteManager : IVoteManager
	{
		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000129 RID: 297 RVA: 0x0000A393 File Offset: 0x00008593
		// (set) Token: 0x0600012A RID: 298 RVA: 0x0000A39B File Offset: 0x0000859B
		public bool CanCallVote { get; private set; }

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x0600012B RID: 299 RVA: 0x0000A3A4 File Offset: 0x000085A4
		// (remove) Token: 0x0600012C RID: 300 RVA: 0x0000A3DC File Offset: 0x000085DC
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action<bool> CanCallVoteChanged;

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x0600012D RID: 301 RVA: 0x0000A414 File Offset: 0x00008614
		// (remove) Token: 0x0600012E RID: 302 RVA: 0x0000A44C File Offset: 0x0000864C
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action CanCallStandardVotesChanged;

		// Token: 0x0600012F RID: 303 RVA: 0x0000A484 File Offset: 0x00008684
		public void Initialize()
		{
			this._netManager.RegisterNetMessage<MsgVoteData>(new ProcessMessage<MsgVoteData>(this.ReceiveVoteData), 3);
			this._netManager.RegisterNetMessage<MsgVoteCanCall>(new ProcessMessage<MsgVoteCanCall>(this.ReceiveVoteCanCall), 3);
			this._client.RunLevelChanged += this.ClientOnRunLevelChanged;
		}

		// Token: 0x06000130 RID: 304 RVA: 0x0000A4D8 File Offset: 0x000086D8
		private void ClientOnRunLevelChanged([Nullable(2)] object sender, RunLevelChangedEventArgs e)
		{
			if (e.NewLevel == 1)
			{
				this.ClearPopupContainer();
				this._votes.Clear();
			}
		}

		// Token: 0x06000131 RID: 305 RVA: 0x0000A4F4 File Offset: 0x000086F4
		public bool CanCallStandardVote(StandardVoteType type, out TimeSpan whenCan)
		{
			return !this._standardVoteTimeouts.TryGetValue(type, out whenCan);
		}

		// Token: 0x06000132 RID: 306 RVA: 0x0000A508 File Offset: 0x00008708
		public void ClearPopupContainer()
		{
			if (this._popupContainer == null)
			{
				return;
			}
			if (!this._popupContainer.Disposed)
			{
				foreach (VotePopup votePopup in this._votePopups.Values)
				{
					votePopup.Orphan();
				}
			}
			this._votePopups.Clear();
			this._popupContainer = null;
		}

		// Token: 0x06000133 RID: 307 RVA: 0x0000A588 File Offset: 0x00008788
		public void SetPopupContainer(Control container)
		{
			if (this._popupContainer != null)
			{
				this.ClearPopupContainer();
			}
			this._popupContainer = container;
			foreach (KeyValuePair<int, VoteManager.ActiveVote> keyValuePair in this._votes)
			{
				int num;
				VoteManager.ActiveVote vote;
				keyValuePair.Deconstruct(out num, out vote);
				int key = num;
				VotePopup votePopup = new VotePopup(vote);
				this._votePopups.Add(key, votePopup);
				this._popupContainer.AddChild(votePopup);
				votePopup.UpdateData();
			}
		}

		// Token: 0x06000134 RID: 308 RVA: 0x0000A624 File Offset: 0x00008824
		private void ReceiveVoteData(MsgVoteData message)
		{
			bool flag = false;
			int voteId = message.VoteId;
			VoteManager.ActiveVote activeVote;
			if (!this._votes.TryGetValue(voteId, out activeVote))
			{
				if (!message.VoteActive)
				{
					return;
				}
				flag = true;
				IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<AudioSystem>().PlayGlobal("/Audio/Effects/voteding.ogg", Filter.Local(), false, null);
				this.SetPopupContainer(this._userInterfaceManager.WindowRoot);
				VoteManager.ActiveVote activeVote2 = new VoteManager.ActiveVote(voteId);
				activeVote2.Entries = (from c in message.Options
				select new VoteManager.VoteEntry(c.Item2)).ToArray<VoteManager.VoteEntry>();
				VoteManager.ActiveVote activeVote3 = activeVote2;
				activeVote = activeVote3;
				this._votes.Add(voteId, activeVote3);
			}
			else if (!message.VoteActive)
			{
				this._votes.Remove(voteId);
				VotePopup votePopup;
				if (this._votePopups.TryGetValue(voteId, out votePopup))
				{
					votePopup.Orphan();
					this._votePopups.Remove(voteId);
				}
				return;
			}
			if (message.IsYourVoteDirty)
			{
				VoteManager.ActiveVote activeVote4 = activeVote;
				byte? yourVote = message.YourVote;
				activeVote4.OurVote = ((yourVote != null) ? new int?((int)yourVote.GetValueOrDefault()) : null);
			}
			activeVote.Initiator = message.VoteInitiator;
			activeVote.Title = message.VoteTitle;
			activeVote.StartTime = this._gameTiming.RealServerToLocal(message.StartTime);
			activeVote.EndTime = this._gameTiming.RealServerToLocal(message.EndTime);
			for (int i = 0; i < message.Options.Length; i++)
			{
				activeVote.Entries[i].Votes = (int)message.Options[i].Item1;
			}
			if (flag && this._popupContainer != null)
			{
				VotePopup votePopup2 = new VotePopup(activeVote);
				this._votePopups.Add(voteId, votePopup2);
				this._popupContainer.AddChild(votePopup2);
			}
			VotePopup votePopup3;
			if (this._votePopups.TryGetValue(voteId, out votePopup3))
			{
				votePopup3.UpdateData();
			}
		}

		// Token: 0x06000135 RID: 309 RVA: 0x0000A810 File Offset: 0x00008A10
		private void ReceiveVoteCanCall(MsgVoteCanCall message)
		{
			if (this.CanCallVote != message.CanCall)
			{
				this.CanCallVote = message.CanCall;
				Action<bool> canCallVoteChanged = this.CanCallVoteChanged;
				if (canCallVoteChanged != null)
				{
					canCallVoteChanged(this.CanCallVote);
				}
			}
			this._standardVoteTimeouts.Clear();
			foreach (ValueTuple<StandardVoteType, TimeSpan> valueTuple in message.VotesUnavailable)
			{
				StandardVoteType item = valueTuple.Item1;
				TimeSpan item2 = valueTuple.Item2;
				TimeSpan value = (item2 == TimeSpan.Zero) ? item2 : this._gameTiming.RealServerToLocal(item2);
				this._standardVoteTimeouts.Add(item, value);
			}
			Action canCallStandardVotesChanged = this.CanCallStandardVotesChanged;
			if (canCallStandardVotesChanged == null)
			{
				return;
			}
			canCallStandardVotesChanged();
		}

		// Token: 0x06000136 RID: 310 RVA: 0x0000A8C0 File Offset: 0x00008AC0
		public void SendCastVote(int voteId, int option)
		{
			this._votes[voteId].OurVote = new int?(option);
			IConsoleShell localShell = this._console.LocalShell;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 2);
			defaultInterpolatedStringHandler.AppendLiteral("vote ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(voteId);
			defaultInterpolatedStringHandler.AppendLiteral(" ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(option);
			localShell.RemoteExecuteCommand(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x040000CF RID: 207
		[Dependency]
		private readonly IClientNetManager _netManager;

		// Token: 0x040000D0 RID: 208
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x040000D1 RID: 209
		[Dependency]
		private readonly IClientConsoleHost _console;

		// Token: 0x040000D2 RID: 210
		[Dependency]
		private readonly IBaseClient _client;

		// Token: 0x040000D3 RID: 211
		[Dependency]
		private readonly IUserInterfaceManager _userInterfaceManager;

		// Token: 0x040000D4 RID: 212
		private readonly Dictionary<StandardVoteType, TimeSpan> _standardVoteTimeouts = new Dictionary<StandardVoteType, TimeSpan>();

		// Token: 0x040000D5 RID: 213
		private readonly Dictionary<int, VoteManager.ActiveVote> _votes = new Dictionary<int, VoteManager.ActiveVote>();

		// Token: 0x040000D6 RID: 214
		private readonly Dictionary<int, VotePopup> _votePopups = new Dictionary<int, VotePopup>();

		// Token: 0x040000D7 RID: 215
		[Nullable(2)]
		private Control _popupContainer;

		// Token: 0x02000045 RID: 69
		[Nullable(0)]
		public sealed class ActiveVote
		{
			// Token: 0x06000138 RID: 312 RVA: 0x0000A955 File Offset: 0x00008B55
			public ActiveVote(int voteId)
			{
				this.Id = voteId;
			}

			// Token: 0x040000DB RID: 219
			public VoteManager.VoteEntry[] Entries;

			// Token: 0x040000DC RID: 220
			public TimeSpan StartTime;

			// Token: 0x040000DD RID: 221
			public TimeSpan EndTime;

			// Token: 0x040000DE RID: 222
			public string Title = "";

			// Token: 0x040000DF RID: 223
			public string Initiator = "";

			// Token: 0x040000E0 RID: 224
			public int? OurVote;

			// Token: 0x040000E1 RID: 225
			public int Id;
		}

		// Token: 0x02000046 RID: 70
		[Nullable(0)]
		public sealed class VoteEntry
		{
			// Token: 0x17000023 RID: 35
			// (get) Token: 0x06000139 RID: 313 RVA: 0x0000A97A File Offset: 0x00008B7A
			public string Text { get; }

			// Token: 0x17000024 RID: 36
			// (get) Token: 0x0600013A RID: 314 RVA: 0x0000A982 File Offset: 0x00008B82
			// (set) Token: 0x0600013B RID: 315 RVA: 0x0000A98A File Offset: 0x00008B8A
			public int Votes { get; set; }

			// Token: 0x0600013C RID: 316 RVA: 0x0000A993 File Offset: 0x00008B93
			public VoteEntry(string text)
			{
				this.Text = text;
			}
		}
	}
}
