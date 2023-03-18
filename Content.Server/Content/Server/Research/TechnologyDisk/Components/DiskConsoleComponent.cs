using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Server.Research.TechnologyDisk.Components
{
	// Token: 0x0200023C RID: 572
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class DiskConsoleComponent : Component
	{
		// Token: 0x0400070D RID: 1805
		[DataField("pricePerDisk", false, 1, false, false, null)]
		[ViewVariables]
		public int PricePerDisk = 2500;

		// Token: 0x0400070E RID: 1806
		[DataField("diskPrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string DiskPrototype = "TechnologyDisk";

		// Token: 0x0400070F RID: 1807
		[DataField("printDuration", false, 1, false, false, null)]
		public TimeSpan PrintDuration = TimeSpan.FromSeconds(1.0);

		// Token: 0x04000710 RID: 1808
		[DataField("printSound", false, 1, false, false, null)]
		public SoundSpecifier PrintSound = new SoundPathSpecifier("/Audio/Machines/printer.ogg", null);
	}
}
