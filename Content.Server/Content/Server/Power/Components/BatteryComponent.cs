using System;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Server.Power.Components
{
	// Token: 0x020002AB RID: 683
	[NullableContext(1)]
	[Nullable(0)]
	[RegisterComponent]
	[Virtual]
	public class BatteryComponent : Component
	{
		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x06000DD9 RID: 3545 RVA: 0x00047977 File Offset: 0x00045B77
		// (set) Token: 0x06000DDA RID: 3546 RVA: 0x0004797F File Offset: 0x00045B7F
		[ViewVariables]
		public float MaxCharge
		{
			get
			{
				return this._maxCharge;
			}
			set
			{
				this.SetMaxCharge(value);
			}
		}

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x06000DDB RID: 3547 RVA: 0x00047988 File Offset: 0x00045B88
		// (set) Token: 0x06000DDC RID: 3548 RVA: 0x00047990 File Offset: 0x00045B90
		[ViewVariables]
		public float CurrentCharge
		{
			get
			{
				return this._currentCharge;
			}
			set
			{
				this.SetCurrentCharge(value);
			}
		}

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x06000DDD RID: 3549 RVA: 0x00047999 File Offset: 0x00045B99
		[ViewVariables]
		public bool IsFullyCharged
		{
			get
			{
				return MathHelper.CloseToPercent(this.CurrentCharge, this.MaxCharge, 1E-05);
			}
		}

		// Token: 0x06000DDE RID: 3550 RVA: 0x000479B5 File Offset: 0x00045BB5
		public virtual bool TryUseCharge(float chargeToUse)
		{
			if (chargeToUse > this.CurrentCharge)
			{
				return false;
			}
			this.CurrentCharge -= chargeToUse;
			return true;
		}

		// Token: 0x06000DDF RID: 3551 RVA: 0x000479D4 File Offset: 0x00045BD4
		public virtual float UseCharge(float toDeduct)
		{
			float chargeChangedBy = Math.Min(this.CurrentCharge, toDeduct);
			this.CurrentCharge -= chargeChangedBy;
			return chargeChangedBy;
		}

		// Token: 0x06000DE0 RID: 3552 RVA: 0x00047A00 File Offset: 0x00045C00
		public void FillFrom(BatteryComponent battery)
		{
			float powerDeficit = this.MaxCharge - this.CurrentCharge;
			if (battery.TryUseCharge(powerDeficit))
			{
				this.CurrentCharge += powerDeficit;
				return;
			}
			this.CurrentCharge += battery.CurrentCharge;
			battery.CurrentCharge = 0f;
		}

		// Token: 0x06000DE1 RID: 3553 RVA: 0x00047A54 File Offset: 0x00045C54
		protected virtual void OnChargeChanged()
		{
			this._entMan.EventBus.RaiseLocalEvent<ChargeChangedEvent>(base.Owner, default(ChargeChangedEvent), false);
		}

		// Token: 0x06000DE2 RID: 3554 RVA: 0x00047A81 File Offset: 0x00045C81
		private void SetMaxCharge(float newMax)
		{
			this._maxCharge = Math.Max(newMax, 0f);
			this._currentCharge = Math.Min(this._currentCharge, this.MaxCharge);
			this.OnChargeChanged();
		}

		// Token: 0x06000DE3 RID: 3555 RVA: 0x00047AB1 File Offset: 0x00045CB1
		private void SetCurrentCharge(float newChargeAmount)
		{
			this._currentCharge = MathHelper.Clamp(newChargeAmount, 0f, this.MaxCharge);
			this.OnChargeChanged();
		}

		// Token: 0x0400082B RID: 2091
		[Dependency]
		private readonly IEntityManager _entMan;

		// Token: 0x0400082C RID: 2092
		[DataField("maxCharge", false, 1, false, false, null)]
		private float _maxCharge;

		// Token: 0x0400082D RID: 2093
		[DataField("startingCharge", false, 1, false, false, null)]
		private float _currentCharge;

		// Token: 0x0400082E RID: 2094
		[DataField("pricePerJoule", false, 1, false, false, null)]
		[ViewVariables]
		public float PricePerJoule = 0.0001f;
	}
}
