using System;
using System.Runtime.CompilerServices;
using Content.Server.Explosion.EntitySystems;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.GhostKick
{
	// Token: 0x0200048E RID: 1166
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GhostKickUserOnTriggerSystem : EntitySystem
	{
		// Token: 0x0600175C RID: 5980 RVA: 0x0007A7F9 File Offset: 0x000789F9
		public override void Initialize()
		{
			base.SubscribeLocalEvent<GhostKickUserOnTriggerComponent, TriggerEvent>(new ComponentEventHandler<GhostKickUserOnTriggerComponent, TriggerEvent>(this.HandleMineTriggered), null, null);
		}

		// Token: 0x0600175D RID: 5981 RVA: 0x0007A810 File Offset: 0x00078A10
		private void HandleMineTriggered(EntityUid uid, GhostKickUserOnTriggerComponent userOnTriggerComponent, TriggerEvent args)
		{
			ActorComponent actor;
			if (!base.TryComp<ActorComponent>(args.User, ref actor))
			{
				return;
			}
			this._ghostKickManager.DoDisconnect(actor.PlayerSession.ConnectedClient, "Tripped over a kick mine, crashed through the fourth wall");
			args.Handled = true;
		}

		// Token: 0x04000E93 RID: 3731
		[Dependency]
		private readonly GhostKickManager _ghostKickManager;
	}
}
