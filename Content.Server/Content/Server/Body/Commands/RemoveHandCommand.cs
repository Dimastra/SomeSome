using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Body.Systems;
using Content.Shared.Administration;
using Content.Shared.Body.Components;
using Content.Shared.Body.Part;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Server.Body.Commands
{
	// Token: 0x0200071D RID: 1821
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	internal sealed class RemoveHandCommand : IConsoleCommand
	{
		// Token: 0x170005B4 RID: 1460
		// (get) Token: 0x0600264E RID: 9806 RVA: 0x000CA320 File Offset: 0x000C8520
		public string Command
		{
			get
			{
				return "removehand";
			}
		}

		// Token: 0x170005B5 RID: 1461
		// (get) Token: 0x0600264F RID: 9807 RVA: 0x000CA327 File Offset: 0x000C8527
		public string Description
		{
			get
			{
				return "Removes a hand from your entity.";
			}
		}

		// Token: 0x170005B6 RID: 1462
		// (get) Token: 0x06002650 RID: 9808 RVA: 0x000CA32E File Offset: 0x000C852E
		public string Help
		{
			get
			{
				return "Usage: " + this.Command;
			}
		}

		// Token: 0x06002651 RID: 9809 RVA: 0x000CA340 File Offset: 0x000C8540
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			if (player == null)
			{
				shell.WriteLine("Only a player can run this command.");
				return;
			}
			if (player.AttachedEntity == null)
			{
				shell.WriteLine("You have no entity.");
				return;
			}
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			BodyComponent body;
			if (!entityManager.TryGetComponent<BodyComponent>(player.AttachedEntity, ref body))
			{
				IRobustRandom random = IoCManager.Resolve<IRobustRandom>();
				string text = "You have no body" + (RandomExtensions.Prob(random, 0.2f) ? " and you must scream." : ".");
				shell.WriteLine(text);
				return;
			}
			BodySystem bodySystem = entityManager.System<BodySystem>();
			ValueTuple<EntityUid, BodyPartComponent> hand = bodySystem.GetBodyChildrenOfType(player.AttachedEntity, BodyPartType.Hand, body).FirstOrDefault<ValueTuple<EntityUid, BodyPartComponent>>();
			ValueTuple<EntityUid, BodyPartComponent> valueTuple = hand;
			EntityUid item = valueTuple.Item1;
			BodyPartComponent item2 = valueTuple.Item2;
			if (item == default(EntityUid) && item2 == null)
			{
				shell.WriteLine("You have no hands.");
				return;
			}
			bodySystem.DropPart(new EntityUid?(hand.Item1), hand.Item2);
		}
	}
}
