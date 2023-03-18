using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Research.TechnologyDisk.Components
{
	// Token: 0x0200023E RID: 574
	[RegisterComponent]
	public sealed class TechnologyDiskComponent : Component
	{
		// Token: 0x04000712 RID: 1810
		[Nullable(new byte[]
		{
			2,
			1
		})]
		[DataField("recipes", false, 1, false, false, null)]
		public List<string> Recipes;
	}
}
