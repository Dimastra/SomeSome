using System;
using System.Runtime.CompilerServices;
using Content.Shared.Body.Organ;
using Robust.Client.Console;
using Robust.Client.GameObjects;
using Robust.Shared.Console;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Commands
{
	// Token: 0x020003AA RID: 938
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class HideMechanismsCommand : IConsoleCommand
	{
		// Token: 0x170004BF RID: 1215
		// (get) Token: 0x06001755 RID: 5973 RVA: 0x0008690A File Offset: 0x00084B0A
		public string Command
		{
			get
			{
				return "hidemechanisms";
			}
		}

		// Token: 0x170004C0 RID: 1216
		// (get) Token: 0x06001756 RID: 5974 RVA: 0x00086911 File Offset: 0x00084B11
		public string Description
		{
			get
			{
				return "Reverts the effects of showmechanisms";
			}
		}

		// Token: 0x170004C1 RID: 1217
		// (get) Token: 0x06001757 RID: 5975 RVA: 0x00086918 File Offset: 0x00084B18
		public string Help
		{
			get
			{
				return this.Command ?? "";
			}
		}

		// Token: 0x06001758 RID: 5976 RVA: 0x0008692C File Offset: 0x00084B2C
		public void Execute(IConsoleShell shell, string argStr, string[] args)
		{
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			foreach (OrganComponent organComponent in entityManager.EntityQuery<OrganComponent>(true))
			{
				SpriteComponent spriteComponent;
				if (entityManager.TryGetComponent<SpriteComponent>(organComponent.Owner, ref spriteComponent))
				{
					spriteComponent.ContainerOccluded = false;
					EntityUid owner = organComponent.Owner;
					IContainer container;
					while (ContainerHelpers.TryGetContainer(owner, ref container, null))
					{
						if (!container.ShowContents)
						{
							spriteComponent.ContainerOccluded = true;
							break;
						}
						owner = container.Owner;
					}
				}
			}
			IoCManager.Resolve<IClientConsoleHost>().ExecuteCommand("hidecontainedcontext");
		}
	}
}
