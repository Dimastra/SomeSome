using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Weapons.Reflect
{
	// Token: 0x020000AD RID: 173
	[RegisterComponent]
	public sealed class ReflectComponent : Component
	{
		// Token: 0x040001DC RID: 476
		[DataField("enabled", false, 1, false, false, null)]
		[ViewVariables]
		public bool Enabled;

		// Token: 0x040001DD RID: 477
		[DataField("chance", false, 1, false, false, null)]
		[ViewVariables]
		public float Chance;

		// Token: 0x040001DE RID: 478
		[DataField("spread", false, 1, false, false, null)]
		[ViewVariables]
		public Angle Spread = Angle.FromDegrees(5.0);

		// Token: 0x040001DF RID: 479
		[Nullable(2)]
		[DataField("onReflect", false, 1, false, false, null)]
		public SoundSpecifier OnReflect;
	}
}
