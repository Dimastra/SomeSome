using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration;
using Content.Server.Mind.Components;
using Content.Shared.Administration;
using Content.Shared.Emoting;
using Content.Shared.Examine;
using Content.Shared.Movement.Components;
using Content.Shared.Speech;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Mind.Commands
{
	// Token: 0x020003AA RID: 938
	[NullableContext(1)]
	[Nullable(0)]
	[AdminCommand(AdminFlags.Admin)]
	public sealed class MakeSentientCommand : IConsoleCommand
	{
		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x06001332 RID: 4914 RVA: 0x00062F8D File Offset: 0x0006118D
		public string Command
		{
			get
			{
				return "makesentient";
			}
		}

		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x06001333 RID: 4915 RVA: 0x00062F94 File Offset: 0x00061194
		public string Description
		{
			get
			{
				return "Makes an entity sentient (able to be controlled by a player)";
			}
		}

		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x06001334 RID: 4916 RVA: 0x00062F9B File Offset: 0x0006119B
		public string Help
		{
			get
			{
				return "makesentient <entity id>";
			}
		}

		// Token: 0x06001335 RID: 4917 RVA: 0x00062FA4 File Offset: 0x000611A4
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteLine("Wrong number of arguments.");
				return;
			}
			EntityUid entId;
			if (!EntityUid.TryParse(args[0], ref entId))
			{
				shell.WriteLine("Invalid argument.");
				return;
			}
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			if (!entityManager.EntityExists(entId))
			{
				shell.WriteLine("Invalid entity specified!");
				return;
			}
			MakeSentientCommand.MakeSentient(entId, entityManager, true, true);
		}

		// Token: 0x06001336 RID: 4918 RVA: 0x00063004 File Offset: 0x00061204
		public static void MakeSentient(EntityUid uid, IEntityManager entityManager, bool allowMovement = true, bool allowSpeech = true)
		{
			entityManager.EnsureComponent<MindComponent>(uid);
			if (allowMovement)
			{
				entityManager.EnsureComponent<InputMoverComponent>(uid);
				entityManager.EnsureComponent<MobMoverComponent>(uid);
				entityManager.EnsureComponent<MovementSpeedModifierComponent>(uid);
			}
			if (allowSpeech)
			{
				entityManager.EnsureComponent<SpeechComponent>(uid);
				entityManager.EnsureComponent<EmotingComponent>(uid);
			}
			entityManager.EnsureComponent<ExaminerComponent>(uid);
		}
	}
}
