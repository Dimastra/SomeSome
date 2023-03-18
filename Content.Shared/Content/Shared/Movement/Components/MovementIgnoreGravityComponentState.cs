using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Movement.Components
{
	// Token: 0x020002EE RID: 750
	[NetSerializable]
	[Serializable]
	public sealed class MovementIgnoreGravityComponentState : ComponentState
	{
		// Token: 0x0600086C RID: 2156 RVA: 0x0001CA20 File Offset: 0x0001AC20
		[NullableContext(1)]
		public MovementIgnoreGravityComponentState(MovementIgnoreGravityComponent component)
		{
			this.Weightless = component.Weightless;
		}

		// Token: 0x04000884 RID: 2180
		public bool Weightless;
	}
}
