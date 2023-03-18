using System;
using System.Runtime.CompilerServices;
using Content.Client.Popups;
using Content.Shared.Popups;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Commands
{
	// Token: 0x020003A7 RID: 935
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class NotifyCommand : IConsoleCommand
	{
		// Token: 0x170004B6 RID: 1206
		// (get) Token: 0x06001745 RID: 5957 RVA: 0x0008668C File Offset: 0x0008488C
		public string Command
		{
			get
			{
				return "notify";
			}
		}

		// Token: 0x170004B7 RID: 1207
		// (get) Token: 0x06001746 RID: 5958 RVA: 0x00086693 File Offset: 0x00084893
		public string Description
		{
			get
			{
				return "Send a notify client side.";
			}
		}

		// Token: 0x170004B8 RID: 1208
		// (get) Token: 0x06001747 RID: 5959 RVA: 0x0008669A File Offset: 0x0008489A
		public string Help
		{
			get
			{
				return "notify <message>";
			}
		}

		// Token: 0x06001748 RID: 5960 RVA: 0x000866A4 File Offset: 0x000848A4
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			string message = args[0];
			IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<PopupSystem>().PopupCursor(message, PopupType.Small);
		}
	}
}
