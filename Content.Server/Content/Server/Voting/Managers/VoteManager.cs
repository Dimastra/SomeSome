using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Administration.Logs;
using Content.Server.Administration.Managers;
using Content.Server.Chat.Managers;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Presets;
using Content.Server.Maps;
using Content.Server.RoundEnd;
using Content.Shared.Administration;
using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Database;
using Content.Shared.Voting;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Server.Voting.Managers
{
	// Token: 0x020000C9 RID: 201
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class VoteManager : IVoteManager
	{
		// Token: 0x0600036A RID: 874 RVA: 0x0001169C File Offset: 0x0000F89C
		public void Initialize()
		{
			this._netManager.RegisterNetMessage<MsgVoteData>(null, 3);
			this._netManager.RegisterNetMessage<MsgVoteCanCall>(null, 3);
			this._netManager.RegisterNetMessage<MsgVoteMenu>(new ProcessMessage<MsgVoteMenu>(this.ReceiveVoteMenu), 3);
			this._playerManager.PlayerStatusChanged += this.PlayerManagerOnPlayerStatusChanged;
			this._adminMgr.OnPermsChanged += this.AdminPermsChanged;
			this._cfg.OnValueChanged<bool>(CCVars.VoteEnabled, delegate(bool value)
			{
				this.DirtyCanCallVoteAll();
			}, false);
			foreach (KeyValuePair<StandardVoteType, CVarDef<bool>> kvp in VoteManager._voteTypesToEnableCVars)
			{
				this._cfg.OnValueChanged<bool>(kvp.Value, delegate(bool value)
				{
					this.DirtyCanCallVoteAll();
				}, false);
			}
		}

		// Token: 0x0600036B RID: 875 RVA: 0x00011784 File Offset: 0x0000F984
		private void ReceiveVoteMenu(MsgVoteMenu message)
		{
			INetChannel sender = message.MsgChannel;
			IPlayerSession session = this._playerManager.GetSessionByChannel(sender);
			ISharedAdminLogManager adminLogger = this._adminLogger;
			LogType type = LogType.Vote;
			LogImpact impact = LogImpact.Low;
			LogStringHandler logStringHandler = new LogStringHandler(17, 1);
			logStringHandler.AppendFormatted<IPlayerSession>(session, "session");
			logStringHandler.AppendLiteral(" opened vote menu");
			adminLogger.Add(type, impact, ref logStringHandler);
		}

		// Token: 0x0600036C RID: 876 RVA: 0x000117D8 File Offset: 0x0000F9D8
		private void AdminPermsChanged(AdminPermsChangedEventArgs obj)
		{
			this.DirtyCanCallVote(obj.Player);
		}

		// Token: 0x0600036D RID: 877 RVA: 0x000117E8 File Offset: 0x0000F9E8
		private void PlayerManagerOnPlayerStatusChanged([Nullable(2)] object sender, SessionStatusEventArgs e)
		{
			if (e.NewStatus == 3)
			{
				foreach (VoteManager.VoteReg voteReg in this._votes.Values)
				{
					this.SendSingleUpdate(voteReg, e.Session);
				}
				this.DirtyCanCallVote(e.Session);
				return;
			}
			if (e.NewStatus == 4)
			{
				foreach (VoteManager.VoteReg voteReg2 in this._votes.Values)
				{
					this.CastVote(voteReg2, e.Session, null);
				}
			}
		}

		// Token: 0x0600036E RID: 878 RVA: 0x000118BC File Offset: 0x0000FABC
		private void CastVote(VoteManager.VoteReg v, IPlayerSession player, int? option)
		{
			if (!this.IsValidOption(v, option))
			{
				throw new ArgumentOutOfRangeException("option", "Invalid vote option ID");
			}
			int existingOption;
			if (v.CastVotes.TryGetValue(player, out existingOption))
			{
				VoteManager.VoteEntry[] entries = v.Entries;
				int num = existingOption;
				entries[num].Votes = entries[num].Votes - 1;
			}
			if (option != null)
			{
				VoteManager.VoteEntry[] entries2 = v.Entries;
				int value = option.Value;
				entries2[value].Votes = entries2[value].Votes + 1;
				v.CastVotes[player] = option.Value;
			}
			else
			{
				v.CastVotes.Remove(player);
			}
			v.VotesDirty.Add(player);
			v.Dirty = true;
		}

		// Token: 0x0600036F RID: 879 RVA: 0x00011964 File Offset: 0x0000FB64
		private bool IsValidOption(VoteManager.VoteReg voteReg, int? option)
		{
			if (option == null)
			{
				return true;
			}
			int? num = option;
			int num2 = 0;
			if (num.GetValueOrDefault() >= num2 & num != null)
			{
				num = option;
				num2 = voteReg.Entries.Length;
				return num.GetValueOrDefault() < num2 & num != null;
			}
			return false;
		}

		// Token: 0x06000370 RID: 880 RVA: 0x000119B8 File Offset: 0x0000FBB8
		public void Update()
		{
			RemQueue<int> remQueue = default(RemQueue<int>);
			foreach (VoteManager.VoteReg v in this._votes.Values)
			{
				if (this._timing.RealTime >= v.EndTime)
				{
					this.EndVote(v);
				}
				if (v.Finished)
				{
					remQueue.Add(v.Id);
				}
				if (v.Dirty)
				{
					this.SendUpdates(v);
				}
			}
			foreach (int id in remQueue)
			{
				this._votes.Remove(id);
				this._voteHandles.Remove(id);
			}
			RemQueue<NetUserId> timeoutRemQueue = default(RemQueue<NetUserId>);
			foreach (KeyValuePair<NetUserId, TimeSpan> keyValuePair in this._voteTimeout)
			{
				NetUserId netUserId;
				TimeSpan t;
				keyValuePair.Deconstruct(out netUserId, out t);
				NetUserId userId = netUserId;
				if (t < this._timing.RealTime)
				{
					timeoutRemQueue.Add(userId);
				}
			}
			foreach (NetUserId userId2 in timeoutRemQueue)
			{
				this._voteTimeout.Remove(userId2);
				IPlayerSession session;
				if (this._playerManager.TryGetSessionById(userId2, ref session))
				{
					this.DirtyCanCallVote(session);
				}
			}
			RemQueue<StandardVoteType> stdTimeoutRemQueue = default(RemQueue<StandardVoteType>);
			foreach (KeyValuePair<StandardVoteType, TimeSpan> keyValuePair2 in this._standardVoteTimeout)
			{
				TimeSpan t;
				StandardVoteType standardVoteType;
				keyValuePair2.Deconstruct(out standardVoteType, out t);
				StandardVoteType type = standardVoteType;
				if (t < this._timing.RealTime)
				{
					stdTimeoutRemQueue.Add(type);
				}
			}
			foreach (StandardVoteType type2 in stdTimeoutRemQueue)
			{
				this._standardVoteTimeout.Remove(type2);
				this.DirtyCanCallVoteAll();
			}
			foreach (IPlayerSession dirtyPlayer in this._playerCanCallVoteDirty)
			{
				if (dirtyPlayer.Status != 4)
				{
					this.SendUpdateCanCallVote(dirtyPlayer);
				}
			}
			this._playerCanCallVoteDirty.Clear();
		}

		// Token: 0x06000371 RID: 881 RVA: 0x00011C9C File Offset: 0x0000FE9C
		public IVoteHandle CreateVote(VoteOptions options)
		{
			int nextVoteId = this._nextVoteId;
			this._nextVoteId = nextVoteId + 1;
			int id = nextVoteId;
			VoteManager.VoteEntry[] entries = (from o in options.Options
			select new VoteManager.VoteEntry(o.Item2, o.Item1)).ToArray<VoteManager.VoteEntry>();
			TimeSpan start = this._timing.RealTime;
			TimeSpan end = start + options.Duration;
			VoteManager.VoteReg reg = new VoteManager.VoteReg(id, entries, options.Title, options.InitiatorText, options.InitiatorPlayer, start, end);
			VoteManager.VoteHandle handle = new VoteManager.VoteHandle(this, reg);
			this._votes.Add(id, reg);
			this._voteHandles.Add(id, handle);
			if (options.InitiatorPlayer != null)
			{
				TimeSpan timeout = options.InitiatorTimeout ?? (options.Duration * 2.0);
				this._voteTimeout[options.InitiatorPlayer.UserId] = this._timing.RealTime + timeout;
			}
			this.DirtyCanCallVoteAll();
			return handle;
		}

		// Token: 0x06000372 RID: 882 RVA: 0x00011DB4 File Offset: 0x0000FFB4
		private void SendUpdates(VoteManager.VoteReg v)
		{
			foreach (IPlayerSession player in this._playerManager.ServerSessions)
			{
				this.SendSingleUpdate(v, player);
			}
			v.VotesDirty.Clear();
			v.Dirty = false;
		}

		// Token: 0x06000373 RID: 883 RVA: 0x00011E1C File Offset: 0x0001001C
		private void SendSingleUpdate(VoteManager.VoteReg v, IPlayerSession player)
		{
			MsgVoteData msg = new MsgVoteData();
			msg.VoteId = v.Id;
			msg.VoteActive = !v.Finished;
			if (!v.Finished)
			{
				msg.VoteTitle = v.Title;
				msg.VoteInitiator = v.InitiatorText;
				msg.StartTime = v.StartTime;
				msg.EndTime = v.EndTime;
			}
			int cast;
			if (v.CastVotes.TryGetValue(player, out cast))
			{
				bool dirty = v.VotesDirty.Contains(player);
				msg.IsYourVoteDirty = dirty;
				if (dirty)
				{
					msg.YourVote = new byte?((byte)cast);
				}
			}
			msg.Options = new ValueTuple<ushort, string>[v.Entries.Length];
			for (int i = 0; i < msg.Options.Length; i++)
			{
				ref VoteManager.VoteEntry entry = ref v.Entries[i];
				msg.Options[i] = new ValueTuple<ushort, string>((ushort)entry.Votes, entry.Text);
			}
			player.ConnectedClient.SendMessage(msg);
		}

		// Token: 0x06000374 RID: 884 RVA: 0x00011F16 File Offset: 0x00010116
		private void DirtyCanCallVoteAll()
		{
			this._playerCanCallVoteDirty.UnionWith(this._playerManager.ServerSessions);
		}

		// Token: 0x06000375 RID: 885 RVA: 0x00011F30 File Offset: 0x00010130
		private void SendUpdateCanCallVote(IPlayerSession player)
		{
			MsgVoteCanCall msg = new MsgVoteCanCall();
			bool isAdmin;
			TimeSpan timeSpan;
			msg.CanCall = this.CanCallVote(player, null, out isAdmin, out timeSpan);
			msg.WhenCanCallVote = timeSpan;
			if (isAdmin)
			{
				msg.VotesUnavailable = Array.Empty<ValueTuple<StandardVoteType, TimeSpan>>();
			}
			else
			{
				List<ValueTuple<StandardVoteType, TimeSpan>> votesUnavailable = new List<ValueTuple<StandardVoteType, TimeSpan>>();
				foreach (StandardVoteType v in this._standardVoteTypeValues)
				{
					bool _isAdmin;
					TimeSpan typeTimeSpan;
					if (!this.CanCallVote(player, new StandardVoteType?(v), out _isAdmin, out typeTimeSpan))
					{
						votesUnavailable.Add(new ValueTuple<StandardVoteType, TimeSpan>(v, typeTimeSpan));
					}
				}
				msg.VotesUnavailable = votesUnavailable.ToArray();
			}
			this._netManager.ServerSendMessage(msg, player.ConnectedClient);
		}

		// Token: 0x06000376 RID: 886 RVA: 0x00011FE0 File Offset: 0x000101E0
		private bool CanCallVote(IPlayerSession initiator, StandardVoteType? voteType, out bool isAdmin, out TimeSpan timeSpan)
		{
			isAdmin = false;
			timeSpan = default(TimeSpan);
			if (this._adminMgr.HasAdminFlag(initiator, AdminFlags.Admin))
			{
				isAdmin = true;
				return true;
			}
			if (!this._cfg.GetCVar<bool>(CCVars.VoteEnabled))
			{
				return false;
			}
			CVarDef<bool> cvar;
			if (voteType != null && VoteManager._voteTypesToEnableCVars.TryGetValue(voteType.Value, out cvar) && !this._cfg.GetCVar<bool>(cvar))
			{
				return false;
			}
			if (this._votes.Count != 0)
			{
				return false;
			}
			if (voteType != null && this._standardVoteTimeout.TryGetValue(voteType.Value, out timeSpan))
			{
				return false;
			}
			StandardVoteType? standardVoteType = voteType;
			StandardVoteType standardVoteType2 = StandardVoteType.Restart;
			if ((standardVoteType.GetValueOrDefault() == standardVoteType2 & standardVoteType != null) && this._cfg.GetCVar<bool>(CCVars.VoteRestartNotAllowedWhenAdminOnline) && this._adminMgr.ActiveAdmins.Count<IPlayerSession>() != 0)
			{
				return false;
			}
			standardVoteType = voteType;
			standardVoteType2 = StandardVoteType.Preset;
			if (standardVoteType.GetValueOrDefault() == standardVoteType2 & standardVoteType != null)
			{
				Dictionary<string, string> presets = this.GetGamePresets();
				if (presets.Count<KeyValuePair<string, string>>() == 1)
				{
					string a = (from x in presets
					select x.Key).Single<string>();
					GamePresetPrototype preset = EntitySystem.Get<GameTicker>().Preset;
					if (a == ((preset != null) ? preset.ID : null))
					{
						return false;
					}
				}
			}
			return !this._voteTimeout.TryGetValue(initiator.UserId, out timeSpan);
		}

		// Token: 0x06000377 RID: 887 RVA: 0x00012144 File Offset: 0x00010344
		public bool CanCallVote(IPlayerSession initiator, StandardVoteType? voteType = null)
		{
			bool flag;
			TimeSpan timeSpan;
			return this.CanCallVote(initiator, voteType, out flag, out timeSpan);
		}

		// Token: 0x06000378 RID: 888 RVA: 0x00012160 File Offset: 0x00010360
		private void EndVote(VoteManager.VoteReg v)
		{
			if (v.Finished)
			{
				return;
			}
			ImmutableArray<object> winners = (from e in (from e in v.Entries
			group e by e.Votes into g
			orderby g.Key descending
			select g).First<IGrouping<int, VoteManager.VoteEntry>>()
			select e.Data).ToImmutableArray<object>();
			v.Finished = true;
			v.Dirty = true;
			VoteFinishedEventArgs args = new VoteFinishedEventArgs((winners.Length == 1) ? winners[0] : null, winners);
			VoteFinishedEventHandler onFinished = v.OnFinished;
			if (onFinished != null)
			{
				onFinished(this._voteHandles[v.Id], args);
			}
			this.DirtyCanCallVoteAll();
		}

		// Token: 0x06000379 RID: 889 RVA: 0x00012248 File Offset: 0x00010448
		private void CancelVote(VoteManager.VoteReg v)
		{
			if (v.Cancelled)
			{
				return;
			}
			v.Cancelled = true;
			v.Finished = true;
			v.Dirty = true;
			VoteCancelledEventHandler onCancelled = v.OnCancelled;
			if (onCancelled != null)
			{
				onCancelled(this._voteHandles[v.Id]);
			}
			this.DirtyCanCallVoteAll();
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x0600037A RID: 890 RVA: 0x0001229B File Offset: 0x0001049B
		public IEnumerable<IVoteHandle> ActiveVotes
		{
			get
			{
				return this._voteHandles.Values;
			}
		}

		// Token: 0x0600037B RID: 891 RVA: 0x000122A8 File Offset: 0x000104A8
		[NullableContext(2)]
		public bool TryGetVote(int voteId, [NotNullWhen(true)] out IVoteHandle vote)
		{
			VoteManager.VoteHandle vHandle;
			if (this._voteHandles.TryGetValue(voteId, out vHandle))
			{
				vote = vHandle;
				return true;
			}
			vote = null;
			return false;
		}

		// Token: 0x0600037C RID: 892 RVA: 0x000122CE File Offset: 0x000104CE
		private void DirtyCanCallVote(IPlayerSession player)
		{
			this._playerCanCallVoteDirty.Add(player);
		}

		// Token: 0x0600037D RID: 893 RVA: 0x000122DD File Offset: 0x000104DD
		private void WirePresetVoteInitiator(VoteOptions options, [Nullable(2)] IPlayerSession player)
		{
			if (player != null)
			{
				options.SetInitiator(player);
				return;
			}
			options.InitiatorText = Loc.GetString("ui-vote-initiator-server");
		}

		// Token: 0x0600037E RID: 894 RVA: 0x000122FC File Offset: 0x000104FC
		[NullableContext(2)]
		public void CreateStandardVote(IPlayerSession initiator, StandardVoteType voteType)
		{
			if (initiator != null)
			{
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Vote;
				LogImpact impact = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(18, 2);
				logStringHandler.AppendFormatted<IPlayerSession>(initiator, "initiator");
				logStringHandler.AppendLiteral(" initiated a ");
				logStringHandler.AppendFormatted(voteType.ToString());
				logStringHandler.AppendLiteral(" vote");
				adminLogger.Add(type, impact, ref logStringHandler);
			}
			else
			{
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.Vote;
				LogImpact impact2 = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(17, 1);
				logStringHandler.AppendLiteral("Initiated a ");
				logStringHandler.AppendFormatted(voteType.ToString());
				logStringHandler.AppendLiteral(" vote");
				adminLogger2.Add(type2, impact2, ref logStringHandler);
			}
			switch (voteType)
			{
			case StandardVoteType.Restart:
				this.CreateRestartVote(initiator);
				break;
			case StandardVoteType.Preset:
				this.CreatePresetVote(initiator);
				break;
			case StandardVoteType.Map:
				this.CreateMapVote(initiator);
				break;
			default:
				throw new ArgumentOutOfRangeException("voteType", voteType, null);
			}
			this._entityManager.EntitySysManager.GetEntitySystem<GameTicker>().UpdateInfoText();
			this.TimeoutStandardVote(voteType);
		}

		// Token: 0x0600037F RID: 895 RVA: 0x00012404 File Offset: 0x00010604
		[NullableContext(2)]
		private void CreateRestartVote(IPlayerSession initiator)
		{
			bool alone = this._playerManager.PlayerCount == 1 && initiator != null;
			VoteOptions options = new VoteOptions
			{
				Title = Loc.GetString("ui-vote-restart-title"),
				Options = 
				{
					new ValueTuple<string, object>(Loc.GetString("ui-vote-restart-yes"), "yes"),
					new ValueTuple<string, object>(Loc.GetString("ui-vote-restart-no"), "no"),
					new ValueTuple<string, object>(Loc.GetString("ui-vote-restart-abstain"), "abstain")
				},
				Duration = (alone ? TimeSpan.FromSeconds((double)this._cfg.GetCVar<int>(CCVars.VoteTimerAlone)) : TimeSpan.FromSeconds((double)this._cfg.GetCVar<int>(CCVars.VoteTimerRestart))),
				InitiatorTimeout = new TimeSpan?(TimeSpan.FromMinutes(5.0))
			};
			if (alone)
			{
				options.InitiatorTimeout = new TimeSpan?(TimeSpan.FromSeconds(10.0));
			}
			this.WirePresetVoteInitiator(options, initiator);
			IVoteHandle vote = this.CreateVote(options);
			vote.OnFinished += delegate(IVoteHandle _, VoteFinishedEventArgs _)
			{
				int votesYes = vote.VotesPerOption["yes"];
				int votesNo = vote.VotesPerOption["no"];
				int total = votesYes + votesNo;
				float ratioRequired = this._cfg.GetCVar<float>(CCVars.VoteRestartRequiredRatio);
				LogStringHandler logStringHandler;
				if (total > 0 && (float)votesYes / (float)total >= ratioRequired)
				{
					ISharedAdminLogManager adminLogger = this._adminLogger;
					LogType type = LogType.Vote;
					LogImpact impact = LogImpact.Medium;
					logStringHandler = new LogStringHandler(25, 2);
					logStringHandler.AppendLiteral("Restart vote succeeded: ");
					logStringHandler.AppendFormatted<int>(votesYes, "votesYes");
					logStringHandler.AppendLiteral("/");
					logStringHandler.AppendFormatted<int>(votesNo, "votesNo");
					adminLogger.Add(type, impact, ref logStringHandler);
					this._chatManager.DispatchServerAnnouncement(Loc.GetString("ui-vote-restart-succeeded"), null);
					this._entityManager.EntitySysManager.GetEntitySystem<RoundEndSystem>().EndRound();
					return;
				}
				ISharedAdminLogManager adminLogger2 = this._adminLogger;
				LogType type2 = LogType.Vote;
				LogImpact impact2 = LogImpact.Medium;
				logStringHandler = new LogStringHandler(22, 2);
				logStringHandler.AppendLiteral("Restart vote failed: ");
				logStringHandler.AppendFormatted<int>(votesYes, "votesYes");
				logStringHandler.AppendLiteral("/");
				logStringHandler.AppendFormatted<int>(votesNo, "votesNo");
				adminLogger2.Add(type2, impact2, ref logStringHandler);
				this._chatManager.DispatchServerAnnouncement(Loc.GetString("ui-vote-restart-failed", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("ratio", ratioRequired)
				}), null);
			};
			if (initiator != null)
			{
				vote.CastVote(initiator, new int?(0));
			}
			foreach (IPlayerSession player in this._playerManager.ServerSessions)
			{
				if (player != initiator)
				{
					vote.CastVote(player, new int?(2));
				}
			}
		}

		// Token: 0x06000380 RID: 896 RVA: 0x000125AC File Offset: 0x000107AC
		[NullableContext(2)]
		private void CreatePresetVote(IPlayerSession initiator)
		{
			Dictionary<string, string> presets = this.GetGamePresets();
			bool alone = this._playerManager.PlayerCount == 1 && initiator != null;
			VoteOptions options = new VoteOptions
			{
				Title = Loc.GetString("ui-vote-gamemode-title"),
				Duration = (alone ? TimeSpan.FromSeconds((double)this._cfg.GetCVar<int>(CCVars.VoteTimerAlone)) : TimeSpan.FromSeconds((double)this._cfg.GetCVar<int>(CCVars.VoteTimerPreset)))
			};
			if (alone)
			{
				options.InitiatorTimeout = new TimeSpan?(TimeSpan.FromSeconds(10.0));
			}
			foreach (KeyValuePair<string, string> keyValuePair in presets)
			{
				string text;
				string text2;
				keyValuePair.Deconstruct(out text, out text2);
				string i = text;
				string v = text2;
				options.Options.Add(new ValueTuple<string, object>(Loc.GetString(v), i));
			}
			this.WirePresetVoteInitiator(options, initiator);
			this.CreateVote(options).OnFinished += delegate(IVoteHandle _, VoteFinishedEventArgs args)
			{
				string picked;
				if (args.Winner == null)
				{
					picked = (string)RandomExtensions.Pick<object>(this._random, args.Winners);
					this._chatManager.DispatchServerAnnouncement(Loc.GetString("ui-vote-gamemode-tie", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("picked", Loc.GetString(presets[picked]))
					}), null);
				}
				else
				{
					picked = (string)args.Winner;
					this._chatManager.DispatchServerAnnouncement(Loc.GetString("ui-vote-gamemode-win", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("winner", Loc.GetString(presets[picked]))
					}), null);
				}
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Vote;
				LogImpact impact = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(22, 1);
				logStringHandler.AppendLiteral("Preset vote finished: ");
				logStringHandler.AppendFormatted(picked);
				adminLogger.Add(type, impact, ref logStringHandler);
				this._entityManager.EntitySysManager.GetEntitySystem<GameTicker>().SetGamePreset(picked, false);
			};
		}

		// Token: 0x06000381 RID: 897 RVA: 0x000126E0 File Offset: 0x000108E0
		[NullableContext(2)]
		private void CreateMapVote(IPlayerSession initiator)
		{
			Dictionary<GameMapPrototype, string> maps = this._gameMapManager.CurrentlyEligibleMaps().ToDictionary((GameMapPrototype map) => map, (GameMapPrototype map) => map.MapName);
			bool alone = this._playerManager.PlayerCount == 1 && initiator != null;
			VoteOptions options = new VoteOptions
			{
				Title = Loc.GetString("ui-vote-map-title"),
				Duration = (alone ? TimeSpan.FromSeconds((double)this._cfg.GetCVar<int>(CCVars.VoteTimerAlone)) : TimeSpan.FromSeconds((double)this._cfg.GetCVar<int>(CCVars.VoteTimerMap)))
			};
			if (alone)
			{
				options.InitiatorTimeout = new TimeSpan?(TimeSpan.FromSeconds(10.0));
			}
			foreach (KeyValuePair<GameMapPrototype, string> keyValuePair in maps)
			{
				GameMapPrototype gameMapPrototype;
				string text;
				keyValuePair.Deconstruct(out gameMapPrototype, out text);
				GameMapPrototype i = gameMapPrototype;
				string v = text;
				options.Options.Add(new ValueTuple<string, object>(v, i));
			}
			this.WirePresetVoteInitiator(options, initiator);
			this.CreateVote(options).OnFinished += delegate(IVoteHandle _, VoteFinishedEventArgs args)
			{
				GameMapPrototype picked;
				if (args.Winner == null)
				{
					picked = (GameMapPrototype)RandomExtensions.Pick<object>(this._random, args.Winners);
					this._chatManager.DispatchServerAnnouncement(Loc.GetString("ui-vote-map-tie", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("picked", maps[picked])
					}), null);
				}
				else
				{
					picked = (GameMapPrototype)args.Winner;
					this._chatManager.DispatchServerAnnouncement(Loc.GetString("ui-vote-map-win", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("winner", maps[picked])
					}), null);
				}
				ISharedAdminLogManager adminLogger = this._adminLogger;
				LogType type = LogType.Vote;
				LogImpact impact = LogImpact.Medium;
				LogStringHandler logStringHandler = new LogStringHandler(19, 1);
				logStringHandler.AppendLiteral("Map vote finished: ");
				logStringHandler.AppendFormatted(picked.MapName);
				adminLogger.Add(type, impact, ref logStringHandler);
				GameTicker ticker = this._entityManager.EntitySysManager.GetEntitySystem<GameTicker>();
				if (ticker.RunLevel == GameRunLevel.PreRoundLobby)
				{
					if (this._gameMapManager.TrySelectMapIfEligible(picked.ID))
					{
						ticker.UpdateInfoText();
						return;
					}
				}
				else
				{
					this._chatManager.DispatchServerAnnouncement(Loc.GetString("ui-vote-map-notlobby"), null);
				}
			};
		}

		// Token: 0x06000382 RID: 898 RVA: 0x00012854 File Offset: 0x00010A54
		private void TimeoutStandardVote(StandardVoteType type)
		{
			TimeSpan timeout = TimeSpan.FromSeconds((double)this._cfg.GetCVar<float>(CCVars.VoteSameTypeTimeout));
			this._standardVoteTimeout[type] = this._timing.RealTime + timeout;
			this.DirtyCanCallVoteAll();
		}

		// Token: 0x06000383 RID: 899 RVA: 0x0001289C File Offset: 0x00010A9C
		private Dictionary<string, string> GetGamePresets()
		{
			Dictionary<string, string> presets = new Dictionary<string, string>();
			foreach (GamePresetPrototype preset in this._prototypeManager.EnumeratePrototypes<GamePresetPrototype>())
			{
				if (preset.ShowInVote && this._playerManager.PlayerCount >= (preset.MinPlayers ?? -2147483648) && this._playerManager.PlayerCount <= (preset.MaxPlayers ?? 2147483647))
				{
					presets[preset.ID] = preset.ModeTitle;
				}
			}
			return presets;
		}

		// Token: 0x04000224 RID: 548
		[Dependency]
		private readonly IServerNetManager _netManager;

		// Token: 0x04000225 RID: 549
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04000226 RID: 550
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000227 RID: 551
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000228 RID: 552
		[Dependency]
		private readonly IChatManager _chatManager;

		// Token: 0x04000229 RID: 553
		[Dependency]
		private readonly IAdminManager _adminMgr;

		// Token: 0x0400022A RID: 554
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x0400022B RID: 555
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x0400022C RID: 556
		[Dependency]
		private readonly IGameMapManager _gameMapManager;

		// Token: 0x0400022D RID: 557
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x0400022E RID: 558
		[Dependency]
		private readonly IAdminLogManager _adminLogger;

		// Token: 0x0400022F RID: 559
		private int _nextVoteId = 1;

		// Token: 0x04000230 RID: 560
		private readonly Dictionary<int, VoteManager.VoteReg> _votes = new Dictionary<int, VoteManager.VoteReg>();

		// Token: 0x04000231 RID: 561
		private readonly Dictionary<int, VoteManager.VoteHandle> _voteHandles = new Dictionary<int, VoteManager.VoteHandle>();

		// Token: 0x04000232 RID: 562
		private readonly Dictionary<StandardVoteType, TimeSpan> _standardVoteTimeout = new Dictionary<StandardVoteType, TimeSpan>();

		// Token: 0x04000233 RID: 563
		private readonly Dictionary<NetUserId, TimeSpan> _voteTimeout = new Dictionary<NetUserId, TimeSpan>();

		// Token: 0x04000234 RID: 564
		private readonly HashSet<IPlayerSession> _playerCanCallVoteDirty = new HashSet<IPlayerSession>();

		// Token: 0x04000235 RID: 565
		private readonly StandardVoteType[] _standardVoteTypeValues = Enum.GetValues<StandardVoteType>();

		// Token: 0x04000236 RID: 566
		private static readonly Dictionary<StandardVoteType, CVarDef<bool>> _voteTypesToEnableCVars = new Dictionary<StandardVoteType, CVarDef<bool>>
		{
			{
				StandardVoteType.Restart,
				CCVars.VoteRestartEnabled
			},
			{
				StandardVoteType.Preset,
				CCVars.VotePresetEnabled
			},
			{
				StandardVoteType.Map,
				CCVars.VoteMapEnabled
			}
		};

		// Token: 0x020008C4 RID: 2244
		[Nullable(0)]
		private sealed class VoteReg
		{
			// Token: 0x170007F4 RID: 2036
			// (get) Token: 0x0600304F RID: 12367 RVA: 0x000FA0B7 File Offset: 0x000F82B7
			[Nullable(2)]
			public IPlayerSession Initiator { [NullableContext(2)] get; }

			// Token: 0x06003050 RID: 12368 RVA: 0x000FA0C0 File Offset: 0x000F82C0
			public VoteReg(int id, VoteManager.VoteEntry[] entries, string title, string initiatorText, [Nullable(2)] IPlayerSession initiator, TimeSpan start, TimeSpan end)
			{
				this.Id = id;
				this.Entries = entries;
				this.Title = title;
				this.InitiatorText = initiatorText;
				this.Initiator = initiator;
				this.StartTime = start;
				this.EndTime = end;
			}

			// Token: 0x04001D99 RID: 7577
			public readonly int Id;

			// Token: 0x04001D9A RID: 7578
			public readonly Dictionary<IPlayerSession, int> CastVotes = new Dictionary<IPlayerSession, int>();

			// Token: 0x04001D9B RID: 7579
			public readonly VoteManager.VoteEntry[] Entries;

			// Token: 0x04001D9C RID: 7580
			public readonly string Title;

			// Token: 0x04001D9D RID: 7581
			public readonly string InitiatorText;

			// Token: 0x04001D9E RID: 7582
			public readonly TimeSpan StartTime;

			// Token: 0x04001D9F RID: 7583
			public readonly TimeSpan EndTime;

			// Token: 0x04001DA0 RID: 7584
			public readonly HashSet<IPlayerSession> VotesDirty = new HashSet<IPlayerSession>();

			// Token: 0x04001DA1 RID: 7585
			public bool Cancelled;

			// Token: 0x04001DA2 RID: 7586
			public bool Finished;

			// Token: 0x04001DA3 RID: 7587
			public bool Dirty = true;

			// Token: 0x04001DA4 RID: 7588
			[Nullable(2)]
			public VoteFinishedEventHandler OnFinished;

			// Token: 0x04001DA5 RID: 7589
			[Nullable(2)]
			public VoteCancelledEventHandler OnCancelled;
		}

		// Token: 0x020008C5 RID: 2245
		[Nullable(0)]
		private struct VoteEntry
		{
			// Token: 0x06003051 RID: 12369 RVA: 0x000FA125 File Offset: 0x000F8325
			public VoteEntry(object data, string text)
			{
				this.Data = data;
				this.Text = text;
				this.Votes = 0;
			}

			// Token: 0x04001DA7 RID: 7591
			public object Data;

			// Token: 0x04001DA8 RID: 7592
			public string Text;

			// Token: 0x04001DA9 RID: 7593
			public int Votes;
		}

		// Token: 0x020008C6 RID: 2246
		[Nullable(0)]
		private sealed class VoteHandle : IVoteHandle
		{
			// Token: 0x170007F5 RID: 2037
			// (get) Token: 0x06003052 RID: 12370 RVA: 0x000FA13C File Offset: 0x000F833C
			public int Id
			{
				get
				{
					return this._reg.Id;
				}
			}

			// Token: 0x170007F6 RID: 2038
			// (get) Token: 0x06003053 RID: 12371 RVA: 0x000FA149 File Offset: 0x000F8349
			public string Title
			{
				get
				{
					return this._reg.Title;
				}
			}

			// Token: 0x170007F7 RID: 2039
			// (get) Token: 0x06003054 RID: 12372 RVA: 0x000FA156 File Offset: 0x000F8356
			public string InitiatorText
			{
				get
				{
					return this._reg.InitiatorText;
				}
			}

			// Token: 0x170007F8 RID: 2040
			// (get) Token: 0x06003055 RID: 12373 RVA: 0x000FA163 File Offset: 0x000F8363
			public bool Finished
			{
				get
				{
					return this._reg.Finished;
				}
			}

			// Token: 0x170007F9 RID: 2041
			// (get) Token: 0x06003056 RID: 12374 RVA: 0x000FA170 File Offset: 0x000F8370
			public bool Cancelled
			{
				get
				{
					return this._reg.Cancelled;
				}
			}

			// Token: 0x170007FA RID: 2042
			// (get) Token: 0x06003057 RID: 12375 RVA: 0x000FA17D File Offset: 0x000F837D
			public IReadOnlyDictionary<object, int> VotesPerOption { get; }

			// Token: 0x1400000D RID: 13
			// (add) Token: 0x06003058 RID: 12376 RVA: 0x000FA185 File Offset: 0x000F8385
			// (remove) Token: 0x06003059 RID: 12377 RVA: 0x000FA1A3 File Offset: 0x000F83A3
			[Nullable(2)]
			public event VoteFinishedEventHandler OnFinished
			{
				[NullableContext(2)]
				add
				{
					VoteManager.VoteReg reg = this._reg;
					reg.OnFinished = (VoteFinishedEventHandler)Delegate.Combine(reg.OnFinished, value);
				}
				[NullableContext(2)]
				remove
				{
					VoteManager.VoteReg reg = this._reg;
					reg.OnFinished = (VoteFinishedEventHandler)Delegate.Remove(reg.OnFinished, value);
				}
			}

			// Token: 0x1400000E RID: 14
			// (add) Token: 0x0600305A RID: 12378 RVA: 0x000FA1C1 File Offset: 0x000F83C1
			// (remove) Token: 0x0600305B RID: 12379 RVA: 0x000FA1DF File Offset: 0x000F83DF
			[Nullable(2)]
			public event VoteCancelledEventHandler OnCancelled
			{
				[NullableContext(2)]
				add
				{
					VoteManager.VoteReg reg = this._reg;
					reg.OnCancelled = (VoteCancelledEventHandler)Delegate.Combine(reg.OnCancelled, value);
				}
				[NullableContext(2)]
				remove
				{
					VoteManager.VoteReg reg = this._reg;
					reg.OnCancelled = (VoteCancelledEventHandler)Delegate.Remove(reg.OnCancelled, value);
				}
			}

			// Token: 0x0600305C RID: 12380 RVA: 0x000FA1FD File Offset: 0x000F83FD
			public VoteHandle(VoteManager mgr, VoteManager.VoteReg reg)
			{
				this._mgr = mgr;
				this._reg = reg;
				this.VotesPerOption = new VoteManager.VoteHandle.VoteDict(reg);
			}

			// Token: 0x0600305D RID: 12381 RVA: 0x000FA21F File Offset: 0x000F841F
			public bool IsValidOption(int optionId)
			{
				return this._mgr.IsValidOption(this._reg, new int?(optionId));
			}

			// Token: 0x0600305E RID: 12382 RVA: 0x000FA238 File Offset: 0x000F8438
			public void CastVote(IPlayerSession session, int? optionId)
			{
				this._mgr.CastVote(this._reg, session, optionId);
			}

			// Token: 0x0600305F RID: 12383 RVA: 0x000FA24D File Offset: 0x000F844D
			public void Cancel()
			{
				this._mgr.CancelVote(this._reg);
			}

			// Token: 0x04001DAA RID: 7594
			private readonly VoteManager _mgr;

			// Token: 0x04001DAB RID: 7595
			private readonly VoteManager.VoteReg _reg;

			// Token: 0x02000BB3 RID: 2995
			[Nullable(0)]
			private sealed class VoteDict : IReadOnlyDictionary<object, int>, IEnumerable<KeyValuePair<object, int>>, IEnumerable, IReadOnlyCollection<KeyValuePair<object, int>>
			{
				// Token: 0x06003A8F RID: 14991 RVA: 0x00134319 File Offset: 0x00132519
				public VoteDict(VoteManager.VoteReg reg)
				{
					this._reg = reg;
				}

				// Token: 0x06003A90 RID: 14992 RVA: 0x00134328 File Offset: 0x00132528
				[return: Nullable(new byte[]
				{
					1,
					0,
					1
				})]
				public IEnumerator<KeyValuePair<object, int>> GetEnumerator()
				{
					return (from e in this._reg.Entries
					select KeyValuePair.Create<object, int>(e.Data, e.Votes)).GetEnumerator();
				}

				// Token: 0x06003A91 RID: 14993 RVA: 0x0013435E File Offset: 0x0013255E
				IEnumerator IEnumerable.GetEnumerator()
				{
					return this.GetEnumerator();
				}

				// Token: 0x170008EA RID: 2282
				// (get) Token: 0x06003A92 RID: 14994 RVA: 0x00134366 File Offset: 0x00132566
				public int Count
				{
					get
					{
						return this._reg.Entries.Length;
					}
				}

				// Token: 0x06003A93 RID: 14995 RVA: 0x00134378 File Offset: 0x00132578
				public bool ContainsKey(object key)
				{
					int num;
					return this.TryGetValue(key, out num);
				}

				// Token: 0x06003A94 RID: 14996 RVA: 0x00134390 File Offset: 0x00132590
				public bool TryGetValue(object key, out int value)
				{
					VoteManager.VoteEntry? entry = Extensions.FirstOrNull<VoteManager.VoteEntry>(this._reg.Entries, (VoteManager.VoteEntry a) => a.Data.Equals(key));
					if (entry != null)
					{
						value = entry.Value.Votes;
						return true;
					}
					value = 0;
					return false;
				}

				// Token: 0x170008EB RID: 2283
				public int this[object key]
				{
					get
					{
						int votes;
						if (!this.TryGetValue(key, out votes))
						{
							throw new KeyNotFoundException();
						}
						return votes;
					}
				}

				// Token: 0x170008EC RID: 2284
				// (get) Token: 0x06003A96 RID: 14998 RVA: 0x00134403 File Offset: 0x00132603
				public IEnumerable<object> Keys
				{
					get
					{
						return from c in this._reg.Entries
						select c.Data;
					}
				}

				// Token: 0x170008ED RID: 2285
				// (get) Token: 0x06003A97 RID: 14999 RVA: 0x00134434 File Offset: 0x00132634
				public IEnumerable<int> Values
				{
					get
					{
						return from c in this._reg.Entries
						select c.Votes;
					}
				}

				// Token: 0x04002C24 RID: 11300
				private readonly VoteManager.VoteReg _reg;
			}
		}
	}
}
