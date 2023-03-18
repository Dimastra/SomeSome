using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Database;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Network;

namespace Content.Server.Administration
{
	// Token: 0x02000803 RID: 2051
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class PlayerLocator : IPlayerLocator, IDisposable
	{
		// Token: 0x06002C69 RID: 11369 RVA: 0x000E7890 File Offset: 0x000E5A90
		public PlayerLocator()
		{
			Version version = typeof(PlayerLocator).Assembly.GetName().Version;
			if (version != null)
			{
				this._httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("SpaceStation14", version.ToString()));
			}
		}

		// Token: 0x06002C6A RID: 11370 RVA: 0x000E78F0 File Offset: 0x000E5AF0
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public Task<LocatedPlayerData> LookupIdByNameAsync(string playerName, CancellationToken cancel = default(CancellationToken))
		{
			PlayerLocator.<LookupIdByNameAsync>d__5 <LookupIdByNameAsync>d__;
			<LookupIdByNameAsync>d__.<>t__builder = AsyncTaskMethodBuilder<LocatedPlayerData>.Create();
			<LookupIdByNameAsync>d__.<>4__this = this;
			<LookupIdByNameAsync>d__.playerName = playerName;
			<LookupIdByNameAsync>d__.cancel = cancel;
			<LookupIdByNameAsync>d__.<>1__state = -1;
			<LookupIdByNameAsync>d__.<>t__builder.Start<PlayerLocator.<LookupIdByNameAsync>d__5>(ref <LookupIdByNameAsync>d__);
			return <LookupIdByNameAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06002C6B RID: 11371 RVA: 0x000E7944 File Offset: 0x000E5B44
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public Task<LocatedPlayerData> LookupIdAsync(NetUserId userId, CancellationToken cancel = default(CancellationToken))
		{
			PlayerLocator.<LookupIdAsync>d__6 <LookupIdAsync>d__;
			<LookupIdAsync>d__.<>t__builder = AsyncTaskMethodBuilder<LocatedPlayerData>.Create();
			<LookupIdAsync>d__.<>4__this = this;
			<LookupIdAsync>d__.userId = userId;
			<LookupIdAsync>d__.cancel = cancel;
			<LookupIdAsync>d__.<>1__state = -1;
			<LookupIdAsync>d__.<>t__builder.Start<PlayerLocator.<LookupIdAsync>d__6>(ref <LookupIdAsync>d__);
			return <LookupIdAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06002C6C RID: 11372 RVA: 0x000E7998 File Offset: 0x000E5B98
		[return: Nullable(new byte[]
		{
			1,
			2
		})]
		public Task<LocatedPlayerData> LookupIdByNameOrIdAsync(string playerName, CancellationToken cancel = default(CancellationToken))
		{
			PlayerLocator.<LookupIdByNameOrIdAsync>d__7 <LookupIdByNameOrIdAsync>d__;
			<LookupIdByNameOrIdAsync>d__.<>t__builder = AsyncTaskMethodBuilder<LocatedPlayerData>.Create();
			<LookupIdByNameOrIdAsync>d__.<>4__this = this;
			<LookupIdByNameOrIdAsync>d__.playerName = playerName;
			<LookupIdByNameOrIdAsync>d__.cancel = cancel;
			<LookupIdByNameOrIdAsync>d__.<>1__state = -1;
			<LookupIdByNameOrIdAsync>d__.<>t__builder.Start<PlayerLocator.<LookupIdByNameOrIdAsync>d__7>(ref <LookupIdByNameOrIdAsync>d__);
			return <LookupIdByNameOrIdAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06002C6D RID: 11373 RVA: 0x000E79EB File Offset: 0x000E5BEB
		void IDisposable.Dispose()
		{
			this._httpClient.Dispose();
		}

		// Token: 0x04001B7B RID: 7035
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04001B7C RID: 7036
		[Dependency]
		private readonly IConfigurationManager _configurationManager;

		// Token: 0x04001B7D RID: 7037
		[Dependency]
		private readonly IServerDbManager _db;

		// Token: 0x04001B7E RID: 7038
		private readonly HttpClient _httpClient = new HttpClient();

		// Token: 0x02000B3E RID: 2878
		[Nullable(0)]
		private sealed class UserDataResponse : IEquatable<PlayerLocator.UserDataResponse>
		{
			// Token: 0x060038E2 RID: 14562 RVA: 0x00127C46 File Offset: 0x00125E46
			public UserDataResponse(string UserName, Guid UserId)
			{
				this.UserName = UserName;
				this.UserId = UserId;
				base..ctor();
			}

			// Token: 0x170008DA RID: 2266
			// (get) Token: 0x060038E3 RID: 14563 RVA: 0x00127C5C File Offset: 0x00125E5C
			[CompilerGenerated]
			private Type EqualityContract
			{
				[CompilerGenerated]
				get
				{
					return typeof(PlayerLocator.UserDataResponse);
				}
			}

			// Token: 0x170008DB RID: 2267
			// (get) Token: 0x060038E4 RID: 14564 RVA: 0x00127C68 File Offset: 0x00125E68
			// (set) Token: 0x060038E5 RID: 14565 RVA: 0x00127C70 File Offset: 0x00125E70
			public string UserName { get; set; }

			// Token: 0x170008DC RID: 2268
			// (get) Token: 0x060038E6 RID: 14566 RVA: 0x00127C79 File Offset: 0x00125E79
			// (set) Token: 0x060038E7 RID: 14567 RVA: 0x00127C81 File Offset: 0x00125E81
			public Guid UserId { get; set; }

			// Token: 0x060038E8 RID: 14568 RVA: 0x00127C8C File Offset: 0x00125E8C
			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("UserDataResponse");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			// Token: 0x060038E9 RID: 14569 RVA: 0x00127CD8 File Offset: 0x00125ED8
			[CompilerGenerated]
			private bool PrintMembers(StringBuilder builder)
			{
				RuntimeHelpers.EnsureSufficientExecutionStack();
				builder.Append("UserName = ");
				builder.Append(this.UserName);
				builder.Append(", UserId = ");
				builder.Append(this.UserId.ToString());
				return true;
			}

			// Token: 0x060038EA RID: 14570 RVA: 0x00127D2B File Offset: 0x00125F2B
			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator !=(PlayerLocator.UserDataResponse left, PlayerLocator.UserDataResponse right)
			{
				return !(left == right);
			}

			// Token: 0x060038EB RID: 14571 RVA: 0x00127D37 File Offset: 0x00125F37
			[NullableContext(2)]
			[CompilerGenerated]
			public static bool operator ==(PlayerLocator.UserDataResponse left, PlayerLocator.UserDataResponse right)
			{
				return left == right || (left != null && left.Equals(right));
			}

			// Token: 0x060038EC RID: 14572 RVA: 0x00127D4B File Offset: 0x00125F4B
			[CompilerGenerated]
			public override int GetHashCode()
			{
				return (EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.<UserName>k__BackingField)) * -1521134295 + EqualityComparer<Guid>.Default.GetHashCode(this.<UserId>k__BackingField);
			}

			// Token: 0x060038ED RID: 14573 RVA: 0x00127D8B File Offset: 0x00125F8B
			[NullableContext(2)]
			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				return this.Equals(obj as PlayerLocator.UserDataResponse);
			}

			// Token: 0x060038EE RID: 14574 RVA: 0x00127D9C File Offset: 0x00125F9C
			[NullableContext(2)]
			[CompilerGenerated]
			public bool Equals(PlayerLocator.UserDataResponse other)
			{
				return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<string>.Default.Equals(this.<UserName>k__BackingField, other.<UserName>k__BackingField) && EqualityComparer<Guid>.Default.Equals(this.<UserId>k__BackingField, other.<UserId>k__BackingField));
			}

			// Token: 0x060038F0 RID: 14576 RVA: 0x00127DFD File Offset: 0x00125FFD
			[CompilerGenerated]
			private UserDataResponse(PlayerLocator.UserDataResponse original)
			{
				this.UserName = original.<UserName>k__BackingField;
				this.UserId = original.<UserId>k__BackingField;
			}

			// Token: 0x060038F1 RID: 14577 RVA: 0x00127E1D File Offset: 0x0012601D
			[CompilerGenerated]
			public void Deconstruct(out string UserName, out Guid UserId)
			{
				UserName = this.UserName;
				UserId = this.UserId;
			}
		}
	}
}
