using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Content.Shared.Actions.ActionTypes;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Doors.Components;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction.Events;
using Content.Shared.Maps;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Toggleable;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared.Blocking
{
	// Token: 0x0200066F RID: 1647
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class BlockingSystem : EntitySystem
	{
		// Token: 0x06001423 RID: 5155 RVA: 0x000434B0 File Offset: 0x000416B0
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeUser();
			base.SubscribeLocalEvent<BlockingComponent, GotEquippedHandEvent>(new ComponentEventHandler<BlockingComponent, GotEquippedHandEvent>(this.OnEquip), null, null);
			base.SubscribeLocalEvent<BlockingComponent, GotUnequippedHandEvent>(new ComponentEventHandler<BlockingComponent, GotUnequippedHandEvent>(this.OnUnequip), null, null);
			base.SubscribeLocalEvent<BlockingComponent, DroppedEvent>(new ComponentEventHandler<BlockingComponent, DroppedEvent>(this.OnDrop), null, null);
			base.SubscribeLocalEvent<BlockingComponent, GetItemActionsEvent>(new ComponentEventHandler<BlockingComponent, GetItemActionsEvent>(this.OnGetActions), null, null);
			base.SubscribeLocalEvent<BlockingComponent, ToggleActionEvent>(new ComponentEventHandler<BlockingComponent, ToggleActionEvent>(this.OnToggleAction), null, null);
			base.SubscribeLocalEvent<BlockingComponent, ComponentShutdown>(new ComponentEventHandler<BlockingComponent, ComponentShutdown>(this.OnShutdown), null, null);
		}

		// Token: 0x06001424 RID: 5156 RVA: 0x00043544 File Offset: 0x00041744
		private void OnEquip(EntityUid uid, BlockingComponent component, GotEquippedHandEvent args)
		{
			component.User = new EntityUid?(args.User);
			PhysicsComponent physicsComponent;
			if (base.TryComp<PhysicsComponent>(args.User, ref physicsComponent) && physicsComponent.BodyType != 4 && !base.HasComp<BlockingUserComponent>(args.User))
			{
				BlockingUserComponent blockingUserComponent = base.EnsureComp<BlockingUserComponent>(args.User);
				blockingUserComponent.BlockingItem = new EntityUid?(uid);
				blockingUserComponent.OriginalBodyType = physicsComponent.BodyType;
			}
		}

		// Token: 0x06001425 RID: 5157 RVA: 0x000435AC File Offset: 0x000417AC
		private void OnUnequip(EntityUid uid, BlockingComponent component, GotUnequippedHandEvent args)
		{
			this.StopBlockingHelper(uid, component, args.User);
		}

		// Token: 0x06001426 RID: 5158 RVA: 0x000435BC File Offset: 0x000417BC
		private void OnDrop(EntityUid uid, BlockingComponent component, DroppedEvent args)
		{
			this.StopBlockingHelper(uid, component, args.User);
		}

		// Token: 0x06001427 RID: 5159 RVA: 0x000435CC File Offset: 0x000417CC
		private void OnGetActions(EntityUid uid, BlockingComponent component, GetItemActionsEvent args)
		{
			InstantActionPrototype act;
			if (component.BlockingToggleAction == null && this._proto.TryIndex<InstantActionPrototype>(component.BlockingToggleActionId, ref act))
			{
				component.BlockingToggleAction = new InstantAction(act);
			}
			if (component.BlockingToggleAction != null)
			{
				args.Actions.Add(component.BlockingToggleAction);
			}
		}

		// Token: 0x06001428 RID: 5160 RVA: 0x0004361C File Offset: 0x0004181C
		private void OnToggleAction(EntityUid uid, BlockingComponent component, ToggleActionEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			EntityQuery<BlockingComponent> blockQuery = base.GetEntityQuery<BlockingComponent>();
			SharedHandsComponent hands;
			if (!base.GetEntityQuery<SharedHandsComponent>().TryGetComponent(args.Performer, ref hands))
			{
				return;
			}
			foreach (EntityUid shield in this._handsSystem.EnumerateHeld(args.Performer, hands).ToArray<EntityUid>())
			{
				BlockingComponent otherBlockComp;
				if (!(shield == uid) && blockQuery.TryGetComponent(shield, ref otherBlockComp) && otherBlockComp.IsBlocking)
				{
					this.CantBlockError(args.Performer);
					return;
				}
			}
			if (component.IsBlocking)
			{
				this.StopBlocking(uid, component, args.Performer);
			}
			else
			{
				this.StartBlocking(uid, component, args.Performer);
			}
			args.Handled = true;
		}

		// Token: 0x06001429 RID: 5161 RVA: 0x000436E2 File Offset: 0x000418E2
		private void OnShutdown(EntityUid uid, BlockingComponent component, ComponentShutdown args)
		{
			if (component.User != null)
			{
				this._actionsSystem.RemoveProvidedActions(component.User.Value, uid, null);
				this.StopBlockingHelper(uid, component, component.User.Value);
			}
		}

		// Token: 0x0600142A RID: 5162 RVA: 0x0004371C File Offset: 0x0004191C
		public bool StartBlocking(EntityUid item, BlockingComponent component, EntityUid user)
		{
			if (component.IsBlocking)
			{
				return false;
			}
			TransformComponent xform = base.Transform(user);
			string shieldName = base.Name(item, null);
			EntityUid blockerName = Identity.Entity(user, this.EntityManager);
			string msgUser = Loc.GetString("action-popup-blocking-user", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("shield", shieldName)
			});
			string msgOther = Loc.GetString("action-popup-blocking-other", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("blockerName", blockerName),
				new ValueTuple<string, object>("shield", shieldName)
			});
			if (component.BlockingToggleAction != null)
			{
				EntityUid? gridUid = xform.GridUid;
				EntityUid parentUid = xform.ParentUid;
				if (gridUid == null || (gridUid != null && gridUid.GetValueOrDefault() != parentUid))
				{
					this.CantBlockError(user);
					return false;
				}
				TileRef? playerTileRef = xform.Coordinates.GetTileRef(null, null);
				if (playerTileRef != null)
				{
					IEnumerable<EntityUid> entitiesIntersecting = this._lookup.GetEntitiesIntersecting(playerTileRef.Value, 46);
					EntityQuery<MobStateComponent> mobQuery = base.GetEntityQuery<MobStateComponent>();
					EntityQuery<DoorComponent> doorQuery = base.GetEntityQuery<DoorComponent>();
					EntityQuery<TransformComponent> xformQuery = base.GetEntityQuery<TransformComponent>();
					foreach (EntityUid uid in entitiesIntersecting)
					{
						if ((uid != user && mobQuery.HasComponent(uid)) || (xformQuery.GetComponent(uid).Anchored && doorQuery.HasComponent(uid)))
						{
							this.TooCloseError(user);
							return false;
						}
					}
				}
				this._transformSystem.AnchorEntity(user, xform);
				if (!xform.Anchored)
				{
					this.CantBlockError(user);
					return false;
				}
				this._actionsSystem.SetToggled(component.BlockingToggleAction, true);
				this._popupSystem.PopupEntity(msgUser, user, user, PopupType.Small);
				this._popupSystem.PopupEntity(msgOther, user, Filter.PvsExcept(user, 2f, null), true, PopupType.Small);
			}
			PhysicsComponent physicsComponent;
			if (base.TryComp<PhysicsComponent>(user, ref physicsComponent))
			{
				this._fixtureSystem.TryCreateFixture(user, component.Shape, "blocking-active", 1f, true, 223, 0, 0.4f, 0f, true, null, physicsComponent, null);
			}
			component.IsBlocking = true;
			return true;
		}

		// Token: 0x0600142B RID: 5163 RVA: 0x00043960 File Offset: 0x00041B60
		private void CantBlockError(EntityUid user)
		{
			string msgError = Loc.GetString("action-popup-blocking-user-cant-block");
			this._popupSystem.PopupEntity(msgError, user, user, PopupType.Small);
		}

		// Token: 0x0600142C RID: 5164 RVA: 0x00043988 File Offset: 0x00041B88
		private void TooCloseError(EntityUid user)
		{
			string msgError = Loc.GetString("action-popup-blocking-user-too-close");
			this._popupSystem.PopupEntity(msgError, user, user, PopupType.Small);
		}

		// Token: 0x0600142D RID: 5165 RVA: 0x000439B0 File Offset: 0x00041BB0
		public bool StopBlocking(EntityUid item, BlockingComponent component, EntityUid user)
		{
			if (!component.IsBlocking)
			{
				return false;
			}
			TransformComponent xform = base.Transform(user);
			string shieldName = base.Name(item, null);
			EntityUid blockerName = Identity.Entity(user, this.EntityManager);
			string msgUser = Loc.GetString("action-popup-blocking-disabling-user", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("shield", shieldName)
			});
			string msgOther = Loc.GetString("action-popup-blocking-disabling-other", new ValueTuple<string, object>[]
			{
				new ValueTuple<string, object>("blockerName", blockerName),
				new ValueTuple<string, object>("shield", shieldName)
			});
			BlockingUserComponent blockingUserComponent;
			PhysicsComponent physicsComponent;
			if (component.BlockingToggleAction != null && base.TryComp<BlockingUserComponent>(user, ref blockingUserComponent) && base.TryComp<PhysicsComponent>(user, ref physicsComponent))
			{
				if (xform.Anchored)
				{
					this._transformSystem.Unanchor(user, xform, true);
				}
				this._actionsSystem.SetToggled(component.BlockingToggleAction, false);
				this._fixtureSystem.DestroyFixture(user, "blocking-active", true, physicsComponent, null, null);
				this._physics.SetBodyType(user, blockingUserComponent.OriginalBodyType, null, physicsComponent, null);
				this._popupSystem.PopupEntity(msgUser, user, user, PopupType.Small);
				this._popupSystem.PopupEntity(msgOther, user, Filter.PvsExcept(user, 2f, null), true, PopupType.Small);
			}
			component.IsBlocking = false;
			return true;
		}

		// Token: 0x0600142E RID: 5166 RVA: 0x00043AF4 File Offset: 0x00041CF4
		private void StopBlockingHelper(EntityUid uid, BlockingComponent component, EntityUid user)
		{
			if (component.IsBlocking)
			{
				this.StopBlocking(uid, component, user);
			}
			EntityQuery<BlockingUserComponent> userQuery = base.GetEntityQuery<BlockingUserComponent>();
			SharedHandsComponent hands;
			if (!base.GetEntityQuery<SharedHandsComponent>().TryGetComponent(user, ref hands))
			{
				return;
			}
			foreach (EntityUid shield in this._handsSystem.EnumerateHeld(user, hands).ToArray<EntityUid>())
			{
				BlockingUserComponent blockingUserComponent;
				if (base.HasComp<BlockingComponent>(shield) && userQuery.TryGetComponent(user, ref blockingUserComponent))
				{
					blockingUserComponent.BlockingItem = new EntityUid?(shield);
					return;
				}
			}
			base.RemComp<BlockingUserComponent>(user);
			component.User = null;
		}

		// Token: 0x0600142F RID: 5167 RVA: 0x00043B98 File Offset: 0x00041D98
		private void InitializeUser()
		{
			base.SubscribeLocalEvent<BlockingUserComponent, DamageChangedEvent>(new ComponentEventHandler<BlockingUserComponent, DamageChangedEvent>(this.OnDamageChanged), null, null);
			base.SubscribeLocalEvent<BlockingUserComponent, DamageModifyEvent>(new ComponentEventHandler<BlockingUserComponent, DamageModifyEvent>(this.OnUserDamageModified), null, null);
			base.SubscribeLocalEvent<BlockingUserComponent, EntParentChangedMessage>(new ComponentEventRefHandler<BlockingUserComponent, EntParentChangedMessage>(this.OnParentChanged), null, null);
			base.SubscribeLocalEvent<BlockingUserComponent, ContainerGettingInsertedAttemptEvent>(new ComponentEventHandler<BlockingUserComponent, ContainerGettingInsertedAttemptEvent>(this.OnInsertAttempt), null, null);
			base.SubscribeLocalEvent<BlockingUserComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<BlockingUserComponent, AnchorStateChangedEvent>(this.OnAnchorChanged), null, null);
			base.SubscribeLocalEvent<BlockingUserComponent, EntityTerminatingEvent>(new ComponentEventRefHandler<BlockingUserComponent, EntityTerminatingEvent>(this.OnEntityTerminating), null, null);
		}

		// Token: 0x06001430 RID: 5168 RVA: 0x00043C1D File Offset: 0x00041E1D
		private void OnParentChanged(EntityUid uid, BlockingUserComponent component, ref EntParentChangedMessage args)
		{
			this.UserStopBlocking(uid, component);
		}

		// Token: 0x06001431 RID: 5169 RVA: 0x00043C27 File Offset: 0x00041E27
		private void OnInsertAttempt(EntityUid uid, BlockingUserComponent component, ContainerGettingInsertedAttemptEvent args)
		{
			this.UserStopBlocking(uid, component);
		}

		// Token: 0x06001432 RID: 5170 RVA: 0x00043C31 File Offset: 0x00041E31
		private void OnAnchorChanged(EntityUid uid, BlockingUserComponent component, ref AnchorStateChangedEvent args)
		{
			if (args.Anchored)
			{
				return;
			}
			this.UserStopBlocking(uid, component);
		}

		// Token: 0x06001433 RID: 5171 RVA: 0x00043C44 File Offset: 0x00041E44
		private void OnDamageChanged(EntityUid uid, BlockingUserComponent component, DamageChangedEvent args)
		{
			if (args.DamageDelta != null && args.DamageIncreased)
			{
				this._damageable.TryChangeDamage(component.BlockingItem, args.DamageDelta, false, true, null, args.Origin);
			}
		}

		// Token: 0x06001434 RID: 5172 RVA: 0x00043C78 File Offset: 0x00041E78
		private void OnUserDamageModified(EntityUid uid, BlockingUserComponent component, DamageModifyEvent args)
		{
			BlockingComponent blockingComponent;
			if (base.TryComp<BlockingComponent>(component.BlockingItem, ref blockingComponent))
			{
				DamageModifierSetPrototype passiveblockModifier;
				if (this._proto.TryIndex<DamageModifierSetPrototype>(blockingComponent.PassiveBlockDamageModifer, ref passiveblockModifier) && !blockingComponent.IsBlocking)
				{
					args.Damage = DamageSpecifier.ApplyModifierSet(args.Damage, passiveblockModifier);
				}
				DamageModifierSetPrototype activeBlockModifier;
				if (this._proto.TryIndex<DamageModifierSetPrototype>(blockingComponent.ActiveBlockDamageModifier, ref activeBlockModifier) && blockingComponent.IsBlocking)
				{
					args.Damage = DamageSpecifier.ApplyModifierSet(args.Damage, activeBlockModifier);
					this._audio.PlayPvs(blockingComponent.BlockSound, component.Owner, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.2f))));
				}
			}
		}

		// Token: 0x06001435 RID: 5173 RVA: 0x00043D28 File Offset: 0x00041F28
		private void OnEntityTerminating(EntityUid uid, BlockingUserComponent component, ref EntityTerminatingEvent args)
		{
			BlockingComponent blockingComponent;
			if (!base.TryComp<BlockingComponent>(component.BlockingItem, ref blockingComponent))
			{
				return;
			}
			this.StopBlockingHelper(component.BlockingItem.Value, blockingComponent, uid);
		}

		// Token: 0x06001436 RID: 5174 RVA: 0x00043D5C File Offset: 0x00041F5C
		private void UserStopBlocking(EntityUid uid, BlockingUserComponent component)
		{
			BlockingComponent blockComp;
			if (base.TryComp<BlockingComponent>(component.BlockingItem, ref blockComp) && blockComp.IsBlocking)
			{
				this.StopBlocking(component.BlockingItem.Value, blockComp, uid);
			}
		}

		// Token: 0x040013CB RID: 5067
		[Dependency]
		private readonly IPrototypeManager _proto;

		// Token: 0x040013CC RID: 5068
		[Dependency]
		private readonly SharedActionsSystem _actionsSystem;

		// Token: 0x040013CD RID: 5069
		[Dependency]
		private readonly SharedTransformSystem _transformSystem;

		// Token: 0x040013CE RID: 5070
		[Dependency]
		private readonly FixtureSystem _fixtureSystem;

		// Token: 0x040013CF RID: 5071
		[Dependency]
		private readonly SharedHandsSystem _handsSystem;

		// Token: 0x040013D0 RID: 5072
		[Dependency]
		private readonly SharedPopupSystem _popupSystem;

		// Token: 0x040013D1 RID: 5073
		[Dependency]
		private readonly EntityLookupSystem _lookup;

		// Token: 0x040013D2 RID: 5074
		[Dependency]
		private readonly SharedPhysicsSystem _physics;

		// Token: 0x040013D3 RID: 5075
		[Dependency]
		private readonly DamageableSystem _damageable;

		// Token: 0x040013D4 RID: 5076
		[Dependency]
		private readonly SharedAudioSystem _audio;
	}
}
