using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.Movement.Components;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Server.Movement
{
	// Token: 0x0200038D RID: 909
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class RotateEyesCommand : IConsoleCommand
	{
		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x060012A8 RID: 4776 RVA: 0x00060ABF File Offset: 0x0005ECBF
		public string Command
		{
			get
			{
				return "rotateeyes";
			}
		}

		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x060012A9 RID: 4777 RVA: 0x00060AC6 File Offset: 0x0005ECC6
		public string Description
		{
			get
			{
				return Loc.GetString("rotateeyes-command-description");
			}
		}

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x060012AA RID: 4778 RVA: 0x00060AD2 File Offset: 0x0005ECD2
		public string Help
		{
			get
			{
				return Loc.GetString("rotateeyes-command-help");
			}
		}

		// Token: 0x060012AB RID: 4779 RVA: 0x00060AE0 File Offset: 0x0005ECE0
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IEntityManager entManager = IoCManager.Resolve<IEntityManager>();
			Angle rotation = Angle.Zero;
			if (args.Length == 1)
			{
				float degrees;
				if (!float.TryParse(args[0], out degrees))
				{
					shell.WriteError(Loc.GetString("parse-float-fail", new ValueTuple<string, object>[]
					{
						new ValueTuple<string, object>("arg", args[0])
					}));
					return;
				}
				rotation = Angle.FromDegrees((double)degrees);
			}
			int count = 0;
			foreach (InputMoverComponent mover in entManager.EntityQuery<InputMoverComponent>(true))
			{
				if (!mover.TargetRelativeRotation.Equals(rotation))
				{
					mover.TargetRelativeRotation = rotation;
					entManager.Dirty(mover, null);
					count++;
				}
			}
			shell.WriteLine(Loc.GetString("rotateeyes-command-count", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("count", count)
			}));
		}
	}
}
