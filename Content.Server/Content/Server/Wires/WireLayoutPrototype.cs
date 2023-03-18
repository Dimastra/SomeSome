using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

namespace Content.Server.Wires
{
	// Token: 0x02000072 RID: 114
	[Prototype("wireLayout", 1)]
	public sealed class WireLayoutPrototype : IPrototype, IInheritingPrototype
	{
		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600016C RID: 364 RVA: 0x000084F7 File Offset: 0x000066F7
		[Nullable(1)]
		[IdDataField(1, null)]
		public string ID { [NullableContext(1)] get; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600016D RID: 365 RVA: 0x000084FF File Offset: 0x000066FF
		// (set) Token: 0x0600016E RID: 366 RVA: 0x00008507 File Offset: 0x00006707
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<WireLayoutPrototype>), 1)]
		public string[] Parents { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; [param: Nullable(new byte[]
		{
			2,
			1
		})] private set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600016F RID: 367 RVA: 0x00008510 File Offset: 0x00006710
		[AbstractDataField(1)]
		public bool Abstract { get; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000170 RID: 368 RVA: 0x00008518 File Offset: 0x00006718
		[DataField("dummyWires", false, 1, false, false, null)]
		public int DummyWires { get; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000171 RID: 369 RVA: 0x00008520 File Offset: 0x00006720
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("wires", false, 1, false, false, null)]
		public List<IWireAction> Wires { [return: Nullable(new byte[]
		{
			2,
			1
		})] get; }
	}
}
