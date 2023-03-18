using System;
using Content.Server.Cuffs.Components;
using Content.Shared.Alert;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Alert.Click
{
	// Token: 0x020007E7 RID: 2023
	[DataDefinition]
	public sealed class RemoveCuffs : IAlertClick
	{
		// Token: 0x06002BE8 RID: 11240 RVA: 0x000E6300 File Offset: 0x000E4500
		public void AlertClicked(EntityUid player)
		{
			CuffableComponent cuffableComponent;
			if (IoCManager.Resolve<IEntityManager>().TryGetComponent<CuffableComponent>(player, ref cuffableComponent))
			{
				cuffableComponent.TryUncuff(player, null);
			}
		}
	}
}
