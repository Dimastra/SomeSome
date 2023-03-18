using System;
using Content.Server.Shuttles.Systems;
using Content.Shared.Alert;
using Content.Shared.Shuttles.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Alert.Click
{
	// Token: 0x020007EC RID: 2028
	[DataDefinition]
	public sealed class StopPiloting : IAlertClick
	{
		// Token: 0x06002BF2 RID: 11250 RVA: 0x000E6480 File Offset: 0x000E4680
		public void AlertClicked(EntityUid player)
		{
			IEntityManager entManager = IoCManager.Resolve<IEntityManager>();
			PilotComponent pilotComponent;
			if (entManager.TryGetComponent<PilotComponent>(player, ref pilotComponent) && pilotComponent.Console != null)
			{
				entManager.System<ShuttleConsoleSystem>().RemovePilot(pilotComponent);
			}
		}
	}
}
