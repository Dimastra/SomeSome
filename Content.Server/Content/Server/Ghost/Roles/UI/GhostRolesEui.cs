using System;
using System.Runtime.CompilerServices;
using Content.Server.EUI;
using Content.Shared.Eui;
using Content.Shared.Ghost.Roles;
using Robust.Shared.GameObjects;

namespace Content.Server.Ghost.Roles.UI
{
	// Token: 0x02000498 RID: 1176
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class GhostRolesEui : BaseEui
	{
		// Token: 0x060017A9 RID: 6057 RVA: 0x0007BF6A File Offset: 0x0007A16A
		[PreserveBaseOverrides]
		public new GhostRolesEuiState GetNewState()
		{
			return new GhostRolesEuiState(EntitySystem.Get<GhostRoleSystem>().GetGhostRolesInfo());
		}

		// Token: 0x060017AA RID: 6058 RVA: 0x0007BF7C File Offset: 0x0007A17C
		public override void HandleMessage(EuiMessageBase msg)
		{
			base.HandleMessage(msg);
			GhostRoleTakeoverRequestMessage req = msg as GhostRoleTakeoverRequestMessage;
			if (req != null)
			{
				EntitySystem.Get<GhostRoleSystem>().Takeover(base.Player, req.Identifier);
				return;
			}
			GhostRoleFollowRequestMessage req2 = msg as GhostRoleFollowRequestMessage;
			if (req2 != null)
			{
				EntitySystem.Get<GhostRoleSystem>().Follow(base.Player, req2.Identifier);
				return;
			}
			if (!(msg is GhostRoleWindowCloseMessage))
			{
				return;
			}
			this.Closed();
		}

		// Token: 0x060017AB RID: 6059 RVA: 0x0007BFE1 File Offset: 0x0007A1E1
		public override void Closed()
		{
			base.Closed();
			EntitySystem.Get<GhostRoleSystem>().CloseEui(base.Player);
		}
	}
}
