using System;
using System.Runtime.CompilerServices;
using Content.Shared.Cuffs.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Cuffs.Components
{
	// Token: 0x0200036B RID: 875
	[RegisterComponent]
	[ComponentReference(typeof(SharedHandcuffComponent))]
	public sealed class HandcuffComponent : SharedHandcuffComponent
	{
		// Token: 0x06001590 RID: 5520 RVA: 0x0007FCB8 File Offset: 0x0007DEB8
		[NullableContext(2)]
		public override void HandleComponentState(ComponentState curState, ComponentState nextState)
		{
			SharedHandcuffComponent.HandcuffedComponentState handcuffedComponentState = curState as SharedHandcuffComponent.HandcuffedComponentState;
			if (handcuffedComponentState == null)
			{
				return;
			}
			if (handcuffedComponentState.IconState == string.Empty)
			{
				return;
			}
			SpriteComponent spriteComponent;
			if (IoCManager.Resolve<IEntityManager>().TryGetComponent<SpriteComponent>(base.Owner, ref spriteComponent))
			{
				spriteComponent.LayerSetState(0, new RSI.StateId(handcuffedComponentState.IconState));
			}
		}
	}
}
