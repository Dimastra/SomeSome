using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.NPC.HTN;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.NPC.Commands
{
	// Token: 0x02000374 RID: 884
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class AddNPCCommand : IConsoleCommand
	{
		// Token: 0x17000282 RID: 642
		// (get) Token: 0x06001212 RID: 4626 RVA: 0x0005EB84 File Offset: 0x0005CD84
		public string Command
		{
			get
			{
				return "addnpc";
			}
		}

		// Token: 0x17000283 RID: 643
		// (get) Token: 0x06001213 RID: 4627 RVA: 0x0005EB8B File Offset: 0x0005CD8B
		public string Description
		{
			get
			{
				return "Add a HTN NPC component with a given root task";
			}
		}

		// Token: 0x17000284 RID: 644
		// (get) Token: 0x06001214 RID: 4628 RVA: 0x0005EB92 File Offset: 0x0005CD92
		public string Help
		{
			get
			{
				return "Usage: addnpc <entityId> <rootTask>\n    entityID: Uid of entity to add the AiControllerComponent to. Open its VV menu to find this.\n    rootTask: Name of a behaviorset to add to the component on initialize.";
			}
		}

		// Token: 0x06001215 RID: 4629 RVA: 0x0005EB9C File Offset: 0x0005CD9C
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 2)
			{
				shell.WriteError("Wrong number of args.");
				return;
			}
			EntityUid entId;
			entId..ctor(int.Parse(args[0]));
			if (!this._entities.EntityExists(entId))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Unable to find entity with uid ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entId);
				shell.WriteError(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			if (this._entities.HasComponent<HTNComponent>(entId))
			{
				shell.WriteError("Entity already has an NPC component.");
				return;
			}
			this._entities.AddComponent<HTNComponent>(entId).RootTask = args[1];
			shell.WriteLine("AI component added.");
		}

		// Token: 0x04000B2F RID: 2863
		[Dependency]
		private readonly IEntityManager _entities;
	}
}
