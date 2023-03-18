using System;
using System.Runtime.CompilerServices;
using Content.Server.Chemistry.Components.SolutionManager;
using Content.Server.Chemistry.EntitySystems;
using Content.Shared.Administration;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Server.Administration.Commands
{
	// Token: 0x0200082F RID: 2095
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class AddReagent : IConsoleCommand
	{
		// Token: 0x17000719 RID: 1817
		// (get) Token: 0x06002DE0 RID: 11744 RVA: 0x000F016C File Offset: 0x000EE36C
		public string Command
		{
			get
			{
				return "addreagent";
			}
		}

		// Token: 0x1700071A RID: 1818
		// (get) Token: 0x06002DE1 RID: 11745 RVA: 0x000F0173 File Offset: 0x000EE373
		public string Description
		{
			get
			{
				return "Add (or remove) some amount of reagent from some solution.";
			}
		}

		// Token: 0x1700071B RID: 1819
		// (get) Token: 0x06002DE2 RID: 11746 RVA: 0x000F017A File Offset: 0x000EE37A
		public string Help
		{
			get
			{
				return "Usage: " + this.Command + " <target> <solution> <reagent> <quantity>";
			}
		}

		// Token: 0x06002DE3 RID: 11747 RVA: 0x000F0194 File Offset: 0x000EE394
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length < 4)
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
			if (!IoCManager.Resolve<IPrototypeManager>().HasIndex<ReagentPrototype>(args[2]))
			{
				shell.WriteLine("Unknown reagent prototype");
				return;
			}
			float quantityFloat;
			if (!float.TryParse(args[3], out quantityFloat))
			{
				shell.WriteLine("Failed to parse quantity");
				return;
			}
			FixedPoint2 quantity = FixedPoint2.New(MathF.Abs(quantityFloat));
			if (quantityFloat > 0f)
			{
				FixedPoint2 fixedPoint;
				EntitySystem.Get<SolutionContainerSystem>().TryAddReagent(uid, solution, args[2], quantity, out fixedPoint, null);
				return;
			}
			EntitySystem.Get<SolutionContainerSystem>().TryRemoveReagent(uid, solution, args[2], quantity);
		}
	}
}
