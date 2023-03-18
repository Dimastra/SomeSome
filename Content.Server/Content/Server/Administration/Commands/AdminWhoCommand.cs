using System;
using System.Runtime.CompilerServices;
using System.Text;
using Content.Server.Administration.Managers;
using Content.Server.Afk;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000830 RID: 2096
	[NullableContext(1)]
	[Nullable(0)]
	[AnyCommand]
	public sealed class AdminWhoCommand : IConsoleCommand
	{
		// Token: 0x1700071C RID: 1820
		// (get) Token: 0x06002DE5 RID: 11749 RVA: 0x000F02C8 File Offset: 0x000EE4C8
		public string Command
		{
			get
			{
				return "adminwho";
			}
		}

		// Token: 0x1700071D RID: 1821
		// (get) Token: 0x06002DE6 RID: 11750 RVA: 0x000F02CF File Offset: 0x000EE4CF
		public string Description
		{
			get
			{
				return "Returns a list of all admins on the server";
			}
		}

		// Token: 0x1700071E RID: 1822
		// (get) Token: 0x06002DE7 RID: 11751 RVA: 0x000F02D6 File Offset: 0x000EE4D6
		public string Help
		{
			get
			{
				return "Usage: adminwho";
			}
		}

		// Token: 0x06002DE8 RID: 11752 RVA: 0x000F02E0 File Offset: 0x000EE4E0
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IAdminManager adminMgr = IoCManager.Resolve<IAdminManager>();
			IAfkManager afk = IoCManager.Resolve<IAfkManager>();
			StringBuilder sb = new StringBuilder();
			bool first = true;
			foreach (IPlayerSession admin in adminMgr.ActiveAdmins)
			{
				if (!first)
				{
					sb.Append('\n');
				}
				first = false;
				AdminData adminData = adminMgr.GetAdminData(admin, false);
				sb.Append(admin.Name);
				string title = adminData.Title;
				if (title != null)
				{
					StringBuilder stringBuilder = sb;
					StringBuilder stringBuilder2 = stringBuilder;
					StringBuilder.AppendInterpolatedStringHandler appendInterpolatedStringHandler = new StringBuilder.AppendInterpolatedStringHandler(4, 1, stringBuilder);
					appendInterpolatedStringHandler.AppendLiteral(": [");
					appendInterpolatedStringHandler.AppendFormatted(title);
					appendInterpolatedStringHandler.AppendLiteral("]");
					stringBuilder2.Append(ref appendInterpolatedStringHandler);
				}
				IPlayerSession player = shell.Player as IPlayerSession;
				if (player != null && adminMgr.HasAdminFlag(player, AdminFlags.Admin) && afk.IsAfk(admin))
				{
					sb.Append(" [AFK]");
				}
			}
			shell.WriteLine(sb.ToString());
		}
	}
}
