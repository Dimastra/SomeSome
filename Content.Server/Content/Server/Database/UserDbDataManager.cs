using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Players.PlayTimeTracking;
using Content.Server.Preferences.Managers;
using Robust.Server.Player;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Server.Database
{
	// Token: 0x020005BF RID: 1471
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class UserDbDataManager
	{
		// Token: 0x06001F6E RID: 8046 RVA: 0x000A509C File Offset: 0x000A329C
		public void ClientConnected(IPlayerSession session)
		{
			CancellationTokenSource cts = new CancellationTokenSource();
			Task task = this.Load(session, cts.Token);
			UserDbDataManager.UserData data = new UserDbDataManager.UserData(cts, task);
			this._users.Add(session.UserId, data);
		}

		// Token: 0x06001F6F RID: 8047 RVA: 0x000A50D8 File Offset: 0x000A32D8
		public void ClientDisconnected(IPlayerSession session)
		{
			UserDbDataManager.UserData data;
			this._users.Remove(session.UserId, out data);
			if (data == null)
			{
				throw new InvalidOperationException("Did not have cached data in ClientDisconnect!");
			}
			data.Cancel.Cancel();
			data.Cancel.Dispose();
			this._prefs.OnClientDisconnected(session);
			this._playTimeTracking.ClientDisconnected(session);
		}

		// Token: 0x06001F70 RID: 8048 RVA: 0x000A513C File Offset: 0x000A333C
		private Task Load(IPlayerSession session, CancellationToken cancel)
		{
			UserDbDataManager.<Load>d__5 <Load>d__;
			<Load>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Load>d__.<>4__this = this;
			<Load>d__.session = session;
			<Load>d__.cancel = cancel;
			<Load>d__.<>1__state = -1;
			<Load>d__.<>t__builder.Start<UserDbDataManager.<Load>d__5>(ref <Load>d__);
			return <Load>d__.<>t__builder.Task;
		}

		// Token: 0x06001F71 RID: 8049 RVA: 0x000A518F File Offset: 0x000A338F
		public Task WaitLoadComplete(IPlayerSession session)
		{
			return this._users[session.UserId].Task;
		}

		// Token: 0x06001F72 RID: 8050 RVA: 0x000A51A7 File Offset: 0x000A33A7
		public bool IsLoadComplete(IPlayerSession session)
		{
			return this._users[session.UserId].Task.IsCompleted;
		}

		// Token: 0x04001386 RID: 4998
		[Dependency]
		private readonly IServerPreferencesManager _prefs;

		// Token: 0x04001387 RID: 4999
		[Dependency]
		private readonly PlayTimeTrackingManager _playTimeTracking;

		// Token: 0x04001388 RID: 5000
		private readonly Dictionary<NetUserId, UserDbDataManager.UserData> _users = new Dictionary<NetUserId, UserDbDataManager.UserData>();

		// Token: 0x02000AAD RID: 2733
		[Nullable(0)]
		private sealed class UserData : IEquatable<UserDbDataManager.UserData>
		{
			// Token: 0x0600357C RID: 13692 RVA: 0x0011C91E File Offset: 0x0011AB1E
			public UserData(CancellationTokenSource Cancel, Task Task)
			{
				this.Cancel = Cancel;
				this.Task = Task;
				base..ctor();
			}

			// Token: 0x1700083D RID: 2109
			// (get) Token: 0x0600357D RID: 13693 RVA: 0x0011C934 File Offset: 0x0011AB34
			[CompilerGenerated]
			private Type EqualityContract
			{
				[CompilerGenerated]
				get
				{
					return typeof(UserDbDataManager.UserData);
				}
			}

			// Token: 0x1700083E RID: 2110
			// (get) Token: 0x0600357E RID: 13694 RVA: 0x0011C940 File Offset: 0x0011AB40
			// (set) Token: 0x0600357F RID: 13695 RVA: 0x0011C948 File Offset: 0x0011AB48
			public CancellationTokenSource Cancel { get; set; }

			// Token: 0x1700083F RID: 2111
			// (get) Token: 0x06003580 RID: 13696 RVA: 0x0011C951 File Offset: 0x0011AB51
			// (set) Token: 0x06003581 RID: 13697 RVA: 0x0011C959 File Offset: 0x0011AB59
			public Task Task { get; set; }

			// Token: 0x06003582 RID: 13698 RVA: 0x0011C964 File Offset: 0x0011AB64
			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("UserData");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x06003583 RID: 13699 RVA: 0x0011C9B0 File Offset: 0x0011ABB0
			[CompilerGenerated]
			private bool PrintMembers(StringBuilder builder)
			{
				RuntimeHelpers.EnsureSufficientExecutionStack();
				builder.Append("Cancel = ");
				builder.Append(this.Cancel);
				builder.Append(", Task = ");
				builder.Append(this.Task);
				return true;
			}

			// Token: 0x06003584 RID: 13700 RVA: 0x0011C9EA File Offset: 0x0011ABEA
			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator !=(UserDbDataManager.UserData left, UserDbDataManager.UserData right)
			{
				return !(left == right);
			}

			// Token: 0x06003585 RID: 13701 RVA: 0x0011C9F6 File Offset: 0x0011ABF6
			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator ==(UserDbDataManager.UserData left, UserDbDataManager.UserData right)
			{
				return left == right || (left != null && left.Equals(right));
			}

			// Token: 0x06003586 RID: 13702 RVA: 0x0011CA0A File Offset: 0x0011AC0A
			[CompilerGenerated]
			public override int GetHashCode()
			{
				return (EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<CancellationTokenSource>.Default.GetHashCode(this.<Cancel>k__BackingField)) * -1521134295 + EqualityComparer<Task>.Default.GetHashCode(this.<Task>k__BackingField);
			}

			// Token: 0x06003587 RID: 13703 RVA: 0x0011CA4A File Offset: 0x0011AC4A
			[NullableContext(2)]
			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				return this.Equals(obj as UserDbDataManager.UserData);
			}

			// Token: 0x06003588 RID: 13704 RVA: 0x0011CA58 File Offset: 0x0011AC58
			[NullableContext(2)]
			[CompilerGenerated]
			public bool Equals(UserDbDataManager.UserData other)
			{
				return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<CancellationTokenSource>.Default.Equals(this.<Cancel>k__BackingField, other.<Cancel>k__BackingField) && EqualityComparer<Task>.Default.Equals(this.<Task>k__BackingField, other.<Task>k__BackingField));
			}

			// Token: 0x0600358A RID: 13706 RVA: 0x0011CAB9 File Offset: 0x0011ACB9
			[CompilerGenerated]
			private UserData(UserDbDataManager.UserData original)
			{
				this.Cancel = original.<Cancel>k__BackingField;
				this.Task = original.<Task>k__BackingField;
			}

			// Token: 0x0600358B RID: 13707 RVA: 0x0011CAD9 File Offset: 0x0011ACD9
			[CompilerGenerated]
			public void Deconstruct(out CancellationTokenSource Cancel, out Task Task)
			{
				Cancel = this.Cancel;
				Task = this.Task;
			}
		}
	}
}
