using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Players;

namespace Content.Server.Radiation.Systems
{
	// Token: 0x02000265 RID: 613
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class RadiationViewCommand : IConsoleCommand
	{
		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x06000C3D RID: 3133 RVA: 0x0004076F File Offset: 0x0003E96F
		public string Command
		{
			get
			{
				return "showradiation";
			}
		}

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x06000C3E RID: 3134 RVA: 0x00040776 File Offset: 0x0003E976
		public string Description
		{
			get
			{
				return Loc.GetString("radiation-command-description");
			}
		}

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x06000C3F RID: 3135 RVA: 0x00040782 File Offset: 0x0003E982
		public string Help
		{
			get
			{
				return Loc.GetString("radiation-command-help");
			}
		}

		// Token: 0x06000C40 RID: 3136 RVA: 0x00040790 File Offset: 0x0003E990
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			ICommonSession session = shell.Player;
			if (session == null)
			{
				return;
			}
			IoCManager.Resolve<IEntityManager>().System<RadiationSystem>().ToggleDebugView(session);
		}
	}
}
