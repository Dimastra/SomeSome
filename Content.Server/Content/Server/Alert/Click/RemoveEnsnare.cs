using System;
using Content.Server.Ensnaring;
using Content.Shared.Alert;
using Content.Shared.Ensnaring.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Alert.Click
{
	// Token: 0x020007E8 RID: 2024
	[DataDefinition]
	public sealed class RemoveEnsnare : IAlertClick
	{
		// Token: 0x06002BEA RID: 11242 RVA: 0x000E6334 File Offset: 0x000E4534
		public void AlertClicked(EntityUid player)
		{
			IEntityManager entManager = IoCManager.Resolve<IEntityManager>();
			EnsnareableComponent ensnareableComponent;
			if (entManager.TryGetComponent<EnsnareableComponent>(player, ref ensnareableComponent))
			{
				foreach (EntityUid ensnare in ensnareableComponent.Container.ContainedEntities)
				{
					EnsnaringComponent ensnaringComponent;
					if (!entManager.TryGetComponent<EnsnaringComponent>(ensnare, ref ensnaringComponent))
					{
						break;
					}
					entManager.EntitySysManager.GetEntitySystem<EnsnareableSystem>().TryFree(player, ensnare, ensnaringComponent, null);
				}
			}
		}
	}
}
