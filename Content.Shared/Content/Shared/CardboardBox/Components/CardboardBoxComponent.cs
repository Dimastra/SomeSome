using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Shared.CardboardBox.Components
{
	// Token: 0x02000638 RID: 1592
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class CardboardBoxComponent : Component
	{
		// Token: 0x04001324 RID: 4900
		[DataField("mover", false, 1, false, false, null)]
		public EntityUid? Mover;

		// Token: 0x04001325 RID: 4901
		[Nullable(1)]
		[ViewVariables]
		[DataField("effect", false, 1, false, false, null)]
		public string Effect = "Exclamation";

		// Token: 0x04001326 RID: 4902
		[Nullable(2)]
		[ViewVariables]
		[DataField("effectSound", false, 1, false, false, null)]
		public SoundSpecifier EffectSound;

		// Token: 0x04001327 RID: 4903
		[ViewVariables]
		[DataField("distance", false, 1, false, false, null)]
		public float Distance = 6f;

		// Token: 0x04001328 RID: 4904
		[DataField("effectCooldown", false, 1, false, false, typeof(TimeOffsetSerializer))]
		public TimeSpan EffectCooldown = TimeSpan.FromSeconds(1.0);

		// Token: 0x04001329 RID: 4905
		[DataField("maxEffectCooldown", false, 1, false, false, typeof(TimeOffsetSerializer))]
		public static readonly TimeSpan MaxEffectCooldown = TimeSpan.FromSeconds(5.0);
	}
}
