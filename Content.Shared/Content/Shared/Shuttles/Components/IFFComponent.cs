using System;
using Content.Shared.Shuttles.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Shuttles.Components
{
	// Token: 0x020001C9 RID: 457
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedShuttleSystem)
	})]
	public sealed class IFFComponent : Component
	{
		// Token: 0x04000524 RID: 1316
		public const bool ShowIFFDefault = true;

		// Token: 0x04000525 RID: 1317
		public static readonly Color IFFColor = Color.Aquamarine;

		// Token: 0x04000526 RID: 1318
		[ViewVariables]
		[DataField("flags", false, 1, false, false, null)]
		public IFFFlags Flags;

		// Token: 0x04000527 RID: 1319
		[ViewVariables]
		[DataField("color", false, 1, false, false, null)]
		public Color Color = IFFComponent.IFFColor;
	}
}
