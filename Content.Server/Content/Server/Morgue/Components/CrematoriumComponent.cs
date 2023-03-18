using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Morgue.Components
{
	// Token: 0x0200039A RID: 922
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class CrematoriumComponent : Component
	{
		// Token: 0x04000B87 RID: 2951
		[ViewVariables]
		public int CookTime = 5;

		// Token: 0x04000B88 RID: 2952
		[DataField("cremateStartSound", false, 1, false, false, null)]
		public SoundSpecifier CremateStartSound = new SoundPathSpecifier("/Audio/Items/lighter1.ogg", null);

		// Token: 0x04000B89 RID: 2953
		[DataField("crematingSound", false, 1, false, false, null)]
		public SoundSpecifier CrematingSound = new SoundPathSpecifier("/Audio/Effects/burning.ogg", null);

		// Token: 0x04000B8A RID: 2954
		[DataField("cremateFinishSound", false, 1, false, false, null)]
		public SoundSpecifier CremateFinishSound = new SoundPathSpecifier("/Audio/Machines/ding.ogg", null);
	}
}
