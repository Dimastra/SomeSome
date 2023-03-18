using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Hands.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Item
{
	// Token: 0x020003A0 RID: 928
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(SharedItemSystem)
	})]
	public sealed class ItemComponent : Component
	{
		// Token: 0x04000A9A RID: 2714
		[ViewVariables]
		[DataField("size", false, 1, false, false, null)]
		[Access]
		public int Size = 5;

		// Token: 0x04000A9B RID: 2715
		[Nullable(1)]
		[Access(new Type[]
		{
			typeof(SharedItemSystem)
		})]
		[DataField("inhandVisuals", false, 1, false, false, null)]
		public Dictionary<HandLocation, List<SharedSpriteComponent.PrototypeLayerData>> InhandVisuals = new Dictionary<HandLocation, List<SharedSpriteComponent.PrototypeLayerData>>();

		// Token: 0x04000A9C RID: 2716
		[Access(new Type[]
		{
			typeof(SharedItemSystem)
		})]
		[ViewVariables]
		[DataField("heldPrefix", false, 1, false, false, null)]
		public string HeldPrefix;

		// Token: 0x04000A9D RID: 2717
		[Access(new Type[]
		{
			typeof(SharedItemSystem)
		})]
		[ViewVariables]
		[DataField("sprite", false, 1, false, false, null)]
		public string RsiPath;
	}
}
