using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Engineering.Components
{
	// Token: 0x0200052D RID: 1325
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class DisassembleOnAltVerbComponent : Component
	{
		// Token: 0x1700040F RID: 1039
		// (get) Token: 0x06001B98 RID: 7064 RVA: 0x00093AEB File Offset: 0x00091CEB
		[Nullable(2)]
		[DataField("prototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Prototype { [NullableContext(2)] get; }

		// Token: 0x17000410 RID: 1040
		// (get) Token: 0x06001B99 RID: 7065 RVA: 0x00093AF3 File Offset: 0x00091CF3
		public CancellationTokenSource TokenSource { get; } = new CancellationTokenSource();

		// Token: 0x040011B0 RID: 4528
		[DataField("doAfter", false, 1, false, false, null)]
		public float DoAfterTime;
	}
}
