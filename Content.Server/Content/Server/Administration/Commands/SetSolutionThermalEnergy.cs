using System;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Server.Chemistry.EntitySystems;
using Content.Shared.Administration;
using Content.Shared.Chemistry.Components;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000864 RID: 2148
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class SetSolutionThermalEnergy : IConsoleCommand
	{
		// Token: 0x170007B4 RID: 1972
		// (get) Token: 0x06002EF7 RID: 12023 RVA: 0x000F3914 File Offset: 0x000F1B14
		public string Command
		{
			get
			{
				return "setsolutionthermalenergy";
			}
		}

		// Token: 0x170007B5 RID: 1973
		// (get) Token: 0x06002EF8 RID: 12024 RVA: 0x000F391B File Offset: 0x000F1B1B
		public string Description
		{
			get
			{
				return "Set the thermal energy of some solution.";
			}
		}

		// Token: 0x170007B6 RID: 1974
		// (get) Token: 0x06002EF9 RID: 12025 RVA: 0x000F3922 File Offset: 0x000F1B22
		public string Help
		{
			get
			{
				return "Usage: " + this.Command + " <target> <solution> <new thermal energy>";
			}
		}

		// Token: 0x06002EFA RID: 12026 RVA: 0x000F393C File Offset: 0x000F1B3C
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length < 3)
			{
				shell.WriteLine("Not enough arguments.\n" + this.Help);
				return;
			}
			EntityUid uid;
			if (!EntityUid.TryParse(args[0], ref uid))
			{
				shell.WriteLine("Invalid entity id.");
				return;
			}
			SolutionContainerManagerComponent man;
			if (!IoCManager.Resolve<IEntityManager>().TryGetComponent<SolutionContainerManagerComponent>(uid, ref man))
			{
				shell.WriteLine("Entity does not have any solutions.");
				return;
			}
			if (!man.Solutions.ContainsKey(args[1]))
			{
				string validSolutions = string.Join(", ", man.Solutions.Keys);
				shell.WriteLine("Entity does not have a \"" + args[1] + "\" solution. Valid solutions are:\n" + validSolutions);
				return;
			}
			Solution solution = man.Solutions[args[1]];
			float quantity;
			if (!float.TryParse(args[2], out quantity))
			{
				shell.WriteLine("Failed to parse new thermal energy.");
				return;
			}
			if (solution.GetHeatCapacity(null) <= 0f)
			{
				if (quantity != 0f)
				{
					shell.WriteLine("Cannot set the thermal energy of a solution with 0 heat capacity to a non-zero number.");
					return;
				}
			}
			else if (quantity <= 0f)
			{
				shell.WriteLine("Cannot set the thermal energy of a solution with heat capacity to a non-positive number.");
				return;
			}
			EntitySystem.Get<SolutionContainerSystem>().SetThermalEnergy(uid, solution, quantity);
		}
	}
}
