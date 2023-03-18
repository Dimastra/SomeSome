using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Radiation.Events
{
	// Token: 0x0200022B RID: 555
	[NetSerializable]
	[Serializable]
	public sealed class OnRadiationOverlayToggledEvent : EntityEventArgs
	{
		// Token: 0x0600062D RID: 1581 RVA: 0x00015CB4 File Offset: 0x00013EB4
		public OnRadiationOverlayToggledEvent(bool isEnabled)
		{
			this.IsEnabled = isEnabled;
		}

		// Token: 0x0400062F RID: 1583
		public readonly bool IsEnabled;
	}
}
