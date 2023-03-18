using System;
using Robust.Shared.GameObjects;

namespace Content.Server.IdentityManagement
{
	// Token: 0x02000458 RID: 1112
	public sealed class IdentityChangedEvent : EntityEventArgs
	{
		// Token: 0x06001672 RID: 5746 RVA: 0x0007672D File Offset: 0x0007492D
		public IdentityChangedEvent(EntityUid characterEntity, EntityUid identityEntity)
		{
			this.CharacterEntity = characterEntity;
			this.IdentityEntity = identityEntity;
		}

		// Token: 0x04000E0A RID: 3594
		public EntityUid CharacterEntity;

		// Token: 0x04000E0B RID: 3595
		public EntityUid IdentityEntity;
	}
}
