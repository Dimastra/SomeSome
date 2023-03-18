using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Shared.Cooldown;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared.Timing
{
	// Token: 0x020000CA RID: 202
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class UseDelaySystem : EntitySystem
	{
		// Token: 0x06000224 RID: 548 RVA: 0x0000AB80 File Offset: 0x00008D80
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<UseDelayComponent, ComponentGetState>(new ComponentEventRefHandler<UseDelayComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<UseDelayComponent, ComponentHandleState>(new ComponentEventRefHandler<UseDelayComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeLocalEvent<UseDelayComponent, EntityPausedEvent>(new ComponentEventRefHandler<UseDelayComponent, EntityPausedEvent>(this.OnPaused), null, null);
			base.SubscribeLocalEvent<UseDelayComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<UseDelayComponent, EntityUnpausedEvent>(this.OnUnpaused), null, null);
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0000ABE4 File Offset: 0x00008DE4
		private void OnPaused(EntityUid uid, UseDelayComponent component, ref EntityPausedEvent args)
		{
			if (component.DelayEndTime != null)
			{
				component.RemainingDelay = this._gameTiming.CurTime - component.DelayEndTime;
			}
			this._activeDelays.Remove(component);
			base.Dirty(component, null);
		}

		// Token: 0x06000226 RID: 550 RVA: 0x0000AC54 File Offset: 0x00008E54
		private void OnUnpaused(EntityUid uid, UseDelayComponent component, ref EntityUnpausedEvent args)
		{
			if (component.RemainingDelay == null)
			{
				return;
			}
			component.DelayEndTime = this._gameTiming.CurTime + component.RemainingDelay;
			base.Dirty(component, null);
			this._activeDelays.Add(component);
		}

		// Token: 0x06000227 RID: 551 RVA: 0x0000ACC4 File Offset: 0x00008EC4
		private void OnHandleState(EntityUid uid, UseDelayComponent component, ref ComponentHandleState args)
		{
			UseDelayComponentState state = args.Current as UseDelayComponentState;
			if (state == null)
			{
				return;
			}
			component.LastUseTime = state.LastUseTime;
			component.Delay = state.Delay;
			component.DelayEndTime = state.DelayEndTime;
			if (component.DelayEndTime == null)
			{
				this._activeDelays.Remove(component);
				return;
			}
			this._activeDelays.Add(component);
		}

		// Token: 0x06000228 RID: 552 RVA: 0x0000AD2D File Offset: 0x00008F2D
		private void OnGetState(EntityUid uid, UseDelayComponent component, ref ComponentGetState args)
		{
			args.State = new UseDelayComponentState(component.LastUseTime, component.Delay, component.DelayEndTime);
		}

		// Token: 0x06000229 RID: 553 RVA: 0x0000AD4C File Offset: 0x00008F4C
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			RemQueue<UseDelayComponent> toRemove = default(RemQueue<UseDelayComponent>);
			TimeSpan curTime = this._gameTiming.CurTime;
			EntityQuery<MetaDataComponent> mQuery = this.EntityManager.GetEntityQuery<MetaDataComponent>();
			foreach (UseDelayComponent delay in this._activeDelays)
			{
				if (delay.DelayEndTime != null && !(curTime > delay.DelayEndTime) && !base.Deleted(delay.Owner, mQuery))
				{
					CancellationTokenSource cancellationTokenSource = delay.CancellationTokenSource;
					if (cancellationTokenSource == null || !cancellationTokenSource.Token.IsCancellationRequested)
					{
						continue;
					}
				}
				toRemove.Add(delay);
			}
			foreach (UseDelayComponent delay2 in toRemove)
			{
				delay2.CancellationTokenSource = null;
				delay2.DelayEndTime = null;
				this._activeDelays.Remove(delay2);
				base.Dirty(delay2, null);
			}
		}

		// Token: 0x0600022A RID: 554 RVA: 0x0000AE94 File Offset: 0x00009094
		[NullableContext(2)]
		public void BeginDelay(EntityUid uid, UseDelayComponent component = null)
		{
			if (!base.Resolve<UseDelayComponent>(uid, ref component, false))
			{
				return;
			}
			if (component.ActiveDelay || base.Deleted(uid, null))
			{
				return;
			}
			component.CancellationTokenSource = new CancellationTokenSource();
			this._activeDelays.Add(component);
			TimeSpan currentTime = this._gameTiming.CurTime;
			component.LastUseTime = currentTime;
			component.DelayEndTime = new TimeSpan?(currentTime + component.Delay);
			base.Dirty(component, null);
			ItemCooldownComponent itemCooldownComponent = base.EnsureComp<ItemCooldownComponent>(component.Owner);
			itemCooldownComponent.CooldownStart = new TimeSpan?(currentTime);
			itemCooldownComponent.CooldownEnd = component.DelayEndTime;
		}

		// Token: 0x0600022B RID: 555 RVA: 0x0000AF2E File Offset: 0x0000912E
		[NullableContext(2)]
		public bool ActiveDelay(EntityUid uid, UseDelayComponent component = null)
		{
			return base.Resolve<UseDelayComponent>(uid, ref component, false) && component.ActiveDelay;
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000AF44 File Offset: 0x00009144
		public void Cancel(UseDelayComponent component)
		{
			CancellationTokenSource cancellationTokenSource = component.CancellationTokenSource;
			if (cancellationTokenSource != null)
			{
				cancellationTokenSource.Cancel();
			}
			component.CancellationTokenSource = null;
			component.DelayEndTime = null;
			this._activeDelays.Remove(component);
			base.Dirty(component, null);
			ItemCooldownComponent cooldown;
			if (base.TryComp<ItemCooldownComponent>(component.Owner, ref cooldown))
			{
				cooldown.CooldownEnd = new TimeSpan?(this._gameTiming.CurTime);
			}
		}

		// Token: 0x0600022D RID: 557 RVA: 0x0000AFB0 File Offset: 0x000091B0
		public void Restart(UseDelayComponent component)
		{
			CancellationTokenSource cancellationTokenSource = component.CancellationTokenSource;
			if (cancellationTokenSource != null)
			{
				cancellationTokenSource.Cancel();
			}
			component.CancellationTokenSource = null;
			this.BeginDelay(component.Owner, component);
		}

		// Token: 0x040002AD RID: 685
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x040002AE RID: 686
		private HashSet<UseDelayComponent> _activeDelays = new HashSet<UseDelayComponent>();
	}
}
