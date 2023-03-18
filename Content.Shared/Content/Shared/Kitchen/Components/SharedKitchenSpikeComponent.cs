using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Kitchen.Components
{
	// Token: 0x02000394 RID: 916
	[NetworkedComponent]
	public abstract class SharedKitchenSpikeComponent : Component
	{
		// Token: 0x04000A7F RID: 2687
		[DataField("delay", false, 1, false, false, null)]
		public float SpikeDelay = 7f;

		// Token: 0x04000A80 RID: 2688
		[Nullable(1)]
		[ViewVariables]
		[DataField("sound", false, 1, false, false, null)]
		public SoundSpecifier SpikeSound = new SoundPathSpecifier("/Audio/Effects/Fluids/splat.ogg", null);

		// Token: 0x020007E6 RID: 2022
		[NetSerializable]
		[Serializable]
		public enum KitchenSpikeVisuals : byte
		{
			// Token: 0x0400184E RID: 6222
			Status
		}

		// Token: 0x020007E7 RID: 2023
		[NetSerializable]
		[Serializable]
		public enum KitchenSpikeStatus : byte
		{
			// Token: 0x04001850 RID: 6224
			Empty,
			// Token: 0x04001851 RID: 6225
			Bloody
		}
	}
}
