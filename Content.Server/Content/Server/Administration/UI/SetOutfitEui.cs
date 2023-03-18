using System;
using System.Runtime.CompilerServices;
using Content.Server.Administration.Managers;
using Content.Server.EUI;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Administration.UI
{
	// Token: 0x02000809 RID: 2057
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class SetOutfitEui : BaseEui
	{
		// Token: 0x06002CA5 RID: 11429 RVA: 0x000E89EC File Offset: 0x000E6BEC
		public SetOutfitEui(EntityUid entity)
		{
			this._target = entity;
			IoCManager.InjectDependencies<SetOutfitEui>(this);
		}

		// Token: 0x06002CA6 RID: 11430 RVA: 0x000E8A02 File Offset: 0x000E6C02
		public override void Opened()
		{
			base.Opened();
			base.StateDirty();
			this._adminManager.OnPermsChanged += this.AdminManagerOnPermsChanged;
		}

		// Token: 0x06002CA7 RID: 11431 RVA: 0x000E8A27 File Offset: 0x000E6C27
		public override EuiStateBase GetNewState()
		{
			return new SetOutfitEuiState
			{
				TargetEntityId = this._target
			};
		}

		// Token: 0x06002CA8 RID: 11432 RVA: 0x000E8A3A File Offset: 0x000E6C3A
		private void AdminManagerOnPermsChanged(AdminPermsChangedEventArgs obj)
		{
			if (obj.Player == base.Player && !this.UserAdminFlagCheck(AdminFlags.Fun))
			{
				base.Close();
			}
		}

		// Token: 0x06002CA9 RID: 11433 RVA: 0x000E8A59 File Offset: 0x000E6C59
		private bool UserAdminFlagCheck(AdminFlags flags)
		{
			return this._adminManager.HasAdminFlag(base.Player, flags);
		}

		// Token: 0x04001B8F RID: 7055
		[Dependency]
		private readonly IAdminManager _adminManager;

		// Token: 0x04001B90 RID: 7056
		private readonly EntityUid _target;
	}
}
