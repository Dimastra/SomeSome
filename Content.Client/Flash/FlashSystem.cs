using System;
using System.Runtime.CompilerServices;
using Content.Shared.Flash;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Client.Flash
{
	// Token: 0x02000317 RID: 791
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class FlashSystem : SharedFlashSystem
	{
		// Token: 0x060013EF RID: 5103 RVA: 0x000752E5 File Offset: 0x000734E5
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<FlashableComponent, ComponentHandleState>(new ComponentEventRefHandler<FlashableComponent, ComponentHandleState>(this.OnFlashableHandleState), null, null);
		}

		// Token: 0x060013F0 RID: 5104 RVA: 0x00075304 File Offset: 0x00073504
		private void OnFlashableHandleState(EntityUid uid, FlashableComponent component, ref ComponentHandleState args)
		{
			FlashableComponentState flashableComponentState = args.Current as FlashableComponentState;
			if (flashableComponentState == null)
			{
				return;
			}
			if (this._playerManager.LocalPlayer != null)
			{
				EntityUid? attachedEntity = this._playerManager.LocalPlayer.Session.AttachedEntity;
				if (attachedEntity == null || (attachedEntity != null && attachedEntity.GetValueOrDefault() != uid))
				{
					return;
				}
			}
			if (flashableComponentState.Time == default(TimeSpan))
			{
				return;
			}
			double totalSeconds = this._gameTiming.CurTime.TotalSeconds;
			double num = flashableComponentState.Time.TotalSeconds + (double)flashableComponentState.Duration;
			if (component.LastFlash.TotalSeconds + (double)component.Duration > num)
			{
				return;
			}
			if (totalSeconds > num)
			{
				return;
			}
			component.LastFlash = flashableComponentState.Time;
			component.Duration = flashableComponentState.Duration;
			this._overlayManager.GetOverlay<FlashOverlay>().ReceiveFlash((double)component.Duration);
		}

		// Token: 0x04000A06 RID: 2566
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04000A07 RID: 2567
		[Dependency]
		private readonly IPlayerManager _playerManager;

		// Token: 0x04000A08 RID: 2568
		[Dependency]
		private readonly IOverlayManager _overlayManager;
	}
}
