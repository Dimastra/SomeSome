using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Damage.Prototypes
{
	// Token: 0x0200053E RID: 1342
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("examinableDamage", 1)]
	public sealed class ExaminableDamagePrototype : IPrototype
	{
		// Token: 0x1700033C RID: 828
		// (get) Token: 0x06001065 RID: 4197 RVA: 0x00035CA8 File Offset: 0x00033EA8
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x04000F66 RID: 3942
		[DataField("messages", false, 1, false, false, null)]
		public string[] Messages = new string[0];
	}
}
