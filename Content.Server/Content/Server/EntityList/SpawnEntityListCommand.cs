using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.EntityList;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Server.EntityList
{
	// Token: 0x02000529 RID: 1321
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Spawn)]
	public sealed class SpawnEntityListCommand : IConsoleCommand
	{
		// Token: 0x1700040C RID: 1036
		// (get) Token: 0x06001B7F RID: 7039 RVA: 0x00093347 File Offset: 0x00091547
		public string Command
		{
			get
			{
				return "spawnentitylist";
			}
		}

		// Token: 0x1700040D RID: 1037
		// (get) Token: 0x06001B80 RID: 7040 RVA: 0x0009334E File Offset: 0x0009154E
		public string Description
		{
			get
			{
				return "Spawns a list of entities around you";
			}
		}

		// Token: 0x1700040E RID: 1038
		// (get) Token: 0x06001B81 RID: 7041 RVA: 0x00093355 File Offset: 0x00091555
		public string Help
		{
			get
			{
				return "Usage: " + this.Command + " <entityListPrototypeId>";
			}
		}

		// Token: 0x06001B82 RID: 7042 RVA: 0x0009336C File Offset: 0x0009156C
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteError("Invalid arguments.\n" + this.Help);
				return;
			}
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteError("You must be a player to run this command.");
				return;
			}
			EntityUid? attachedEntity = player.AttachedEntity;
			if (attachedEntity == null)
			{
				shell.WriteError("You must have an entity to run this command.");
				return;
			}
			EntityUid attached = attachedEntity.GetValueOrDefault();
			IPrototypeManager prototypeManager = IoCManager.Resolve<IPrototypeManager>();
			EntityListPrototype prototype;
			if (!prototypeManager.TryIndex<EntityListPrototype>(args[0], ref prototype))
			{
				shell.WriteError("No EntityListPrototype found with id " + args[0]);
				return;
			}
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			int i = 0;
			foreach (EntityPrototype entity in prototype.Entities(prototypeManager))
			{
				entityManager.SpawnEntity(entity.ID, entityManager.GetComponent<TransformComponent>(attached).Coordinates);
				i++;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Spawned ");
			defaultInterpolatedStringHandler.AppendFormatted<int>(i);
			defaultInterpolatedStringHandler.AppendLiteral(" entities.");
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
