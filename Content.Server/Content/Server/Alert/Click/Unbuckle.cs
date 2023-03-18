using System;
using Content.Server.Buckle.Systems;
using Content.Shared.Alert;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Alert.Click
{
	// Token: 0x020007EF RID: 2031
	[DataDefinition]
	public sealed class Unbuckle : IAlertClick
	{
		// Token: 0x06002BF8 RID: 11256 RVA: 0x000E6532 File Offset: 0x000E4732
		public void AlertClicked(EntityUid player)
		{
			IoCManager.Resolve<IEntityManager>().System<BuckleSystem>().TryUnbuckle(player, player, false, null);
		}
	}
}
