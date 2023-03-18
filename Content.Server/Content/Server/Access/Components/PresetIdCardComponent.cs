using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Access.Components
{
	// Token: 0x02000882 RID: 2178
	[RegisterComponent]
	public sealed class PresetIdCardComponent : Component
	{
		// Token: 0x04001C94 RID: 7316
		[Nullable(2)]
		[DataField("job", false, 1, false, false, null)]
		public readonly string JobName;
	}
}
