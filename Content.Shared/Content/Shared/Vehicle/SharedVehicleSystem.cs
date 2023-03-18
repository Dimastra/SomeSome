using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Audio;
using Content.Shared.Buckle.Components;
using Content.Shared.Item;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Physics.Pull;
using Content.Shared.Tag;
using Content.Shared.Vehicle.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Vehicle
{
	// Token: 0x020000A1 RID: 161
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedVehicleSystem : EntitySystem
	{
		// Token: 0x060001C8 RID: 456 RVA: 0x00009B60 File Offset: 0x00007D60
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<InVehicleComponent, GettingPickedUpAttemptEvent>(new ComponentEventHandler<InVehicleComponent, GettingPickedUpAttemptEvent>(this.OnPickupAttempt), null, null);
			base.SubscribeLocalEvent<RiderComponent, PullAttemptEvent>(new ComponentEventHandler<RiderComponent, PullAttemptEvent>(this.OnRiderPull), null, null);
			base.SubscribeLocalEvent<VehicleComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<VehicleComponent, RefreshMovementSpeedModifiersEvent>(this.OnVehicleModifier), null, null);
			base.SubscribeLocalEvent<VehicleComponent, ComponentStartup>(new ComponentEventHandler<VehicleComponent, ComponentStartup>(this.OnVehicleStartup), null, null);
			base.SubscribeLocalEvent<VehicleComponent, MoveEvent>(new ComponentEventRefHandler<VehicleComponent, MoveEvent>(this.OnVehicleRotate), null, null);
			base.SubscribeLocalEvent<VehicleComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<VehicleComponent, EntInsertedIntoContainerMessage>(this.OnEntInserted), null, null);
			base.SubscribeLocalEvent<VehicleComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<VehicleComponent, EntRemovedFromContainerMessage>(this.OnEntRemoved), null, null);
			base.SubscribeLocalEvent<VehicleComponent, GetAdditionalAccessEvent>(new ComponentEventRefHandler<VehicleComponent, GetAdditionalAccessEvent>(this.OnGetAdditionalAccess), null, null);
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x00009C14 File Offset: 0x00007E14
		private void OnEntInserted(EntityUid uid, VehicleComponent component, EntInsertedIntoContainerMessage args)
		{
			if (args.Container.ID != "key_slot" || !this._tagSystem.HasTag(args.Entity, "VehicleKey"))
			{
				return;
			}
			base.EnsureComp<InVehicleComponent>(args.Entity).Vehicle = component;
			component.HasKey = true;
			this._ambientSound.SetAmbience(uid, true, null);
			this._tagSystem.AddTag(uid, "DoorBumpOpener");
			this._modifier.RefreshMovementSpeedModifiers(uid, null);
		}

		// Token: 0x060001CA RID: 458 RVA: 0x00009C98 File Offset: 0x00007E98
		private void OnEntRemoved(EntityUid uid, VehicleComponent component, EntRemovedFromContainerMessage args)
		{
			if (args.Container.ID != "key_slot" || !base.RemComp<InVehicleComponent>(args.Entity))
			{
				return;
			}
			component.HasKey = false;
			this._ambientSound.SetAmbience(uid, false, null);
			this._tagSystem.RemoveTag(uid, "DoorBumpOpener");
			this._modifier.RefreshMovementSpeedModifiers(uid, null);
		}

		// Token: 0x060001CB RID: 459 RVA: 0x00009CFF File Offset: 0x00007EFF
		private void OnVehicleModifier(EntityUid uid, VehicleComponent component, RefreshMovementSpeedModifiersEvent args)
		{
			if (!component.HasKey)
			{
				args.ModifySpeed(0f, 0f);
			}
		}

		// Token: 0x060001CC RID: 460 RVA: 0x00009D1C File Offset: 0x00007F1C
		private void OnPickupAttempt(EntityUid uid, InVehicleComponent component, GettingPickedUpAttemptEvent args)
		{
			if (component.Vehicle != null)
			{
				if (component.Vehicle.Rider == null)
				{
					return;
				}
				EntityUid? rider = component.Vehicle.Rider;
				EntityUid user = args.User;
				if (rider != null && (rider == null || !(rider.GetValueOrDefault() != user)))
				{
					return;
				}
			}
			args.Cancel();
		}

		// Token: 0x060001CD RID: 461 RVA: 0x00009D84 File Offset: 0x00007F84
		private void OnVehicleRotate(EntityUid uid, VehicleComponent component, ref MoveEvent args)
		{
			if (args.NewRotation == args.OldRotation)
			{
				return;
			}
			if (!base.HasComp<InputMoverComponent>(uid))
			{
				this.UpdateAutoAnimate(uid, false);
				return;
			}
			this.UpdateBuckleOffset(args.Component, component);
			this.UpdateDrawDepth(uid, this.GetDrawDepth(args.Component, component.NorthOnly));
		}

		// Token: 0x060001CE RID: 462 RVA: 0x00009DE0 File Offset: 0x00007FE0
		private void OnVehicleStartup(EntityUid uid, VehicleComponent component, ComponentStartup args)
		{
			this.UpdateDrawDepth(uid, 2);
			StrapComponent strap;
			if (base.TryComp<StrapComponent>(uid, ref strap))
			{
				component.BaseBuckleOffset = strap.BuckleOffset;
				strap.BuckleOffsetUnclamped = Vector2.Zero;
			}
			this._modifier.RefreshMovementSpeedModifiers(uid, null);
		}

		// Token: 0x060001CF RID: 463 RVA: 0x00009E24 File Offset: 0x00008024
		protected int GetDrawDepth(TransformComponent xform, bool northOnly)
		{
			double degrees;
			int result;
			if (northOnly)
			{
				degrees = xform.LocalRotation.Degrees;
				if (degrees >= 135.0)
				{
					if (degrees > 225.0)
					{
						result = 5;
					}
					else
					{
						result = 2;
					}
				}
				else
				{
					result = 5;
				}
				return result;
			}
			degrees = xform.LocalRotation.Degrees;
			if (degrees >= 45.0)
			{
				if (degrees > 315.0)
				{
					result = 5;
				}
				else
				{
					result = 2;
				}
			}
			else
			{
				result = 5;
			}
			return result;
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x00009EA0 File Offset: 0x000080A0
		protected void UpdateBuckleOffset(TransformComponent xform, VehicleComponent component)
		{
			StrapComponent strap;
			if (!base.TryComp<StrapComponent>(component.Owner, ref strap))
			{
				return;
			}
			Vector2 oldOffset = strap.BuckleOffsetUnclamped;
			StrapComponent strapComponent = strap;
			double degrees = xform.LocalRotation.Degrees;
			Vector2 buckleOffsetUnclamped;
			if (degrees >= 45.0)
			{
				if (degrees > 135.0)
				{
					if (degrees >= 225.0)
					{
						if (degrees > 315.0)
						{
							buckleOffsetUnclamped = new ValueTuple<float, float>(0f, component.SouthOverride);
						}
						else
						{
							buckleOffsetUnclamped = new ValueTuple<float, float>(component.BaseBuckleOffset.X * -1f, component.BaseBuckleOffset.Y);
						}
					}
					else
					{
						buckleOffsetUnclamped = new ValueTuple<float, float>(0f, component.NorthOverride);
					}
				}
				else
				{
					buckleOffsetUnclamped = component.BaseBuckleOffset;
				}
			}
			else
			{
				buckleOffsetUnclamped = new ValueTuple<float, float>(0f, component.SouthOverride);
			}
			strapComponent.BuckleOffsetUnclamped = buckleOffsetUnclamped;
			if (!oldOffset.Equals(strap.BuckleOffsetUnclamped))
			{
				base.Dirty(strap, null);
			}
			foreach (EntityUid buckledEntity in strap.BuckledEntities)
			{
				TransformComponent buckleXform = base.Transform(buckledEntity);
				this._transform.SetLocalPositionNoLerp(buckleXform, strap.BuckleOffset);
			}
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x00009FFC File Offset: 0x000081FC
		private void OnGetAdditionalAccess(EntityUid uid, VehicleComponent component, ref GetAdditionalAccessEvent args)
		{
			if (component.Rider == null)
			{
				return;
			}
			EntityUid rider = component.Rider.Value;
			args.Entities.Add(rider);
			HashSet<EntityUid> items;
			this._access.FindAccessItemsInventory(rider, out items);
			args.Entities = args.Entities.Union(items).ToHashSet<EntityUid>();
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x0000A056 File Offset: 0x00008256
		protected void UpdateDrawDepth(EntityUid uid, int drawDepth)
		{
			this.Appearance.SetData(uid, VehicleVisuals.DrawDepth, drawDepth, null);
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0000A071 File Offset: 0x00008271
		protected void UpdateAutoAnimate(EntityUid uid, bool autoAnimate)
		{
			this.Appearance.SetData(uid, VehicleVisuals.AutoAnimate, autoAnimate, null);
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x0000A08C File Offset: 0x0000828C
		private void OnRiderPull(EntityUid uid, RiderComponent component, PullAttemptEvent args)
		{
			if (component.Vehicle != null)
			{
				args.Cancelled = true;
			}
		}

		// Token: 0x0400023A RID: 570
		[Dependency]
		protected readonly SharedAppearanceSystem Appearance;

		// Token: 0x0400023B RID: 571
		[Dependency]
		private readonly MovementSpeedModifierSystem _modifier;

		// Token: 0x0400023C RID: 572
		[Dependency]
		private readonly SharedAmbientSoundSystem _ambientSound;

		// Token: 0x0400023D RID: 573
		[Dependency]
		private readonly SharedTransformSystem _transform;

		// Token: 0x0400023E RID: 574
		[Dependency]
		private readonly TagSystem _tagSystem;

		// Token: 0x0400023F RID: 575
		[Dependency]
		private readonly AccessReaderSystem _access;

		// Token: 0x04000240 RID: 576
		private const string KeySlot = "key_slot";

		// Token: 0x02000790 RID: 1936
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		protected sealed class RiderComponentState : ComponentState
		{
			// Token: 0x04001796 RID: 6038
			public EntityUid? Entity;
		}
	}
}
