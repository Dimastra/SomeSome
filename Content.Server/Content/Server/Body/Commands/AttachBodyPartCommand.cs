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

namespace Content.Server.Body.Commands
{
	// Token: 0x0200071B RID: 1819
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Fun)]
	public sealed class AttachBodyPartCommand : IConsoleCommand
	{
		// Token: 0x170005AE RID: 1454
		// (get) Token: 0x06002644 RID: 9796 RVA: 0x000C9CFC File Offset: 0x000C7EFC
		public string Command
		{
			get
			{
				return "attachbodypart";
			}
		}

		// Token: 0x170005AF RID: 1455
		// (get) Token: 0x06002645 RID: 9797 RVA: 0x000C9D03 File Offset: 0x000C7F03
		public string Description
		{
			get
			{
				return "Attaches a body part to you or someone else.";
			}
		}

		// Token: 0x170005B0 RID: 1456
		// (get) Token: 0x06002646 RID: 9798 RVA: 0x000C9D0A File Offset: 0x000C7F0A
		public string Help
		{
			get
			{
				return this.Command + " <partEntityUid> / " + this.Command + " <entityUid> <partEntityUid>";
			}
		}

		// Token: 0x06002647 RID: 9799 RVA: 0x000C9D28 File Offset: 0x000C7F28
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IPlayerSession player = shell.Player as IPlayerSession;
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			int num = args.Length;
			EntityUid partUid;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler;
			EntityUid bodyId;
			if (num != 1)
			{
				if (num != 2)
				{
					shell.WriteLine(this.Help);
					return;
				}
				EntityUid entityUid;
				if (!EntityUid.TryParse(args[0], ref entityUid))
				{
					shell.WriteLine(args[0] + " is not a valid entity uid.");
					return;
				}
				if (!EntityUid.TryParse(args[1], ref partUid))
				{
					shell.WriteLine(args[1] + " is not a valid entity uid.");
					return;
				}
				if (!entityManager.EntityExists(entityUid))
				{
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 1);
					defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(entityUid);
					defaultInterpolatedStringHandler.AppendLiteral(" is not a valid entity.");
					shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
					return;
				}
				bodyId = entityUid;
			}
			else
			{
				if (player == null)
				{
					shell.WriteLine("You need to specify an entity to attach the part to if you aren't a player.\n" + this.Help);
					return;
				}
				if (player.AttachedEntity == null)
				{
					shell.WriteLine("You need to specify an entity to attach the part to if you aren't attached to an entity.\n" + this.Help);
					return;
				}
				if (!EntityUid.TryParse(args[0], ref partUid))
				{
					shell.WriteLine(args[0] + " is not a valid entity uid.");
					return;
				}
				bodyId = player.AttachedEntity.Value;
			}
			BodyComponent body;
			if (!entityManager.TryGetComponent<BodyComponent>(bodyId, ref body))
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 3);
				defaultInterpolatedStringHandler.AppendLiteral("Entity ");
				defaultInterpolatedStringHandler.AppendFormatted(entityManager.GetComponent<MetaDataComponent>(bodyId).EntityName);
				defaultInterpolatedStringHandler.AppendLiteral(" with uid ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(bodyId);
				defaultInterpolatedStringHandler.AppendLiteral(" does not have a ");
				defaultInterpolatedStringHandler.AppendFormatted("BodyComponent");
				defaultInterpolatedStringHandler.AppendLiteral(".");
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			if (!entityManager.EntityExists(partUid))
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 1);
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(partUid);
				defaultInterpolatedStringHandler.AppendLiteral(" is not a valid entity.");
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			BodyPartComponent part;
			if (!entityManager.TryGetComponent<BodyPartComponent>(partUid, ref part))
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 3);
				defaultInterpolatedStringHandler.AppendLiteral("Entity ");
				defaultInterpolatedStringHandler.AppendFormatted(entityManager.GetComponent<MetaDataComponent>(partUid).EntityName);
				defaultInterpolatedStringHandler.AppendLiteral(" with uid ");
				defaultInterpolatedStringHandler.AppendFormatted(args[0]);
				defaultInterpolatedStringHandler.AppendLiteral(" does not have a ");
				defaultInterpolatedStringHandler.AppendFormatted("BodyPartComponent");
				defaultInterpolatedStringHandler.AppendLiteral(".");
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			BodySystem bodySystem = entityManager.System<BodySystem>();
			if (bodySystem.BodyHasChild(new EntityUid?(bodyId), new EntityUid?(partUid), body, part))
			{
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(61, 4);
				defaultInterpolatedStringHandler.AppendLiteral("Body part ");
				defaultInterpolatedStringHandler.AppendFormatted(entityManager.GetComponent<MetaDataComponent>(partUid).EntityName);
				defaultInterpolatedStringHandler.AppendLiteral(" with uid ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(partUid);
				defaultInterpolatedStringHandler.AppendLiteral(" is already attached to entity ");
				defaultInterpolatedStringHandler.AppendFormatted(entityManager.GetComponent<MetaDataComponent>(bodyId).EntityName);
				defaultInterpolatedStringHandler.AppendLiteral(" with uid ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(bodyId);
				shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 1);
			defaultInterpolatedStringHandler.AppendLiteral("AttachBodyPartVerb-");
			defaultInterpolatedStringHandler.AppendFormatted<EntityUid>(partUid);
			string slotId = defaultInterpolatedStringHandler.ToStringAndClear();
			BodyPartSlot rootSlot;
			if (bodySystem.TryCreateBodyRootSlot(new EntityUid?(bodyId), slotId, out rootSlot, body))
			{
				bodySystem.DropPart(new EntityUid?(partUid), part);
				bodySystem.AttachPart(new EntityUid?(partUid), rootSlot, part);
			}
			else
			{
				ValueTuple<EntityUid, BodyPartComponent> attachAt = bodySystem.GetBodyChildren(new EntityUid?(bodyId), body).First<ValueTuple<EntityUid, BodyPartComponent>>();
				if (!bodySystem.TryCreatePartSlotAndAttach(new EntityUid?(attachAt.Item1), slotId, new EntityUid?(partUid), attachAt.Item2, part))
				{
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 2);
					defaultInterpolatedStringHandler.AppendLiteral("Could not create slot ");
					defaultInterpolatedStringHandler.AppendFormatted(slotId);
					defaultInterpolatedStringHandler.AppendLiteral(" on entity ");
					defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(entityManager.ToPrettyString(bodyId));
					shell.WriteError(defaultInterpolatedStringHandler.ToStringAndClear());
					return;
				}
			}
			defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Attached part ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(entityManager.ToPrettyString(partUid));
			defaultInterpolatedStringHandler.AppendLiteral(" to ");
			defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(entityManager.ToPrettyString(bodyId));
			shell.WriteLine(defaultInterpolatedStringHandler.ToStringAndClear());
		}
	}
}
