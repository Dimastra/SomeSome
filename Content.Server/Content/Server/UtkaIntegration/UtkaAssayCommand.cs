using System;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Managers;
using Robust.Shared.IoC;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000DA RID: 218
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class UtkaAssayCommand : IUtkaCommand
	{
		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060003F0 RID: 1008 RVA: 0x00014D78 File Offset: 0x00012F78
		public string Name
		{
			get
			{
				return "asay";
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060003F1 RID: 1009 RVA: 0x00014D7F File Offset: 0x00012F7F
		public Type RequestMessageType
		{
			get
			{
				return typeof(UtkaAsayRequest);
			}
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x00014D8C File Offset: 0x00012F8C
		public void Execute(UtkaTCPSession session, UtkaBaseMessage baseMessage)
		{
			UtkaAsayRequest message = baseMessage as UtkaAsayRequest;
			if (message == null)
			{
				return;
			}
			string ckey = message.ACkey;
			if (string.IsNullOrWhiteSpace(message.Message) || string.IsNullOrWhiteSpace(ckey))
			{
				return;
			}
			IoCManager.Resolve<IChatManager>().SendHookAdminChat(ckey, message.Message);
		}
	}
}
