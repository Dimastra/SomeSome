using System;
using System.Runtime.CompilerServices;
using Content.Shared.Alert;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Server.Alert
{
	// Token: 0x020007E3 RID: 2019
	internal sealed class ServerAlertsSystem : AlertsSystem
	{
		// Token: 0x06002BD9 RID: 11225 RVA: 0x000E608F File Offset: 0x000E428F
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AlertsComponent, PlayerAttachedEvent>(new ComponentEventHandler<AlertsComponent, PlayerAttachedEvent>(this.OnPlayerAttached), null, null);
		}

		// Token: 0x06002BDA RID: 11226 RVA: 0x000E60AB File Offset: 0x000E42AB
		[NullableContext(1)]
		private void OnPlayerAttached(EntityUid uid, AlertsComponent component, PlayerAttachedEvent args)
		{
			base.Dirty(component, null);
		}
	}
}
