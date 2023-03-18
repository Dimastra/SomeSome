using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.DeviceNetwork
{
	// Token: 0x02000511 RID: 1297
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("deviceFrequency", 1)]
	[NetSerializable]
	[Serializable]
	public sealed class DeviceFrequencyPrototype : IPrototype
	{
		// Token: 0x17000325 RID: 805
		// (get) Token: 0x06000FC6 RID: 4038 RVA: 0x00032ED6 File Offset: 0x000310D6
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x04000EF7 RID: 3831
		[DataField("frequency", false, 1, true, false, null)]
		public uint Frequency;

		// Token: 0x04000EF8 RID: 3832
		[Nullable(2)]
		[DataField("name", false, 1, false, false, null)]
		public string Name;
	}
}
