using System;
using System.Runtime.CompilerServices;
using Content.Shared.ActionBlocker;
using Content.Shared.Cuffs.Components;
using Content.Shared.Humanoid;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Client.Cuffs.Components
{
	// Token: 0x0200036A RID: 874
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentReference(typeof(SharedCuffableComponent))]
	public sealed class CuffableComponent : SharedCuffableComponent
	{
		// Token: 0x0600158D RID: 5517 RVA: 0x0007FB68 File Offset: 0x0007DD68
		[NullableContext(2)]
		public override void HandleComponentState(ComponentState curState, ComponentState nextState)
		{
			SharedCuffableComponent.CuffableComponentState cuffableComponentState = curState as SharedCuffableComponent.CuffableComponentState;
			if (cuffableComponentState == null)
			{
				return;
			}
			base.CanStillInteract = cuffableComponentState.CanStillInteract;
			this._sysMan.GetEntitySystem<ActionBlockerSystem>().UpdateCanMove(base.Owner, null);
			SpriteComponent spriteComponent;
			if (this._entityManager.TryGetComponent<SpriteComponent>(base.Owner, ref spriteComponent))
			{
				spriteComponent.LayerSetVisible(HumanoidVisualLayers.Handcuffs, cuffableComponentState.NumHandsCuffed > 0);
				spriteComponent.LayerSetColor(HumanoidVisualLayers.Handcuffs, cuffableComponentState.Color);
				if (cuffableComponentState.NumHandsCuffed > 0)
				{
					if (this._currentRSI != cuffableComponentState.RSI)
					{
						this._currentRSI = cuffableComponentState.RSI;
						if (this._currentRSI != null)
						{
							spriteComponent.LayerSetState(HumanoidVisualLayers.Handcuffs, new RSI.StateId(cuffableComponentState.IconState), new ResourcePath(this._currentRSI, "/"));
						}
					}
					else
					{
						spriteComponent.LayerSetState(HumanoidVisualLayers.Handcuffs, new RSI.StateId(cuffableComponentState.IconState));
					}
				}
			}
			CuffedStateChangeEvent cuffedStateChangeEvent = default(CuffedStateChangeEvent);
			this._entityManager.EventBus.RaiseLocalEvent<CuffedStateChangeEvent>(base.Owner, ref cuffedStateChangeEvent, false);
		}

		// Token: 0x0600158E RID: 5518 RVA: 0x0007FC78 File Offset: 0x0007DE78
		protected override void OnRemove()
		{
			base.OnRemove();
			SpriteComponent spriteComponent;
			if (this._entityManager.TryGetComponent<SpriteComponent>(base.Owner, ref spriteComponent))
			{
				spriteComponent.LayerSetVisible(HumanoidVisualLayers.Handcuffs, false);
			}
		}

		// Token: 0x04000B46 RID: 2886
		[Nullable(2)]
		[ViewVariables]
		private string _currentRSI;

		// Token: 0x04000B47 RID: 2887
		[Dependency]
		private readonly IEntityManager _entityManager;

		// Token: 0x04000B48 RID: 2888
		[Dependency]
		private readonly IEntitySystemManager _sysMan;
	}
}
