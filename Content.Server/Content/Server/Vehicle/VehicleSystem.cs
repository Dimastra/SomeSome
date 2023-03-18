using System;
using System.Runtime.CompilerServices;
using Content.Server.Buckle.Systems;
using Content.Server.Hands.Systems;
using Content.Server.Light.Components;
using Content.Server.Standing;
using Content.Shared.Actions;
using Content.Shared.Buckle.Components;
using Content.Shared.Hands;
using Content.Shared.Mobs;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Vehicle;
using Content.Shared.Vehicle.Components;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;

namespace Content.Server.Vehicle
{
	// Token: 0x020000D7 RID: 215
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class VehicleSystem : SharedVehicleSystem
	{
		// Token: 0x060003DE RID: 990 RVA: 0x00014928 File Offset: 0x00012B28
		public override void Initialize()
		{
			base.Initialize();
			this.InitializeRider();
			base.SubscribeLocalEvent<VehicleComponent, HonkActionEvent>(new ComponentEventHandler<VehicleComponent, HonkActionEvent>(this.OnHonk), null, null);
			base.SubscribeLocalEvent<VehicleComponent, BuckleChangeEvent>(new ComponentEventHandler<VehicleComponent, BuckleChangeEvent>(this.OnBuckleChange), null, null);
		}

		// Token: 0x060003DF RID: 991 RVA: 0x00014960 File Offset: 0x00012B60
		private void OnHonk(EntityUid uid, VehicleComponent vehicle, HonkActionEvent args)
		{
			if (args.Handled || vehicle.HornSound == null)
			{
				return;
			}
			IPlayingAudioStream honkPlayingStream = vehicle.HonkPlayingStream;
			if (honkPlayingStream != null)
			{
				honkPlayingStream.Stop();
			}
			vehicle.HonkPlayingStream = SoundSystem.Play(vehicle.HornSound.GetSound(null, null), Filter.Pvs(uid, 2f, null, null, null), uid, new AudioParams?(vehicle.HornSound.Params));
			args.Handled = true;
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x000149D0 File Offset: 0x00012BD0
		public override void Update(float frameTime)
		{
			foreach (ValueTuple<VehicleComponent, InputMoverComponent> valueTuple in base.EntityQuery<VehicleComponent, InputMoverComponent>(false))
			{
				VehicleComponent vehicle = valueTuple.Item1;
				InputMoverComponent mover = valueTuple.Item2;
				if (this._mover.GetVelocityInput(mover).Item2 == Vector2.Zero)
				{
					base.UpdateAutoAnimate(vehicle.Owner, false);
				}
				else
				{
					base.UpdateAutoAnimate(vehicle.Owner, true);
				}
			}
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x00014A5C File Offset: 0x00012C5C
		private void OnBuckleChange(EntityUid uid, VehicleComponent component, BuckleChangeEvent args)
		{
			if (!args.Buckling)
			{
				this._actionsSystem.RemoveProvidedActions(args.BuckledEntity, uid, null);
				this._virtualItemSystem.DeleteInHandsMatching(args.BuckledEntity, uid);
				base.RemComp<RiderComponent>(args.BuckledEntity);
				base.RemComp<RelayInputMoverComponent>(args.BuckledEntity);
				component.Rider = null;
				return;
			}
			if (!this._virtualItemSystem.TrySpawnVirtualItemInHand(uid, args.BuckledEntity))
			{
				this.UnbuckleFromVehicle(args.BuckledEntity);
				return;
			}
			base.EnsureComp<InputMoverComponent>(uid);
			RiderComponent riderComponent = base.EnsureComp<RiderComponent>(args.BuckledEntity);
			component.Rider = new EntityUid?(args.BuckledEntity);
			RelayInputMoverComponent relay = base.EnsureComp<RelayInputMoverComponent>(args.BuckledEntity);
			this._mover.SetRelay(args.BuckledEntity, uid, relay);
			riderComponent.Vehicle = new EntityUid?(uid);
			base.UpdateBuckleOffset(base.Transform(uid), component);
			base.UpdateDrawDepth(uid, base.GetDrawDepth(base.Transform(uid), component.NorthOnly));
			ActionsComponent actions;
			UnpoweredFlashlightComponent flashlight;
			if (base.TryComp<ActionsComponent>(args.BuckledEntity, ref actions) && base.TryComp<UnpoweredFlashlightComponent>(uid, ref flashlight))
			{
				this._actionsSystem.AddAction(args.BuckledEntity, flashlight.ToggleAction, new EntityUid?(uid), actions, true);
			}
			if (component.HornSound != null)
			{
				this._actionsSystem.AddAction(args.BuckledEntity, component.HornAction, new EntityUid?(uid), actions, true);
			}
			this._joints.ClearJoints(args.BuckledEntity, null);
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x00014BCC File Offset: 0x00012DCC
		private void InitializeRider()
		{
			base.SubscribeLocalEvent<RiderComponent, ComponentGetState>(new ComponentEventRefHandler<RiderComponent, ComponentGetState>(this.OnRiderGetState), null, null);
			base.SubscribeLocalEvent<RiderComponent, VirtualItemDeletedEvent>(new ComponentEventHandler<RiderComponent, VirtualItemDeletedEvent>(this.OnVirtualItemDeleted), null, null);
			base.SubscribeLocalEvent<RiderComponent, FellDownEvent>(new ComponentEventHandler<RiderComponent, FellDownEvent>(this.OnFallDown), null, null);
			base.SubscribeLocalEvent<RiderComponent, MobStateChangedEvent>(new ComponentEventHandler<RiderComponent, MobStateChangedEvent>(this.OnMobStateChanged), null, null);
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x00014C29 File Offset: 0x00012E29
		private void OnRiderGetState(EntityUid uid, RiderComponent component, ref ComponentGetState args)
		{
			args.State = new SharedVehicleSystem.RiderComponentState
			{
				Entity = component.Vehicle
			};
		}

		// Token: 0x060003E4 RID: 996 RVA: 0x00014C44 File Offset: 0x00012E44
		private void OnVirtualItemDeleted(EntityUid uid, RiderComponent component, VirtualItemDeletedEvent args)
		{
			if (args.BlockingEntity == component.Vehicle)
			{
				this.UnbuckleFromVehicle(uid);
			}
		}

		// Token: 0x060003E5 RID: 997 RVA: 0x00014C81 File Offset: 0x00012E81
		private void OnFallDown(EntityUid uid, RiderComponent rider, FellDownEvent args)
		{
			this.UnbuckleFromVehicle(uid);
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x00014C8C File Offset: 0x00012E8C
		private void OnMobStateChanged(EntityUid uid, RiderComponent rider, MobStateChangedEvent args)
		{
			MobState newMobState = args.NewMobState;
			if (newMobState == MobState.Critical || newMobState == MobState.Dead)
			{
				this.UnbuckleFromVehicle(uid);
			}
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x00014CB0 File Offset: 0x00012EB0
		public void UnbuckleFromVehicle(EntityUid uid)
		{
			this._buckle.TryUnbuckle(uid, uid, true, null);
		}

		// Token: 0x04000268 RID: 616
		[Dependency]
		private readonly BuckleSystem _buckle;

		// Token: 0x04000269 RID: 617
		[Dependency]
		private readonly HandVirtualItemSystem _virtualItemSystem;

		// Token: 0x0400026A RID: 618
		[Dependency]
		private readonly SharedActionsSystem _actionsSystem;

		// Token: 0x0400026B RID: 619
		[Dependency]
		private readonly SharedJointSystem _joints;

		// Token: 0x0400026C RID: 620
		[Dependency]
		private readonly SharedMoverController _mover;
	}
}
