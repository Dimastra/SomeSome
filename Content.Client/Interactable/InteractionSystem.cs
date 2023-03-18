using System;
using Content.Client.Storage;
using Content.Shared.Interaction;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;

namespace Content.Client.Interactable
{
	// Token: 0x020002A8 RID: 680
	public sealed class InteractionSystem : SharedInteractionSystem
	{
		// Token: 0x0600113B RID: 4411 RVA: 0x0006673C File Offset: 0x0006493C
		public override bool CanAccessViaStorage(EntityUid user, EntityUid target)
		{
			IContainer container;
			ClientStorageComponent clientStorageComponent;
			return this.EntityManager.EntityExists(target) && ContainerHelpers.TryGetContainer(target, ref container, null) && base.TryComp<ClientStorageComponent>(container.Owner, ref clientStorageComponent);
		}
	}
}
