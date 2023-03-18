using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Toggleable
{
	// Token: 0x020000F5 RID: 245
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class ToggleableLightVisualsComponent : Component
	{
		// Token: 0x04000327 RID: 807
		[DataField("spriteLayer", false, 1, false, false, null)]
		public string SpriteLayer = "light";

		// Token: 0x04000328 RID: 808
		[DataField("inhandVisuals", false, 1, false, false, null)]
		public Dictionary<HandLocation, List<SharedSpriteComponent.PrototypeLayerData>> InhandVisuals = new Dictionary<HandLocation, List<SharedSpriteComponent.PrototypeLayerData>>();

		// Token: 0x04000329 RID: 809
		[DataField("clothingVisuals", false, 1, false, false, null)]
		public readonly Dictionary<string, List<SharedSpriteComponent.PrototypeLayerData>> ClothingVisuals = new Dictionary<string, List<SharedSpriteComponent.PrototypeLayerData>>();
	}
}
