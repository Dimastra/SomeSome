using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Server.Atmos.Piping.EntitySystems
{
	// Token: 0x0200075E RID: 1886
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class AtmosDeviceSystem : EntitySystem
	{
		// Token: 0x060027F3 RID: 10227 RVA: 0x000D1698 File Offset: 0x000CF898
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<AtmosDeviceComponent, ComponentInit>(new ComponentEventHandler<AtmosDeviceComponent, ComponentInit>(this.OnDeviceInitialize), null, null);
			base.SubscribeLocalEvent<AtmosDeviceComponent, ComponentShutdown>(new ComponentEventHandler<AtmosDeviceComponent, ComponentShutdown>(this.OnDeviceShutdown), null, null);
			base.SubscribeLocalEvent<AtmosDeviceComponent, EntParentChangedMessage>(new ComponentEventRefHandler<AtmosDeviceComponent, EntParentChangedMessage>(this.OnDeviceParentChanged), null, null);
			base.SubscribeLocalEvent<AtmosDeviceComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<AtmosDeviceComponent, AnchorStateChangedEvent>(this.OnDeviceAnchorChanged), null, null);
		}

		// Token: 0x060027F4 RID: 10228 RVA: 0x000D16FC File Offset: 0x000CF8FC
		private bool CanJoinAtmosphere(AtmosDeviceComponent component, TransformComponent transform)
		{
			return (!component.RequireAnchored || transform.Anchored) && transform.GridUid != null;
		}

		// Token: 0x060027F5 RID: 10229 RVA: 0x000D172C File Offset: 0x000CF92C
		public void JoinAtmosphere(AtmosDeviceComponent component)
		{
			TransformComponent transform = base.Transform(component.Owner);
			if (!this.CanJoinAtmosphere(component, transform))
			{
				return;
			}
			if (!this._atmosphereSystem.AddAtmosDevice(transform.GridUid.Value, component))
			{
				if (!component.JoinSystem)
				{
					return;
				}
				this._joinedDevices.Add(component);
				component.JoinedSystem = true;
			}
			component.LastProcess = this._gameTiming.CurTime;
			base.RaiseLocalEvent<AtmosDeviceEnabledEvent>(component.Owner, new AtmosDeviceEnabledEvent(), false);
		}

		// Token: 0x060027F6 RID: 10230 RVA: 0x000D17B0 File Offset: 0x000CF9B0
		public void LeaveAtmosphere(AtmosDeviceComponent component)
		{
			if (component.JoinedGrid != null && !this._atmosphereSystem.RemoveAtmosDevice(component.JoinedGrid.Value, component))
			{
				component.JoinedGrid = null;
				return;
			}
			if (component.JoinedSystem)
			{
				this._joinedDevices.Remove(component);
				component.JoinedSystem = false;
			}
			component.LastProcess = TimeSpan.Zero;
			base.RaiseLocalEvent<AtmosDeviceDisabledEvent>(component.Owner, new AtmosDeviceDisabledEvent(), false);
		}

		// Token: 0x060027F7 RID: 10231 RVA: 0x000D1832 File Offset: 0x000CFA32
		public void RejoinAtmosphere(AtmosDeviceComponent component)
		{
			this.LeaveAtmosphere(component);
			this.JoinAtmosphere(component);
		}

		// Token: 0x060027F8 RID: 10232 RVA: 0x000D1842 File Offset: 0x000CFA42
		private void OnDeviceInitialize(EntityUid uid, AtmosDeviceComponent component, ComponentInit args)
		{
			this.JoinAtmosphere(component);
		}

		// Token: 0x060027F9 RID: 10233 RVA: 0x000D184B File Offset: 0x000CFA4B
		private void OnDeviceShutdown(EntityUid uid, AtmosDeviceComponent component, ComponentShutdown args)
		{
			this.LeaveAtmosphere(component);
		}

		// Token: 0x060027FA RID: 10234 RVA: 0x000D1854 File Offset: 0x000CFA54
		private void OnDeviceAnchorChanged(EntityUid uid, AtmosDeviceComponent component, ref AnchorStateChangedEvent args)
		{
			if (!component.RequireAnchored)
			{
				return;
			}
			if (args.Anchored)
			{
				this.JoinAtmosphere(component);
				return;
			}
			this.LeaveAtmosphere(component);
		}

		// Token: 0x060027FB RID: 10235 RVA: 0x000D1876 File Offset: 0x000CFA76
		private void OnDeviceParentChanged(EntityUid uid, AtmosDeviceComponent component, ref EntParentChangedMessage args)
		{
			this.RejoinAtmosphere(component);
		}

		// Token: 0x060027FC RID: 10236 RVA: 0x000D1880 File Offset: 0x000CFA80
		public override void Update(float frameTime)
		{
			this._timer += frameTime;
			if (this._timer < this._atmosphereSystem.AtmosTime)
			{
				return;
			}
			this._timer -= this._atmosphereSystem.AtmosTime;
			TimeSpan time = this._gameTiming.CurTime;
			foreach (AtmosDeviceComponent device in this._joinedDevices)
			{
				base.RaiseLocalEvent<AtmosDeviceUpdateEvent>(device.Owner, this._updateEvent, false);
				device.LastProcess = time;
			}
		}

		// Token: 0x040018E4 RID: 6372
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x040018E5 RID: 6373
		[Dependency]
		private readonly AtmosphereSystem _atmosphereSystem;

		// Token: 0x040018E6 RID: 6374
		private readonly AtmosDeviceUpdateEvent _updateEvent = new AtmosDeviceUpdateEvent();

		// Token: 0x040018E7 RID: 6375
		private float _timer;

		// Token: 0x040018E8 RID: 6376
		private readonly HashSet<AtmosDeviceComponent> _joinedDevices = new HashSet<AtmosDeviceComponent>();
	}
}
