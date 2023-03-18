using System;
using Content.Shared.Alert;
using Content.Shared.Pulling;
using Content.Shared.Pulling.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Alert.Click
{
	// Token: 0x020007ED RID: 2029
	[DataDefinition]
	public sealed class StopPulling : IAlertClick
	{
		// Token: 0x06002BF4 RID: 11252 RVA: 0x000E64BC File Offset: 0x000E46BC
		public void AlertClicked(EntityUid player)
		{
			IEntityManager entManager = IoCManager.Resolve<IEntityManager>();
			SharedPullingSystem ps = entManager.System<SharedPullingSystem>();
			EntityUid playerTarget = ps.GetPulled(player);
			SharedPullableComponent playerPullable;
			if (playerTarget != default(EntityUid) && entManager.TryGetComponent<SharedPullableComponent>(playerTarget, ref playerPullable))
			{
				ps.TryStopPull(playerPullable, null);
			}
		}
	}
}
