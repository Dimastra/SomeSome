using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.ParticleAccelerator.Components;
using Content.Server.Singularity.Components;
using Content.Server.Singularity.EntitySystems;
using Content.Shared.Administration;
using Content.Shared.Singularity.Components;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Singularity
{
	// Token: 0x020001E2 RID: 482
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class StartSingularityEngineCommand : IConsoleCommand
	{
		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06000923 RID: 2339 RVA: 0x0002E041 File Offset: 0x0002C241
		public string Command
		{
			get
			{
				return "startsingularityengine";
			}
		}

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x06000924 RID: 2340 RVA: 0x0002E048 File Offset: 0x0002C248
		public string Description
		{
			get
			{
				return "Automatically turns on the particle accelerator and containment field emitters.";
			}
		}

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x06000925 RID: 2341 RVA: 0x0002E04F File Offset: 0x0002C24F
		public string Help
		{
			get
			{
				return this.Command ?? "";
			}
		}

		// Token: 0x06000926 RID: 2342 RVA: 0x0002E060 File Offset: 0x0002C260
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Invalid amount of arguments: ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(args.Length);
				defaultInterpolatedStringHandler.AppendLiteral(".\n");
				defaultInterpolatedStringHandler.AppendFormatted(this.Help);
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			IEntitySystemManager entitySystemManager = IoCManager.Resolve<IEntitySystemManager>();
			foreach (EmitterComponent comp in entityManager.EntityQuery<EmitterComponent>(false))
			{
				entitySystemManager.GetEntitySystem<EmitterSystem>().SwitchOn(comp);
			}
			foreach (RadiationCollectorComponent comp2 in entityManager.EntityQuery<RadiationCollectorComponent>(false))
			{
				entitySystemManager.GetEntitySystem<RadiationCollectorSystem>().SetCollectorEnabled(comp2.Owner, true, null, comp2);
			}
			foreach (ParticleAcceleratorControlBoxComponent particleAcceleratorControlBoxComponent in entityManager.EntityQuery<ParticleAcceleratorControlBoxComponent>(false))
			{
				particleAcceleratorControlBoxComponent.RescanParts(null);
				particleAcceleratorControlBoxComponent.SetStrength(ParticleAcceleratorPowerState.Level0, null);
				particleAcceleratorControlBoxComponent.SwitchOn(null);
			}
			shell.WriteLine("Done!");
		}
	}
}
