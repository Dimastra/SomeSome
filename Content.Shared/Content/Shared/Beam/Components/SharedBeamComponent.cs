using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Beam.Components
{
	// Token: 0x02000677 RID: 1655
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedBeamComponent : Component
	{
		// Token: 0x040013E8 RID: 5096
		[DataField("hitTargets", false, 1, false, false, null)]
		public HashSet<EntityUid> HitTargets = new HashSet<EntityUid>();

		// Token: 0x040013E9 RID: 5097
		[DataField("virtualBeamController", false, 1, false, false, null)]
		public EntityUid? VirtualBeamController;

		// Token: 0x040013EA RID: 5098
		[DataField("originBeam", false, 1, false, false, null)]
		public EntityUid OriginBeam;

		// Token: 0x040013EB RID: 5099
		[DataField("beamShooter", false, 1, false, false, null)]
		public EntityUid BeamShooter;

		// Token: 0x040013EC RID: 5100
		[DataField("createdBeams", false, 1, false, false, null)]
		public HashSet<EntityUid> CreatedBeams = new HashSet<EntityUid>();

		// Token: 0x040013ED RID: 5101
		[Nullable(2)]
		[ViewVariables]
		[DataField("sound", false, 1, false, false, null)]
		public SoundSpecifier Sound;
	}
}
