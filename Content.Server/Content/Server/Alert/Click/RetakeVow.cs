using System;
using Content.Server.Abilities.Mime;
using Content.Shared.Alert;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Alert.Click
{
	// Token: 0x020007EA RID: 2026
	[DataDefinition]
	public sealed class RetakeVow : IAlertClick
	{
		// Token: 0x06002BEE RID: 11246 RVA: 0x000E63F8 File Offset: 0x000E45F8
		public void AlertClicked(EntityUid player)
		{
			IEntityManager entManager = IoCManager.Resolve<IEntityManager>();
			MimePowersComponent mimePowers;
			if (entManager.TryGetComponent<MimePowersComponent>(player, ref mimePowers))
			{
				entManager.System<MimePowersSystem>().RetakeVow(player, mimePowers);
			}
		}
	}
}
