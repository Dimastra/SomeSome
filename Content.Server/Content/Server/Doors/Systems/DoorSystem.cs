using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Server.Access;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Construction;
using Content.Server.Power.EntitySystems;
using Content.Server.Tools.Systems;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Database;
using Content.Shared.Doors;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Emag.Systems;
using Content.Shared.Interaction;
using Content.Shared.Tools;
using Content.Shared.Tools.Components;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;

namespace Content.Server.Doors.Systems
{
	// Token: 0x02000545 RID: 1349
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class DoorSystem : SharedDoorSystem
	{
		// Token: 0x06001C47 RID: 7239 RVA: 0x00096898 File Offset: 0x00094A98
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<DoorComponent, InteractUsingEvent>(new ComponentEventHandler<DoorComponent, InteractUsingEvent>(this.OnInteractUsing), null, new Type[]
			{
				typeof(ConstructionSystem)
			});
			base.SubscribeLocalEvent<DoorComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<DoorComponent, GetVerbsEvent<AlternativeVerb>>(this.OnDoorAltVerb), null, null);
			base.SubscribeLocalEvent<DoorComponent, PryFinishedEvent>(new ComponentEventHandler<DoorComponent, PryFinishedEvent>(this.OnPryFinished), null, null);
			base.SubscribeLocalEvent<DoorComponent, PryCancelledEvent>(new ComponentEventHandler<DoorComponent, PryCancelledEvent>(this.OnPryCancelled), null, null);
			base.SubscribeLocalEvent<DoorComponent, WeldableAttemptEvent>(new ComponentEventHandler<DoorComponent, WeldableAttemptEvent>(this.OnWeldAttempt), null, null);
			base.SubscribeLocalEvent<DoorComponent, WeldableChangedEvent>(new ComponentEventHandler<DoorComponent, WeldableChangedEvent>(this.OnWeldChanged), null, null);
			base.SubscribeLocalEvent<DoorComponent, GotEmaggedEvent>(new ComponentEventRefHandler<DoorComponent, GotEmaggedEvent>(this.OnEmagged), null, null);
		}

		// Token: 0x06001C48 RID: 7240 RVA: 0x00096949 File Offset: 0x00094B49
		protected override void OnActivate(EntityUid uid, DoorComponent door, ActivateInWorldEvent args)
		{
			if (args.Handled || !door.ClickOpen)
			{
				return;
			}
			base.TryToggleDoor(uid, door, new EntityUid?(args.User), false);
			args.Handled = true;
		}

		// Token: 0x06001C49 RID: 7241 RVA: 0x00096978 File Offset: 0x00094B78
		[NullableContext(2)]
		protected override void SetCollidable(EntityUid uid, bool collidable, DoorComponent door = null, PhysicsComponent physics = null, OccluderComponent occluder = null)
		{
			if (!base.Resolve<DoorComponent>(uid, ref door, true))
			{
				return;
			}
			AirtightComponent airtight;
			if (door.ChangeAirtight && base.TryComp<AirtightComponent>(uid, ref airtight))
			{
				this._airtightSystem.SetAirblocked(uid, airtight, collidable, null);
			}
			base.RaiseLocalEvent<AccessReaderChangeEvent>(new AccessReaderChangeEvent(uid, collidable));
			base.SetCollidable(uid, collidable, door, physics, occluder);
		}

		// Token: 0x06001C4A RID: 7242 RVA: 0x000969CE File Offset: 0x00094BCE
		protected override void PlaySound(EntityUid uid, SoundSpecifier soundSpecifier, AudioParams audioParams, EntityUid? predictingPlayer, bool predicted)
		{
			if (predicted && predictingPlayer == null)
			{
				return;
			}
			if (predicted)
			{
				this.Audio.PlayPredicted(soundSpecifier, uid, predictingPlayer, new AudioParams?(audioParams));
				return;
			}
			this.Audio.PlayPvs(soundSpecifier, uid, new AudioParams?(audioParams));
		}

		// Token: 0x06001C4B RID: 7243 RVA: 0x00096A10 File Offset: 0x00094C10
		private void OnInteractUsing(EntityUid uid, DoorComponent door, InteractUsingEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			ToolComponent tool;
			if (!base.TryComp<ToolComponent>(args.Used, ref tool))
			{
				return;
			}
			if (tool.Qualities.Contains(door.PryingQuality))
			{
				args.Handled = this.TryPryDoor(uid, args.Used, args.User, door, false);
			}
		}

		// Token: 0x06001C4C RID: 7244 RVA: 0x00096A65 File Offset: 0x00094C65
		private void OnWeldAttempt(EntityUid uid, DoorComponent component, WeldableAttemptEvent args)
		{
			if (component.CurrentlyCrushing.Count > 0)
			{
				args.Cancel();
				return;
			}
			if (component.State != DoorState.Closed && component.State != DoorState.Welded)
			{
				args.Cancel();
			}
		}

		// Token: 0x06001C4D RID: 7245 RVA: 0x00096A93 File Offset: 0x00094C93
		private void OnWeldChanged(EntityUid uid, DoorComponent component, WeldableChangedEvent args)
		{
			if (component.State == DoorState.Closed)
			{
				base.SetState(uid, DoorState.Welded, component);
				return;
			}
			if (component.State == DoorState.Welded)
			{
				base.SetState(uid, DoorState.Closed, component);
			}
		}

		// Token: 0x06001C4E RID: 7246 RVA: 0x00096ABC File Offset: 0x00094CBC
		private void OnDoorAltVerb(EntityUid uid, DoorComponent component, GetVerbsEvent<AlternativeVerb> args)
		{
			if (!args.CanInteract || !args.CanAccess)
			{
				return;
			}
			ToolComponent tool;
			if (!base.TryComp<ToolComponent>(args.User, ref tool) || !tool.Qualities.Contains(component.PryingQuality))
			{
				return;
			}
			args.Verbs.Add(new AlternativeVerb
			{
				Text = Loc.GetString("door-pry"),
				Impact = LogImpact.Low,
				Act = delegate()
				{
					this.TryPryDoor(uid, args.User, args.User, component, true);
				}
			});
		}

		// Token: 0x06001C4F RID: 7247 RVA: 0x00096B74 File Offset: 0x00094D74
		public bool TryPryDoor(EntityUid target, EntityUid tool, EntityUid user, DoorComponent door, bool force = false)
		{
			if (door.BeingPried)
			{
				return false;
			}
			if (door.State == DoorState.Welded)
			{
				return false;
			}
			if (!force)
			{
				BeforeDoorPryEvent canEv = new BeforeDoorPryEvent(user, tool);
				base.RaiseLocalEvent<BeforeDoorPryEvent>(target, canEv, false);
				if (canEv.Cancelled)
				{
					return true;
				}
			}
			DoorGetPryTimeModifierEvent modEv = new DoorGetPryTimeModifierEvent(user);
			base.RaiseLocalEvent<DoorGetPryTimeModifierEvent>(target, modEv, false);
			door.BeingPried = true;
			ToolEventData toolEvData = new ToolEventData(new PryFinishedEvent(), 0f, new PryCancelledEvent(), new EntityUid?(target));
			this._toolSystem.UseTool(tool, user, new EntityUid?(target), modEv.PryTimeModifier * door.PryTime, new string[]
			{
				door.PryingQuality
			}, toolEvData, 0f, null, null, null);
			return true;
		}

		// Token: 0x06001C50 RID: 7248 RVA: 0x00096C26 File Offset: 0x00094E26
		private void OnPryCancelled(EntityUid uid, DoorComponent door, PryCancelledEvent args)
		{
			door.BeingPried = false;
		}

		// Token: 0x06001C51 RID: 7249 RVA: 0x00096C30 File Offset: 0x00094E30
		private void OnPryFinished(EntityUid uid, DoorComponent door, PryFinishedEvent args)
		{
			door.BeingPried = false;
			if (door.State == DoorState.Closed)
			{
				this.StartOpening(uid, door, null, false);
				return;
			}
			if (door.State == DoorState.Open)
			{
				this.StartClosing(uid, door, null, false);
			}
		}

		// Token: 0x06001C52 RID: 7250 RVA: 0x00096C7C File Offset: 0x00094E7C
		[NullableContext(2)]
		public override bool HasAccess(EntityUid uid, EntityUid? user = null, AccessReaderComponent access = null)
		{
			if (user == null || this.AccessType == SharedDoorSystem.AccessTypes.AllowAll)
			{
				return true;
			}
			AirlockComponent airlock;
			if (base.TryComp<AirlockComponent>(uid, ref airlock) && airlock.EmergencyAccess)
			{
				return true;
			}
			if (!base.Resolve<AccessReaderComponent>(uid, ref access, false))
			{
				return true;
			}
			bool isExternal = access.AccessLists.Any((HashSet<string> list) => list.Contains("External"));
			SharedDoorSystem.AccessTypes accessType = this.AccessType;
			bool result;
			if (accessType != SharedDoorSystem.AccessTypes.AllowAllIdExternal)
			{
				if (accessType != SharedDoorSystem.AccessTypes.AllowAllNoExternal)
				{
					result = this._accessReaderSystem.IsAllowed(user.Value, access);
				}
				else
				{
					result = !isExternal;
				}
			}
			else
			{
				result = (!isExternal || this._accessReaderSystem.IsAllowed(user.Value, access));
			}
			return result;
		}

		// Token: 0x06001C53 RID: 7251 RVA: 0x00096D34 File Offset: 0x00094F34
		protected override void HandleCollide(EntityUid uid, DoorComponent door, ref StartCollideEvent args)
		{
			if (!door.BumpOpen)
			{
				return;
			}
			if (door.State != DoorState.Closed)
			{
				return;
			}
			EntityUid otherUid = args.OtherFixture.Body.Owner;
			if (this.Tags.HasTag(otherUid, "DoorBumpOpener"))
			{
				base.TryOpen(uid, door, new EntityUid?(otherUid), false, false);
			}
		}

		// Token: 0x06001C54 RID: 7252 RVA: 0x00096D88 File Offset: 0x00094F88
		private void OnEmagged(EntityUid uid, DoorComponent door, ref GotEmaggedEvent args)
		{
			AirlockComponent airlockComponent;
			if (base.TryComp<AirlockComponent>(uid, ref airlockComponent))
			{
				if (airlockComponent.BoltsDown || !this.IsPowered(uid, this.EntityManager, null))
				{
					return;
				}
				if (door.State == DoorState.Closed)
				{
					base.SetState(uid, DoorState.Emagging, door);
					this.PlaySound(uid, door.SparkSound, AudioParams.Default.WithVolume(8f), new EntityUid?(args.UserUid), false);
					args.Handled = true;
				}
			}
		}

		// Token: 0x06001C55 RID: 7253 RVA: 0x00096DFC File Offset: 0x00094FFC
		[NullableContext(2)]
		public override void StartOpening(EntityUid uid, DoorComponent door = null, EntityUid? user = null, bool predicted = false)
		{
			if (!base.Resolve<DoorComponent>(uid, ref door, true))
			{
				return;
			}
			int state = (int)door.State;
			base.SetState(uid, DoorState.Opening, door);
			if (door.OpenSound != null)
			{
				this.PlaySound(uid, door.OpenSound, AudioParams.Default.WithVolume(-5f), user, predicted);
			}
			AirlockComponent airlockComponent;
			if (state == 6 && base.TryComp<AirlockComponent>(uid, ref airlockComponent))
			{
				this._airlock.SetBoltsWithAudio(uid, airlockComponent, !airlockComponent.BoltsDown);
			}
		}

		// Token: 0x06001C56 RID: 7254 RVA: 0x00096E70 File Offset: 0x00095070
		protected override void CheckDoorBump(DoorComponent component, PhysicsComponent body)
		{
			if (component.BumpOpen)
			{
				foreach (PhysicsComponent other in this.PhysicsSystem.GetContactingEntities(body, true))
				{
					if (this.Tags.HasTag(other.Owner, "DoorBumpOpener") && base.TryOpen(component.Owner, component, new EntityUid?(other.Owner), false, true))
					{
						break;
					}
				}
			}
		}

		// Token: 0x04001233 RID: 4659
		[Dependency]
		private readonly AccessReaderSystem _accessReaderSystem;

		// Token: 0x04001234 RID: 4660
		[Dependency]
		private readonly AirlockSystem _airlock;

		// Token: 0x04001235 RID: 4661
		[Dependency]
		private readonly AirtightSystem _airtightSystem;

		// Token: 0x04001236 RID: 4662
		[Dependency]
		private readonly ConstructionSystem _constructionSystem;

		// Token: 0x04001237 RID: 4663
		[Dependency]
		private readonly SharedToolSystem _toolSystem;

		// Token: 0x04001238 RID: 4664
		[Dependency]
		private readonly SharedContainerSystem _containerSystem;
	}
}
