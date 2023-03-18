using System;
using System.Runtime.CompilerServices;
using Content.Shared.Security;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Security
{
	// Token: 0x0200015C RID: 348
	public sealed class DeployableBarrierVisualizer : AppearanceVisualizer
	{
		// Token: 0x06000926 RID: 2342 RVA: 0x00035C6C File Offset: 0x00033E6C
		[NullableContext(1)]
		[Obsolete("Subscribe to AppearanceChangeEvent instead.")]
		public override void OnChangeData(AppearanceComponent component)
		{
			base.OnChangeData(component);
			SpriteComponent spriteComponent;
			if (!IoCManager.Resolve<IEntityManager>().TryGetComponent<SpriteComponent>(component.Owner, ref spriteComponent))
			{
				return;
			}
			DeployableBarrierState deployableBarrierState;
			if (!component.TryGetData<DeployableBarrierState>(DeployableBarrierVisuals.State, ref deployableBarrierState))
			{
				return;
			}
			if (deployableBarrierState == DeployableBarrierState.Idle)
			{
				spriteComponent.LayerSetState(0, "idle");
				return;
			}
			if (deployableBarrierState != DeployableBarrierState.Deployed)
			{
				return;
			}
			spriteComponent.LayerSetState(0, "deployed");
		}
	}
}
