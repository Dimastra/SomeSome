using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Materials
{
	// Token: 0x0200032E RID: 814
	[NetSerializable]
	[Serializable]
	public sealed class InsertingMaterialStorageComponentState : ComponentState
	{
		// Token: 0x0600095A RID: 2394 RVA: 0x0001F6BC File Offset: 0x0001D8BC
		public InsertingMaterialStorageComponentState(TimeSpan endTime, Color? materialColor)
		{
			this.EndTime = endTime;
			this.MaterialColor = materialColor;
		}

		// Token: 0x04000945 RID: 2373
		public TimeSpan EndTime;

		// Token: 0x04000946 RID: 2374
		public Color? MaterialColor;
	}
}
