using System;
using System.Runtime.CompilerServices;
using Content.Server.Power.Components;
using Content.Shared.Power;
using Content.Shared.Rounding;
using Content.Shared.SMES;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Server.Power.SMES
{
	// Token: 0x02000277 RID: 631
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	public sealed class SmesComponent : Component
	{
		// Token: 0x06000CAB RID: 3243 RVA: 0x000422B0 File Offset: 0x000404B0
		protected override void Initialize()
		{
			base.Initialize();
			ComponentExt.EnsureComponentWarn<ServerAppearanceComponent>(base.Owner, null);
		}

		// Token: 0x06000CAC RID: 3244 RVA: 0x000422C8 File Offset: 0x000404C8
		public void OnUpdate()
		{
			int newLevel = this.GetNewChargeLevel();
			if (newLevel != this._lastChargeLevel && this._lastChargeLevelChange + TimeSpan.FromSeconds(1.0) < this._gameTiming.CurTime)
			{
				this._lastChargeLevel = newLevel;
				this._lastChargeLevelChange = this._gameTiming.CurTime;
				AppearanceComponent appearance;
				if (this._entMan.TryGetComponent<AppearanceComponent>(base.Owner, ref appearance))
				{
					appearance.SetData(SmesVisuals.LastChargeLevel, newLevel);
				}
			}
			ChargeState newChargeState = this.GetNewChargeState();
			if (newChargeState != this._lastChargeState && this._lastChargeStateChange + TimeSpan.FromSeconds(1.0) < this._gameTiming.CurTime)
			{
				this._lastChargeState = newChargeState;
				this._lastChargeStateChange = this._gameTiming.CurTime;
				AppearanceComponent appearance2;
				if (this._entMan.TryGetComponent<AppearanceComponent>(base.Owner, ref appearance2))
				{
					appearance2.SetData(SmesVisuals.LastChargeState, newChargeState);
				}
			}
		}

		// Token: 0x06000CAD RID: 3245 RVA: 0x000423CC File Offset: 0x000405CC
		private int GetNewChargeLevel()
		{
			BatteryComponent battery;
			if (!this._entMan.TryGetComponent<BatteryComponent>(base.Owner, ref battery))
			{
				return 0;
			}
			return ContentHelpers.RoundToLevels((double)battery.CurrentCharge, (double)battery.MaxCharge, 6);
		}

		// Token: 0x06000CAE RID: 3246 RVA: 0x00042404 File Offset: 0x00040604
		private ChargeState GetNewChargeState()
		{
			PowerNetworkBatteryComponent battery = this._entMan.GetComponent<PowerNetworkBatteryComponent>(base.Owner);
			float num = battery.CurrentSupply - battery.CurrentReceiving;
			ChargeState result;
			if (num <= 0f)
			{
				if (num >= 0f)
				{
					result = ChargeState.Still;
				}
				else
				{
					result = ChargeState.Charging;
				}
			}
			else
			{
				result = ChargeState.Discharging;
			}
			return result;
		}

		// Token: 0x040007C1 RID: 1985
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x040007C2 RID: 1986
		[Dependency]
		private readonly IGameTiming _gameTiming;

		// Token: 0x040007C3 RID: 1987
		private int _lastChargeLevel;

		// Token: 0x040007C4 RID: 1988
		private TimeSpan _lastChargeLevelChange;

		// Token: 0x040007C5 RID: 1989
		private ChargeState _lastChargeState;

		// Token: 0x040007C6 RID: 1990
		private TimeSpan _lastChargeStateChange;

		// Token: 0x040007C7 RID: 1991
		private const int VisualsChangeDelay = 1;
	}
}
