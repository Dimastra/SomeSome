using System;
using Content.Shared.Light.Component;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Server.Light.Components
{
	// Token: 0x02000419 RID: 1049
	[RegisterComponent]
	public sealed class ExpendableLightComponent : SharedExpendableLightComponent
	{
		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x06001551 RID: 5457 RVA: 0x0006FFC0 File Offset: 0x0006E1C0
		[ViewVariables]
		public bool Activated
		{
			get
			{
				ExpendableLightState currentState = base.CurrentState;
				return currentState == ExpendableLightState.Lit || currentState == ExpendableLightState.Fading;
			}
		}

		// Token: 0x04000D33 RID: 3379
		[ViewVariables]
		public float StateExpiryTime;
	}
}
