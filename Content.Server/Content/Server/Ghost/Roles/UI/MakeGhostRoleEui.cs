using System;
using System.Runtime.CompilerServices;
using Content.Server.EUI;
using Content.Shared.Eui;
using Content.Shared.Ghost.Roles;
using Robust.Shared.GameObjects;

namespace Content.Server.Ghost.Roles.UI
{
	// Token: 0x02000499 RID: 1177
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MakeGhostRoleEui : BaseEui
	{
		// Token: 0x060017AD RID: 6061 RVA: 0x0007C001 File Offset: 0x0007A201
		public MakeGhostRoleEui(EntityUid entityUid)
		{
			this.EntityUid = entityUid;
		}

		// Token: 0x17000339 RID: 825
		// (get) Token: 0x060017AE RID: 6062 RVA: 0x0007C010 File Offset: 0x0007A210
		public EntityUid EntityUid { get; }

		// Token: 0x060017AF RID: 6063 RVA: 0x0007C018 File Offset: 0x0007A218
		public override EuiStateBase GetNewState()
		{
			return new MakeGhostRoleEuiState(this.EntityUid);
		}

		// Token: 0x060017B0 RID: 6064 RVA: 0x0007C025 File Offset: 0x0007A225
		public override void HandleMessage(EuiMessageBase msg)
		{
			base.HandleMessage(msg);
			if (msg is MakeGhostRoleWindowClosedMessage)
			{
				this.Closed();
			}
		}

		// Token: 0x060017B1 RID: 6065 RVA: 0x0007C03C File Offset: 0x0007A23C
		public override void Closed()
		{
			base.Closed();
			EntitySystem.Get<GhostRoleSystem>().CloseMakeGhostRoleEui(base.Player);
		}
	}
}
