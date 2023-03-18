using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Mech.Equipment.Components
{
	// Token: 0x020003C8 RID: 968
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class MechGrabberComponent : Component
	{
		// Token: 0x04000C54 RID: 3156
		[DataField("grabEnergyDelta", false, 1, false, false, null)]
		public float GrabEnergyDelta = -30f;

		// Token: 0x04000C55 RID: 3157
		[DataField("grabDelay", false, 1, false, false, null)]
		public float GrabDelay = 2.5f;

		// Token: 0x04000C56 RID: 3158
		[DataField("depositOffset", false, 1, false, false, null)]
		public Vector2 DepositOffset = new Vector2(0f, -1f);

		// Token: 0x04000C57 RID: 3159
		[DataField("maxContents", false, 1, false, false, null)]
		public int MaxContents = 10;

		// Token: 0x04000C58 RID: 3160
		[DataField("grabSound", false, 1, false, false, null)]
		public SoundSpecifier GrabSound = new SoundPathSpecifier("/Audio/Mecha/sound_mecha_hydraulic.ogg", null);

		// Token: 0x04000C59 RID: 3161
		[Nullable(2)]
		public IPlayingAudioStream AudioStream;

		// Token: 0x04000C5A RID: 3162
		[ViewVariables]
		public Container ItemContainer;
	}
}
