using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.Movement.Systems;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Server.Movement
{
	// Token: 0x0200038C RID: 908
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class LockEyesCommand : IConsoleCommand
	{
		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x060012A3 RID: 4771 RVA: 0x00060A2B File Offset: 0x0005EC2B
		public string Command
		{
			get
			{
				return "lockeyes";
			}
		}

		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x060012A4 RID: 4772 RVA: 0x00060A32 File Offset: 0x0005EC32
		public string Description
		{
			get
			{
				return Loc.GetString("lockeyes-command-description");
			}
		}

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x060012A5 RID: 4773 RVA: 0x00060A3E File Offset: 0x0005EC3E
		public string Help
		{
			get
			{
				return Loc.GetString("lockeyes-command-help");
			}
		}

		// Token: 0x060012A6 RID: 4774 RVA: 0x00060A4C File Offset: 0x0005EC4C
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
				return;
			}
			bool value;
			if (!bool.TryParse(args[0], out value))
			{
				shell.WriteError(Loc.GetString("parse-bool-fail", new ValueTuple<string, object>[]
				{
					new ValueTuple<string, object>("args", args[0])
				}));
				return;
			}
			IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<SharedMoverController>().CameraRotationLocked = value;
		}
	}
}
