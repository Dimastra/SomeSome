using System;
using System.Runtime.CompilerServices;
using Content.Server.Advertisements;
using Content.Server.Chat.Systems;
using Content.Server.Power.Components;
using Content.Shared.VendingMachines;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.Advertise
{
	// Token: 0x020007FA RID: 2042
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AdvertiseSystem : EntitySystem
	{
		// Token: 0x06002C25 RID: 11301 RVA: 0x000E6DDC File Offset: 0x000E4FDC
		public override void Initialize()
		{
			base.SubscribeLocalEvent<AdvertiseComponent, ComponentInit>(new ComponentEventHandler<AdvertiseComponent, ComponentInit>(this.OnComponentInit), null, null);
			base.SubscribeLocalEvent<AdvertiseComponent, PowerChangedEvent>(new ComponentEventRefHandler<AdvertiseComponent, PowerChangedEvent>(this.OnPowerChanged), null, null);
			base.SubscribeLocalEvent<ApcPowerReceiverComponent, AdvertiseEnableChangeAttemptEvent>(new ComponentEventHandler<ApcPowerReceiverComponent, AdvertiseEnableChangeAttemptEvent>(this.OnPowerReceiverEnableChangeAttempt), null, null);
			base.SubscribeLocalEvent<VendingMachineComponent, AdvertiseEnableChangeAttemptEvent>(new ComponentEventHandler<VendingMachineComponent, AdvertiseEnableChangeAttemptEvent>(this.OnVendingEnableChangeAttempt), null, null);
		}

		// Token: 0x06002C26 RID: 11302 RVA: 0x000E6E39 File Offset: 0x000E5039
		private void OnComponentInit(EntityUid uid, AdvertiseComponent advertise, ComponentInit args)
		{
			this.RefreshTimer(uid, true, advertise);
		}

		// Token: 0x06002C27 RID: 11303 RVA: 0x000E6E44 File Offset: 0x000E5044
		private void OnPowerChanged(EntityUid uid, AdvertiseComponent advertise, ref PowerChangedEvent args)
		{
			this.SetEnabled(uid, args.Powered, advertise);
		}

		// Token: 0x06002C28 RID: 11304 RVA: 0x000E6E54 File Offset: 0x000E5054
		[NullableContext(2)]
		public void RefreshTimer(EntityUid uid, bool minimumBound = true, AdvertiseComponent advertise = null)
		{
			if (!base.Resolve<AdvertiseComponent>(uid, ref advertise, true))
			{
				return;
			}
			int minWait = Math.Max(1, advertise.MinimumWait);
			int maxWait = Math.Max(minWait, advertise.MaximumWait);
			int waitSeconds = minimumBound ? this._random.Next(minWait, maxWait) : this._random.Next(maxWait);
			advertise.NextAdvertisementTime = this._gameTiming.CurTime.Add(TimeSpan.FromSeconds((double)waitSeconds));
		}

		// Token: 0x06002C29 RID: 11305 RVA: 0x000E6EC8 File Offset: 0x000E50C8
		[NullableContext(2)]
		public void SayAdvertisement(EntityUid uid, bool refresh = true, AdvertiseComponent advertise = null)
		{
			if (!base.Resolve<AdvertiseComponent>(uid, ref advertise, true))
			{
				return;
			}
			AdvertisementsPackPrototype advertisements;
			if (this._prototypeManager.TryIndex<AdvertisementsPackPrototype>(advertise.PackPrototypeId, ref advertisements))
			{
				this._chat.TrySendInGameICMessage(advertise.Owner, Loc.GetString(RandomExtensions.Pick<string>(this._random, advertisements.Advertisements)), InGameICChatType.Speak, true, false, null, null, null, true, false);
			}
			if (refresh)
			{
				this.RefreshTimer(uid, true, advertise);
			}
		}

		// Token: 0x06002C2A RID: 11306 RVA: 0x000E6F34 File Offset: 0x000E5134
		[NullableContext(2)]
		public void SetEnabled(EntityUid uid, bool enabled, AdvertiseComponent advertise = null)
		{
			if (!base.Resolve<AdvertiseComponent>(uid, ref advertise, true))
			{
				return;
			}
			AdvertiseEnableChangeAttemptEvent attemptEvent = new AdvertiseEnableChangeAttemptEvent(enabled, advertise.Enabled);
			base.RaiseLocalEvent<AdvertiseEnableChangeAttemptEvent>(uid, attemptEvent, false);
			if (attemptEvent.Cancelled)
			{
				return;
			}
			if (enabled)
			{
				this.RefreshTimer(uid, !advertise.Enabled, advertise);
			}
			advertise.Enabled = enabled;
		}

		// Token: 0x06002C2B RID: 11307 RVA: 0x000E6F88 File Offset: 0x000E5188
		private void OnPowerReceiverEnableChangeAttempt(EntityUid uid, ApcPowerReceiverComponent component, AdvertiseEnableChangeAttemptEvent args)
		{
			if (args.NewState && !component.Powered)
			{
				args.Cancel();
			}
		}

		// Token: 0x06002C2C RID: 11308 RVA: 0x000E6FA0 File Offset: 0x000E51A0
		private void OnVendingEnableChangeAttempt(EntityUid uid, VendingMachineComponent component, AdvertiseEnableChangeAttemptEvent args)
		{
			if (args.NewState && component.Broken)
			{
				args.Cancel();
			}
		}

		// Token: 0x06002C2D RID: 11309 RVA: 0x000E6FB8 File Offset: 0x000E51B8
		public override void Update(float frameTime)
		{
			this._timer += frameTime;
			if (this._timer < 5f)
			{
				return;
			}
			this._timer -= 5f;
			TimeSpan curTime = this._gameTiming.CurTime;
			foreach (AdvertiseComponent advertise in this.EntityManager.EntityQuery<AdvertiseComponent>(false))
			{
				if (advertise.Enabled && !(advertise.NextAdvertisementTime > curTime))
				{
					this.SayAdvertisement(advertise.Owner, true, advertise);
				}
			}
		}

		// Token: 0x04001B5A RID: 7002
		[Dependency]
		private readonly IPrototypeManager _prototypeManager;

		// Token: 0x04001B5B RID: 7003
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04001B5C RID: 7004
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x04001B5D RID: 7005
		[Dependency]
		private readonly ChatSystem _chat;

		// Token: 0x04001B5E RID: 7006
		private const float UpdateTimer = 5f;

		// Token: 0x04001B5F RID: 7007
		private float _timer;
	}
}
