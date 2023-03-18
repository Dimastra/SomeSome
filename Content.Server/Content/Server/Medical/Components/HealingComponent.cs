using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Medical.Components
{
	// Token: 0x020003BC RID: 956
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class HealingComponent : Component
	{
		// Token: 0x04000C0E RID: 3086
		[Nullable(1)]
		[DataField("damage", false, 1, true, false, null)]
		[ViewVariables]
		public DamageSpecifier Damage;

		// Token: 0x04000C0F RID: 3087
		[DataField("bloodlossModifier", false, 1, false, false, null)]
		[ViewVariables]
		public float BloodlossModifier;

		// Token: 0x04000C10 RID: 3088
		[DataField("damageContainer", false, 1, false, false, typeof(PrototypeIdSerializer<DamageContainerPrototype>))]
		public string DamageContainerID;

		// Token: 0x04000C11 RID: 3089
		[ViewVariables]
		[DataField("delay", false, 1, false, false, null)]
		public float Delay = 3f;

		// Token: 0x04000C12 RID: 3090
		[DataField("cancelToken", false, 1, false, false, null)]
		public CancellationTokenSource CancelToken;

		// Token: 0x04000C13 RID: 3091
		[DataField("selfHealPenaltyMultiplier", false, 1, false, false, null)]
		public float SelfHealPenaltyMultiplier = 3f;

		// Token: 0x04000C14 RID: 3092
		[DataField("healingBeginSound", false, 1, false, false, null)]
		public SoundSpecifier HealingBeginSound;

		// Token: 0x04000C15 RID: 3093
		[DataField("healingEndSound", false, 1, false, false, null)]
		public SoundSpecifier HealingEndSound;
	}
}
