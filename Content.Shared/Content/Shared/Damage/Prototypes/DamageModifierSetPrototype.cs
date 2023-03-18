using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Damage.Prototypes
{
	// Token: 0x0200053C RID: 1340
	[NullableContext(1)]
	[Nullable(0)]
	[Prototype("damageModifierSet", 1)]
	public sealed class DamageModifierSetPrototype : DamageModifierSet, IPrototype
	{
		// Token: 0x17000338 RID: 824
		// (get) Token: 0x0600105D RID: 4189 RVA: 0x00035C66 File Offset: 0x00033E66
		[ViewVariables]
		[IdDataField(1, null)]
		public string ID { get; }
	}
}
