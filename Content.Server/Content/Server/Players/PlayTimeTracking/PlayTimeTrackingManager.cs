using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Database;
using Content.Shared.CCVar;
using Content.Shared.Players.PlayTimeTracking;
using Robust.Server.Player;
using Robust.Shared.Asynchronous;
using Robust.Shared.Collections;
using Robust.Shared.Configuration;
using Robust.Shared.Exceptions;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Server.Players.PlayTimeTracking
{
	// Token: 0x020002D4 RID: 724
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PlayTimeTrackingManager
	{
		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000E8F RID: 3727 RVA: 0x00049F9C File Offset: 0x0004819C
		// (remove) Token: 0x06000E90 RID: 3728 RVA: 0x00049FD4 File Offset: 0x000481D4
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event CalcPlayTimeTrackersCallback CalcTrackers;

		// Token: 0x06000E91 RID: 3729 RVA: 0x0004A009 File Offset: 0x00048209
		public void Initialize()
		{
			this._sawmill = Logger.GetSawmill("play_time");
			this._net.RegisterNetMessage<MsgPlayTime>(null, 3);
			this._cfg.OnValueChanged<float>(CCVars.PlayTimeSaveInterval, delegate(float f)
			{
				this._saveInterval = TimeSpan.FromSeconds((double)f);
			}, true);
		}

		// Token: 0x06000E92 RID: 3730 RVA: 0x0004A045 File Offset: 0x00048245
		public void Shutdown()
		{
			this.Save();
			this._task.BlockWaitOnTask(Task.WhenAll(this._pendingSaveTasks));
		}

		// Token: 0x06000E93 RID: 3731 RVA: 0x0004A063 File Offset: 0x00048263
		public void Update()
		{
			this.UpdateDirtyPlayers();
			if (this._timing.RealTime < this._lastSave + this._saveInterval)
			{
				return;
			}
			this.Save();
		}

		// Token: 0x06000E94 RID: 3732 RVA: 0x0004A098 File Offset: 0x00048298
		private void UpdateDirtyPlayers()
		{
			if (this._playersDirty.Count == 0)
			{
				return;
			}
			TimeSpan time = this._timing.RealTime;
			foreach (IPlayerSession player in this._playersDirty)
			{
				PlayTimeTrackingManager.PlayTimeData data;
				if (this._playTimeData.TryGetValue(player, out data))
				{
					if (data.NeedRefreshTackers)
					{
						this.RefreshSingleTracker(player, data, time);
					}
					if (data.NeedSendTimers)
					{
						this.SendPlayTimes(player);
						data.NeedSendTimers = false;
					}
					data.IsDirty = false;
				}
			}
			this._playersDirty.Clear();
		}

		// Token: 0x06000E95 RID: 3733 RVA: 0x0004A148 File Offset: 0x00048348
		private void RefreshSingleTracker(IPlayerSession dirty, PlayTimeTrackingManager.PlayTimeData data, TimeSpan time)
		{
			PlayTimeTrackingManager.FlushSingleTracker(data, time);
			data.NeedRefreshTackers = false;
			data.ActiveTrackers.Clear();
			try
			{
				CalcPlayTimeTrackersCallback calcTrackers = this.CalcTrackers;
				if (calcTrackers != null)
				{
					calcTrackers(dirty, data.ActiveTrackers);
				}
			}
			catch (Exception e)
			{
				this._runtimeLog.LogException(e, "PlayTime CalcTrackers");
				data.ActiveTrackers.Clear();
			}
		}

		// Token: 0x06000E96 RID: 3734 RVA: 0x0004A1B8 File Offset: 0x000483B8
		public void FlushAllTrackers()
		{
			TimeSpan time = this._timing.RealTime;
			foreach (PlayTimeTrackingManager.PlayTimeData data in this._playTimeData.Values)
			{
				PlayTimeTrackingManager.FlushSingleTracker(data, time);
			}
		}

		// Token: 0x06000E97 RID: 3735 RVA: 0x0004A21C File Offset: 0x0004841C
		public void FlushTracker(IPlayerSession player)
		{
			TimeSpan time = this._timing.RealTime;
			PlayTimeTrackingManager.FlushSingleTracker(this._playTimeData[player], time);
		}

		// Token: 0x06000E98 RID: 3736 RVA: 0x0004A248 File Offset: 0x00048448
		private static void FlushSingleTracker(PlayTimeTrackingManager.PlayTimeData data, TimeSpan time)
		{
			TimeSpan delta = time - data.LastUpdate;
			data.LastUpdate = time;
			foreach (string active in data.ActiveTrackers)
			{
				PlayTimeTrackingManager.AddTimeToTracker(data, active, delta);
			}
		}

		// Token: 0x06000E99 RID: 3737 RVA: 0x0004A2B0 File Offset: 0x000484B0
		private void SendPlayTimes(IPlayerSession pSession)
		{
			Dictionary<string, TimeSpan> roles = this.GetTrackerTimes(pSession);
			MsgPlayTime msg = new MsgPlayTime
			{
				Trackers = roles
			};
			this._net.ServerSendMessage(msg, pSession.ConnectedClient);
		}

		// Token: 0x06000E9A RID: 3738 RVA: 0x0004A2E4 File Offset: 0x000484E4
		public void Save()
		{
			PlayTimeTrackingManager.<Save>d__24 <Save>d__;
			<Save>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Save>d__.<>4__this = this;
			<Save>d__.<>1__state = -1;
			<Save>d__.<>t__builder.Start<PlayTimeTrackingManager.<Save>d__24>(ref <Save>d__);
		}

		// Token: 0x06000E9B RID: 3739 RVA: 0x0004A31C File Offset: 0x0004851C
		public void SaveSession(IPlayerSession session)
		{
			PlayTimeTrackingManager.<SaveSession>d__25 <SaveSession>d__;
			<SaveSession>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<SaveSession>d__.<>4__this = this;
			<SaveSession>d__.session = session;
			<SaveSession>d__.<>1__state = -1;
			<SaveSession>d__.<>t__builder.Start<PlayTimeTrackingManager.<SaveSession>d__25>(ref <SaveSession>d__);
		}

		// Token: 0x06000E9C RID: 3740 RVA: 0x0004A35C File Offset: 0x0004855C
		private void TrackPending(Task task)
		{
			PlayTimeTrackingManager.<TrackPending>d__26 <TrackPending>d__;
			<TrackPending>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<TrackPending>d__.<>4__this = this;
			<TrackPending>d__.task = task;
			<TrackPending>d__.<>1__state = -1;
			<TrackPending>d__.<>t__builder.Start<PlayTimeTrackingManager.<TrackPending>d__26>(ref <TrackPending>d__);
		}

		// Token: 0x06000E9D RID: 3741 RVA: 0x0004A39C File Offset: 0x0004859C
		private Task DoSaveAsync()
		{
			PlayTimeTrackingManager.<DoSaveAsync>d__27 <DoSaveAsync>d__;
			<DoSaveAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DoSaveAsync>d__.<>4__this = this;
			<DoSaveAsync>d__.<>1__state = -1;
			<DoSaveAsync>d__.<>t__builder.Start<PlayTimeTrackingManager.<DoSaveAsync>d__27>(ref <DoSaveAsync>d__);
			return <DoSaveAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06000E9E RID: 3742 RVA: 0x0004A3E0 File Offset: 0x000485E0
		private Task DoSaveSessionAsync(IPlayerSession session)
		{
			PlayTimeTrackingManager.<DoSaveSessionAsync>d__28 <DoSaveSessionAsync>d__;
			<DoSaveSessionAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DoSaveSessionAsync>d__.<>4__this = this;
			<DoSaveSessionAsync>d__.session = session;
			<DoSaveSessionAsync>d__.<>1__state = -1;
			<DoSaveSessionAsync>d__.<>t__builder.Start<PlayTimeTrackingManager.<DoSaveSessionAsync>d__28>(ref <DoSaveSessionAsync>d__);
			return <DoSaveSessionAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06000E9F RID: 3743 RVA: 0x0004A42C File Offset: 0x0004862C
		public Task LoadData(IPlayerSession session, CancellationToken cancel)
		{
			PlayTimeTrackingManager.<LoadData>d__29 <LoadData>d__;
			<LoadData>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<LoadData>d__.<>4__this = this;
			<LoadData>d__.session = session;
			<LoadData>d__.cancel = cancel;
			<LoadData>d__.<>1__state = -1;
			<LoadData>d__.<>t__builder.Start<PlayTimeTrackingManager.<LoadData>d__29>(ref <LoadData>d__);
			return <LoadData>d__.<>t__builder.Task;
		}

		// Token: 0x06000EA0 RID: 3744 RVA: 0x0004A47F File Offset: 0x0004867F
		public void ClientDisconnected(IPlayerSession session)
		{
			this.SaveSession(session);
			this._playTimeData.Remove(session);
		}

		// Token: 0x06000EA1 RID: 3745 RVA: 0x0004A498 File Offset: 0x00048698
		public void AddTimeToTracker(IPlayerSession id, string tracker, TimeSpan time)
		{
			PlayTimeTrackingManager.PlayTimeData data;
			if (!this._playTimeData.TryGetValue(id, out data) || !data.Initialized)
			{
				throw new InvalidOperationException("Play time info is not yet loaded for this player!");
			}
			PlayTimeTrackingManager.AddTimeToTracker(data, tracker, time);
		}

		// Token: 0x06000EA2 RID: 3746 RVA: 0x0004A4D0 File Offset: 0x000486D0
		private unsafe static void AddTimeToTracker(PlayTimeTrackingManager.PlayTimeData data, string tracker, TimeSpan time)
		{
			bool flag;
			*CollectionsMarshal.GetValueRefOrAddDefault<string, TimeSpan>(data.TrackerTimes, tracker, out flag) += time;
			data.DbTrackersDirty.Add(tracker);
		}

		// Token: 0x06000EA3 RID: 3747 RVA: 0x0004A509 File Offset: 0x00048709
		public void AddTimeToOverallPlaytime(IPlayerSession id, TimeSpan time)
		{
			this.AddTimeToTracker(id, "Overall", time);
		}

		// Token: 0x06000EA4 RID: 3748 RVA: 0x0004A518 File Offset: 0x00048718
		public TimeSpan GetOverallPlaytime(IPlayerSession id)
		{
			return this.GetPlayTimeForTracker(id, "Overall");
		}

		// Token: 0x06000EA5 RID: 3749 RVA: 0x0004A528 File Offset: 0x00048728
		public Dictionary<string, TimeSpan> GetTrackerTimes(IPlayerSession id)
		{
			PlayTimeTrackingManager.PlayTimeData data;
			if (!this._playTimeData.TryGetValue(id, out data) || !data.Initialized)
			{
				throw new InvalidOperationException("Play time info is not yet loaded for this player!");
			}
			return data.TrackerTimes;
		}

		// Token: 0x06000EA6 RID: 3750 RVA: 0x0004A560 File Offset: 0x00048760
		public TimeSpan GetPlayTimeForTracker(IPlayerSession id, string tracker)
		{
			PlayTimeTrackingManager.PlayTimeData data;
			if (!this._playTimeData.TryGetValue(id, out data) || !data.Initialized)
			{
				throw new InvalidOperationException("Play time info is not yet loaded for this player!");
			}
			return data.TrackerTimes.GetValueOrDefault(tracker);
		}

		// Token: 0x06000EA7 RID: 3751 RVA: 0x0004A59C File Offset: 0x0004879C
		public void QueueRefreshTrackers(IPlayerSession player)
		{
			PlayTimeTrackingManager.PlayTimeData data = this.DirtyPlayer(player);
			if (data != null)
			{
				data.NeedRefreshTackers = true;
			}
		}

		// Token: 0x06000EA8 RID: 3752 RVA: 0x0004A5BC File Offset: 0x000487BC
		public void QueueSendTimers(IPlayerSession player)
		{
			PlayTimeTrackingManager.PlayTimeData data = this.DirtyPlayer(player);
			if (data != null)
			{
				data.NeedSendTimers = true;
			}
		}

		// Token: 0x06000EA9 RID: 3753 RVA: 0x0004A5DC File Offset: 0x000487DC
		[return: Nullable(2)]
		private PlayTimeTrackingManager.PlayTimeData DirtyPlayer(IPlayerSession player)
		{
			PlayTimeTrackingManager.PlayTimeData data;
			if (!this._playTimeData.TryGetValue(player, out data) || !data.Initialized)
			{
				return null;
			}
			if (!data.IsDirty)
			{
				data.IsDirty = true;
				this._playersDirty.Add(player);
			}
			return data;
		}

		// Token: 0x04000899 RID: 2201
		[Dependency]
		private readonly IServerDbManager _db;

		// Token: 0x0400089A RID: 2202
		[Dependency]
		private readonly IServerNetManager _net;

		// Token: 0x0400089B RID: 2203
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x0400089C RID: 2204
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x0400089D RID: 2205
		[Dependency]
		private readonly ITaskManager _task;

		// Token: 0x0400089E RID: 2206
		[Dependency]
		private readonly IRuntimeLog _runtimeLog;

		// Token: 0x0400089F RID: 2207
		private ISawmill _sawmill;

		// Token: 0x040008A0 RID: 2208
		[Nullable(new byte[]
		{
			0,
			1
		})]
		private ValueList<IPlayerSession> _playersDirty;

		// Token: 0x040008A1 RID: 2209
		private TimeSpan _saveInterval;

		// Token: 0x040008A2 RID: 2210
		private TimeSpan _lastSave;

		// Token: 0x040008A3 RID: 2211
		private readonly List<Task> _pendingSaveTasks = new List<Task>();

		// Token: 0x040008A4 RID: 2212
		private readonly Dictionary<IPlayerSession, PlayTimeTrackingManager.PlayTimeData> _playTimeData = new Dictionary<IPlayerSession, PlayTimeTrackingManager.PlayTimeData>();

		// Token: 0x0200094B RID: 2379
		[Nullable(0)]
		private sealed class PlayTimeData
		{
			// Token: 0x04001FAC RID: 8108
			public bool IsDirty;

			// Token: 0x04001FAD RID: 8109
			public bool NeedRefreshTackers;

			// Token: 0x04001FAE RID: 8110
			public bool NeedSendTimers;

			// Token: 0x04001FAF RID: 8111
			public readonly HashSet<string> ActiveTrackers = new HashSet<string>();

			// Token: 0x04001FB0 RID: 8112
			public TimeSpan LastUpdate;

			// Token: 0x04001FB1 RID: 8113
			public bool Initialized;

			// Token: 0x04001FB2 RID: 8114
			public readonly Dictionary<string, TimeSpan> TrackerTimes = new Dictionary<string, TimeSpan>();

			// Token: 0x04001FB3 RID: 8115
			public readonly HashSet<string> DbTrackersDirty = new HashSet<string>();
		}
	}
}
