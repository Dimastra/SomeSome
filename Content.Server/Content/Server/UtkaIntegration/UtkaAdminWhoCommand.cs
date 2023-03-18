using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Managers;
using Robust.Server.Player;
using Robust.Shared.IoC;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000D9 RID: 217
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class UtkaAdminWhoCommand : IUtkaCommand
	{
		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060003EC RID: 1004 RVA: 0x00014CCA File Offset: 0x00012ECA
		public string Name
		{
			get
			{
				return "adminwho";
			}
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060003ED RID: 1005 RVA: 0x00014CD1 File Offset: 0x00012ED1
		public Type RequestMessageType
		{
			get
			{
				return typeof(UtkaAdminWhoRequest);
			}
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x00014CE0 File Offset: 0x00012EE0
		public void Execute(UtkaTCPSession session, UtkaBaseMessage baseMessage)
		{
			if (!(baseMessage is UtkaAdminWhoRequest))
			{
				return;
			}
			IoCManager.InjectDependencies<UtkaAdminWhoCommand>(this);
			List<IPlayerSession> list = IoCManager.Resolve<IAdminManager>().ActiveAdmins.ToList<IPlayerSession>();
			List<string> adminsList = new List<string>();
			foreach (IPlayerSession admin in list)
			{
				adminsList.Add(admin.Name);
			}
			UtkaAdminWhoResponse toUtkaMessage = new UtkaAdminWhoResponse
			{
				Admins = adminsList
			};
			this._utkaSocketWrapper.SendMessageToAll(toUtkaMessage);
		}

		// Token: 0x0400026D RID: 621
		[Dependency]
		private readonly UtkaTCPWrapper _utkaSocketWrapper;
	}
}
