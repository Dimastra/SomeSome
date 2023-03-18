using System;
using Content.Server.Body.Systems;
using Content.Shared.Alert;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Alert.Click
{
	// Token: 0x020007EE RID: 2030
	[DataDefinition]
	public sealed class ToggleInternals : IAlertClick
	{
		// Token: 0x06002BF6 RID: 11254 RVA: 0x000E6515 File Offset: 0x000E4715
		public void AlertClicked(EntityUid player)
		{
			IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<InternalsSystem>().ToggleInternals(player, player, false, null);
		}
	}
}
