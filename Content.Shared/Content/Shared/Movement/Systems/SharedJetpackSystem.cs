using System;
using System.Runtime.CompilerServices;
using Content.Shared.Actions;
using Content.Shared.Gravity;
using Content.Shared.Interaction.Events;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Popups;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared.Movement.Systems
{
	// Token: 0x020002DA RID: 730
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class SharedJetpackSystem : EntitySystem
	{
		// Token: 0x06000801 RID: 2049 RVA: 0x0001A7E4 File Offset: 0x000189E4
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<JetpackComponent, GetItemActionsEvent>(new ComponentEventHandler<JetpackComponent, GetItemActionsEvent>(this.OnJetpackGetAction), null, null);
			base.SubscribeLocalEvent<JetpackComponent, DroppedEvent>(new ComponentEventHandler<JetpackComponent, DroppedEvent>(this.OnJetpackDropped), null, null);
			base.SubscribeLocalEvent<JetpackComponent, ToggleJetpackEvent>(new ComponentEventHandler<JetpackComponent, ToggleJetpackEvent>(this.OnJetpackToggle), null, null);
			base.SubscribeLocalEvent<JetpackComponent, CanWeightlessMoveEvent>(new ComponentEventRefHandler<JetpackComponent, CanWeightlessMoveEvent>(this.OnJetpackCanWeightlessMove), null, null);
			base.SubscribeLocalEvent<JetpackUserComponent, CanWeightlessMoveEvent>(new ComponentEventRefHandler<JetpackUserComponent, CanWeightlessMoveEvent>(this.OnJetpackUserCanWeightless), null, null);
			base.SubscribeLocalEvent<JetpackUserComponent, EntParentChangedMessage>(new ComponentEventRefHandler<JetpackUserComponent, EntParentChangedMessage>(this.OnJetpackUserEntParentChanged), null, null);
			base.SubscribeLocalEvent<JetpackUserComponent, ComponentGetState>(new ComponentEventRefHandler<JetpackUserComponent, ComponentGetState>(this.OnJetpackUserGetState), null, null);
			base.SubscribeLocalEvent<JetpackUserComponent, ComponentHandleState>(new ComponentEventRefHandler<JetpackUserComponent, ComponentHandleState>(this.OnJetpackUserHandleState), null, null);
			base.SubscribeLocalEvent<GravityChangedEvent>(new EntityEventRefHandler<GravityChangedEvent>(this.OnJetpackUserGravityChanged), null, null);
		}

		// Token: 0x06000802 RID: 2050 RVA: 0x0001A8AB File Offset: 0x00018AAB
		private void OnJetpackCanWeightlessMove(EntityUid uid, JetpackComponent component, ref CanWeightlessMoveEvent args)
		{
			args.CanMove = true;
		}

		// Token: 0x06000803 RID: 2051 RVA: 0x0001A8B4 File Offset: 0x00018AB4
		private void OnJetpackUserGravityChanged(ref GravityChangedEvent ev)
		{
			EntityUid gridUid = ev.ChangedGridIndex;
			EntityQuery<JetpackComponent> jetpackQuery = base.GetEntityQuery<JetpackComponent>();
			foreach (ValueTuple<JetpackUserComponent, TransformComponent> valueTuple in base.EntityQuery<JetpackUserComponent, TransformComponent>(true))
			{
				JetpackUserComponent user = valueTuple.Item1;
				EntityUid? gridUid2 = valueTuple.Item2.GridUid;
				EntityUid entityUid = gridUid;
				JetpackComponent jetpack;
				if (gridUid2 != null && (gridUid2 == null || gridUid2.GetValueOrDefault() == entityUid) && ev.HasGravity && jetpackQuery.TryGetComponent(user.Jetpack, ref jetpack))
				{
					if (this._timing.IsFirstTimePredicted)
					{
						this._popups.PopupEntity(Loc.GetString("jetpack-to-grid"), user.Jetpack, user.Owner, PopupType.Small);
					}
					this.SetEnabled(jetpack, false, new EntityUid?(user.Owner));
				}
			}
		}

		// Token: 0x06000804 RID: 2052 RVA: 0x0001A9A8 File Offset: 0x00018BA8
		private void OnJetpackUserHandleState(EntityUid uid, JetpackUserComponent component, ref ComponentHandleState args)
		{
			SharedJetpackSystem.JetpackUserComponentState state = args.Current as SharedJetpackSystem.JetpackUserComponentState;
			if (state == null)
			{
				return;
			}
			component.Jetpack = state.Jetpack;
		}

		// Token: 0x06000805 RID: 2053 RVA: 0x0001A9D1 File Offset: 0x00018BD1
		private void OnJetpackUserGetState(EntityUid uid, JetpackUserComponent component, ref ComponentGetState args)
		{
			args.State = new SharedJetpackSystem.JetpackUserComponentState
			{
				Jetpack = component.Jetpack
			};
		}

		// Token: 0x06000806 RID: 2054 RVA: 0x0001A9EA File Offset: 0x00018BEA
		private void OnJetpackDropped(EntityUid uid, JetpackComponent component, DroppedEvent args)
		{
			this.SetEnabled(component, false, new EntityUid?(args.User));
		}

		// Token: 0x06000807 RID: 2055 RVA: 0x0001A9FF File Offset: 0x00018BFF
		private void OnJetpackUserCanWeightless(EntityUid uid, JetpackUserComponent component, ref CanWeightlessMoveEvent args)
		{
			args.CanMove = true;
		}

		// Token: 0x06000808 RID: 2056 RVA: 0x0001AA08 File Offset: 0x00018C08
		private void OnJetpackUserEntParentChanged(EntityUid uid, JetpackUserComponent component, ref EntParentChangedMessage args)
		{
			JetpackComponent jetpack;
			if (base.TryComp<JetpackComponent>(component.Jetpack, ref jetpack) && !this.CanEnableOnGrid(args.Transform.GridUid))
			{
				this.SetEnabled(jetpack, false, new EntityUid?(uid));
				if (this._timing.IsFirstTimePredicted && this._network.IsClient)
				{
					this._popups.PopupEntity(Loc.GetString("jetpack-to-grid"), uid, uid, PopupType.Small);
				}
			}
		}

		// Token: 0x06000809 RID: 2057 RVA: 0x0001AA78 File Offset: 0x00018C78
		private void SetupUser(EntityUid uid, JetpackComponent component)
		{
			JetpackUserComponent jetpackUserComponent = base.EnsureComp<JetpackUserComponent>(uid);
			RelayInputMoverComponent relay = base.EnsureComp<RelayInputMoverComponent>(uid);
			this._mover.SetRelay(uid, component.Owner, relay);
			jetpackUserComponent.Jetpack = component.Owner;
		}

		// Token: 0x0600080A RID: 2058 RVA: 0x0001AAB2 File Offset: 0x00018CB2
		private void RemoveUser(EntityUid uid)
		{
			if (!base.RemComp<JetpackUserComponent>(uid))
			{
				return;
			}
			base.RemComp<RelayInputMoverComponent>(uid);
		}

		// Token: 0x0600080B RID: 2059 RVA: 0x0001AAC8 File Offset: 0x00018CC8
		private void OnJetpackToggle(EntityUid uid, JetpackComponent component, ToggleJetpackEvent args)
		{
			if (args.Handled)
			{
				return;
			}
			TransformComponent xform;
			if (base.TryComp<TransformComponent>(uid, ref xform) && !this.CanEnableOnGrid(xform.GridUid))
			{
				if (this._timing.IsFirstTimePredicted)
				{
					this._popups.PopupEntity(Loc.GetString("jetpack-no-station"), uid, args.Performer, PopupType.Small);
				}
				return;
			}
			this.SetEnabled(component, !this.IsEnabled(uid), null);
		}

		// Token: 0x0600080C RID: 2060 RVA: 0x0001AB3C File Offset: 0x00018D3C
		private bool CanEnableOnGrid(EntityUid? gridUid)
		{
			if (gridUid != null)
			{
				return !base.EnsureComp<GravityComponent>(gridUid.Value).EnabledVV;
			}
			return gridUid == null;
		}

		// Token: 0x0600080D RID: 2061 RVA: 0x0001AB69 File Offset: 0x00018D69
		private void OnJetpackGetAction(EntityUid uid, JetpackComponent component, GetItemActionsEvent args)
		{
			args.Actions.Add(component.ToggleAction);
		}

		// Token: 0x0600080E RID: 2062 RVA: 0x0001AB7D File Offset: 0x00018D7D
		private bool IsEnabled(EntityUid uid)
		{
			return base.HasComp<ActiveJetpackComponent>(uid);
		}

		// Token: 0x0600080F RID: 2063 RVA: 0x0001AB88 File Offset: 0x00018D88
		public void SetEnabled(JetpackComponent component, bool enabled, EntityUid? user = null)
		{
			if (this.IsEnabled(component.Owner) == enabled || (enabled && !this.CanEnable(component)))
			{
				return;
			}
			if (enabled)
			{
				base.EnsureComp<ActiveJetpackComponent>(component.Owner);
			}
			else
			{
				base.RemComp<ActiveJetpackComponent>(component.Owner);
			}
			if (user == null)
			{
				IContainer container;
				this.Container.TryGetContainingContainer(component.Owner, ref container, null, null);
				user = ((container != null) ? new EntityUid?(container.Owner) : null);
			}
			if (user == null && enabled)
			{
				return;
			}
			if (user != null)
			{
				if (enabled)
				{
					this.SetupUser(user.Value, component);
				}
				else
				{
					this.RemoveUser(user.Value);
				}
				this.MovementSpeedModifier.RefreshMovementSpeedModifiers(user.Value, null);
			}
			this.Appearance.SetData(component.Owner, JetpackVisuals.Enabled, enabled, null);
			base.Dirty(component, null);
		}

		// Token: 0x06000810 RID: 2064 RVA: 0x0001AC7B File Offset: 0x00018E7B
		public bool IsUserFlying(EntityUid uid)
		{
			return base.HasComp<JetpackUserComponent>(uid);
		}

		// Token: 0x06000811 RID: 2065 RVA: 0x0001AC84 File Offset: 0x00018E84
		protected virtual bool CanEnable(JetpackComponent component)
		{
			return true;
		}

		// Token: 0x04000830 RID: 2096
		[Dependency]
		private readonly IGameTiming _timing;

		// Token: 0x04000831 RID: 2097
		[Dependency]
		private readonly INetManager _network;

		// Token: 0x04000832 RID: 2098
		[Dependency]
		protected readonly MovementSpeedModifierSystem MovementSpeedModifier;

		// Token: 0x04000833 RID: 2099
		[Dependency]
		protected readonly SharedAppearanceSystem Appearance;

		// Token: 0x04000834 RID: 2100
		[Dependency]
		protected readonly SharedContainerSystem Container;

		// Token: 0x04000835 RID: 2101
		[Dependency]
		private readonly SharedPopupSystem _popups;

		// Token: 0x04000836 RID: 2102
		[Dependency]
		private readonly SharedMoverController _mover;

		// Token: 0x020007CB RID: 1995
		[NullableContext(0)]
		[NetSerializable]
		[Serializable]
		protected sealed class JetpackUserComponentState : ComponentState
		{
			// Token: 0x04001817 RID: 6167
			public EntityUid Jetpack;
		}
	}
}
