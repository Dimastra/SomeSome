using System;
using Content.Shared.Spawners.EntitySystems;
using Robust.Shared.GameObjects;

namespace Content.Client.Spawners
{
	// Token: 0x0200013C RID: 316
	public sealed class TimedDespawnSystem : SharedTimedDespawnSystem
	{
		// Token: 0x0600085F RID: 2143 RVA: 0x00030AE1 File Offset: 0x0002ECE1
		protected override bool CanDelete(EntityUid uid)
		{
			return uid.IsClientSide();
		}
	}
}
