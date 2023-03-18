using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Access.Components;
using Content.Shared.Damage;
using Content.Shared.Doors.Components;
using Content.Shared.Hands.Components;
using Content.Shared.Interaction;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;

namespace Content.Shared.Doors.Systems
{
	// Token: 0x020004E9 RID: 1257
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedDoorSystem : EntitySystem
	{
		// Token: 0x06000F32 RID: 3890 RVA: 0x000309A0 File Offset: 0x0002EBA0
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DoorComponent, ComponentInit>(new ComponentEventHandler<DoorComponent, ComponentInit>(this.OnInit), null, null);
			base.SubscribeLocalEvent<DoorComponent, ComponentRemove>(new ComponentEventHandler<DoorComponent, ComponentRemove>(this.OnRemove), null, null);
			base.SubscribeLocalEvent<DoorComponent, ComponentGetState>(new ComponentEventRefHandler<DoorComponent, ComponentGetState>(this.OnGetState), null, null);
			base.SubscribeLocalEvent<DoorComponent, ComponentHandleState>(new ComponentEventRefHandler<DoorComponent, ComponentHandleState>(this.OnHandleState), null, null);
			base.SubscribeLocalEvent<DoorComponent, ActivateInWorldEvent>(new ComponentEventHandler<DoorComponent, ActivateInWorldEvent>(this.OnActivate), null, null);
			base.SubscribeLocalEvent<DoorComponent, StartCollideEvent>(new ComponentEventRefHandler<DoorComponent, StartCollideEvent>(this.HandleCollide), null, null);
			base.SubscribeLocalEvent<DoorComponent, PreventCollideEvent>(new ComponentEventRefHandler<DoorComponent, PreventCollideEvent>(this.PreventCollision), null, null);
		}

		// Token: 0x06000F33 RID: 3891 RVA: 0x00030A44 File Offset: 0x0002EC44
		private void OnInit(EntityUid uid, DoorComponent door, ComponentInit args)
		{
			if (door.NextStateChange != null)
			{
				this._activeDoors.Add(door);
			}
			else
			{
				if (door.State == DoorState.Opening)
				{
					door.State = DoorState.Open;
					door.Partial = false;
				}
				if (door.State == DoorState.Closing)
				{
					door.State = DoorState.Closed;
					door.Partial = false;
				}
			}
			bool collidable = door.State == DoorState.Closed || (door.State == DoorState.Closing && door.Partial) || (door.State == DoorState.Opening && !door.Partial);
			this.SetCollidable(uid, collidable, door, null, null);
			this.UpdateAppearance(uid, door);
		}

		// Token: 0x06000F34 RID: 3892 RVA: 0x00030AE0 File Offset: 0x0002ECE0
		private void OnRemove(EntityUid uid, DoorComponent door, ComponentRemove args)
		{
			this._activeDoors.Remove(door);
		}

		// Token: 0x06000F35 RID: 3893 RVA: 0x00030AEF File Offset: 0x0002ECEF
		private void OnGetState(EntityUid uid, DoorComponent door, ref ComponentGetState args)
		{
			args.State = new DoorComponentState(door);
		}

		// Token: 0x06000F36 RID: 3894 RVA: 0x00030B00 File Offset: 0x0002ED00
		private void OnHandleState(EntityUid uid, DoorComponent door, ref ComponentHandleState args)
		{
			DoorComponentState state = args.Current as DoorComponentState;
			if (state == null)
			{
				return;
			}
			if (!door.CurrentlyCrushing.Equals(state.CurrentlyCrushing))
			{
				door.CurrentlyCrushing = new HashSet<EntityUid>(state.CurrentlyCrushing);
			}
			door.State = state.DoorState;
			door.NextStateChange = state.NextStateChange;
			door.Partial = state.Partial;
			if (state.NextStateChange == null)
			{
				this._activeDoors.Remove(door);
			}
			else
			{
				this._activeDoors.Add(door);
			}
			base.RaiseLocalEvent<DoorStateChangedEvent>(uid, new DoorStateChangedEvent(door.State), false);
			this.UpdateAppearance(uid, door);
		}

		// Token: 0x06000F37 RID: 3895 RVA: 0x00030BAC File Offset: 0x0002EDAC
		[NullableContext(2)]
		protected void SetState(EntityUid uid, DoorState state, DoorComponent door = null)
		{
			if (!base.Resolve<DoorComponent>(uid, ref door, true))
			{
				return;
			}
			switch (state)
			{
			case DoorState.Closed:
				door.Partial = false;
				break;
			case DoorState.Closing:
				this._activeDoors.Add(door);
				door.NextStateChange = new TimeSpan?(this.GameTiming.CurTime + door.CloseTimeOne);
				break;
			case DoorState.Open:
				door.Partial = false;
				if (door.NextStateChange == null)
				{
					this._activeDoors.Remove(door);
				}
				break;
			case DoorState.Opening:
				this._activeDoors.Add(door);
				door.NextStateChange = new TimeSpan?(this.GameTiming.CurTime + door.OpenTimeOne);
				break;
			case DoorState.Denying:
				this._activeDoors.Add(door);
				door.NextStateChange = new TimeSpan?(this.GameTiming.CurTime + door.DenyDuration);
				break;
			case DoorState.Emagging:
				this._activeDoors.Add(door);
				door.NextStateChange = new TimeSpan?(this.GameTiming.CurTime + door.EmagDuration);
				break;
			}
			door.State = state;
			base.Dirty(door, null);
			base.RaiseLocalEvent<DoorStateChangedEvent>(uid, new DoorStateChangedEvent(state), false);
			this.UpdateAppearance(uid, door);
		}

		// Token: 0x06000F38 RID: 3896 RVA: 0x00030D04 File Offset: 0x0002EF04
		[NullableContext(2)]
		protected virtual void UpdateAppearance(EntityUid uid, DoorComponent door = null)
		{
			if (!base.Resolve<DoorComponent>(uid, ref door, true))
			{
				return;
			}
			AppearanceComponent appearance;
			if (!base.TryComp<AppearanceComponent>(uid, ref appearance))
			{
				return;
			}
			this._appearance.SetData(uid, DoorVisuals.State, door.State, null);
		}

		// Token: 0x06000F39 RID: 3897 RVA: 0x00030D48 File Offset: 0x0002EF48
		protected virtual void OnActivate(EntityUid uid, DoorComponent door, ActivateInWorldEvent args)
		{
			args.Handled = true;
		}

		// Token: 0x06000F3A RID: 3898 RVA: 0x00030D54 File Offset: 0x0002EF54
		[NullableContext(2)]
		public void Deny(EntityUid uid, DoorComponent door = null, EntityUid? user = null, bool predicted = false)
		{
			if (!base.Resolve<DoorComponent>(uid, ref door, true))
			{
				return;
			}
			if (door.State != DoorState.Closed)
			{
				return;
			}
			BeforeDoorDeniedEvent ev = new BeforeDoorDeniedEvent();
			base.RaiseLocalEvent<BeforeDoorDeniedEvent>(uid, ev, false);
			if (ev.Cancelled)
			{
				return;
			}
			this.SetState(uid, DoorState.Denying, door);
			if (door.DenySound != null)
			{
				this.PlaySound(uid, door.DenySound, AudioParams.Default.WithVolume(-3f), user, predicted);
			}
		}

		// Token: 0x06000F3B RID: 3899 RVA: 0x00030DBF File Offset: 0x0002EFBF
		[NullableContext(2)]
		public bool TryToggleDoor(EntityUid uid, DoorComponent door = null, EntityUid? user = null, bool predicted = false)
		{
			if (!base.Resolve<DoorComponent>(uid, ref door, true))
			{
				return false;
			}
			if (door.State == DoorState.Closed)
			{
				return this.TryOpen(uid, door, user, predicted, false);
			}
			return door.State == DoorState.Open && this.TryClose(uid, door, user, predicted);
		}

		// Token: 0x06000F3C RID: 3900 RVA: 0x00030DFA File Offset: 0x0002EFFA
		[NullableContext(2)]
		public bool TryOpen(EntityUid uid, DoorComponent door = null, EntityUid? user = null, bool predicted = false, bool quiet = false)
		{
			if (!base.Resolve<DoorComponent>(uid, ref door, true))
			{
				return false;
			}
			if (!this.CanOpen(uid, door, user, quiet))
			{
				return false;
			}
			this.StartOpening(uid, door, user, predicted);
			return true;
		}

		// Token: 0x06000F3D RID: 3901 RVA: 0x00030E28 File Offset: 0x0002F028
		[NullableContext(2)]
		public bool CanOpen(EntityUid uid, DoorComponent door = null, EntityUid? user = null, bool quiet = true)
		{
			if (!base.Resolve<DoorComponent>(uid, ref door, true))
			{
				return false;
			}
			if (door.State == DoorState.Welded)
			{
				return false;
			}
			BeforeDoorOpenedEvent ev = new BeforeDoorOpenedEvent();
			base.RaiseLocalEvent<BeforeDoorOpenedEvent>(uid, ev, false);
			if (ev.Cancelled)
			{
				return false;
			}
			if (!this.HasAccess(uid, user, null))
			{
				if (!quiet)
				{
					this.Deny(uid, door, null, false);
				}
				return false;
			}
			return true;
		}

		// Token: 0x06000F3E RID: 3902 RVA: 0x00030E8C File Offset: 0x0002F08C
		[NullableContext(2)]
		public virtual void StartOpening(EntityUid uid, DoorComponent door = null, EntityUid? user = null, bool predicted = false)
		{
			if (!base.Resolve<DoorComponent>(uid, ref door, true))
			{
				return;
			}
			this.SetState(uid, DoorState.Opening, door);
			if (door.OpenSound != null)
			{
				this.PlaySound(uid, door.OpenSound, AudioParams.Default.WithVolume(-5f), user, predicted);
			}
			SharedHandsComponent hands;
			if (user != null && base.TryComp<SharedHandsComponent>(user.Value, ref hands) && hands.Hands.Count == 0)
			{
				this.PlaySound(uid, door.TryOpenDoorSound, AudioParams.Default.WithVolume(-2f), user, predicted);
			}
		}

		// Token: 0x06000F3F RID: 3903 RVA: 0x00030F1C File Offset: 0x0002F11C
		[NullableContext(2)]
		public void OnPartialOpen(EntityUid uid, DoorComponent door = null)
		{
			if (!base.Resolve<DoorComponent>(uid, ref door, true))
			{
				return;
			}
			this.SetCollidable(uid, false, door, null, null);
			door.Partial = true;
			door.NextStateChange = new TimeSpan?(this.GameTiming.CurTime + door.CloseTimeTwo);
			this._activeDoors.Add(door);
			base.Dirty(door, null);
		}

		// Token: 0x06000F40 RID: 3904 RVA: 0x00030F7E File Offset: 0x0002F17E
		[NullableContext(2)]
		public bool TryClose(EntityUid uid, DoorComponent door = null, EntityUid? user = null, bool predicted = false)
		{
			if (!base.Resolve<DoorComponent>(uid, ref door, true))
			{
				return false;
			}
			if (!this.CanClose(uid, door, user, false))
			{
				return false;
			}
			this.StartClosing(uid, door, user, predicted);
			return true;
		}

		// Token: 0x06000F41 RID: 3905 RVA: 0x00030FA8 File Offset: 0x0002F1A8
		[NullableContext(2)]
		public bool CanClose(EntityUid uid, DoorComponent door = null, EntityUid? user = null, bool quiet = true)
		{
			if (!base.Resolve<DoorComponent>(uid, ref door, true))
			{
				return false;
			}
			if (door.State == DoorState.Welded)
			{
				return false;
			}
			BeforeDoorClosedEvent ev = new BeforeDoorClosedEvent(door.PerformCollisionCheck);
			base.RaiseLocalEvent<BeforeDoorClosedEvent>(uid, ev, false);
			return !ev.Cancelled && this.HasAccess(uid, user, null) && (!ev.PerformCollisionCheck || !this.GetColliding(uid, null).Any<EntityUid>());
		}

		// Token: 0x06000F42 RID: 3906 RVA: 0x00031014 File Offset: 0x0002F214
		[NullableContext(2)]
		public virtual void StartClosing(EntityUid uid, DoorComponent door = null, EntityUid? user = null, bool predicted = false)
		{
			if (!base.Resolve<DoorComponent>(uid, ref door, true))
			{
				return;
			}
			this.SetState(uid, DoorState.Closing, door);
			if (door.CloseSound != null)
			{
				this.PlaySound(uid, door.CloseSound, AudioParams.Default.WithVolume(-5f), user, predicted);
			}
		}

		// Token: 0x06000F43 RID: 3907 RVA: 0x00031054 File Offset: 0x0002F254
		[NullableContext(2)]
		public bool OnPartialClose(EntityUid uid, DoorComponent door = null, PhysicsComponent physics = null)
		{
			if (!base.Resolve<DoorComponent, PhysicsComponent>(uid, ref door, ref physics, true))
			{
				return false;
			}
			door.Partial = true;
			base.Dirty(door, null);
			if (!this.CanClose(uid, door, null, true))
			{
				door.NextStateChange = new TimeSpan?(this.GameTiming.CurTime + door.OpenTimeTwo);
				door.State = DoorState.Opening;
				this.UpdateAppearance(uid, door);
				return false;
			}
			this.SetCollidable(uid, true, door, physics, null);
			door.NextStateChange = new TimeSpan?(this.GameTiming.CurTime + door.CloseTimeTwo);
			this._activeDoors.Add(door);
			this.Crush(uid, door, physics);
			return true;
		}

		// Token: 0x06000F44 RID: 3908 RVA: 0x0003110C File Offset: 0x0002F30C
		[NullableContext(2)]
		protected virtual void SetCollidable(EntityUid uid, bool collidable, DoorComponent door = null, PhysicsComponent physics = null, OccluderComponent occluder = null)
		{
			if (!base.Resolve<DoorComponent>(uid, ref door, true))
			{
				return;
			}
			if (base.Resolve<PhysicsComponent>(uid, ref physics, false))
			{
				this.PhysicsSystem.SetCanCollide(uid, collidable, true, false, null, physics);
			}
			if (!collidable)
			{
				door.CurrentlyCrushing.Clear();
			}
			if (door.Occludes)
			{
				this._occluder.SetEnabled(uid, collidable, occluder);
			}
		}

		// Token: 0x06000F45 RID: 3909 RVA: 0x0003116C File Offset: 0x0002F36C
		[NullableContext(2)]
		public void Crush(EntityUid uid, DoorComponent door = null, PhysicsComponent physics = null)
		{
			if (!base.Resolve<DoorComponent>(uid, ref door, true))
			{
				return;
			}
			if (!door.CanCrush)
			{
				return;
			}
			TimeSpan stunTime = door.DoorStunTime + door.OpenTimeOne;
			foreach (EntityUid entity in this.GetColliding(uid, physics))
			{
				door.CurrentlyCrushing.Add(entity);
				if (door.CrushDamage != null)
				{
					this._damageableSystem.TryChangeDamage(new EntityUid?(entity), door.CrushDamage, false, true, null, new EntityUid?(uid));
				}
				this._stunSystem.TryParalyze(entity, stunTime, true, null);
			}
			if (door.CurrentlyCrushing.Count == 0)
			{
				return;
			}
			door.NextStateChange = new TimeSpan?(this.GameTiming.CurTime + door.DoorStunTime);
			door.Partial = false;
		}

		// Token: 0x06000F46 RID: 3910 RVA: 0x00031258 File Offset: 0x0002F458
		public IEnumerable<EntityUid> GetColliding(EntityUid uid, [Nullable(2)] PhysicsComponent physics = null)
		{
			if (!base.Resolve<PhysicsComponent>(uid, ref physics, true))
			{
				yield break;
			}
			Box2 doorAABB = this._entityLookup.GetWorldAABB(uid, null);
			foreach (PhysicsComponent otherPhysics in this.PhysicsSystem.GetCollidingEntities(base.Transform(uid).MapID, ref doorAABB))
			{
				if (otherPhysics != physics && otherPhysics.CanCollide && otherPhysics.BodyType != 4 && ((physics.CollisionMask & otherPhysics.CollisionLayer) != 0 || (otherPhysics.CollisionMask & physics.CollisionLayer) != 0) && this._entityLookup.GetWorldAABB(otherPhysics.Owner, null).IntersectPercentage(ref doorAABB) >= 0.2f)
				{
					yield return otherPhysics.Owner;
				}
			}
			IEnumerator<PhysicsComponent> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06000F47 RID: 3911 RVA: 0x00031276 File Offset: 0x0002F476
		private void PreventCollision(EntityUid uid, DoorComponent component, ref PreventCollideEvent args)
		{
			if (component.CurrentlyCrushing.Contains(args.BodyB.Owner))
			{
				args.Cancelled = true;
			}
		}

		// Token: 0x06000F48 RID: 3912 RVA: 0x00031297 File Offset: 0x0002F497
		protected virtual void HandleCollide(EntityUid uid, DoorComponent door, ref StartCollideEvent args)
		{
		}

		// Token: 0x06000F49 RID: 3913 RVA: 0x00031299 File Offset: 0x0002F499
		[NullableContext(2)]
		public virtual bool HasAccess(EntityUid uid, EntityUid? user = null, AccessReaderComponent access = null)
		{
			return true;
		}

		// Token: 0x06000F4A RID: 3914 RVA: 0x0003129C File Offset: 0x0002F49C
		[NullableContext(2)]
		public void SetNextStateChange(EntityUid uid, TimeSpan? delay, DoorComponent door = null)
		{
			if (!base.Resolve<DoorComponent>(uid, ref door, false))
			{
				return;
			}
			if (door.State != DoorState.Open && door.State != DoorState.Closed)
			{
				return;
			}
			if (delay == null || delay.Value <= TimeSpan.Zero)
			{
				door.NextStateChange = null;
				this._activeDoors.Remove(door);
				return;
			}
			door.NextStateChange = new TimeSpan?(this.GameTiming.CurTime + delay.Value);
			this._activeDoors.Add(door);
		}

		// Token: 0x06000F4B RID: 3915 RVA: 0x00031330 File Offset: 0x0002F530
		public override void Update(float frameTime)
		{
			TimeSpan time = this.GameTiming.CurTime;
			foreach (DoorComponent door in this._activeDoors.ToList<DoorComponent>())
			{
				if (door.Deleted || door.NextStateChange == null)
				{
					this._activeDoors.Remove(door);
				}
				else if (!base.Paused(door.Owner, null))
				{
					if (door.NextStateChange.Value < time)
					{
						this.NextState(door, time);
					}
					PhysicsComponent doorBody;
					if (door.State == DoorState.Closed && base.TryComp<PhysicsComponent>(door.Owner, ref doorBody))
					{
						this._activeDoors.Remove(door);
						this.CheckDoorBump(door, doorBody);
					}
				}
			}
		}

		// Token: 0x06000F4C RID: 3916 RVA: 0x00031410 File Offset: 0x0002F610
		protected virtual void CheckDoorBump(DoorComponent component, PhysicsComponent body)
		{
		}

		// Token: 0x06000F4D RID: 3917 RVA: 0x00031414 File Offset: 0x0002F614
		private void NextState(DoorComponent door, TimeSpan time)
		{
			door.NextStateChange = null;
			if (door.CurrentlyCrushing.Count > 0)
			{
				this.StartOpening(door.Owner, door, null, true);
			}
			switch (door.State)
			{
			case DoorState.Closing:
				if (door.Partial)
				{
					this.SetState(door.Owner, DoorState.Closed, door);
					return;
				}
				this.OnPartialClose(door.Owner, door, null);
				return;
			case DoorState.Open:
				if (!this.TryClose(door.Owner, door, null, true))
				{
					door.NextStateChange = new TimeSpan?(time + TimeSpan.FromSeconds(1.0));
					return;
				}
				break;
			case DoorState.Opening:
				if (door.Partial)
				{
					this.SetState(door.Owner, DoorState.Open, door);
					return;
				}
				this.OnPartialOpen(door.Owner, door);
				return;
			case DoorState.Welded:
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(51, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Welded door was in the list of active doors. Door: ");
				defaultInterpolatedStringHandler.AppendFormatted<EntityStringRepresentation>(base.ToPrettyString(door.Owner));
				Logger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				break;
			}
			case DoorState.Denying:
				this.SetState(door.Owner, DoorState.Closed, door);
				return;
			case DoorState.Emagging:
				this.StartOpening(door.Owner, door, null, false);
				return;
			default:
				return;
			}
		}

		// Token: 0x06000F4E RID: 3918
		protected abstract void PlaySound(EntityUid uid, SoundSpecifier soundSpecifier, AudioParams audioParams, EntityUid? predictingPlayer, bool predicted);

		// Token: 0x04000E3B RID: 3643
		[Dependency]
		protected readonly IGameTiming GameTiming;

		// Token: 0x04000E3C RID: 3644
		[Dependency]
		protected readonly SharedPhysicsSystem PhysicsSystem;

		// Token: 0x04000E3D RID: 3645
		[Dependency]
		private readonly DamageableSystem _damageableSystem;

		// Token: 0x04000E3E RID: 3646
		[Dependency]
		private readonly SharedStunSystem _stunSystem;

		// Token: 0x04000E3F RID: 3647
		[Dependency]
		protected readonly TagSystem Tags;

		// Token: 0x04000E40 RID: 3648
		[Dependency]
		protected readonly SharedAudioSystem Audio;

		// Token: 0x04000E41 RID: 3649
		[Dependency]
		private readonly EntityLookupSystem _entityLookup;

		// Token: 0x04000E42 RID: 3650
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;

		// Token: 0x04000E43 RID: 3651
		[Dependency]
		private readonly OccluderSystem _occluder;

		// Token: 0x04000E44 RID: 3652
		public const float IntersectPercentage = 0.2f;

		// Token: 0x04000E45 RID: 3653
		private readonly HashSet<DoorComponent> _activeDoors = new HashSet<DoorComponent>();

		// Token: 0x04000E46 RID: 3654
		public SharedDoorSystem.AccessTypes AccessType;

		// Token: 0x0200081C RID: 2076
		[NullableContext(0)]
		public enum AccessTypes
		{
			// Token: 0x040018E4 RID: 6372
			Id,
			// Token: 0x040018E5 RID: 6373
			AllowAllIdExternal,
			// Token: 0x040018E6 RID: 6374
			AllowAllNoExternal,
			// Token: 0x040018E7 RID: 6375
			AllowAll
		}
	}
}
