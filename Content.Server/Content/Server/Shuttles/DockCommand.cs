using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Shuttles.Components;
using Content.Server.Shuttles.Systems;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Shuttles
{
	// Token: 0x020001F5 RID: 501
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Mapping)]
	public sealed class DockCommand : IConsoleCommand
	{
		// Token: 0x17000182 RID: 386
		// (get) Token: 0x060009B4 RID: 2484 RVA: 0x0003118E File Offset: 0x0002F38E
		public string Command
		{
			get
			{
				return "dock";
			}
		}

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x060009B5 RID: 2485 RVA: 0x00031195 File Offset: 0x0002F395
		public string Description
		{
			get
			{
				return "Attempts to dock 2 airlocks together. Doesn't check whether it is valid.";
			}
		}

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x060009B6 RID: 2486 RVA: 0x0003119C File Offset: 0x0002F39C
		public string Help
		{
			get
			{
				return this.Command + " <airlock entityuid1> <airlock entityuid2>";
			}
		}

		// Token: 0x060009B7 RID: 2487 RVA: 0x000311B0 File Offset: 0x0002F3B0
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 2)
			{
				shell.WriteError("Invalid number of args supplied");
				return;
			}
			EntityUid airlock;
			if (!EntityUid.TryParse(args[0], ref airlock))
			{
				shell.WriteError("Invalid EntityUid " + args[0]);
				return;
			}
			EntityUid airlock2;
			if (!EntityUid.TryParse(args[1], ref airlock2))
			{
				shell.WriteError("Invalid EntityUid " + args[1]);
				return;
			}
			IEntityManager entManager = IoCManager.Resolve<IEntityManager>();
			DockingComponent dock;
			if (!entManager.TryGetComponent<DockingComponent>(airlock, ref dock))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
				defaultInterpolatedStringHandler.AppendLiteral("No docking component found on ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(airlock);
				shell.WriteError(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			DockingComponent dock2;
			if (!entManager.TryGetComponent<DockingComponent>(airlock2, ref dock2))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
				defaultInterpolatedStringHandler.AppendLiteral("No docking component found on ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(airlock2);
				shell.WriteError(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			EntitySystem.Get<DockingSystem>().Dock(dock, dock2);
		}
	}
}
