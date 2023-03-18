using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Content.Server.Chat.Managers;
using Content.Server.Database;
using Content.Server.Players;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Robust.Server.Console;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.ContentPack;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Players;
using Robust.Shared.Utility;

namespace Content.Server.Administration.Managers
{
	// Token: 0x02000815 RID: 2069
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AdminManager : IAdminManager, IPostInjectInit, IConGroupControllerImplementation
	{
		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06002D1B RID: 11547 RVA: 0x000EDD90 File Offset: 0x000EBF90
		// (remove) Token: 0x06002D1C RID: 11548 RVA: 0x000EDDC8 File Offset: 0x000EBFC8
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[Nullable(new byte[]
		{
			2,
			1
		})]
		public event Action<AdminPermsChangedEventArgs> OnPermsChanged;

		// Token: 0x170006F6 RID: 1782
		// (get) Token: 0x06002D1D RID: 11549 RVA: 0x000EDE00 File Offset: 0x000EC000
		public IEnumerable<IPlayerSession> ActiveAdmins
		{
			get
			{
				return from p in this._admins
				where p.Value.Data.Active
				select p.Key;
			}
		}

		// Token: 0x170006F7 RID: 1783
		// (get) Token: 0x06002D1E RID: 11550 RVA: 0x000EDE5B File Offset: 0x000EC05B
		public IEnumerable<IPlayerSession> AdminsWithFlag
		{
			get
			{
				return from p in this.ActiveAdmins
				where this._adminManager.HasAdminFlag(p, AdminFlags.Admin)
				select p;
			}
		}

		// Token: 0x170006F8 RID: 1784
		// (get) Token: 0x06002D1F RID: 11551 RVA: 0x000EDE74 File Offset: 0x000EC074
		public IEnumerable<IPlayerSession> AllAdmins
		{
			get
			{
				return from p in this._admins
				select p.Key;
			}
		}

		// Token: 0x06002D20 RID: 11552 RVA: 0x000EDEA0 File Offset: 0x000EC0A0
		public bool IsAdmin(IPlayerSession session, bool includeDeAdmin = false)
		{
			return this.GetAdminData(session, includeDeAdmin) != null;
		}

		// Token: 0x06002D21 RID: 11553 RVA: 0x000EDEB0 File Offset: 0x000EC0B0
		[return: Nullable(2)]
		public AdminData GetAdminData(IPlayerSession session, bool includeDeAdmin = false)
		{
			AdminManager.AdminReg reg;
			if (this._admins.TryGetValue(session, out reg) && (reg.Data.Active || includeDeAdmin))
			{
				return reg.Data;
			}
			return null;
		}

		// Token: 0x06002D22 RID: 11554 RVA: 0x000EDEE4 File Offset: 0x000EC0E4
		[NullableContext(2)]
		public AdminData GetAdminData(EntityUid uid, bool includeDeAdmin = false)
		{
			ICommonSession session;
			if (this._playerManager.TryGetSessionByEntity(uid, ref session))
			{
				IPlayerSession playerSession = session as IPlayerSession;
				if (playerSession != null)
				{
					return this.GetAdminData(playerSession, includeDeAdmin);
				}
			}
			return null;
		}

		// Token: 0x06002D23 RID: 11555 RVA: 0x000EDF18 File Offset: 0x000EC118
		public void DeAdmin(IPlayerSession session)
		{
			AdminManager.AdminReg reg;
			if (!this._admins.TryGetValue(session, out reg))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Player ");
				defaultInterpolatedStringHandler.AppendFormatted<IPlayerSession>(session);
				defaultInterpolatedStringHandler.AppendLiteral(" is not an admin");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			if (!reg.Data.Active)
			{
				return;
			}
			this._chat.SendAdminAnnouncement(Loc.GetString("admin-manager-self-de-admin-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("exAdminName", session.Name)
			}));
			this._chat.DispatchServerMessage(session, Loc.GetString("admin-manager-became-normal-player-message"), false);
			session.ContentData().ExplicitlyDeadminned = true;
			reg.Data.Active = false;
			this.SendPermsChangedEvent(session);
			this.UpdateAdminStatus(session);
		}

		// Token: 0x06002D24 RID: 11556 RVA: 0x000EDFEC File Offset: 0x000EC1EC
		public void ReAdmin(IPlayerSession session)
		{
			AdminManager.AdminReg reg;
			if (!this._admins.TryGetValue(session, out reg))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Player ");
				defaultInterpolatedStringHandler.AppendFormatted<IPlayerSession>(session);
				defaultInterpolatedStringHandler.AppendLiteral(" is not an admin");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			if (reg.Data.Active)
			{
				return;
			}
			this._chat.DispatchServerMessage(session, Loc.GetString("admin-manager-became-admin-message"), false);
			session.ContentData().ExplicitlyDeadminned = false;
			reg.Data.Active = true;
			this._chat.SendAdminAnnouncement(Loc.GetString("admin-manager-self-re-admin-message", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("newAdminName", session.Name)
			}));
			this.SendPermsChangedEvent(session);
			this.UpdateAdminStatus(session);
		}

		// Token: 0x06002D25 RID: 11557 RVA: 0x000EE0C0 File Offset: 0x000EC2C0
		public void ReloadAdmin(IPlayerSession player)
		{
			AdminManager.<ReloadAdmin>d__26 <ReloadAdmin>d__;
			<ReloadAdmin>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<ReloadAdmin>d__.<>4__this = this;
			<ReloadAdmin>d__.player = player;
			<ReloadAdmin>d__.<>1__state = -1;
			<ReloadAdmin>d__.<>t__builder.Start<AdminManager.<ReloadAdmin>d__26>(ref <ReloadAdmin>d__);
		}

		// Token: 0x06002D26 RID: 11558 RVA: 0x000EE100 File Offset: 0x000EC300
		public void ReloadAdminsWithRank(int rankId)
		{
			IEnumerable<AdminManager.AdminReg> values = this._admins.Values;
			Func<AdminManager.AdminReg, bool> <>9__0;
			Func<AdminManager.AdminReg, bool> predicate;
			if ((predicate = <>9__0) == null)
			{
				predicate = (<>9__0 = delegate(AdminManager.AdminReg p)
				{
					int? rankId2 = p.RankId;
					int rankId3 = rankId;
					return rankId2.GetValueOrDefault() == rankId3 & rankId2 != null;
				});
			}
			foreach (AdminManager.AdminReg dat in values.Where(predicate).ToArray<AdminManager.AdminReg>())
			{
				this.ReloadAdmin(dat.Session);
			}
		}

		// Token: 0x06002D27 RID: 11559 RVA: 0x000EE170 File Offset: 0x000EC370
		public void Initialize()
		{
			this._netMgr.RegisterNetMessage<MsgUpdateAdminStatus>(null, 3);
			foreach (KeyValuePair<string, IConsoleCommand> keyValuePair in this._consoleHost.AvailableCommands)
			{
				string text;
				IConsoleCommand cmd;
				keyValuePair.Deconstruct(out text, out cmd);
				string cmdName = text;
				ValueTuple<bool, AdminFlags[]> requiredFlag = AdminManager.GetRequiredFlag(cmd);
				bool isAvail = requiredFlag.Item1;
				AdminFlags[] flagsReq = requiredFlag.Item2;
				if (isAvail)
				{
					if (flagsReq.Length != 0)
					{
						this._commandPermissions.AdminCommands.Add(cmdName, flagsReq);
					}
					else
					{
						this._commandPermissions.AnyCommands.Add(cmdName);
					}
				}
			}
			Stream efs;
			if (this._res.TryContentFileRead(new ResourcePath("/engineCommandPerms.yml", "/"), ref efs))
			{
				this._commandPermissions.LoadPermissionsFromStream(efs);
			}
		}

		// Token: 0x06002D28 RID: 11560 RVA: 0x000EE248 File Offset: 0x000EC448
		public void PromoteHost(IPlayerSession player)
		{
			this._promotedPlayers.Add(player.UserId);
			this.ReloadAdmin(player);
		}

		// Token: 0x06002D29 RID: 11561 RVA: 0x000EE263 File Offset: 0x000EC463
		void IPostInjectInit.PostInject()
		{
			this._playerManager.PlayerStatusChanged += this.PlayerStatusChanged;
			this._conGroup.Implementation = this;
		}

		// Token: 0x06002D2A RID: 11562 RVA: 0x000EE288 File Offset: 0x000EC488
		private void UpdateAdminStatus(IPlayerSession session)
		{
			MsgUpdateAdminStatus msg = new MsgUpdateAdminStatus();
			List<string> commands = new List<string>(this._commandPermissions.AnyCommands);
			AdminManager.AdminReg adminData;
			if (this._admins.TryGetValue(session, out adminData))
			{
				msg.Admin = adminData.Data;
				Func<AdminFlags, bool> <>9__2;
				commands.AddRange(from p in this._commandPermissions.AdminCommands.Where(delegate(KeyValuePair<string, AdminFlags[]> p)
				{
					IEnumerable<AdminFlags> value = p.Value;
					Func<AdminFlags, bool> predicate;
					if ((predicate = <>9__2) == null)
					{
						predicate = (<>9__2 = ((AdminFlags f) => adminData.Data.HasFlag(f)));
					}
					return value.Any(predicate);
				})
				select p.Key);
			}
			msg.AvailableCommands = commands.ToArray();
			this._netMgr.ServerSendMessage(msg, session.ConnectedClient);
		}

		// Token: 0x06002D2B RID: 11563 RVA: 0x000EE33C File Offset: 0x000EC53C
		private void PlayerStatusChanged([Nullable(2)] object sender, SessionStatusEventArgs e)
		{
			if (e.NewStatus == 2)
			{
				this.UpdateAdminStatus(e.Session);
				return;
			}
			if (e.NewStatus == 3)
			{
				this.LoginAdminMaybe(e.Session);
				return;
			}
			if (e.NewStatus == 4 && this._admins.Remove(e.Session) && this._cfg.GetCVar<bool>(CCVars.AdminAnnounceLogout))
			{
				this._chat.SendAdminAnnouncement(Loc.GetString("admin-manager-admin-logout-message", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("name", e.Session.Name)
				}));
			}
		}

		// Token: 0x06002D2C RID: 11564 RVA: 0x000EE3DC File Offset: 0x000EC5DC
		private void LoginAdminMaybe(IPlayerSession session)
		{
			AdminManager.<LoginAdminMaybe>d__33 <LoginAdminMaybe>d__;
			<LoginAdminMaybe>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<LoginAdminMaybe>d__.<>4__this = this;
			<LoginAdminMaybe>d__.session = session;
			<LoginAdminMaybe>d__.<>1__state = -1;
			<LoginAdminMaybe>d__.<>t__builder.Start<AdminManager.<LoginAdminMaybe>d__33>(ref <LoginAdminMaybe>d__);
		}

		// Token: 0x06002D2D RID: 11565 RVA: 0x000EE41C File Offset: 0x000EC61C
		[return: TupleElementNames(new string[]
		{
			"dat",
			"rankId",
			"specialLogin"
		})]
		[return: Nullable(new byte[]
		{
			1,
			0,
			1
		})]
		private Task<ValueTuple<AdminData, int?, bool>?> LoadAdminData(IPlayerSession session)
		{
			AdminManager.<LoadAdminData>d__34 <LoadAdminData>d__;
			<LoadAdminData>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<AdminData, int?, bool>?>.Create();
			<LoadAdminData>d__.<>4__this = this;
			<LoadAdminData>d__.session = session;
			<LoadAdminData>d__.<>1__state = -1;
			<LoadAdminData>d__.<>t__builder.Start<AdminManager.<LoadAdminData>d__34>(ref <LoadAdminData>d__);
			return <LoadAdminData>d__.<>t__builder.Task;
		}

		// Token: 0x06002D2E RID: 11566 RVA: 0x000EE468 File Offset: 0x000EC668
		private static bool IsLocal(IPlayerSession player)
		{
			IPAddress addr = player.ConnectedClient.RemoteEndPoint.Address;
			if (addr.IsIPv4MappedToIPv6)
			{
				addr = addr.MapToIPv4();
			}
			return object.Equals(addr, IPAddress.Loopback) || object.Equals(addr, IPAddress.IPv6Loopback);
		}

		// Token: 0x06002D2F RID: 11567 RVA: 0x000EE4B0 File Offset: 0x000EC6B0
		public bool CanCommand(IPlayerSession session, string cmdName)
		{
			if (this._commandPermissions.AnyCommands.Contains(cmdName))
			{
				return true;
			}
			AdminFlags[] flagsReq;
			if (!this._commandPermissions.AdminCommands.TryGetValue(cmdName, out flagsReq))
			{
				return false;
			}
			AdminData data = this.GetAdminData(session, false);
			if (data == null)
			{
				return false;
			}
			foreach (AdminFlags flagReq in flagsReq)
			{
				if (data.HasFlag(flagReq))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002D30 RID: 11568 RVA: 0x000EE51C File Offset: 0x000EC71C
		[return: TupleElementNames(new string[]
		{
			"isAvail",
			"flagsReq"
		})]
		[return: Nullable(new byte[]
		{
			0,
			1
		})]
		private static ValueTuple<bool, AdminFlags[]> GetRequiredFlag(IConsoleCommand cmd)
		{
			MemberInfo type = cmd.GetType();
			ConsoleHost.RegisteredCommand registered = cmd as ConsoleHost.RegisteredCommand;
			if (registered != null)
			{
				type = registered.Callback.Method;
			}
			if (Attribute.IsDefined(type, typeof(AnyCommandAttribute)))
			{
				return new ValueTuple<bool, AdminFlags[]>(true, Array.Empty<AdminFlags>());
			}
			AdminFlags[] attribs = (from AdminCommandAttribute p in type.GetCustomAttributes(typeof(AdminCommandAttribute))
			select p.Flags).ToArray<AdminFlags>();
			return new ValueTuple<bool, AdminFlags[]>(attribs.Length != 0, attribs);
		}

		// Token: 0x06002D31 RID: 11569 RVA: 0x000EE5AE File Offset: 0x000EC7AE
		public bool CanViewVar(IPlayerSession session)
		{
			return this.CanCommand(session, "vv");
		}

		// Token: 0x06002D32 RID: 11570 RVA: 0x000EE5BC File Offset: 0x000EC7BC
		public bool CanAdminPlace(IPlayerSession session)
		{
			AdminData adminData = this.GetAdminData(session, false);
			return adminData != null && adminData.CanAdminPlace();
		}

		// Token: 0x06002D33 RID: 11571 RVA: 0x000EE5D1 File Offset: 0x000EC7D1
		public bool CanScript(IPlayerSession session)
		{
			AdminData adminData = this.GetAdminData(session, false);
			return adminData != null && adminData.CanScript();
		}

		// Token: 0x06002D34 RID: 11572 RVA: 0x000EE5E6 File Offset: 0x000EC7E6
		public bool CanAdminMenu(IPlayerSession session)
		{
			AdminData adminData = this.GetAdminData(session, false);
			return adminData != null && adminData.CanAdminMenu();
		}

		// Token: 0x06002D35 RID: 11573 RVA: 0x000EE5FB File Offset: 0x000EC7FB
		public bool CanAdminReloadPrototypes(IPlayerSession session)
		{
			AdminData adminData = this.GetAdminData(session, false);
			return adminData != null && adminData.CanAdminReloadPrototypes();
		}

		// Token: 0x06002D36 RID: 11574 RVA: 0x000EE610 File Offset: 0x000EC810
		private void SendPermsChangedEvent(IPlayerSession session)
		{
			AdminData adminData = this.GetAdminData(session, false);
			AdminFlags? flags = (adminData != null) ? new AdminFlags?(adminData.Flags) : null;
			Action<AdminPermsChangedEventArgs> onPermsChanged = this.OnPermsChanged;
			if (onPermsChanged == null)
			{
				return;
			}
			onPermsChanged(new AdminPermsChangedEventArgs(session, flags));
		}

		// Token: 0x04001BF0 RID: 7152
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04001BF1 RID: 7153
		[Dependency]
		private readonly IServerDbManager _dbManager;

		// Token: 0x04001BF2 RID: 7154
		[Dependency]
		private readonly IConfigurationManager _cfg;

		// Token: 0x04001BF3 RID: 7155
		[Dependency]
		private readonly IServerNetManager _netMgr;

		// Token: 0x04001BF4 RID: 7156
		[Dependency]
		private readonly IConGroupController _conGroup;

		// Token: 0x04001BF5 RID: 7157
		[Dependency]
		private readonly IResourceManager _res;

		// Token: 0x04001BF6 RID: 7158
		[Dependency]
		private readonly IServerConsoleHost _consoleHost;

		// Token: 0x04001BF7 RID: 7159
		[Dependency]
		private readonly IChatManager _chat;

		// Token: 0x04001BF8 RID: 7160
		[Dependency]
		private readonly IAdminManager _adminManager;

		// Token: 0x04001BF9 RID: 7161
		private readonly Dictionary<IPlayerSession, AdminManager.AdminReg> _admins = new Dictionary<IPlayerSession, AdminManager.AdminReg>();

		// Token: 0x04001BFA RID: 7162
		private readonly HashSet<NetUserId> _promotedPlayers = new HashSet<NetUserId>();

		// Token: 0x04001BFC RID: 7164
		private readonly AdminCommandPermissions _commandPermissions = new AdminCommandPermissions();

		// Token: 0x02000B73 RID: 2931
		[Nullable(0)]
		private sealed class AdminReg
		{
			// Token: 0x060039F2 RID: 14834 RVA: 0x0012EFAD File Offset: 0x0012D1AD
			public AdminReg(IPlayerSession session, AdminData data)
			{
				this.Data = data;
				this.Session = session;
			}

			// Token: 0x04002AEC RID: 10988
			public readonly IPlayerSession Session;

			// Token: 0x04002AED RID: 10989
			public AdminData Data;

			// Token: 0x04002AEE RID: 10990
			public int? RankId;

			// Token: 0x04002AEF RID: 10991
			public bool IsSpecialLogin;
		}
	}
}
