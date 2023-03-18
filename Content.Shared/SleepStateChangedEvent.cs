using System;
using Robust.Shared.GameObjects;

// Token: 0x02000008 RID: 8
public sealed class SleepStateChangedEvent : EntityEventArgs
{
	// Token: 0x06000008 RID: 8 RVA: 0x000020AD File Offset: 0x000002AD
	public SleepStateChangedEvent(bool fellAsleep)
	{
		this.FellAsleep = fellAsleep;
	}

	// Token: 0x04000004 RID: 4
	public bool FellAsleep;
}
