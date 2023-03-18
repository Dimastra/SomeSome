using System;
using System.Runtime.CompilerServices;
using Content.Shared.Nutrition.Components;
using Robust.Shared.GameObjects;

namespace Content.Client.Nutrition.Components
{
	// Token: 0x02000205 RID: 517
	[RegisterComponent]
	[ComponentReference(typeof(SharedHungerComponent))]
	public sealed class HungerComponent : SharedHungerComponent
	{
		// Token: 0x170002DE RID: 734
		// (get) Token: 0x06000D8A RID: 3466 RVA: 0x00051DB1 File Offset: 0x0004FFB1
		public override HungerThreshold CurrentHungerThreshold
		{
			get
			{
				return this._currentHungerThreshold;
			}
		}

		// Token: 0x06000D8B RID: 3467 RVA: 0x00051DBC File Offset: 0x0004FFBC
		[NullableContext(2)]
		public override void HandleComponentState(ComponentState curState, ComponentState nextState)
		{
			base.HandleComponentState(curState, nextState);
			SharedHungerComponent.HungerComponentState hungerComponentState = curState as SharedHungerComponent.HungerComponentState;
			if (hungerComponentState == null)
			{
				return;
			}
			this._currentHungerThreshold = hungerComponentState.CurrentThreshold;
		}

		// Token: 0x040006B8 RID: 1720
		private HungerThreshold _currentHungerThreshold;
	}
}
