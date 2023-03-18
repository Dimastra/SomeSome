using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Content.Server.Administration.Managers;
using Content.Server.Database;
using Content.Server.EUI;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Robust.Server.Player;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;

namespace Content.Server.Administration.UI
{
	// Token: 0x02000808 RID: 2056
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PermissionsEui : BaseEui
	{
		// Token: 0x06002C91 RID: 11409 RVA: 0x000E8407 File Offset: 0x000E6607
		public PermissionsEui()
		{
			IoCManager.InjectDependencies<PermissionsEui>(this);
		}

		// Token: 0x06002C92 RID: 11410 RVA: 0x000E842C File Offset: 0x000E662C
		public override void Opened()
		{
			base.Opened();
			base.StateDirty();
			this.LoadFromDb();
			this._adminManager.OnPermsChanged += this.AdminManagerOnPermsChanged;
		}

		// Token: 0x06002C93 RID: 11411 RVA: 0x000E8457 File Offset: 0x000E6657
		public override void Closed()
		{
			base.Closed();
			this._adminManager.OnPermsChanged -= this.AdminManagerOnPermsChanged;
		}

		// Token: 0x06002C94 RID: 11412 RVA: 0x000E8476 File Offset: 0x000E6676
		private void AdminManagerOnPermsChanged(AdminPermsChangedEventArgs obj)
		{
			if (obj.Player == base.Player && !this.UserAdminFlagCheck(AdminFlags.Permissions))
			{
				base.Close();
			}
		}

		// Token: 0x06002C95 RID: 11413 RVA: 0x000E8498 File Offset: 0x000E6698
		public override EuiStateBase GetNewState()
		{
			if (this._isLoading)
			{
				return new PermissionsEuiState
				{
					IsLoading = true
				};
			}
			PermissionsEuiState permissionsEuiState = new PermissionsEuiState();
			permissionsEuiState.Admins = this._admins.Select(delegate([TupleElementNames(new string[]
			{
				"a",
				"lastUserName"
			})] ValueTuple<Admin, string> p)
			{
				PermissionsEuiState.AdminData result = default(PermissionsEuiState.AdminData);
				result.PosFlags = AdminFlagsHelper.NamesToFlags(from f in p.Item1.Flags
				where !f.Negative
				select f.Flag);
				result.NegFlags = AdminFlagsHelper.NamesToFlags(from f in p.Item1.Flags
				where f.Negative
				select f.Flag);
				result.Title = p.Item1.Title;
				result.RankId = p.Item1.AdminRankId;
				result.UserId = new NetUserId(p.Item1.UserId);
				result.UserName = p.Item2;
				return result;
			}).ToArray<PermissionsEuiState.AdminData>();
			permissionsEuiState.AdminRanks = this._adminRanks.ToDictionary((AdminRank a) => a.Id, delegate(AdminRank a)
			{
				PermissionsEuiState.AdminRankData result = default(PermissionsEuiState.AdminRankData);
				result.Flags = AdminFlagsHelper.NamesToFlags(from p in a.Flags
				select p.Flag);
				result.Name = a.Name;
				return result;
			});
			return permissionsEuiState;
		}

		// Token: 0x06002C96 RID: 11414 RVA: 0x000E8544 File Offset: 0x000E6744
		public override void HandleMessage(EuiMessageBase msg)
		{
			PermissionsEui.<HandleMessage>d__11 <HandleMessage>d__;
			<HandleMessage>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<HandleMessage>d__.<>4__this = this;
			<HandleMessage>d__.msg = msg;
			<HandleMessage>d__.<>1__state = -1;
			<HandleMessage>d__.<>t__builder.Start<PermissionsEui.<HandleMessage>d__11>(ref <HandleMessage>d__);
		}

		// Token: 0x06002C97 RID: 11415 RVA: 0x000E8584 File Offset: 0x000E6784
		private Task HandleRemoveAdminRank(PermissionsEuiMsg.RemoveAdminRank rr)
		{
			PermissionsEui.<HandleRemoveAdminRank>d__12 <HandleRemoveAdminRank>d__;
			<HandleRemoveAdminRank>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<HandleRemoveAdminRank>d__.<>4__this = this;
			<HandleRemoveAdminRank>d__.rr = rr;
			<HandleRemoveAdminRank>d__.<>1__state = -1;
			<HandleRemoveAdminRank>d__.<>t__builder.Start<PermissionsEui.<HandleRemoveAdminRank>d__12>(ref <HandleRemoveAdminRank>d__);
			return <HandleRemoveAdminRank>d__.<>t__builder.Task;
		}

		// Token: 0x06002C98 RID: 11416 RVA: 0x000E85D0 File Offset: 0x000E67D0
		private Task HandleUpdateAdminRank(PermissionsEuiMsg.UpdateAdminRank ur)
		{
			PermissionsEui.<HandleUpdateAdminRank>d__13 <HandleUpdateAdminRank>d__;
			<HandleUpdateAdminRank>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<HandleUpdateAdminRank>d__.<>4__this = this;
			<HandleUpdateAdminRank>d__.ur = ur;
			<HandleUpdateAdminRank>d__.<>1__state = -1;
			<HandleUpdateAdminRank>d__.<>t__builder.Start<PermissionsEui.<HandleUpdateAdminRank>d__13>(ref <HandleUpdateAdminRank>d__);
			return <HandleUpdateAdminRank>d__.<>t__builder.Task;
		}

		// Token: 0x06002C99 RID: 11417 RVA: 0x000E861C File Offset: 0x000E681C
		private Task HandleAddAdminRank(PermissionsEuiMsg.AddAdminRank ar)
		{
			PermissionsEui.<HandleAddAdminRank>d__14 <HandleAddAdminRank>d__;
			<HandleAddAdminRank>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<HandleAddAdminRank>d__.<>4__this = this;
			<HandleAddAdminRank>d__.ar = ar;
			<HandleAddAdminRank>d__.<>1__state = -1;
			<HandleAddAdminRank>d__.<>t__builder.Start<PermissionsEui.<HandleAddAdminRank>d__14>(ref <HandleAddAdminRank>d__);
			return <HandleAddAdminRank>d__.<>t__builder.Task;
		}

		// Token: 0x06002C9A RID: 11418 RVA: 0x000E8668 File Offset: 0x000E6868
		private Task HandleRemoveAdmin(PermissionsEuiMsg.RemoveAdmin ra)
		{
			PermissionsEui.<HandleRemoveAdmin>d__15 <HandleRemoveAdmin>d__;
			<HandleRemoveAdmin>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<HandleRemoveAdmin>d__.<>4__this = this;
			<HandleRemoveAdmin>d__.ra = ra;
			<HandleRemoveAdmin>d__.<>1__state = -1;
			<HandleRemoveAdmin>d__.<>t__builder.Start<PermissionsEui.<HandleRemoveAdmin>d__15>(ref <HandleRemoveAdmin>d__);
			return <HandleRemoveAdmin>d__.<>t__builder.Task;
		}

		// Token: 0x06002C9B RID: 11419 RVA: 0x000E86B4 File Offset: 0x000E68B4
		private Task HandleUpdateAdmin(PermissionsEuiMsg.UpdateAdmin ua)
		{
			PermissionsEui.<HandleUpdateAdmin>d__16 <HandleUpdateAdmin>d__;
			<HandleUpdateAdmin>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<HandleUpdateAdmin>d__.<>4__this = this;
			<HandleUpdateAdmin>d__.ua = ua;
			<HandleUpdateAdmin>d__.<>1__state = -1;
			<HandleUpdateAdmin>d__.<>t__builder.Start<PermissionsEui.<HandleUpdateAdmin>d__16>(ref <HandleUpdateAdmin>d__);
			return <HandleUpdateAdmin>d__.<>t__builder.Task;
		}

		// Token: 0x06002C9C RID: 11420 RVA: 0x000E8700 File Offset: 0x000E6900
		private Task HandleCreateAdmin(PermissionsEuiMsg.AddAdmin ca)
		{
			PermissionsEui.<HandleCreateAdmin>d__17 <HandleCreateAdmin>d__;
			<HandleCreateAdmin>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<HandleCreateAdmin>d__.<>4__this = this;
			<HandleCreateAdmin>d__.ca = ca;
			<HandleCreateAdmin>d__.<>1__state = -1;
			<HandleCreateAdmin>d__.<>t__builder.Start<PermissionsEui.<HandleCreateAdmin>d__17>(ref <HandleCreateAdmin>d__);
			return <HandleCreateAdmin>d__.<>t__builder.Task;
		}

		// Token: 0x06002C9D RID: 11421 RVA: 0x000E874C File Offset: 0x000E694C
		private bool CheckCreatePerms(AdminFlags posFlags, AdminFlags negFlags)
		{
			if ((posFlags & negFlags) != AdminFlags.None)
			{
				return false;
			}
			if (!this.UserAdminFlagCheck(posFlags))
			{
				string text = "admin.perms";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(55, 1);
				defaultInterpolatedStringHandler.AppendFormatted<IPlayerSession>(base.Player);
				defaultInterpolatedStringHandler.AppendLiteral(" tried to grant admin powers above their authorization.");
				Logger.WarningS(text, defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			return true;
		}

		// Token: 0x06002C9E RID: 11422 RVA: 0x000E87A0 File Offset: 0x000E69A0
		[return: TupleElementNames(new string[]
		{
			"bad",
			null
		})]
		[return: Nullable(new byte[]
		{
			1,
			0,
			2
		})]
		private Task<ValueTuple<bool, string>> FetchAndCheckRank(int? rankId)
		{
			PermissionsEui.<FetchAndCheckRank>d__19 <FetchAndCheckRank>d__;
			<FetchAndCheckRank>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, string>>.Create();
			<FetchAndCheckRank>d__.<>4__this = this;
			<FetchAndCheckRank>d__.rankId = rankId;
			<FetchAndCheckRank>d__.<>1__state = -1;
			<FetchAndCheckRank>d__.<>t__builder.Start<PermissionsEui.<FetchAndCheckRank>d__19>(ref <FetchAndCheckRank>d__);
			return <FetchAndCheckRank>d__.<>t__builder.Task;
		}

		// Token: 0x06002C9F RID: 11423 RVA: 0x000E87EC File Offset: 0x000E69EC
		private void LoadFromDb()
		{
			PermissionsEui.<LoadFromDb>d__20 <LoadFromDb>d__;
			<LoadFromDb>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<LoadFromDb>d__.<>4__this = this;
			<LoadFromDb>d__.<>1__state = -1;
			<LoadFromDb>d__.<>t__builder.Start<PermissionsEui.<LoadFromDb>d__20>(ref <LoadFromDb>d__);
		}

		// Token: 0x06002CA0 RID: 11424 RVA: 0x000E8824 File Offset: 0x000E6A24
		private static List<AdminFlag> GenAdminFlagList(AdminFlags posFlags, AdminFlags negFlags)
		{
			IEnumerable<string> source = AdminFlagsHelper.FlagsToNames(posFlags);
			string[] negFlagList = AdminFlagsHelper.FlagsToNames(negFlags);
			return (from f in source
			select new AdminFlag
			{
				Negative = false,
				Flag = f
			}).Concat(from f in negFlagList
			select new AdminFlag
			{
				Negative = true,
				Flag = f
			}).ToList<AdminFlag>();
		}

		// Token: 0x06002CA1 RID: 11425 RVA: 0x000E8891 File Offset: 0x000E6A91
		private static List<AdminRankFlag> GenRankFlagList(AdminFlags flags)
		{
			return (from f in AdminFlagsHelper.FlagsToNames(flags)
			select new AdminRankFlag
			{
				Flag = f
			}).ToList<AdminRankFlag>();
		}

		// Token: 0x06002CA2 RID: 11426 RVA: 0x000E88C2 File Offset: 0x000E6AC2
		private bool UserAdminFlagCheck(AdminFlags flags)
		{
			if (flags == (AdminFlags)2147483648U)
			{
				return this._adminManager.HasAdminFlag(base.Player, (AdminFlags)2147483648U);
			}
			return this._adminManager.HasAdminFlag(base.Player, AdminFlags.Permissions);
		}

		// Token: 0x06002CA3 RID: 11427 RVA: 0x000E88F8 File Offset: 0x000E6AF8
		private bool CanTouchAdmin(Admin admin)
		{
			AdminFlags adminFlags = AdminFlagsHelper.NamesToFlags(from f in admin.Flags
			where !f.Negative
			select f.Flag);
			AdminRank adminRank = admin.AdminRank;
			IEnumerable<string> enumerable;
			if (adminRank == null)
			{
				enumerable = null;
			}
			else
			{
				enumerable = from f in adminRank.Flags
				select f.Flag;
			}
			AdminFlags rankFlags = AdminFlagsHelper.NamesToFlags(enumerable ?? Array.Empty<string>());
			AdminFlags totalFlags = adminFlags | rankFlags;
			return this.UserAdminFlagCheck(totalFlags);
		}

		// Token: 0x06002CA4 RID: 11428 RVA: 0x000E89A8 File Offset: 0x000E6BA8
		private bool CanTouchRank(AdminRank rank)
		{
			AdminFlags rankFlags = AdminFlagsHelper.NamesToFlags(from f in rank.Flags
			select f.Flag);
			return this.UserAdminFlagCheck(rankFlags);
		}

		// Token: 0x04001B89 RID: 7049
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04001B8A RID: 7050
		[Dependency]
		private readonly IServerDbManager _db;

		// Token: 0x04001B8B RID: 7051
		[Dependency]
		private readonly IAdminManager _adminManager;

		// Token: 0x04001B8C RID: 7052
		private bool _isLoading;

		// Token: 0x04001B8D RID: 7053
		[TupleElementNames(new string[]
		{
			"a",
			"lastUserName"
		})]
		[Nullable(new byte[]
		{
			1,
			0,
			1,
			2
		})]
		private readonly List<ValueTuple<Admin, string>> _admins = new List<ValueTuple<Admin, string>>();

		// Token: 0x04001B8E RID: 7054
		private readonly List<AdminRank> _adminRanks = new List<AdminRank>();
	}
}
