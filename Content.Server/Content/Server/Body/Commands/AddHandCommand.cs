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
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Body.Commands
{
	// Token: 0x0200071A RID: 1818
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	internal sealed class AddHandCommand : IConsoleCommand
	{
		// Token: 0x170005AB RID: 1451
		// (get) Token: 0x0600263F RID: 9791 RVA: 0x000C98A7 File Offset: 0x000C7AA7
		public string Command
		{
			get
			{
				return "addhand";
			}
		}

		// Token: 0x170005AC RID: 1452
		// (get) Token: 0x06002640 RID: 9792 RVA: 0x000C98AE File Offset: 0x000C7AAE
		public string Description
		{
			get
			{
				return "Adds a hand to your entity.";
			}
		}

		// Token: 0x170005AD RID: 1453
		// (get) Token: 0x06002641 RID: 9793 RVA: 0x000C98B8 File Offset: 0x000C7AB8
		public string Help
		{
			get
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(76, 4);
				defaultInterpolatedStringHandler.AppendLiteral("Usage: ");
				defaultInterpolatedStringHandler.AppendFormatted(this.Command);
				defaultInterpolatedStringHandler.AppendLiteral(" <entityUid> <handPrototypeId> / ");
				defaultInterpolatedStringHandler.AppendFormatted(this.Command);
				defaultInterpolatedStringHandler.AppendLiteral(" <entityUid> / ");
				defaultInterpolatedStringHandler.AppendFormatted(this.Command);
				defaultInterpolatedStringHandler.AppendLiteral(" <handPrototypeId> / ");
				defaultInterpolatedStringHandler.AppendFormatted(this.Command);
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}
		}

		// Token: 0x06002642 RID: 9794 RVA: 0x000C993C File Offset: 0x000C7B3C
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			IPrototypeManager prototypeManager = IoCManager.Resolve<IPrototypeManager>();
			EntityUid entity;
			EntityUid hand;
			switch (args.Length)
			{
			case 0:
				if (player == null)
				{
					shell.WriteLine("Only a player can run this command without arguments.");
					return;
				}
				if (player.AttachedEntity == null)
				{
					shell.WriteLine("You don't have an entity to add a hand to.");
					return;
				}
				entity = player.AttachedEntity.Value;
				hand = entityManager.SpawnEntity("LeftHandHuman", entityManager.GetComponent<TransformComponent>(entity).Coordinates);
				break;
			case 1:
			{
				EntityUid uid;
				if (EntityUid.TryParse(args[0], ref uid))
				{
					if (!entityManager.EntityExists(uid))
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 1);
						defaultInterpolatedStringHandler.AppendLiteral("No entity found with uid ");
						defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid);
						shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
						return;
					}
					entity = uid;
					hand = entityManager.SpawnEntity("LeftHandHuman", entityManager.GetComponent<TransformComponent>(entity).Coordinates);
				}
				else
				{
					if (player == null)
					{
						shell.WriteLine("You must specify an entity to add a hand to when using this command from the server terminal.");
						return;
					}
					if (player.AttachedEntity == null)
					{
						shell.WriteLine("You don't have an entity to add a hand to.");
						return;
					}
					entity = player.AttachedEntity.Value;
					hand = entityManager.SpawnEntity(args[0], entityManager.GetComponent<TransformComponent>(entity).Coordinates);
				}
				break;
			}
			case 2:
			{
				EntityUid uid2;
				if (!EntityUid.TryParse(args[0], ref uid2))
				{
					shell.WriteLine(args[0] + " is not a valid entity uid.");
					return;
				}
				if (!entityManager.EntityExists(uid2))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
					defaultInterpolatedStringHandler.AppendLiteral("No entity exists with uid ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(uid2);
					defaultInterpolatedStringHandler.AppendLiteral(".");
					shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
					return;
				}
				entity = uid2;
				if (!prototypeManager.HasIndex<EntityPrototype>(args[1]))
				{
					shell.WriteLine("No hand entity exists with id " + args[1] + ".");
					return;
				}
				hand = entityManager.SpawnEntity(args[1], entityManager.GetComponent<TransformComponent>(entity).Coordinates);
				break;
			}
			default:
				shell.WriteLine(this.Help);
				return;
			}
			BodyComponent body;
			if (!entityManager.TryGetComponent<BodyComponent>(entity, ref body) || body.Root == null)
			{
				IRobustRandom random = IoCManager.Resolve<IRobustRandom>();
				string text = "You have no body" + (RandomExtensions.Prob(random, 0.2f) ? " and you must scream." : ".");
				shell.WriteLine(text);
				return;
			}
			BodyPartComponent part;
			if (!entityManager.TryGetComponent<BodyPartComponent>(hand, ref part))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Hand entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(hand);
				defaultInterpolatedStringHandler.AppendLiteral(" does not have a ");
				defaultInterpolatedStringHandler.AppendFormatted("BodyPartComponent");
				defaultInterpolatedStringHandler.AppendLiteral(" component.");
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			BodySystem bodySystem = entityManager.System<BodySystem>();
			ValueTuple<EntityUid, BodyPartComponent> attachAt = bodySystem.GetBodyChildrenOfType(new EntityUid?(entity), BodyPartType.Arm, body).FirstOrDefault<ValueTuple<EntityUid, BodyPartComponent>>();
			ValueTuple<EntityUid, BodyPartComponent> valueTuple = attachAt;
			EntityUid item = valueTuple.Item1;
			BodyPartComponent item2 = valueTuple.Item2;
			if (item == default(EntityUid) && item2 == null)
			{
				attachAt = bodySystem.GetBodyChildren(new EntityUid?(entity), body).First<ValueTuple<EntityUid, BodyPartComponent>>();
			}
			string slotId = part.GetHashCode().ToString();
			if (!bodySystem.TryCreatePartSlotAndAttach(new EntityUid?(attachAt.Item1), slotId, new EntityUid?(hand), attachAt.Item2, part))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(42, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Couldn't create a slot with id ");
				defaultInterpolatedStringHandler.AppendFormatted(slotId);
				defaultInterpolatedStringHandler.AppendLiteral(" on entity ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(entityManager.ToPrettyString(entity));
				shell.WriteError(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			shell.WriteLine("Added hand to entity " + entityManager.GetComponent<MetaDataComponent>(entity).EntityName);
		}

		// Token: 0x040017C8 RID: 6088
		public const string DefaultHandPrototype = "LeftHandHuman";
	}
}
