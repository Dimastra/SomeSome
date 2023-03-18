using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Mech.Equipment.Components
{
	// Token: 0x02000325 RID: 805
	public sealed class MechEquipmentInstallFinished : EntityEventArgs
	{
		// Token: 0x06000936 RID: 2358 RVA: 0x0001EB06 File Offset: 0x0001CD06
		public MechEquipmentInstallFinished(EntityUid mech)
		{
			this.Mech = mech;
		}

		// Token: 0x0400091A RID: 2330
		public EntityUid Mech;
	}
}
