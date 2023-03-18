using System;
using System.Runtime.CompilerServices;
using Robust.Shared.GameObjects;

namespace Content.Server.Wires
{
	// Token: 0x0200006E RID: 110
	[NullableContext(1)]
	[Nullable(0)]
	public abstract class BaseToggleWireAction : BaseWireAction
	{
		// Token: 0x0600013F RID: 319
		public abstract void ToggleValue(EntityUid owner, bool setting);

		// Token: 0x06000140 RID: 320
		public abstract bool GetValue(EntityUid owner);

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000141 RID: 321 RVA: 0x000080D1 File Offset: 0x000062D1
		[Nullable(2)]
		public virtual object TimeoutKey { [NullableContext(2)] get; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000142 RID: 322 RVA: 0x000080D9 File Offset: 0x000062D9
		public virtual int Delay { get; } = 30;

		// Token: 0x06000143 RID: 323 RVA: 0x000080E1 File Offset: 0x000062E1
		public override bool Cut(EntityUid user, Wire wire)
		{
			base.Cut(user, wire);
			this.ToggleValue(wire.Owner, false);
			if (this.TimeoutKey != null)
			{
				this.WiresSystem.TryCancelWireAction(wire.Owner, this.TimeoutKey);
			}
			return true;
		}

		// Token: 0x06000144 RID: 324 RVA: 0x0000811A File Offset: 0x0000631A
		public override bool Mend(EntityUid user, Wire wire)
		{
			base.Mend(user, wire);
			this.ToggleValue(wire.Owner, true);
			return true;
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00008134 File Offset: 0x00006334
		public override void Pulse(EntityUid user, Wire wire)
		{
			base.Pulse(user, wire);
			this.ToggleValue(wire.Owner, !this.GetValue(wire.Owner));
			if (this.TimeoutKey != null)
			{
				this.WiresSystem.StartWireAction(wire.Owner, (float)this.Delay, this.TimeoutKey, new TimedWireEvent(new WireActionDelegate(this.AwaitPulseCancel), wire));
			}
		}

		// Token: 0x06000146 RID: 326 RVA: 0x0000819C File Offset: 0x0000639C
		public override void Update(Wire wire)
		{
			if (this.TimeoutKey != null && !base.IsPowered(wire.Owner))
			{
				this.WiresSystem.TryCancelWireAction(wire.Owner, this.TimeoutKey);
			}
		}

		// Token: 0x06000147 RID: 327 RVA: 0x000081CC File Offset: 0x000063CC
		private void AwaitPulseCancel(Wire wire)
		{
			if (!wire.IsCut)
			{
				this.ToggleValue(wire.Owner, !this.GetValue(wire.Owner));
			}
		}
	}
}
