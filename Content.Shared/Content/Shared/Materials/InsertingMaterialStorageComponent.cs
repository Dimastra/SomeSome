using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Materials
{
	// Token: 0x0200032D RID: 813
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class InsertingMaterialStorageComponent : Component
	{
		// Token: 0x04000943 RID: 2371
		[DataField("endTime", false, 1, false, false, typeof(TimeOffsetSerializer))]
		[ViewVariables]
		public TimeSpan EndTime;

		// Token: 0x04000944 RID: 2372
		[ViewVariables]
		public Color? MaterialColor;
	}
}
