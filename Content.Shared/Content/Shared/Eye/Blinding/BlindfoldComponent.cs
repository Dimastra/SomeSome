using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Eye.Blinding
{
	// Token: 0x02000497 RID: 1175
	[RegisterComponent]
	[NetworkedComponent]
	public sealed class BlindfoldComponent : Component
	{
		// Token: 0x04000D6B RID: 3435
		[ViewVariables]
		public bool IsActive;
	}
}
