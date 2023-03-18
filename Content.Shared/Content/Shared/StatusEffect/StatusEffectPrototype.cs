using System;
using System.Runtime.CompilerServices;
using Content.Shared.Alert;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.StatusEffect
{
	// Token: 0x02000155 RID: 341
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("statusEffect", 1)]
	public sealed class StatusEffectPrototype : IPrototype
	{
		// Token: 0x170000BF RID: 191
		// (get) Token: 0x0600041C RID: 1052 RVA: 0x00010698 File Offset: 0x0000E898
		[IdDataField(1, null)]
		public string ID { get; }

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x0600041D RID: 1053 RVA: 0x000106A0 File Offset: 0x0000E8A0
		[DataField("alert", false, 1, false, false, null)]
		public AlertType? Alert { get; }

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x0600041E RID: 1054 RVA: 0x000106A8 File Offset: 0x0000E8A8
		[DataField("alwaysAllowed", false, 1, false, false, null)]
		public bool AlwaysAllowed { get; }
	}
}
