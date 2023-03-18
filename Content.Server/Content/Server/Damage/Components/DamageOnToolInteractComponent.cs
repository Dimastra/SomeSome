using System;
using System.Runtime.CompilerServices;
using Content.Shared.Damage;
using Content.Shared.Tools;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Server.Damage.Components
{
	// Token: 0x020005CB RID: 1483
	[NullableContext(2)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class DamageOnToolInteractComponent : Component
	{
		// Token: 0x170004AB RID: 1195
		// (get) Token: 0x06001FA7 RID: 8103 RVA: 0x000A5C01 File Offset: 0x000A3E01
		[Nullable(1)]
		[DataField("tools", false, 1, false, false, null)]
		public PrototypeFlags<ToolQualityPrototype> Tools { [NullableContext(1)] get; } = new PrototypeFlags<ToolQualityPrototype>();

		// Token: 0x170004AC RID: 1196
		// (get) Token: 0x06001FA8 RID: 8104 RVA: 0x000A5C09 File Offset: 0x000A3E09
		[DataField("weldingDamage", false, 1, false, false, null)]
		[ViewVariables]
		public DamageSpecifier WeldingDamage { get; }

		// Token: 0x170004AD RID: 1197
		// (get) Token: 0x06001FA9 RID: 8105 RVA: 0x000A5C11 File Offset: 0x000A3E11
		[DataField("defaultDamage", false, 1, false, false, null)]
		[ViewVariables]
		public DamageSpecifier DefaultDamage { get; }
	}
}
