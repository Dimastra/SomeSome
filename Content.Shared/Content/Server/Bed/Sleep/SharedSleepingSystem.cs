using System;
using System.Runtime.CompilerServices;
using Content.Shared.Bed.Sleep;
using Content.Shared.Eye.Blinding;
using Content.Shared.Speech;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Server.Bed.Sleep
{
	// Token: 0x0200000D RID: 13
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedSleepingSystem : EntitySystem
	{
		// Token: 0x0600000E RID: 14 RVA: 0x00002104 File Offset: 0x00000304
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<SleepingComponent, ComponentInit>(new ComponentEventHandler<SleepingComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<SleepingComponent, ComponentShutdown>(new ComponentEventHandler<SleepingComponent, ComponentShutdown>(this.OnShutdown), null, null);
			base.SubscribeLocalEvent<SleepingComponent, SpeakAttemptEvent>(new ComponentEventHandler<SleepingComponent, SpeakAttemptEvent>(this.OnSpeakAttempt), null, null);
			base.SubscribeLocalEvent<SleepingComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<SleepingComponent, EntityUnpausedEvent>(this.OnSleepUnpaused), null, null);
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002167 File Offset: 0x00000367
		private void OnSleepUnpaused(EntityUid uid, SleepingComponent component, ref EntityUnpausedEvent args)
		{
			component.CoolDownEnd += args.PausedTime;
			base.Dirty(component, null);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002188 File Offset: 0x00000388
		private void OnInit(EntityUid uid, SleepingComponent component, ComponentInit args)
		{
			SleepStateChangedEvent ev = new SleepStateChangedEvent(true);
			base.RaiseLocalEvent<SleepStateChangedEvent>(uid, ev, false);
			this._blindingSystem.AdjustBlindSources(uid, 1, null);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000021B4 File Offset: 0x000003B4
		private void OnShutdown(EntityUid uid, SleepingComponent component, ComponentShutdown args)
		{
			SleepStateChangedEvent ev = new SleepStateChangedEvent(false);
			base.RaiseLocalEvent<SleepStateChangedEvent>(uid, ev, false);
			this._blindingSystem.AdjustBlindSources(uid, -1, null);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000021DF File Offset: 0x000003DF
		private void OnSpeakAttempt(EntityUid uid, SleepingComponent component, SpeakAttemptEvent args)
		{
			args.Cancel();
		}

		// Token: 0x0400000B RID: 11
		[Dependency]
		private readonly SharedBlindingSystem _blindingSystem;
	}
}
