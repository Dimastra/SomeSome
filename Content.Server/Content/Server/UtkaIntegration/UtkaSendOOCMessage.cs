using System;
using System.Runtime.CompilerServices;
using Content.Server.Chat.Managers;
using Robust.Shared.IoC;

namespace Content.Server.UtkaIntegration
{
	// Token: 0x020000DC RID: 220
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class UtkaSendOOCMessage : IUtkaCommand
	{
		// Token: 0x1700009A RID: 154
		// (get) Token: 0x060003F9 RID: 1017 RVA: 0x00014EC1 File Offset: 0x000130C1
		public string Name
		{
			get
			{
				return "ooc";
			}
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x060003FA RID: 1018 RVA: 0x00014EC8 File Offset: 0x000130C8
		public Type RequestMessageType
		{
			get
			{
				return typeof(UtkaOOCRequest);
			}
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x00014ED4 File Offset: 0x000130D4
		public void Execute(UtkaTCPSession session, UtkaBaseMessage baseMessage)
		{
			UtkaOOCRequest message = baseMessage as UtkaOOCRequest;
			if (message == null)
			{
				return;
			}
			if (string.IsNullOrWhiteSpace(message.Message) || string.IsNullOrWhiteSpace(message.CKey))
			{
				return;
			}
			IoCManager.Resolve<IChatManager>().SendHookOOC(message.CKey ?? "", message.Message ?? "");
		}
	}
}
