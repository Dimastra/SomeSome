using System;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Shared.Alert;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Alert.Click
{
	// Token: 0x020007E9 RID: 2025
	[DataDefinition]
	public sealed class ResistFire : IAlertClick
	{
		// Token: 0x06002BEC RID: 11244 RVA: 0x000E63C4 File Offset: 0x000E45C4
		public void AlertClicked(EntityUid player)
		{
			IEntityManager entManager = IoCManager.Resolve<IEntityManager>();
			FlammableComponent flammable;
			if (entManager.TryGetComponent<FlammableComponent>(player, ref flammable))
			{
				entManager.System<FlammableSystem>().Resist(player, flammable);
			}
		}
	}
}
