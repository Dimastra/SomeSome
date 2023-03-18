using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Content.Shared.Tools;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Doors.Components
{
	// Token: 0x020004EC RID: 1260
	[NullableContext(1)]
	[Nullable(0)]
	[NetworkedComponent]
	[RegisterComponent]
	public sealed class DoorComponent : Component, ISerializationHooks
	{
		// Token: 0x04000E55 RID: 3669
		[ViewVariables]
		[DataField("state", false, 1, false, false, null)]
		public DoorState State;

		// Token: 0x04000E56 RID: 3670
		[DataField("closeTimeOne", false, 1, false, false, null)]
		public readonly TimeSpan CloseTimeOne = TimeSpan.FromSeconds(0.4000000059604645);

		// Token: 0x04000E57 RID: 3671
		[DataField("closeTimeTwo", false, 1, false, false, null)]
		public readonly TimeSpan CloseTimeTwo = TimeSpan.FromSeconds(0.20000000298023224);

		// Token: 0x04000E58 RID: 3672
		[DataField("openTimeOne", false, 1, false, false, null)]
		public readonly TimeSpan OpenTimeOne = TimeSpan.FromSeconds(0.4000000059604645);

		// Token: 0x04000E59 RID: 3673
		[DataField("openTimeTwo", false, 1, false, false, null)]
		public readonly TimeSpan OpenTimeTwo = TimeSpan.FromSeconds(0.20000000298023224);

		// Token: 0x04000E5A RID: 3674
		[DataField("denyDuration", false, 1, false, false, null)]
		public readonly TimeSpan DenyDuration = TimeSpan.FromSeconds(0.44999998807907104);

		// Token: 0x04000E5B RID: 3675
		[DataField("emagDuration", false, 1, false, false, null)]
		public readonly TimeSpan EmagDuration = TimeSpan.FromSeconds(0.800000011920929);

		// Token: 0x04000E5C RID: 3676
		public TimeSpan? NextStateChange;

		// Token: 0x04000E5D RID: 3677
		[DataField("partial", false, 1, false, false, null)]
		public bool Partial;

		// Token: 0x04000E5E RID: 3678
		public bool BeingPried;

		// Token: 0x04000E5F RID: 3679
		[Nullable(2)]
		[DataField("openSound", false, 1, false, false, null)]
		public SoundSpecifier OpenSound;

		// Token: 0x04000E60 RID: 3680
		[Nullable(2)]
		[DataField("closeSound", false, 1, false, false, null)]
		public SoundSpecifier CloseSound;

		// Token: 0x04000E61 RID: 3681
		[Nullable(2)]
		[DataField("denySound", false, 1, false, false, null)]
		public SoundSpecifier DenySound;

		// Token: 0x04000E62 RID: 3682
		[DataField("tryOpenDoorSound", false, 1, false, false, null)]
		public SoundSpecifier TryOpenDoorSound = new SoundPathSpecifier("/Audio/Effects/bang.ogg", null);

		// Token: 0x04000E63 RID: 3683
		[DataField("sparkSound", false, 1, false, false, null)]
		public SoundSpecifier SparkSound = new SoundCollectionSpecifier("sparks", null);

		// Token: 0x04000E64 RID: 3684
		[DataField("doorStunTime", false, 1, false, false, null)]
		public readonly TimeSpan DoorStunTime = TimeSpan.FromSeconds(2.0);

		// Token: 0x04000E65 RID: 3685
		[Nullable(2)]
		[DataField("crushDamage", false, 1, false, false, null)]
		public DamageSpecifier CrushDamage;

		// Token: 0x04000E66 RID: 3686
		[DataField("canCrush", false, 1, false, false, null)]
		public readonly bool CanCrush = true;

		// Token: 0x04000E67 RID: 3687
		[DataField("performCollisionCheck", false, 1, false, false, null)]
		public readonly bool PerformCollisionCheck = true;

		// Token: 0x04000E68 RID: 3688
		[DataField("currentlyCrushing", false, 1, false, false, null)]
		public HashSet<EntityUid> CurrentlyCrushing = new HashSet<EntityUid>();

		// Token: 0x04000E69 RID: 3689
		[DataField("pryingQuality", false, 1, false, false, typeof(PrototypeIdSerializer<ToolQualityPrototype>))]
		public string PryingQuality = "Prying";

		// Token: 0x04000E6A RID: 3690
		[DataField("pryTime", false, 1, false, false, null)]
		public float PryTime = 1.5f;

		// Token: 0x04000E6B RID: 3691
		[DataField("changeAirtight", false, 1, false, false, null)]
		public bool ChangeAirtight = true;

		// Token: 0x04000E6C RID: 3692
		[ViewVariables]
		[DataField("occludes", false, 1, false, false, null)]
		public bool Occludes = true;

		// Token: 0x04000E6D RID: 3693
		[ViewVariables]
		[DataField("bumpOpen", false, 1, false, false, null)]
		public bool BumpOpen = true;

		// Token: 0x04000E6E RID: 3694
		[ViewVariables]
		[DataField("clickOpen", false, 1, false, false, null)]
		public bool ClickOpen = true;

		// Token: 0x04000E6F RID: 3695
		[DataField("openDrawDepth", false, 1, false, false, typeof(ConstantSerializer<DrawDepth>))]
		public int OpenDrawDepth = 5;

		// Token: 0x04000E70 RID: 3696
		[DataField("closedDrawDepth", false, 1, false, false, typeof(ConstantSerializer<DrawDepth>))]
		public int ClosedDrawDepth = 5;
	}
}
