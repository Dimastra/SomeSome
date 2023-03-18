using System;
using Content.Server.Abilities.Mime;
using Content.Shared.Alert;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Alert.Click
{
	// Token: 0x020007E6 RID: 2022
	[DataDefinition]
	public sealed class BreakVow : IAlertClick
	{
		// Token: 0x06002BE6 RID: 11238 RVA: 0x000E62CC File Offset: 0x000E44CC
		public void AlertClicked(EntityUid player)
		{
			IEntityManager entManager = IoCManager.Resolve<IEntityManager>();
			MimePowersComponent mimePowers;
			if (entManager.TryGetComponent<MimePowersComponent>(player, ref mimePowers))
			{
				entManager.System<MimePowersSystem>().BreakVow(player, mimePowers);
			}
		}
	}
}
