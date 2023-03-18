using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Stunnable
{
	// Token: 0x0200010F RID: 271
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedStunSystem)
	})]
	public sealed class SlowedDownComponent : Component
	{
		// Token: 0x17000096 RID: 150
		// (get) Token: 0x0600031F RID: 799 RVA: 0x0000DEAB File Offset: 0x0000C0AB
		// (set) Token: 0x06000320 RID: 800 RVA: 0x0000DEB3 File Offset: 0x0000C0B3
		public float SprintSpeedModifier { get; set; } = 0.5f;

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x06000321 RID: 801 RVA: 0x0000DEBC File Offset: 0x0000C0BC
		// (set) Token: 0x06000322 RID: 802 RVA: 0x0000DEC4 File Offset: 0x0000C0C4
		public float WalkSpeedModifier { get; set; } = 0.5f;
	}
}
