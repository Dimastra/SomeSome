using System;
using Robust.Shared.GameObjects;

namespace Content.Server.Light.Events
{
	// Token: 0x02000409 RID: 1033
	public sealed class LightToggleEvent : EntityEventArgs
	{
		// Token: 0x060014E1 RID: 5345 RVA: 0x0006D54C File Offset: 0x0006B74C
		public LightToggleEvent(bool isOn)
		{
			this.IsOn = isOn;
		}

		// Token: 0x04000CFE RID: 3326
		public bool IsOn;
	}
}
