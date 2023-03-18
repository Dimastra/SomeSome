using System;
using Content.Shared.ActionBlocker;
using Content.Shared.Alert;
using Content.Shared.Pulling;
using Content.Shared.Pulling.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Alert.Click
{
	// Token: 0x020007EB RID: 2027
	[DataDefinition]
	public sealed class StopBeingPulled : IAlertClick
	{
		// Token: 0x06002BF0 RID: 11248 RVA: 0x000E642C File Offset: 0x000E462C
		public void AlertClicked(EntityUid player)
		{
			IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
			if (!entityManager.System<ActionBlockerSystem>().CanInteract(player, null))
			{
				return;
			}
			SharedPullableComponent playerPullable;
			if (entityManager.TryGetComponent<SharedPullableComponent>(player, ref playerPullable))
			{
				entityManager.System<SharedPullingSystem>().TryStopPull(playerPullable, null);
			}
		}
	}
}
