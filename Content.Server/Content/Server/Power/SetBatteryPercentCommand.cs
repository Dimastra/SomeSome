using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Power.Components;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Power
{
	// Token: 0x02000275 RID: 629
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Debug)]
	public sealed class SetBatteryPercentCommand : IConsoleCommand
	{
		// Token: 0x170001CE RID: 462
		// (get) Token: 0x06000CA4 RID: 3236 RVA: 0x00042162 File Offset: 0x00040362
		public string Command
		{
			get
			{
				return "setbatterypercent";
			}
		}

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x06000CA5 RID: 3237 RVA: 0x00042169 File Offset: 0x00040369
		public string Description
		{
			get
			{
				return "Drains or recharges a battery by entity uid and percentage, i.e.: forall with Battery do setbatterypercent $ID 0";
			}
		}

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x06000CA6 RID: 3238 RVA: 0x00042170 File Offset: 0x00040370
		public string Help
		{
			get
			{
				return this.Command + " <id> <percent>";
			}
		}

		// Token: 0x06000CA7 RID: 3239 RVA: 0x00042184 File Offset: 0x00040384
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 2)
			{
				shell.WriteLine("Invalid amount of arguments.\n" + this.Help);
				return;
			}
			EntityUid id;
			if (!EntityUid.TryParse(args[0], ref id))
			{
				shell.WriteLine(args[0] + " is not a valid entity id.");
				return;
			}
			float percent;
			if (!float.TryParse(args[1], out percent))
			{
				shell.WriteLine(args[1] + " is not a valid float (percentage).");
				return;
			}
			BatteryComponent battery;
			if (!IoCManager.Resolve<IEntityManager>().TryGetComponent<BatteryComponent>(id, ref battery))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 1);
				defaultInterpolatedStringHandler.AppendLiteral("No battery found with id ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(id);
				defaultInterpolatedStringHandler.AppendLiteral(".");
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			battery.CurrentCharge = battery.MaxCharge * percent / 100f;
		}
	}
}
