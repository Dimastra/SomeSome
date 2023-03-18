using System;
using Robust.Shared.GameObjects;

namespace Content.Shared.Destructible
{
	// Token: 0x0200051E RID: 1310
	public abstract class SharedDestructibleSystem : EntitySystem
	{
		// Token: 0x06000FDB RID: 4059 RVA: 0x00033100 File Offset: 0x00031300
		public void DestroyEntity(EntityUid owner)
		{
			DestructionEventArgs eventArgs = new DestructionEventArgs();
			base.RaiseLocalEvent<DestructionEventArgs>(owner, eventArgs, false);
			base.QueueDel(owner);
		}

		// Token: 0x06000FDC RID: 4060 RVA: 0x00033124 File Offset: 0x00031324
		public void BreakEntity(EntityUid owner)
		{
			BreakageEventArgs eventArgs = new BreakageEventArgs();
			base.RaiseLocalEvent<BreakageEventArgs>(owner, eventArgs, false);
		}
	}
}
