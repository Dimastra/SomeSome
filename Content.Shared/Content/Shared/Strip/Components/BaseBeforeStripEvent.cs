using System;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Strip.Components
{
	// Token: 0x02000118 RID: 280
	public abstract class BaseBeforeStripEvent : EntityEventArgs, IInventoryRelayEvent
	{
		// Token: 0x1700009A RID: 154
		// (get) Token: 0x06000337 RID: 823 RVA: 0x0000E0EF File Offset: 0x0000C2EF
		public float Time
		{
			get
			{
				return MathF.Max(this.InitialTime * this.Multiplier + this.Additive, 0f);
			}
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000338 RID: 824 RVA: 0x0000E10F File Offset: 0x0000C30F
		public SlotFlags TargetSlots { get; } = 1024;

		// Token: 0x06000339 RID: 825 RVA: 0x0000E117 File Offset: 0x0000C317
		public BaseBeforeStripEvent(float initialTime, bool stealth = false)
		{
			this.InitialTime = initialTime;
			this.Stealth = stealth;
		}

		// Token: 0x0400035F RID: 863
		public readonly float InitialTime;

		// Token: 0x04000360 RID: 864
		public float Additive;

		// Token: 0x04000361 RID: 865
		public float Multiplier = 1f;

		// Token: 0x04000362 RID: 866
		public bool Stealth;
	}
}
