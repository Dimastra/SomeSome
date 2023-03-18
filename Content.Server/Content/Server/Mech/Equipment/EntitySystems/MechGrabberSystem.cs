using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Content.Server.DoAfter;
using Content.Server.Interaction;
using Content.Server.Mech.Components;
using Content.Server.Mech.Equipment.Components;
using Content.Server.Mech.Systems;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Mech;
using Content.Shared.Mech.Equipment.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.Physics;
using Content.Shared.Wall;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;

namespace Content.Server.Mech.Equipment.EntitySystems
{
	// Token: 0x020003C7 RID: 967
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class MechGrabberSystem : EntitySystem
	{
		// Token: 0x060013F0 RID: 5104 RVA: 0x00067B70 File Offset: 0x00065D70
		public override void Initialize()
		{
			base.SubscribeLocalEvent<MechGrabberComponent, MechEquipmentUiMessageRelayEvent>(new ComponentEventHandler<MechGrabberComponent, MechEquipmentUiMessageRelayEvent>(this.OnGrabberMessage), null, null);
			base.SubscribeLocalEvent<MechGrabberComponent, ComponentStartup>(new ComponentEventHandler<MechGrabberComponent, ComponentStartup>(this.OnStartup), null, null);
			base.SubscribeLocalEvent<MechGrabberComponent, MechEquipmentUiStateReadyEvent>(new ComponentEventHandler<MechGrabberComponent, MechEquipmentUiStateReadyEvent>(this.OnUiStateReady), null, null);
			base.SubscribeLocalEvent<MechGrabberComponent, MechEquipmentRemovedEvent>(new ComponentEventRefHandler<MechGrabberComponent, MechEquipmentRemovedEvent>(this.OnEquipmentRemoved), null, null);
			base.SubscribeLocalEvent<MechGrabberComponent, AttemptRemoveMechEquipmentEvent>(new ComponentEventRefHandler<MechGrabberComponent, AttemptRemoveMechEquipmentEvent>(this.OnAttemptRemove), null, null);
			base.SubscribeLocalEvent<MechGrabberComponent, InteractNoHandEvent>(new ComponentEventHandler<MechGrabberComponent, InteractNoHandEvent>(this.OnInteract), null, null);
			base.SubscribeLocalEvent<MechGrabberComponent, DoAfterEvent>(new ComponentEventHandler<MechGrabberComponent, DoAfterEvent>(this.OnMechGrab), null, null);
		}

		// Token: 0x060013F1 RID: 5105 RVA: 0x00067C0C File Offset: 0x00065E0C
		private void OnGrabberMessage(EntityUid uid, MechGrabberComponent component, MechEquipmentUiMessageRelayEvent args)
		{
			MechGrabberEjectMessage msg = args.Message as MechGrabberEjectMessage;
			if (msg == null)
			{
				return;
			}
			MechEquipmentComponent equipmentComponent;
			if (!base.TryComp<MechEquipmentComponent>(uid, ref equipmentComponent) || equipmentComponent.EquipmentOwner == null)
			{
				return;
			}
			EntityUid mech = equipmentComponent.EquipmentOwner.Value;
			EntityCoordinates targetCoords;
			targetCoords..ctor(mech, component.DepositOffset);
			if (!this._interaction.InRangeUnobstructed(mech, targetCoords, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false))
			{
				return;
			}
			if (!component.ItemContainer.Contains(msg.Item))
			{
				return;
			}
			this.RemoveItem(uid, mech, msg.Item, component);
		}

		// Token: 0x060013F2 RID: 5106 RVA: 0x00067C9C File Offset: 0x00065E9C
		[NullableContext(2)]
		public void RemoveItem(EntityUid uid, EntityUid mech, EntityUid toRemove, MechGrabberComponent component = null)
		{
			if (!base.Resolve<MechGrabberComponent>(uid, ref component, true))
			{
				return;
			}
			component.ItemContainer.Remove(toRemove, null, null, null, true, false, null, null);
			TransformComponent mechxform = base.Transform(mech);
			TransformComponent xform = base.Transform(toRemove);
			xform.AttachToGridOrMap();
			Vector2 offset = this._transform.GetWorldPosition(mechxform) + this._transform.GetWorldRotation(mechxform).RotateVec(ref component.DepositOffset);
			this._transform.SetWorldPosition(xform, offset);
			this._transform.SetWorldRotation(xform, Angle.Zero);
			this._mech.UpdateUserInterface(mech, null);
		}

		// Token: 0x060013F3 RID: 5107 RVA: 0x00067D4C File Offset: 0x00065F4C
		private void OnEquipmentRemoved(EntityUid uid, MechGrabberComponent component, ref MechEquipmentRemovedEvent args)
		{
			MechEquipmentComponent equipmentComponent;
			if (!base.TryComp<MechEquipmentComponent>(uid, ref equipmentComponent) || equipmentComponent.EquipmentOwner == null)
			{
				return;
			}
			EntityUid mech = equipmentComponent.EquipmentOwner.Value;
			foreach (EntityUid item in new List<EntityUid>(component.ItemContainer.ContainedEntities))
			{
				this.RemoveItem(uid, mech, item, component);
			}
		}

		// Token: 0x060013F4 RID: 5108 RVA: 0x00067DD4 File Offset: 0x00065FD4
		private void OnAttemptRemove(EntityUid uid, MechGrabberComponent component, ref AttemptRemoveMechEquipmentEvent args)
		{
			args.Cancelled = component.ItemContainer.ContainedEntities.Any<EntityUid>();
		}

		// Token: 0x060013F5 RID: 5109 RVA: 0x00067DEC File Offset: 0x00065FEC
		private void OnStartup(EntityUid uid, MechGrabberComponent component, ComponentStartup args)
		{
			component.ItemContainer = this._container.EnsureContainer<Container>(uid, "item-container", null);
		}

		// Token: 0x060013F6 RID: 5110 RVA: 0x00067E08 File Offset: 0x00066008
		private void OnUiStateReady(EntityUid uid, MechGrabberComponent component, MechEquipmentUiStateReadyEvent args)
		{
			MechGrabberUiState state = new MechGrabberUiState
			{
				Contents = component.ItemContainer.ContainedEntities.ToList<EntityUid>(),
				MaxContents = component.MaxContents
			};
			args.States.Add(uid, state);
		}

		// Token: 0x060013F7 RID: 5111 RVA: 0x00067E4C File Offset: 0x0006604C
		private void OnInteract(EntityUid uid, MechGrabberComponent component, InteractNoHandEvent args)
		{
			if (!args.Handled)
			{
				EntityUid? target2 = args.Target;
				if (target2 != null)
				{
					EntityUid target = target2.GetValueOrDefault();
					PhysicsComponent physics;
					if ((base.TryComp<PhysicsComponent>(target, ref physics) && physics.BodyType == 4) || base.HasComp<WallMountComponent>(target) || base.HasComp<MobStateComponent>(target))
					{
						return;
					}
					if (base.Transform(target).Anchored)
					{
						return;
					}
					if (component.ItemContainer.ContainedEntities.Count >= component.MaxContents)
					{
						return;
					}
					MechComponent mech;
					if (base.TryComp<MechComponent>(args.User, ref mech))
					{
						target2 = mech.PilotSlot.ContainedEntity;
						EntityUid entityUid = target;
						if (target2 == null || (target2 != null && !(target2.GetValueOrDefault() == entityUid)))
						{
							if (mech.Energy + component.GrabEnergyDelta < 0)
							{
								return;
							}
							if (!this._interaction.InRangeUnobstructed(args.User, target, 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, false))
							{
								return;
							}
							args.Handled = true;
							component.AudioStream = this._audio.PlayPvs(component.GrabSound, uid, null);
							SharedDoAfterSystem doAfter = this._doAfter;
							EntityUid user = args.User;
							float grabDelay = component.GrabDelay;
							target2 = new EntityUid?(target);
							EntityUid? used = new EntityUid?(uid);
							doAfter.DoAfter(new DoAfterEventArgs(user, grabDelay, default(CancellationToken), target2, used)
							{
								BreakOnTargetMove = true,
								BreakOnUserMove = true
							});
							return;
						}
					}
					return;
				}
			}
		}

		// Token: 0x060013F8 RID: 5112 RVA: 0x00067FC4 File Offset: 0x000661C4
		private void OnMechGrab(EntityUid uid, MechGrabberComponent component, DoAfterEvent args)
		{
			if (args.Cancelled)
			{
				IPlayingAudioStream audioStream = component.AudioStream;
				if (audioStream == null)
				{
					return;
				}
				audioStream.Stop();
				return;
			}
			else
			{
				if (args.Handled || args.Args.Target == null)
				{
					return;
				}
				MechEquipmentComponent equipmentComponent;
				if (!base.TryComp<MechEquipmentComponent>(uid, ref equipmentComponent) || equipmentComponent.EquipmentOwner == null)
				{
					return;
				}
				if (!this._mech.TryChangeEnergy(equipmentComponent.EquipmentOwner.Value, component.GrabEnergyDelta, null))
				{
					return;
				}
				component.ItemContainer.Insert(args.Args.Target.Value, null, null, null, null, null);
				this._mech.UpdateUserInterface(equipmentComponent.EquipmentOwner.Value, null);
				args.Handled = true;
				return;
			}
		}

		// Token: 0x04000C4E RID: 3150
		[Dependency]
		private readonly SharedContainerSystem _container;

		// Token: 0x04000C4F RID: 3151
		[Dependency]
		private readonly MechSystem _mech;

		// Token: 0x04000C50 RID: 3152
		[Dependency]
		private readonly DoAfterSystem _doAfter;

		// Token: 0x04000C51 RID: 3153
		[Dependency]
		private readonly InteractionSystem _interaction;

		// Token: 0x04000C52 RID: 3154
		[Dependency]
		private readonly SharedAudioSystem _audio;

		// Token: 0x04000C53 RID: 3155
		[Dependency]
		private readonly TransformSystem _transform;
	}
}
