using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Alert
{
	// Token: 0x02000724 RID: 1828
	[NetSerializable]
	[Serializable]
	public sealed class ClickAlertEvent : EntityEventArgs
	{
		// Token: 0x06001631 RID: 5681 RVA: 0x00048B05 File Offset: 0x00046D05
		public ClickAlertEvent(AlertType alertType)
		{
			this.Type = alertType;
		}

		// Token: 0x0400167F RID: 5759
		public readonly AlertType Type;
	}
}
