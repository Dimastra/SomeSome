using System;
using System.Runtime.CompilerServices;
using Content.Shared.Tools;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Toilet
{
	// Token: 0x020000C3 RID: 195
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class ToiletComponent : Component
	{
		// Token: 0x04000296 RID: 662
		[DataField("pryLidTime", false, 1, false, false, null)]
		public float PryLidTime = 1f;

		// Token: 0x04000297 RID: 663
		[DataField("pryingQuality", false, 1, false, false, typeof(PrototypeIdSerializer<ToolQualityPrototype>))]
		public string PryingQuality = "Prying";

		// Token: 0x04000298 RID: 664
		[DataField("toggleSound", false, 1, false, false, null)]
		public SoundSpecifier ToggleSound = new SoundPathSpecifier("/Audio/Effects/toilet_seat_down.ogg", null);

		// Token: 0x04000299 RID: 665
		public bool LidOpen;

		// Token: 0x0400029A RID: 666
		public bool IsSeatUp;

		// Token: 0x0400029B RID: 667
		public bool IsPrying;
	}
}
