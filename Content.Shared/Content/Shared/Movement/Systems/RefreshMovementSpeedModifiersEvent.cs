using System;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Movement.Systems
{
	// Token: 0x020002D9 RID: 729
	public sealed class RefreshMovementSpeedModifiersEvent : EntityEventArgs, IInventoryRelayEvent
	{
		// Token: 0x1700018A RID: 394
		// (get) Token: 0x060007FA RID: 2042 RVA: 0x0001A773 File Offset: 0x00018973
		public SlotFlags TargetSlots { get; } = -4097;

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x060007FB RID: 2043 RVA: 0x0001A77B File Offset: 0x0001897B
		// (set) Token: 0x060007FC RID: 2044 RVA: 0x0001A783 File Offset: 0x00018983
		public float WalkSpeedModifier { get; private set; } = 1f;

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x060007FD RID: 2045 RVA: 0x0001A78C File Offset: 0x0001898C
		// (set) Token: 0x060007FE RID: 2046 RVA: 0x0001A794 File Offset: 0x00018994
		public float SprintSpeedModifier { get; private set; } = 1f;

		// Token: 0x060007FF RID: 2047 RVA: 0x0001A79D File Offset: 0x0001899D
		public void ModifySpeed(float walk, float sprint)
		{
			this.WalkSpeedModifier *= walk;
			this.SprintSpeedModifier *= sprint;
		}
	}
}
