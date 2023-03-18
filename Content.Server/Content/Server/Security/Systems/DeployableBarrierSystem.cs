using System;
using System.Runtime.CompilerServices;
using Content.Server.Pulling;
using Content.Server.Security.Components;
using Content.Shared.Lock;
using Content.Shared.Pulling.Components;
using Content.Shared.Security;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Security.Systems
{
	// Token: 0x02000214 RID: 532
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DeployableBarrierSystem : EntitySystem
	{
		// Token: 0x06000A8D RID: 2701 RVA: 0x00037633 File Offset: 0x00035833
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DeployableBarrierComponent, ComponentStartup>(new ComponentEventHandler<DeployableBarrierComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<DeployableBarrierComponent, LockToggledEvent>(new ComponentEventRefHandler<DeployableBarrierComponent, LockToggledEvent>(this.OnLockToggled), null, null);
		}

		// Token: 0x06000A8E RID: 2702 RVA: 0x00037664 File Offset: 0x00035864
		private void OnStartup(EntityUid uid, DeployableBarrierComponent component, ComponentStartup args)
		{
			LockComponent lockComponent;
			if (!base.TryComp<LockComponent>(uid, ref lockComponent))
			{
				return;
			}
			this.ToggleBarrierDeploy(uid, lockComponent.Locked);
		}

		// Token: 0x06000A8F RID: 2703 RVA: 0x0003768A File Offset: 0x0003588A
		private void OnLockToggled(EntityUid uid, DeployableBarrierComponent component, ref LockToggledEvent args)
		{
			this.ToggleBarrierDeploy(uid, args.Locked);
		}

		// Token: 0x06000A90 RID: 2704 RVA: 0x0003769C File Offset: 0x0003589C
		private void ToggleBarrierDeploy(EntityUid uid, bool isDeployed)
		{
			base.Transform(uid).Anchored = isDeployed;
			DeployableBarrierState state = isDeployed ? DeployableBarrierState.Deployed : DeployableBarrierState.Idle;
			this._appearance.SetData(uid, DeployableBarrierVisuals.State, state, null);
			SharedPullableComponent pullable;
			if (base.TryComp<SharedPullableComponent>(uid, ref pullable))
			{
				this._pulling.TryStopPull(pullable, null);
			}
			PointLightComponent light;
			if (base.TryComp<PointLightComponent>(uid, ref light))
			{
				light.Enabled = isDeployed;
			}
		}

		// Token: 0x04000675 RID: 1653
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04000676 RID: 1654
		[Dependency]
		private readonly PullingSystem _pulling;
	}
}
