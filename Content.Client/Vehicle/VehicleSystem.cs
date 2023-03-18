using System;
using System.Runtime.CompilerServices;
using Content.Shared.Vehicle;
using Content.Shared.Vehicle.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Client.Vehicle
{
	// Token: 0x02000067 RID: 103
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class VehicleSystem : SharedVehicleSystem
	{
		// Token: 0x060001ED RID: 493 RVA: 0x0000DAAC File Offset: 0x0000BCAC
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RiderComponent, ComponentStartup>(new ComponentEventHandler<RiderComponent, ComponentStartup>(this.OnRiderStartup), null, null);
			base.SubscribeLocalEvent<RiderComponent, ComponentShutdown>(new ComponentEventHandler<RiderComponent, ComponentShutdown>(this.OnRiderShutdown), null, null);
			base.SubscribeLocalEvent<RiderComponent, ComponentHandleState>(new ComponentEventRefHandler<RiderComponent, ComponentHandleState>(this.OnRiderHandleState), null, null);
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000DAFC File Offset: 0x0000BCFC
		private void OnRiderStartup(EntityUid uid, RiderComponent component, ComponentStartup args)
		{
			EyeComponent eyeComponent;
			if (base.TryComp<EyeComponent>(uid, ref eyeComponent))
			{
				EyeComponent eyeComponent2 = eyeComponent;
				EntityUid? target = eyeComponent2.Target;
				if (target == null)
				{
					eyeComponent2.Target = component.Vehicle;
				}
			}
		}

		// Token: 0x060001EF RID: 495 RVA: 0x0000DB34 File Offset: 0x0000BD34
		private void OnRiderShutdown(EntityUid uid, RiderComponent component, ComponentShutdown args)
		{
			EyeComponent eyeComponent;
			if (base.TryComp<EyeComponent>(uid, ref eyeComponent) && eyeComponent.Target == component.Vehicle)
			{
				eyeComponent.Target = null;
			}
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000DB9C File Offset: 0x0000BD9C
		private void OnRiderHandleState(EntityUid uid, RiderComponent component, ref ComponentHandleState args)
		{
			SharedVehicleSystem.RiderComponentState riderComponentState = args.Current as SharedVehicleSystem.RiderComponentState;
			if (riderComponentState == null)
			{
				return;
			}
			EyeComponent eyeComponent;
			if (base.TryComp<EyeComponent>(uid, ref eyeComponent) && eyeComponent.Target == component.Vehicle)
			{
				eyeComponent.Target = riderComponentState.Entity;
			}
			component.Vehicle = riderComponentState.Entity;
		}
	}
}
