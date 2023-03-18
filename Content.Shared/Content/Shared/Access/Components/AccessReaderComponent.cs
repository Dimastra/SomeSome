using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Access.Components
{
	// Token: 0x0200077B RID: 1915
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class AccessReaderComponent : Component
	{
		// Token: 0x0400175A RID: 5978
		public HashSet<string> DenyTags = new HashSet<string>();

		// Token: 0x0400175B RID: 5979
		[DataField("access", false, 1, false, false, null)]
		public List<HashSet<string>> AccessLists = new List<HashSet<string>>();
	}
}
