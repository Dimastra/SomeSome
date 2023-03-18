using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Anomaly.Effects.Components
{
	// Token: 0x02000706 RID: 1798
	[RegisterComponent]
	[NetworkedComponent]
	[Access(new Type[]
	{
		typeof(BluespaceAnomalySystem)
	})]
	public sealed class BluespaceAnomalyComponent : Component
	{
		// Token: 0x040015E0 RID: 5600
		[DataField("maxShuffleRadius", false, 1, false, false, null)]
		[ViewVariables]
		public float MaxShuffleRadius = 10f;

		// Token: 0x040015E1 RID: 5601
		[DataField("maxPortalRadius", false, 1, false, false, null)]
		[ViewVariables]
		public float MaxPortalRadius = 25f;

		// Token: 0x040015E2 RID: 5602
		[DataField("minPortalRadius", false, 1, false, false, null)]
		[ViewVariables]
		public float MinPortalRadius = 10f;

		// Token: 0x040015E3 RID: 5603
		[DataField("superCriticalTeleportRadius", false, 1, false, false, null)]
		[ViewVariables]
		public float SupercriticalTeleportRadius = 50f;

		// Token: 0x040015E4 RID: 5604
		[Nullable(1)]
		[DataField("teleportSound", false, 1, false, false, null)]
		[ViewVariables]
		public SoundSpecifier TeleportSound = new SoundPathSpecifier("/Audio/Effects/teleport_arrival.ogg", null);
	}
}
