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
	// Token: 0x02000863 RID: 2147
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class SetSolutionTemperature : IConsoleCommand
	{
		// Token: 0x170007B1 RID: 1969
		// (get) Token: 0x06002EF2 RID: 12018 RVA: 0x000F37F9 File Offset: 0x000F19F9
		public string Command
		{
			get
			{
				return "setsolutiontemperature";
			}
		}

		// Token: 0x170007B2 RID: 1970
		// (get) Token: 0x06002EF3 RID: 12019 RVA: 0x000F3800 File Offset: 0x000F1A00
		public string Description
		{
			get
			{
				return "Set the temperature of some solution.";
			}
		}

		// Token: 0x170007B3 RID: 1971
		// (get) Token: 0x06002EF4 RID: 12020 RVA: 0x000F3807 File Offset: 0x000F1A07
		public string Help
		{
			get
			{
				return "Usage: " + this.Command + " <target> <solution> <new temperature>";
			}
		}

		// Token: 0x06002EF5 RID: 12021 RVA: 0x000F3820 File Offset: 0x000F1A20
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
				shell.WriteLine("Failed to parse new temperature.");
				return;
			}
			if (quantity <= 0f)
			{
				shell.WriteLine("Cannot set the temperature of a solution to a non-positive number.");
				return;
			}
			EntitySystem.Get<SolutionContainerSystem>().SetTemperature(uid, solution, quantity);
		}
	}
}
