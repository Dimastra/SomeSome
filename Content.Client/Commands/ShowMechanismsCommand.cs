using System;
using System.Runtime.CompilerServices;
using Content.Shared.Body.Organ;
using Robust.Client.Console;
using Robust.Client.GameObjects;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Commands
{
	// Token: 0x020003AE RID: 942
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class ShowMechanismsCommand : IConsoleCommand
	{
		// Token: 0x170004CB RID: 1227
		// (get) Token: 0x0600176A RID: 5994 RVA: 0x00086B95 File Offset: 0x00084D95
		public string Command
		{
			get
			{
				return "showmechanisms";
			}
		}

		// Token: 0x170004CC RID: 1228
		// (get) Token: 0x0600176B RID: 5995 RVA: 0x00086B9C File Offset: 0x00084D9C
		public string Description
		{
			get
			{
				return "Makes mechanisms visible, even when they shouldn't be.";
			}
		}

		// Token: 0x170004CD RID: 1229
		// (get) Token: 0x0600176C RID: 5996 RVA: 0x00086BA3 File Offset: 0x00084DA3
		public string Help
		{
			get
			{
				return this.Command ?? "";
			}
		}

		// Token: 0x0600176D RID: 5997 RVA: 0x00086BB4 File Offset: 0x00084DB4
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			foreach (OrganComponent organComponent in entityManager.EntityQuery<OrganComponent>(true))
			{
				SpriteComponent spriteComponent;
				if (entityManager.TryGetComponent<SpriteComponent>(organComponent.Owner, ref spriteComponent))
				{
					spriteComponent.ContainerOccluded = false;
				}
			}
			IoCManager.Resolve<IClientConsoleHost>().ExecuteCommand("showcontainedcontext");
		}

		// Token: 0x04000BFE RID: 3070
		public const string CommandName = "showmechanisms";
	}
}
