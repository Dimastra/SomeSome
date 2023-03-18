using System;
using System.Runtime.CompilerServices;
using Content.Client.Shuttles.Systems;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Shuttles.Commands
{
	// Token: 0x02000157 RID: 343
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ShowEmergencyShuttleCommand : IConsoleCommand
	{
		// Token: 0x170001AE RID: 430
		// (get) Token: 0x0600090D RID: 2317 RVA: 0x000358B6 File Offset: 0x00033AB6
		public string Command
		{
			get
			{
				return "showemergencyshuttle";
			}
		}

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x0600090E RID: 2318 RVA: 0x000358BD File Offset: 0x00033ABD
		public string Description
		{
			get
			{
				return "Shows the expected position of the emergency shuttle";
			}
		}

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x0600090F RID: 2319 RVA: 0x000358C4 File Offset: 0x00033AC4
		public string Help
		{
			get
			{
				return this.Command ?? "";
			}
		}

		// Token: 0x06000910 RID: 2320 RVA: 0x000358D8 File Offset: 0x00033AD8
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			ShuttleSystem entitySystem = IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<ShuttleSystem>();
			ShuttleSystem shuttleSystem = entitySystem;
			shuttleSystem.EnableShuttlePosition = !shuttleSystem.EnableShuttlePosition;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Set emergency shuttle debug to ");
			defaultInterpolatedStringHandler.AppendFormatted<bool>(entitySystem.EnableShuttlePosition);
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
