using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Content.Shared.Alert;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Nutrition.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;
using Robust.Shared.ViewVariables;

namespace Content.Server.Nutrition.Components
{
	// Token: 0x0200031B RID: 795
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[ComponentReference(typeof(SharedHungerComponent))]
	public sealed class HungerComponent : SharedHungerComponent
	{
		// Token: 0x17000263 RID: 611
		// (get) Token: 0x06001073 RID: 4211 RVA: 0x00054E58 File Offset: 0x00053058
		// (set) Token: 0x06001074 RID: 4212 RVA: 0x00054E60 File Offset: 0x00053060
		[ViewVariables]
		public float BaseDecayRate
		{
			get
			{
				return this._baseDecayRate;
			}
			set
			{
				this._baseDecayRate = value;
			}
		}

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x06001075 RID: 4213 RVA: 0x00054E69 File Offset: 0x00053069
		// (set) Token: 0x06001076 RID: 4214 RVA: 0x00054E71 File Offset: 0x00053071
		[ViewVariables]
		public float ActualDecayRate
		{
			get
			{
				return this._actualDecayRate;
			}
			set
			{
				this._actualDecayRate = value;
			}
		}

		// Token: 0x17000265 RID: 613
		// (get) Token: 0x06001077 RID: 4215 RVA: 0x00054E7A File Offset: 0x0005307A
		[ViewVariables]
		public override HungerThreshold CurrentHungerThreshold
		{
			get
			{
				return this._currentHungerThreshold;
			}
		}

		// Token: 0x17000266 RID: 614
		// (get) Token: 0x06001078 RID: 4216 RVA: 0x00054E82 File Offset: 0x00053082
		// (set) Token: 0x06001079 RID: 4217 RVA: 0x00054E8A File Offset: 0x0005308A
		[ViewVariables]
		public float CurrentHunger
		{
			get
			{
				return this._currentHunger;
			}
			set
			{
				this._currentHunger = value;
			}
		}

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x0600107A RID: 4218 RVA: 0x00054E93 File Offset: 0x00053093
		[ViewVariables]
		public Dictionary<HungerThreshold, float> HungerThresholds
		{
			get
			{
				return this._hungerThresholds;
			}
		}

		// Token: 0x0600107B RID: 4219 RVA: 0x00054E9C File Offset: 0x0005309C
		public void HungerThresholdEffect(bool force = false)
		{
			if (this._currentHungerThreshold != this._lastHungerThreshold || force)
			{
				MovementSpeedModifierComponent movementSlowdownComponent;
				if (this._lastHungerThreshold == HungerThreshold.Starving && this._currentHungerThreshold != HungerThreshold.Dead && this._entMan.TryGetComponent<MovementSpeedModifierComponent>(base.Owner, ref movementSlowdownComponent))
				{
					EntitySystem.Get<MovementSpeedModifierSystem>().RefreshMovementSpeedModifiers(base.Owner, null);
				}
				AlertType alertId;
				if (HungerComponent.HungerThresholdAlertTypes.TryGetValue(this._currentHungerThreshold, out alertId))
				{
					EntitySystem.Get<AlertsSystem>().ShowAlert(base.Owner, alertId, null, null);
				}
				else
				{
					EntitySystem.Get<AlertsSystem>().ClearAlertCategory(base.Owner, AlertCategory.Hunger);
				}
				switch (this._currentHungerThreshold)
				{
				case HungerThreshold.Dead:
					return;
				case HungerThreshold.Starving:
					EntitySystem.Get<MovementSpeedModifierSystem>().RefreshMovementSpeedModifiers(base.Owner, null);
					this._lastHungerThreshold = this._currentHungerThreshold;
					this._actualDecayRate = this._baseDecayRate * 0.6f;
					return;
				case HungerThreshold.Peckish:
					this._lastHungerThreshold = this._currentHungerThreshold;
					this._actualDecayRate = this._baseDecayRate * 0.8f;
					return;
				case HungerThreshold.Okay:
					this._lastHungerThreshold = this._currentHungerThreshold;
					this._actualDecayRate = this._baseDecayRate;
					return;
				case HungerThreshold.Overfed:
					this._lastHungerThreshold = this._currentHungerThreshold;
					this._actualDecayRate = this._baseDecayRate * 1.2f;
					return;
				}
				string text = "hunger";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
				defaultInterpolatedStringHandler.AppendLiteral("No hunger threshold found for ");
				defaultInterpolatedStringHandler.AppendFormatted<HungerThreshold>(this._currentHungerThreshold);
				Logger.ErrorS(text, defaultInterpolatedStringHandler.ToStringAndClear());
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
				defaultInterpolatedStringHandler.AppendLiteral("No hunger threshold found for ");
				defaultInterpolatedStringHandler.AppendFormatted<HungerThreshold>(this._currentHungerThreshold);
				throw new ArgumentOutOfRangeException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}

		// Token: 0x0600107C RID: 4220 RVA: 0x00055068 File Offset: 0x00053268
		protected override void Startup()
		{
			base.Startup();
			if (this._currentHunger < 0f)
			{
				this._currentHunger = (float)this._random.Next((int)this._hungerThresholds[HungerThreshold.Peckish] + 10, (int)this._hungerThresholds[HungerThreshold.Okay] - 1);
			}
			this._currentHungerThreshold = this.GetHungerThreshold(this._currentHunger);
			this._lastHungerThreshold = HungerThreshold.Okay;
			this.HungerThresholdEffect(true);
			base.Dirty(null);
		}

		// Token: 0x0600107D RID: 4221 RVA: 0x000550E0 File Offset: 0x000532E0
		public HungerThreshold GetHungerThreshold(float food)
		{
			HungerThreshold result = HungerThreshold.Dead;
			float value = this.HungerThresholds[HungerThreshold.Overfed];
			foreach (KeyValuePair<HungerThreshold, float> threshold in this._hungerThresholds)
			{
				if (threshold.Value <= value && threshold.Value >= food)
				{
					result = threshold.Key;
					value = threshold.Value;
				}
			}
			return result;
		}

		// Token: 0x0600107E RID: 4222 RVA: 0x00055160 File Offset: 0x00053360
		public void UpdateFood(float amount)
		{
			this._currentHunger = Math.Clamp(this._currentHunger + amount, this.HungerThresholds[HungerThreshold.Dead], this.HungerThresholds[HungerThreshold.Overfed]);
		}

		// Token: 0x0600107F RID: 4223 RVA: 0x0005518D File Offset: 0x0005338D
		public void OnUpdate(float frametime)
		{
			this.UpdateFood(-frametime * this.ActualDecayRate);
			this.UpdateCurrentThreshold();
		}

		// Token: 0x06001080 RID: 4224 RVA: 0x000551A4 File Offset: 0x000533A4
		private void UpdateCurrentThreshold()
		{
			HungerThreshold calculatedHungerThreshold = this.GetHungerThreshold(this._currentHunger);
			if (calculatedHungerThreshold != this._currentHungerThreshold)
			{
				this._currentHungerThreshold = calculatedHungerThreshold;
				this.HungerThresholdEffect(false);
				base.Dirty(null);
			}
		}

		// Token: 0x06001081 RID: 4225 RVA: 0x000551DC File Offset: 0x000533DC
		public void ResetFood()
		{
			this._currentHunger = this.HungerThresholds[HungerThreshold.Okay];
			this.UpdateCurrentThreshold();
		}

		// Token: 0x06001082 RID: 4226 RVA: 0x000551F6 File Offset: 0x000533F6
		public override ComponentState GetComponentState()
		{
			return new SharedHungerComponent.HungerComponentState(this._currentHungerThreshold);
		}

		// Token: 0x04000994 RID: 2452
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x04000995 RID: 2453
		[Dependency]
		private readonly IRobustRandom _random;

		// Token: 0x04000996 RID: 2454
		[DataField("baseDecayRate", false, 1, false, false, null)]
		private float _baseDecayRate = 0.016666668f;

		// Token: 0x04000997 RID: 2455
		private float _actualDecayRate;

		// Token: 0x04000998 RID: 2456
		private HungerThreshold _currentHungerThreshold;

		// Token: 0x04000999 RID: 2457
		private HungerThreshold _lastHungerThreshold;

		// Token: 0x0400099A RID: 2458
		[DataField("startingHunger", false, 1, false, false, null)]
		private float _currentHunger = -1f;

		// Token: 0x0400099B RID: 2459
		[DataField("thresholds", false, 1, false, false, typeof(DictionarySerializer<HungerThreshold, float>))]
		private Dictionary<HungerThreshold, float> _hungerThresholds = new Dictionary<HungerThreshold, float>
		{
			{
				HungerThreshold.Overfed,
				200f
			},
			{
				HungerThreshold.Okay,
				150f
			},
			{
				HungerThreshold.Peckish,
				100f
			},
			{
				HungerThreshold.Starving,
				50f
			},
			{
				HungerThreshold.Dead,
				0f
			}
		};

		// Token: 0x0400099C RID: 2460
		public static readonly Dictionary<HungerThreshold, AlertType> HungerThresholdAlertTypes = new Dictionary<HungerThreshold, AlertType>
		{
			{
				HungerThreshold.Peckish,
				AlertType.Peckish
			},
			{
				HungerThreshold.Starving,
				AlertType.Starving
			},
			{
				HungerThreshold.Dead,
				AlertType.Starving
			}
		};
	}
}
