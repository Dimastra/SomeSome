using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Nutrition.Components
{
	// Token: 0x02000319 RID: 793
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class FlavorProfileComponent : Component
	{
		// Token: 0x1700025C RID: 604
		// (get) Token: 0x06001066 RID: 4198 RVA: 0x00054CB7 File Offset: 0x00052EB7
		[DataField("flavors", false, 1, false, false, null)]
		public HashSet<string> Flavors { get; } = new HashSet<string>();

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x06001067 RID: 4199 RVA: 0x00054CBF File Offset: 0x00052EBF
		[DataField("ignoreReagents", false, 1, false, false, null)]
		public HashSet<string> IgnoreReagents { get; } = new HashSet<string>
		{
			"Nutriment",
			"Vitamin",
			"Protein"
		};
	}
}
