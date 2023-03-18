using System;
using System.Runtime.CompilerServices;
using Content.Shared.Construction.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Anomaly.Components
{
	// Token: 0x020007CB RID: 1995
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class AnomalyVesselComponent : Component
	{
		// Token: 0x04001AD8 RID: 6872
		[ViewVariables]
		public EntityUid? Anomaly;

		// Token: 0x04001AD9 RID: 6873
		[ViewVariables]
		public float PointMultiplier = 1f;

		// Token: 0x04001ADA RID: 6874
		[DataField("machinePartPointModifier", false, 1, false, false, typeof(PrototypeIdSerializer<MachinePartPrototype>))]
		public string MachinePartPointModifier = "ScanningModule";

		// Token: 0x04001ADB RID: 6875
		[DataField("partRatingPointModifier", false, 1, false, false, null)]
		public float PartRatingPointModifier = 1.5f;

		// Token: 0x04001ADC RID: 6876
		[DataField("maxBeepInterval", false, 1, false, false, null)]
		public TimeSpan MaxBeepInterval = TimeSpan.FromSeconds(2.0);

		// Token: 0x04001ADD RID: 6877
		[DataField("minBeepInterval", false, 1, false, false, null)]
		public TimeSpan MinBeepInterval = TimeSpan.FromSeconds(0.75);

		// Token: 0x04001ADE RID: 6878
		[DataField("nextBeep", false, 1, false, false, typeof(TimeOffsetSerializer))]
		public TimeSpan NextBeep = TimeSpan.Zero;

		// Token: 0x04001ADF RID: 6879
		[DataField("beepSound", false, 1, false, false, null)]
		public SoundSpecifier BeepSound = new SoundPathSpecifier("/Audio/Machines/vessel_warning.ogg", null);
	}
}
