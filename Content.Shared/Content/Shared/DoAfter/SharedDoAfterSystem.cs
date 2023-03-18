using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Mobs;
using Content.Shared.Stunnable;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Timing;

namespace Content.Shared.DoAfter
{
	// Token: 0x020004F9 RID: 1273
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedDoAfterSystem : EntitySystem
	{
		// Token: 0x06000F5D RID: 3933 RVA: 0x00031974 File Offset: 0x0002FB74
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DoAfterComponent, DamageChangedEvent>(new ComponentEventHandler<DoAfterComponent, DamageChangedEvent>(this.OnDamage), null, null);
			base.SubscribeLocalEvent<DoAfterComponent, MobStateChangedEvent>(new ComponentEventHandler<DoAfterComponent, MobStateChangedEvent>(this.OnStateChanged), null, null);
			base.SubscribeLocalEvent<DoAfterComponent, ComponentGetState>(new ComponentEventRefHandler<DoAfterComponent, ComponentGetState>(this.OnDoAfterGetState), null, null);
		}

		// Token: 0x06000F5E RID: 3934 RVA: 0x000319C4 File Offset: 0x0002FBC4
		private void Add(EntityUid entity, DoAfterComponent component, DoAfter doAfter)
		{
			doAfter.ID = component.RunningIndex;
			doAfter.Delay = doAfter.EventArgs.Delay;
			component.DoAfters.Add(component.RunningIndex, doAfter);
			base.EnsureComp<ActiveDoAfterComponent>(entity);
			component.RunningIndex += 1;
			base.Dirty(component, null);
		}

		// Token: 0x06000F5F RID: 3935 RVA: 0x00031A1F File Offset: 0x0002FC1F
		private void OnDoAfterGetState(EntityUid uid, DoAfterComponent component, ref ComponentGetState args)
		{
			args.State = new DoAfterComponentState(component.DoAfters);
		}

		// Token: 0x06000F60 RID: 3936 RVA: 0x00031A34 File Offset: 0x0002FC34
		private void Cancelled(DoAfterComponent component, DoAfter doAfter)
		{
			DoAfter index;
			if (!component.DoAfters.TryGetValue(doAfter.ID, out index))
			{
				return;
			}
			component.DoAfters.Remove(doAfter.ID);
			if (component.DoAfters.Count == 0)
			{
				base.RemComp<ActiveDoAfterComponent>(component.Owner);
			}
			base.RaiseNetworkEvent(new CancelledDoAfterMessage(component.Owner, index.ID));
		}

		// Token: 0x06000F61 RID: 3937 RVA: 0x00031A9C File Offset: 0x0002FC9C
		private void Finished(DoAfterComponent component, DoAfter doAfter)
		{
			if (!component.DoAfters.ContainsKey(doAfter.ID))
			{
				return;
			}
			component.DoAfters.Remove(doAfter.ID);
			if (component.DoAfters.Count == 0)
			{
				base.RemComp<ActiveDoAfterComponent>(component.Owner);
			}
		}

		// Token: 0x06000F62 RID: 3938 RVA: 0x00031AEC File Offset: 0x0002FCEC
		private void OnStateChanged(EntityUid uid, DoAfterComponent component, MobStateChangedEvent args)
		{
			if (args.NewMobState != MobState.Dead || args.NewMobState != MobState.Critical)
			{
				return;
			}
			foreach (KeyValuePair<byte, DoAfter> keyValuePair in component.DoAfters)
			{
				byte b;
				DoAfter doAfter2;
				keyValuePair.Deconstruct(out b, out doAfter2);
				DoAfter doAfter = doAfter2;
				this.Cancel(uid, doAfter, component);
			}
		}

		// Token: 0x06000F63 RID: 3939 RVA: 0x00031B64 File Offset: 0x0002FD64
		private void OnDamage(EntityUid uid, DoAfterComponent component, DamageChangedEvent args)
		{
			if (!args.InterruptsDoAfters || !args.DamageIncreased || args.DamageDelta == null)
			{
				return;
			}
			foreach (DoAfter doAfter in component.DoAfters.Values)
			{
				if (doAfter.EventArgs.BreakOnDamage)
				{
					DamageSpecifier damageDelta = args.DamageDelta;
					float? num = (damageDelta != null) ? new float?(damageDelta.Total.Float()) : null;
					if (((num != null) ? new FixedPoint2?(num.GetValueOrDefault()) : null) > doAfter.EventArgs.DamageThreshold)
					{
						this.Cancel(uid, doAfter, component);
					}
				}
			}
		}

		// Token: 0x06000F64 RID: 3940 RVA: 0x00031C74 File Offset: 0x0002FE74
		public override void Update(float frameTime)
		{
			base.Update(frameTime);
			foreach (ValueTuple<ActiveDoAfterComponent, DoAfterComponent> valueTuple in this.EntityManager.EntityQuery<ActiveDoAfterComponent, DoAfterComponent>(false))
			{
				DoAfterComponent comp = valueTuple.Item2;
				foreach (DoAfter doAfter in comp.DoAfters.Values.ToArray<DoAfter>())
				{
					this.Run(comp.Owner, comp, doAfter);
					switch (doAfter.Status)
					{
					case DoAfterStatus.Running:
						break;
					case DoAfterStatus.Cancelled:
						this._pending.Enqueue(doAfter);
						break;
					case DoAfterStatus.Finished:
						this._pending.Enqueue(doAfter);
						break;
					default:
						throw new ArgumentOutOfRangeException();
					}
				}
				DoAfter doAfter2;
				while (this._pending.TryDequeue(out doAfter2))
				{
					if (doAfter2.Status == DoAfterStatus.Cancelled)
					{
						this.Cancelled(comp, doAfter2);
						if (doAfter2.Done != null)
						{
							doAfter2.Done(true);
						}
					}
					if (doAfter2.Status == DoAfterStatus.Finished)
					{
						this.Finished(comp, doAfter2);
						if (doAfter2.Done != null)
						{
							doAfter2.Done(false);
						}
					}
				}
			}
		}

		// Token: 0x06000F65 RID: 3941 RVA: 0x00031DAC File Offset: 0x0002FFAC
		[Obsolete("Use the synchronous version instead, DoAfter")]
		public Task<DoAfterStatus> WaitDoAfter(DoAfterEventArgs eventArgs)
		{
			SharedDoAfterSystem.<WaitDoAfter>d__10 <WaitDoAfter>d__;
			<WaitDoAfter>d__.<>t__builder = AsyncTaskMethodBuilder<DoAfterStatus>.Create();
			<WaitDoAfter>d__.<>4__this = this;
			<WaitDoAfter>d__.eventArgs = eventArgs;
			<WaitDoAfter>d__.<>1__state = -1;
			<WaitDoAfter>d__.<>t__builder.Start<SharedDoAfterSystem.<WaitDoAfter>d__10>(ref <WaitDoAfter>d__);
			return <WaitDoAfter>d__.<>t__builder.Task;
		}

		// Token: 0x06000F66 RID: 3942 RVA: 0x00031DF8 File Offset: 0x0002FFF8
		public void DoAfter<[Nullable(2)] T>(DoAfterEventArgs eventArgs, T data)
		{
			this.CreateDoAfter(eventArgs).Done = delegate(bool cancelled)
			{
				this.Send<T>(data, cancelled, eventArgs);
			};
		}

		// Token: 0x06000F67 RID: 3943 RVA: 0x00031E40 File Offset: 0x00030040
		public DoAfter DoAfter(DoAfterEventArgs eventArgs)
		{
			DoAfter doAfter = this.CreateDoAfter(eventArgs);
			doAfter.Done = delegate(bool cancelled)
			{
				this.Send(cancelled, eventArgs);
			};
			return doAfter;
		}

		// Token: 0x06000F68 RID: 3944 RVA: 0x00031E80 File Offset: 0x00030080
		private DoAfter CreateDoAfter(DoAfterEventArgs eventArgs)
		{
			eventArgs.CancelToken = default(CancellationToken);
			DoAfter doAfter = new DoAfter(eventArgs, this.EntityManager);
			DoAfterComponent doAfterComponent = base.Comp<DoAfterComponent>(eventArgs.User);
			doAfter.ID = doAfterComponent.RunningIndex;
			doAfter.StartTime = this.GameTiming.CurTime;
			this.Add(eventArgs.User, doAfterComponent, doAfter);
			return doAfter;
		}

		// Token: 0x06000F69 RID: 3945 RVA: 0x00031EE0 File Offset: 0x000300E0
		private void Run(EntityUid entity, DoAfterComponent comp, DoAfter doAfter)
		{
			DoAfterStatus status = doAfter.Status;
			if (status != DoAfterStatus.Running)
			{
				if (status - DoAfterStatus.Cancelled <= 1)
				{
					return;
				}
				throw new ArgumentOutOfRangeException();
			}
			else
			{
				doAfter.Elapsed = this.GameTiming.CurTime - doAfter.StartTime;
				if (!this.IsFinished(doAfter))
				{
					if (this.IsCancelled(doAfter))
					{
						this.Cancel(entity, doAfter, comp);
					}
					return;
				}
				if (!this.TryPostCheck(doAfter))
				{
					this.Cancel(entity, doAfter, comp);
					return;
				}
				doAfter.Tcs.SetResult(DoAfterStatus.Finished);
				return;
			}
		}

		// Token: 0x06000F6A RID: 3946 RVA: 0x00031F5B File Offset: 0x0003015B
		private bool TryPostCheck(DoAfter doAfter)
		{
			Func<bool> postCheck = doAfter.EventArgs.PostCheck;
			return postCheck == null || postCheck();
		}

		// Token: 0x06000F6B RID: 3947 RVA: 0x00031F74 File Offset: 0x00030174
		private bool IsFinished(DoAfter doAfter)
		{
			TimeSpan delay = TimeSpan.FromSeconds((double)doAfter.EventArgs.Delay);
			return !(doAfter.Elapsed <= delay);
		}

		// Token: 0x06000F6C RID: 3948 RVA: 0x00031FA4 File Offset: 0x000301A4
		private bool IsCancelled(DoAfter doAfter)
		{
			DoAfterEventArgs eventArgs = doAfter.EventArgs;
			EntityQuery<TransformComponent> xForm = base.GetEntityQuery<TransformComponent>();
			if (base.Exists(eventArgs.User))
			{
				EntityUid? target2 = eventArgs.Target;
				if (target2 != null)
				{
					EntityUid target = target2.GetValueOrDefault();
					if (!base.Exists(target))
					{
						return true;
					}
				}
				if (eventArgs.CancelToken.IsCancellationRequested)
				{
					return true;
				}
				if (eventArgs.BreakOnUserMove && !xForm.GetComponent(eventArgs.User).Coordinates.InRange(this.EntityManager, doAfter.UserGrid, eventArgs.MovementThreshold))
				{
					return true;
				}
				if (eventArgs.Target != null && eventArgs.BreakOnTargetMove && !xForm.GetComponent(eventArgs.Target.Value).Coordinates.InRange(this.EntityManager, doAfter.TargetGrid, eventArgs.MovementThreshold))
				{
					return true;
				}
				if (eventArgs.ExtraCheck != null && !eventArgs.ExtraCheck())
				{
					return true;
				}
				if (eventArgs.BreakOnStun && base.HasComp<StunnedComponent>(eventArgs.User))
				{
					return true;
				}
				if (eventArgs.NeedHand)
				{
					SharedHandsComponent handsComp;
					if (!base.TryComp<SharedHandsComponent>(eventArgs.User, ref handsComp))
					{
						if (doAfter.ActiveHand != null)
						{
							return true;
						}
					}
					else
					{
						Hand activeHand = handsComp.ActiveHand;
						string currentActiveHand = (activeHand != null) ? activeHand.Name : null;
						if (doAfter.ActiveHand != currentActiveHand)
						{
							return true;
						}
						EntityUid? activeHandEntity = handsComp.ActiveHandEntity;
						if (doAfter.ActiveItem != activeHandEntity)
						{
							return true;
						}
					}
				}
				if (eventArgs.DistanceThreshold != null)
				{
					TransformComponent userXform = xForm.GetComponent(eventArgs.User);
					if (eventArgs.Target != null && !eventArgs.User.Equals(eventArgs.Target))
					{
						EntityCoordinates targetCoords = xForm.GetComponent(eventArgs.Target.Value).Coordinates;
						if (!userXform.Coordinates.InRange(this.EntityManager, targetCoords, eventArgs.DistanceThreshold.Value))
						{
							return true;
						}
					}
					if (eventArgs.Used != null)
					{
						EntityCoordinates usedCoords = xForm.GetComponent(eventArgs.Used.Value).Coordinates;
						if (!userXform.Coordinates.InRange(this.EntityManager, usedCoords, eventArgs.DistanceThreshold.Value))
						{
							return true;
						}
					}
				}
				return false;
			}
			return true;
		}

		// Token: 0x06000F6D RID: 3949 RVA: 0x0003221C File Offset: 0x0003041C
		public void Cancel(EntityUid entity, DoAfter doAfter, [Nullable(2)] DoAfterComponent comp = null)
		{
			if (!base.Resolve<DoAfterComponent>(entity, ref comp, false))
			{
				return;
			}
			if (comp.CancelledDoAfters.ContainsKey(doAfter.ID))
			{
				return;
			}
			if (!comp.DoAfters.ContainsKey(doAfter.ID))
			{
				return;
			}
			doAfter.Cancelled = true;
			doAfter.CancelledTime = this.GameTiming.CurTime;
			DoAfter doAfterMessage = comp.DoAfters[doAfter.ID];
			comp.CancelledDoAfters.Add(doAfter.ID, doAfterMessage);
			if (doAfter.Status == DoAfterStatus.Running)
			{
				doAfter.Tcs.SetResult(DoAfterStatus.Cancelled);
			}
		}

		// Token: 0x06000F6E RID: 3950 RVA: 0x000322B0 File Offset: 0x000304B0
		private void Send(bool cancelled, DoAfterEventArgs args)
		{
			DoAfterEvent ev = new DoAfterEvent(cancelled, args);
			this.RaiseDoAfterEvent<DoAfterEvent>(ev, args);
		}

		// Token: 0x06000F6F RID: 3951 RVA: 0x000322D0 File Offset: 0x000304D0
		private void Send<[Nullable(2)] T>(T data, bool cancelled, DoAfterEventArgs args)
		{
			DoAfterEvent<T> ev = new DoAfterEvent<T>(data, cancelled, args);
			this.RaiseDoAfterEvent<DoAfterEvent<T>>(ev, args);
		}

		// Token: 0x06000F70 RID: 3952 RVA: 0x000322F0 File Offset: 0x000304F0
		private void RaiseDoAfterEvent<TEvent>(TEvent ev, DoAfterEventArgs args)
		{
			if (this.EntityManager.EntityExists(args.User) && args.RaiseOnUser)
			{
				base.RaiseLocalEvent<TEvent>(args.User, ev, args.Broadcast);
			}
			EntityUid? entityUid = args.Target;
			if (entityUid != null)
			{
				EntityUid target = entityUid.GetValueOrDefault();
				if (this.EntityManager.EntityExists(target) && args.RaiseOnTarget)
				{
					base.RaiseLocalEvent<TEvent>(target, ev, args.Broadcast);
				}
			}
			entityUid = args.Used;
			if (entityUid != null)
			{
				EntityUid used = entityUid.GetValueOrDefault();
				if (this.EntityManager.EntityExists(used) && args.RaiseOnUsed)
				{
					base.RaiseLocalEvent<TEvent>(used, ev, args.Broadcast);
				}
			}
		}

		// Token: 0x04000EB5 RID: 3765
		[Dependency]
		protected readonly IGameTiming GameTiming;

		// Token: 0x04000EB6 RID: 3766
		private readonly Queue<DoAfter> _pending = new Queue<DoAfter>();
	}
}
