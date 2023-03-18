using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Maps;
using Robust.Server.Console;
using Robust.Server.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Players;

namespace Content.Server.Maps
{
	// Token: 0x020003D7 RID: 983
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GridDraggingSystem : SharedGridDraggingSystem
	{
		// Token: 0x06001431 RID: 5169 RVA: 0x00068A7B File Offset: 0x00066C7B
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeNetworkEvent<GridDragRequestPosition>(new EntitySessionEventHandler<GridDragRequestPosition>(this.OnRequestDrag), null, null);
			base.SubscribeNetworkEvent<GridDragVelocityRequest>(new EntitySessionEventHandler<GridDragVelocityRequest>(this.OnRequestVelocity), null, null);
		}

		// Token: 0x06001432 RID: 5170 RVA: 0x00068AAB File Offset: 0x00066CAB
		public bool IsEnabled(ICommonSession session)
		{
			return this._draggers.Contains(session);
		}

		// Token: 0x06001433 RID: 5171 RVA: 0x00068ABC File Offset: 0x00066CBC
		public void Toggle(ICommonSession session)
		{
			if (!(session is IPlayerSession))
			{
				return;
			}
			if (!this._draggers.Add(session))
			{
				this._draggers.Remove(session);
			}
			base.RaiseNetworkEvent(new GridDragToggleMessage
			{
				Enabled = this._draggers.Contains(session)
			}, session.ConnectedClient);
		}

		// Token: 0x06001434 RID: 5172 RVA: 0x00068B10 File Offset: 0x00066D10
		private void OnRequestVelocity(GridDragVelocityRequest ev, EntitySessionEventArgs args)
		{
			IPlayerSession playerSession = args.SenderSession as IPlayerSession;
			if (playerSession == null || !this._admin.CanCommand(playerSession, "griddrag") || !base.Exists(ev.Grid) || base.Deleted(ev.Grid, null))
			{
				return;
			}
			PhysicsComponent gridBody = base.Comp<PhysicsComponent>(ev.Grid);
			this._physics.SetLinearVelocity(ev.Grid, ev.LinearVelocity, true, true, null, gridBody);
			this._physics.SetAngularVelocity(ev.Grid, 0f, true, null, gridBody);
		}

		// Token: 0x06001435 RID: 5173 RVA: 0x00068BA0 File Offset: 0x00066DA0
		private void OnRequestDrag(GridDragRequestPosition msg, EntitySessionEventArgs args)
		{
			IPlayerSession playerSession = args.SenderSession as IPlayerSession;
			if (playerSession == null || !this._admin.CanCommand(playerSession, "griddrag") || !base.Exists(msg.Grid) || base.Deleted(msg.Grid, null))
			{
				return;
			}
			base.Transform(msg.Grid).WorldPosition = msg.WorldPosition;
		}

		// Token: 0x04000C81 RID: 3201
		[Dependency]
		private readonly IConGroupController _admin;

		// Token: 0x04000C82 RID: 3202
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x04000C83 RID: 3203
		private readonly HashSet<ICommonSession> _draggers = new HashSet<ICommonSession>();
	}
}
