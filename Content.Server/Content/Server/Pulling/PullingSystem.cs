using System;
using System.Runtime.CompilerServices;
using Content.Shared.Input;
using Content.Shared.Pulling;
using Content.Shared.Pulling.Components;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.Players;

namespace Content.Server.Pulling
{
	// Token: 0x0200026A RID: 618
	public sealed class PullingSystem : SharedPullingSystem
	{
		// Token: 0x06000C4C RID: 3148 RVA: 0x00040878 File Offset: 0x0003EA78
		public override void Initialize()
		{
			base.Initialize();
			base.UpdatesAfter.Add(typeof(PhysicsSystem));
			base.SubscribeLocalEvent<SharedPullableComponent, PullableMoveMessage>(new ComponentEventHandler<SharedPullableComponent, PullableMoveMessage>(base.OnPullableMove), null, null);
			base.SubscribeLocalEvent<SharedPullableComponent, PullableStopMovingMessage>(new ComponentEventHandler<SharedPullableComponent, PullableStopMovingMessage>(base.OnPullableStopMove), null, null);
			CommandBinds.Builder.Bind(ContentKeyFunctions.ReleasePulledObject, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(this.HandleReleasePulledObject), null, true, true)).Register<PullingSystem>();
		}

		// Token: 0x06000C4D RID: 3149 RVA: 0x000408F4 File Offset: 0x0003EAF4
		[NullableContext(2)]
		private void HandleReleasePulledObject(ICommonSession session)
		{
			EntityUid? entityUid = (session != null) ? session.AttachedEntity : null;
			if (entityUid != null)
			{
				EntityUid player = entityUid.GetValueOrDefault();
				if (player.Valid)
				{
					EntityUid? pulled;
					if (!base.TryGetPulled(player, out pulled))
					{
						return;
					}
					SharedPullableComponent pullable;
					if (!this.EntityManager.TryGetComponent<SharedPullableComponent>(pulled.Value, ref pullable))
					{
						return;
					}
					base.TryStopPull(pullable, null);
					return;
				}
			}
		}
	}
}
