using System;
using System.Runtime.CompilerServices;
using Content.Server.Popups;
using Content.Server.Power.Components;
using Content.Server.Singularity.Components;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Radiation.Events;
using Content.Shared.Singularity.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Timing;

namespace Content.Server.Singularity.EntitySystems
{
	// Token: 0x020001EC RID: 492
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class RadiationCollectorSystem : EntitySystem
	{
		// Token: 0x06000982 RID: 2434 RVA: 0x000304A3 File Offset: 0x0002E6A3
		public override void Initialize()
		{
			base.Initialize();
			base.SubscribeLocalEvent<RadiationCollectorComponent, InteractHandEvent>(new ComponentEventHandler<RadiationCollectorComponent, InteractHandEvent>(this.OnInteractHand), null, null);
			base.SubscribeLocalEvent<RadiationCollectorComponent, OnIrradiatedEvent>(new ComponentEventHandler<RadiationCollectorComponent, OnIrradiatedEvent>(this.OnRadiation), null, null);
		}

		// Token: 0x06000983 RID: 2435 RVA: 0x000304D4 File Offset: 0x0002E6D4
		private void OnInteractHand(EntityUid uid, RadiationCollectorComponent component, InteractHandEvent args)
		{
			TimeSpan curTime = this._gameTiming.CurTime;
			if (curTime < component.CoolDownEnd)
			{
				return;
			}
			this.ToggleCollector(uid, new EntityUid?(args.User), component);
			component.CoolDownEnd = curTime + component.Cooldown;
		}

		// Token: 0x06000984 RID: 2436 RVA: 0x00030524 File Offset: 0x0002E724
		private void OnRadiation(EntityUid uid, RadiationCollectorComponent component, OnIrradiatedEvent args)
		{
			if (!component.Enabled)
			{
				return;
			}
			BatteryComponent batteryComponent;
			if (base.TryComp<BatteryComponent>(uid, ref batteryComponent))
			{
				float charge = args.TotalRads * component.ChargeModifier;
				batteryComponent.CurrentCharge += charge;
			}
		}

		// Token: 0x06000985 RID: 2437 RVA: 0x00030561 File Offset: 0x0002E761
		[NullableContext(2)]
		public void ToggleCollector(EntityUid uid, EntityUid? user = null, RadiationCollectorComponent component = null)
		{
			if (!base.Resolve<RadiationCollectorComponent>(uid, ref component, true))
			{
				return;
			}
			this.SetCollectorEnabled(uid, !component.Enabled, user, component);
		}

		// Token: 0x06000986 RID: 2438 RVA: 0x00030584 File Offset: 0x0002E784
		[NullableContext(2)]
		public void SetCollectorEnabled(EntityUid uid, bool enabled, EntityUid? user = null, RadiationCollectorComponent component = null)
		{
			if (!base.Resolve<RadiationCollectorComponent>(uid, ref component, true))
			{
				return;
			}
			component.Enabled = enabled;
			if (user != null)
			{
				string msg = component.Enabled ? "radiation-collector-component-use-on" : "radiation-collector-component-use-off";
				this._popupSystem.PopupEntity(Loc.GetString(msg), uid, PopupType.Small);
			}
			this.UpdateAppearance(uid, component, null);
		}

		// Token: 0x06000987 RID: 2439 RVA: 0x000305E4 File Offset: 0x0002E7E4
		[NullableContext(2)]
		private void UpdateAppearance(EntityUid uid, RadiationCollectorComponent component, AppearanceComponent appearance = null)
		{
			if (!base.Resolve<RadiationCollectorComponent, AppearanceComponent>(uid, ref component, ref appearance, true))
			{
				return;
			}
			RadiationCollectorVisualState state = component.Enabled ? RadiationCollectorVisualState.Active : RadiationCollectorVisualState.Deactive;
			this._appearance.SetData(uid, RadiationCollectorVisuals.VisualState, state, appearance);
		}

		// Token: 0x040005B5 RID: 1461
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x040005B6 RID: 1462
		[Dependency]
		private readonly PopupSystem _popupSystem;

		// Token: 0x040005B7 RID: 1463
		[Dependency]
		private readonly SharedAppearanceSystem _appearance;
	}
}
