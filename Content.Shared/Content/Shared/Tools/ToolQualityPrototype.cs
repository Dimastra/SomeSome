using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Utility;

namespace Content.Shared.Tools
{
	// Token: 0x020000B2 RID: 178
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("tool", 1)]
	public sealed class ToolQualityPrototype : IPrototype
	{
		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060001FD RID: 509 RVA: 0x0000A909 File Offset: 0x00008B09
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060001FE RID: 510 RVA: 0x0000A911 File Offset: 0x00008B11
		[DataField("name", false, 1, false, false, null)]
		public string Name { get; } = string.Empty;

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060001FF RID: 511 RVA: 0x0000A919 File Offset: 0x00008B19
		[DataField("toolName", false, 1, false, false, null)]
		public string ToolName { get; } = string.Empty;

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000200 RID: 512 RVA: 0x0000A921 File Offset: 0x00008B21
		[Nullable(2)]
		[DataField("icon", false, 1, false, false, null)]
		public SpriteSpecifier Icon { [NullableContext(2)] get; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000201 RID: 513 RVA: 0x0000A929 File Offset: 0x00008B29
		[DataField("spawn", false, 1, true, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
		public string Spawn { get; } = string.Empty;
	}
}
