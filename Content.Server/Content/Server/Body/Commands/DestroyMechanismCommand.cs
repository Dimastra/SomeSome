using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Body.Systems;
using Content.Shared.Administration;
using Content.Shared.Body.Components;
using Content.Shared.Body.Organ;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.Body.Commands
{
	// Token: 0x0200071C RID: 1820
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	internal sealed class DestroyMechanismCommand : IConsoleCommand
	{
		// Token: 0x170005B1 RID: 1457
		// (get) Token: 0x06002649 RID: 9801 RVA: 0x000CA170 File Offset: 0x000C8370
		public string Command
		{
			get
			{
				return "destroymechanism";
			}
		}

		// Token: 0x170005B2 RID: 1458
		// (get) Token: 0x0600264A RID: 9802 RVA: 0x000CA177 File Offset: 0x000C8377
		public string Description
		{
			get
			{
				return "Destroys a mechanism from your entity";
			}
		}

		// Token: 0x170005B3 RID: 1459
		// (get) Token: 0x0600264B RID: 9803 RVA: 0x000CA17E File Offset: 0x000C837E
		public string Help
		{
			get
			{
				return "Usage: " + this.Command + " <mechanism>";
			}
		}

		// Token: 0x0600264C RID: 9804 RVA: 0x000CA198 File Offset: 0x000C8398
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteLine("Only a player can run this command.");
				return;
			}
			if (args.Length == 0)
			{
				shell.WriteLine(this.Help);
				return;
			}
			EntityUid? attachedEntity = player.AttachedEntity;
			if (attachedEntity == null)
			{
				shell.WriteLine("You have no entity.");
				return;
			}
			EntityUid attached = attachedEntity.GetValueOrDefault();
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			IComponentFactory fac = IoCManager.Resolve<IComponentFactory>();
			BodyComponent body;
			if (!entityManager.TryGetComponent<BodyComponent>(attached, ref body))
			{
				IRobustRandom random = IoCManager.Resolve<IRobustRandom>();
				string text = "You have no body" + (RandomExtensions.Prob(random, 0.2f) ? " and you must scream." : ".");
				shell.WriteLine(text);
				return;
			}
			string mechanismName = string.Join(" ", args).ToLowerInvariant();
			BodySystem bodySystem = entityManager.System<BodySystem>();
			foreach (ValueTuple<EntityUid, OrganComponent> organ in bodySystem.GetBodyOrgans(new EntityUid?(body.Owner), body))
			{
				if (fac.GetComponentName(organ.Item2.GetType()).ToLowerInvariant() == mechanismName)
				{
					bodySystem.DeleteOrgan(new EntityUid?(organ.Item1), organ.Item2);
					shell.WriteLine("Mechanism with name " + mechanismName + " has been destroyed.");
					return;
				}
			}
			shell.WriteLine("No mechanism was found with name " + mechanismName + ".");
		}
	}
}
