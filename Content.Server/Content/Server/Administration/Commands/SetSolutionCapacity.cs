using System;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Server.Chemistry.EntitySystems;
using Content.Shared.Administration;
using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Administration.Commands
{
	// Token: 0x02000862 RID: 2146
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class SetSolutionCapacity : IConsoleCommand
	{
		// Token: 0x170007AE RID: 1966
		// (get) Token: 0x06002EED RID: 12013 RVA: 0x000F36D4 File Offset: 0x000F18D4
		public string Command
		{
			get
			{
				return "setsolutioncapacity";
			}
		}

		// Token: 0x170007AF RID: 1967
		// (get) Token: 0x06002EEE RID: 12014 RVA: 0x000F36DB File Offset: 0x000F18DB
		public string Description
		{
			get
			{
				return "Set the capacity (maximum volume) of some solution.";
			}
		}

		// Token: 0x170007B0 RID: 1968
		// (get) Token: 0x06002EEF RID: 12015 RVA: 0x000F36E2 File Offset: 0x000F18E2
		public string Help
		{
			get
			{
				return "Usage: " + this.Command + " <target> <solution> <new capacity>";
			}
		}

		// Token: 0x06002EF0 RID: 12016 RVA: 0x000F36FC File Offset: 0x000F18FC
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
			float quantityFloat;
			if (!float.TryParse(args[2], out quantityFloat))
			{
				shell.WriteLine("Failed to parse new capacity.");
				return;
			}
			if (quantityFloat < 0f)
			{
				shell.WriteLine("Cannot set the maximum volume of a solution to a negative number.");
				return;
			}
			FixedPoint2 quantity = FixedPoint2.New(quantityFloat);
			EntitySystem.Get<SolutionContainerSystem>().SetCapacity(uid, solution, quantity);
		}
	}
}
