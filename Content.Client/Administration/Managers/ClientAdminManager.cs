using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Content.Shared.Administration;
using Robust.Client.Console;
using Robust.Shared.Console;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Utility;

namespace Content.Client.Administration.Managers
{
	// Token: 0x020004E4 RID: 1252
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ClientAdminManager : IClientAdminManager, IClientConGroupImplementation, IPostInjectInit
	{
		// Token: 0x140000C3 RID: 195
		// (add) Token: 0x06001FDB RID: 8155 RVA: 0x000B9AE4 File Offset: 0x000B7CE4
		// (remove) Token: 0x06001FDC RID: 8156 RVA: 0x000B9B1C File Offset: 0x000B7D1C
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action AdminStatusUpdated;

		// Token: 0x06001FDD RID: 8157 RVA: 0x000B9B51 File Offset: 0x000B7D51
		public bool IsActive()
		{
			AdminData adminData = this._adminData;
			return adminData != null && adminData.Active;
		}

		// Token: 0x06001FDE RID: 8158 RVA: 0x000B9B64 File Offset: 0x000B7D64
		public bool HasFlag(AdminFlags flag)
		{
			AdminData adminData = this._adminData;
			return adminData != null && adminData.HasFlag(flag);
		}

		// Token: 0x06001FDF RID: 8159 RVA: 0x000B9B78 File Offset: 0x000B7D78
		public bool CanCommand(string cmdName)
		{
			return (this._adminData != null && this._adminData.HasFlag((AdminFlags)2147483648U)) || this._localCommandPermissions.CanCommand(cmdName, this._adminData) || this._availableCommands.Contains(cmdName);
		}

		// Token: 0x06001FE0 RID: 8160 RVA: 0x000B9BB8 File Offset: 0x000B7DB8
		public bool CanViewVar()
		{
			return this.CanCommand("vv");
		}

		// Token: 0x06001FE1 RID: 8161 RVA: 0x000B9BC5 File Offset: 0x000B7DC5
		public bool CanAdminPlace()
		{
			AdminData adminData = this._adminData;
			return adminData != null && adminData.CanAdminPlace();
		}

		// Token: 0x06001FE2 RID: 8162 RVA: 0x000B9BD8 File Offset: 0x000B7DD8
		public bool CanScript()
		{
			AdminData adminData = this._adminData;
			return adminData != null && adminData.CanScript();
		}

		// Token: 0x06001FE3 RID: 8163 RVA: 0x000B9BEB File Offset: 0x000B7DEB
		public bool CanAdminMenu()
		{
			AdminData adminData = this._adminData;
			return adminData != null && adminData.CanAdminMenu();
		}

		// Token: 0x06001FE4 RID: 8164 RVA: 0x000B9C00 File Offset: 0x000B7E00
		public void Initialize()
		{
			this._netMgr.RegisterNetMessage<MsgUpdateAdminStatus>(new ProcessMessage<MsgUpdateAdminStatus>(this.UpdateMessageRx), 3);
			Stream fs;
			if (this._res.TryContentFileRead(new ResourcePath("/clientCommandPerms.yml", "/"), ref fs))
			{
				this._localCommandPermissions.LoadPermissionsFromStream(fs);
			}
		}

		// Token: 0x06001FE5 RID: 8165 RVA: 0x000B9C50 File Offset: 0x000B7E50
		private void UpdateMessageRx(MsgUpdateAdminStatus message)
		{
			this._availableCommands.Clear();
			foreach (KeyValuePair<string, IConsoleCommand> keyValuePair in IoCManager.Resolve<IClientConsoleHost>().AvailableCommands)
			{
				string text;
				IConsoleCommand consoleCommand;
				keyValuePair.Deconstruct(out text, out consoleCommand);
				string item = text;
				if (Attribute.GetCustomAttribute(consoleCommand.GetType(), typeof(AnyCommandAttribute)) != null)
				{
					this._availableCommands.Add(item);
				}
			}
			this._availableCommands.UnionWith(message.AvailableCommands);
			string text2 = "admin";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Have ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(message.AvailableCommands.Length);
			defaultInterpolatedStringHandler.AppendLiteral(" commands available");
			Logger.DebugS(text2, defaultInterpolatedStringHandler.ToStringAndClear());
			this._adminData = message.Admin;
			if (this._adminData != null)
			{
				string value = string.Join("|", AdminFlagsHelper.FlagsToNames(this._adminData.Flags));
				string text3 = "admin";
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 3);
				defaultInterpolatedStringHandler.AppendLiteral("Updated admin status: ");
				defaultInterpolatedStringHandler.AppendFormatted<bool>(this._adminData.Active);
				defaultInterpolatedStringHandler.AppendLiteral("/");
				defaultInterpolatedStringHandler.AppendFormatted(this._adminData.Title);
				defaultInterpolatedStringHandler.AppendLiteral("/");
				defaultInterpolatedStringHandler.AppendFormatted(value);
				Logger.InfoS(text3, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else
			{
				Logger.InfoS("admin", "Updated admin status: Not admin");
			}
			Action adminStatusUpdated = this.AdminStatusUpdated;
			if (adminStatusUpdated != null)
			{
				adminStatusUpdated();
			}
			Action conGroupUpdated = this.ConGroupUpdated;
			if (conGroupUpdated == null)
			{
				return;
			}
			conGroupUpdated();
		}

		// Token: 0x140000C4 RID: 196
		// (add) Token: 0x06001FE6 RID: 8166 RVA: 0x000B9DFC File Offset: 0x000B7FFC
		// (remove) Token: 0x06001FE7 RID: 8167 RVA: 0x000B9E34 File Offset: 0x000B8034
		[Nullable(2)]
		[method: NullableContext(2)]
		[Nullable(2)]
		public event Action ConGroupUpdated;

		// Token: 0x06001FE8 RID: 8168 RVA: 0x000B9E69 File Offset: 0x000B8069
		void IPostInjectInit.PostInject()
		{
			this._conGroup.Implementation = this;
		}

		// Token: 0x04000F37 RID: 3895
		[Dependency]
		private readonly IClientNetManager _netMgr;

		// Token: 0x04000F38 RID: 3896
		[Dependency]
		private readonly IClientConGroupController _conGroup;

		// Token: 0x04000F39 RID: 3897
		[Dependency]
		private readonly IResourceManager _res;

		// Token: 0x04000F3A RID: 3898
		[Nullable(2)]
		private AdminData _adminData;

		// Token: 0x04000F3B RID: 3899
		private readonly HashSet<string> _availableCommands = new HashSet<string>();

		// Token: 0x04000F3C RID: 3900
		private readonly AdminCommandPermissions _localCommandPermissions = new AdminCommandPermissions();
	}
}
