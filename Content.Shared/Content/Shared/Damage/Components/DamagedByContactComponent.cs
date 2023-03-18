using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Damage.Components
{
	// Token: 0x02000543 RID: 1347
	[NetworkedComponent]
	[RegisterComponent]
	public sealed class DamagedByContactComponent : Component
	{
		// Token: 0x04000F6E RID: 3950
		[DataField("nextSecond", false, 1, false, false, typeof(TimeOffsetSerializer))]
		[ViewVariables]
		public TimeSpan NextSecond = TimeSpan.Zero;

		// Token: 0x04000F6F RID: 3951
		[Nullable(2)]
		[ViewVariables]
		public DamageSpecifier Damage;
	}
}
