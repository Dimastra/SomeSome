using System;
using Content.Shared.Targeting;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CombatMode
{
	// Token: 0x0200059A RID: 1434
	public static class CombatModeSystemMessages
	{
		// Token: 0x0200084D RID: 2125
		[NetSerializable]
		[Serializable]
		public sealed class SetTargetZoneMessage : EntityEventArgs
		{
			// Token: 0x0600194B RID: 6475 RVA: 0x0004FD68 File Offset: 0x0004DF68
			public SetTargetZoneMessage(TargetingZone targetZone)
			{
				this.TargetZone = targetZone;
			}

			// Token: 0x17000525 RID: 1317
			// (get) Token: 0x0600194C RID: 6476 RVA: 0x0004FD77 File Offset: 0x0004DF77
			public TargetingZone TargetZone { get; }
		}
	}
}
