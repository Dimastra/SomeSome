using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Power.EntitySystems;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;

namespace Content.Server.Power.Commands
{
	// Token: 0x020002BF RID: 703
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class PowerStatCommand : IConsoleCommand
	{
		// Token: 0x17000206 RID: 518
		// (get) Token: 0x06000E32 RID: 3634 RVA: 0x00047EFF File Offset: 0x000460FF
		public string Command
		{
			get
			{
				return "powerstat";
			}
		}

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x06000E33 RID: 3635 RVA: 0x00047F06 File Offset: 0x00046106
		public string Description
		{
			get
			{
				return "Shows statistics for pow3r";
			}
		}

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x06000E34 RID: 3636 RVA: 0x00047F0D File Offset: 0x0004610D
		public string Help
		{
			get
			{
				return "Usage: powerstat";
			}
		}

		// Token: 0x06000E35 RID: 3637 RVA: 0x00047F14 File Offset: 0x00046114
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			PowerStatistics stats = EntitySystem.Get<PowerNetSystem>().GetStatistics();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
			defaultInterpolatedStringHandler.AppendLiteral("networks: ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(stats.CountNetworks);
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
			defaultInterpolatedStringHandler.AppendLiteral("loads: ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(stats.CountLoads);
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
			defaultInterpolatedStringHandler.AppendLiteral("supplies: ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(stats.CountSupplies);
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
			defaultInterpolatedStringHandler.AppendLiteral("batteries: ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(stats.CountBatteries);
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
