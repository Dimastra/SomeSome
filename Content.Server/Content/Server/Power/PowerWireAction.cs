using System;
using System.Runtime.CompilerServices;
using Content.Server.Electrocution;
using Content.Server.Power.Components;
using Content.Server.Wires;
using Content.Shared.Power;
using Content.Shared.Wires;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Power
{
	// Token: 0x02000274 RID: 628
	[NullableContext(1)]
	[Nullable(0)]
	public sealed class PowerWireAction : BaseWireAction
	{
		// Token: 0x170001CB RID: 459
		// (get) Token: 0x06000C8E RID: 3214 RVA: 0x00041AA9 File Offset: 0x0003FCA9
		// (set) Token: 0x06000C8F RID: 3215 RVA: 0x00041AB1 File Offset: 0x0003FCB1
		public override Color Color { get; set; } = Color.Red;

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x06000C90 RID: 3216 RVA: 0x00041ABA File Offset: 0x0003FCBA
		// (set) Token: 0x06000C91 RID: 3217 RVA: 0x00041AC2 File Offset: 0x0003FCC2
		public override string Name { get; set; } = "wire-name-power";

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x06000C92 RID: 3218 RVA: 0x00041ACB File Offset: 0x0003FCCB
		public override object StatusKey { get; } = PowerWireActionKey.Status;

		// Token: 0x06000C93 RID: 3219 RVA: 0x00041AD4 File Offset: 0x0003FCD4
		public override StatusLightState? GetLightState(Wire wire)
		{
			int main;
			if (this.WiresSystem.TryGetData<int>(wire.Owner, PowerWireActionKey.MainWire, out main, null) && main != wire.Id)
			{
				return null;
			}
			bool pulsed;
			if (!this.AllWiresMended(wire.Owner) || (this.WiresSystem.TryGetData<bool>(wire.Owner, PowerWireActionKey.Pulsed, out pulsed, null) && pulsed))
			{
				return new StatusLightState?(StatusLightState.BlinkingSlow);
			}
			return new StatusLightState?(this.AllWiresCut(wire.Owner) ? StatusLightState.Off : StatusLightState.On);
		}

		// Token: 0x06000C94 RID: 3220 RVA: 0x00041B5C File Offset: 0x0003FD5C
		private bool AllWiresCut(EntityUid owner)
		{
			int? cut;
			int? count;
			if (this.WiresSystem.TryGetData<int?>(owner, PowerWireActionKey.CutWires, out cut, null) && this.WiresSystem.TryGetData<int?>(owner, PowerWireActionKey.WireCount, out count, null))
			{
				int? num = count;
				int? num2 = cut;
				return num.GetValueOrDefault() == num2.GetValueOrDefault() & num != null == (num2 != null);
			}
			return false;
		}

		// Token: 0x06000C95 RID: 3221 RVA: 0x00041BC0 File Offset: 0x0003FDC0
		private bool AllWiresMended(EntityUid owner)
		{
			int? cut;
			if (this.WiresSystem.TryGetData<int?>(owner, PowerWireActionKey.CutWires, out cut, null))
			{
				int? num = cut;
				int num2 = 0;
				return num.GetValueOrDefault() == num2 & num != null;
			}
			return false;
		}

		// Token: 0x06000C96 RID: 3222 RVA: 0x00041BFC File Offset: 0x0003FDFC
		private void SetPower(EntityUid owner, bool pulsed)
		{
			ApcPowerReceiverComponent power;
			if (!this.EntityManager.TryGetComponent<ApcPowerReceiverComponent>(owner, ref power))
			{
				return;
			}
			if (pulsed)
			{
				power.PowerDisabled = true;
				return;
			}
			if (this.AllWiresCut(owner))
			{
				power.PowerDisabled = true;
				return;
			}
			bool isPulsed;
			if (this.WiresSystem.TryGetData<bool>(owner, PowerWireActionKey.Pulsed, out isPulsed, null) && isPulsed)
			{
				return;
			}
			power.PowerDisabled = false;
		}

		// Token: 0x06000C97 RID: 3223 RVA: 0x00041C58 File Offset: 0x0003FE58
		private void SetWireCuts(EntityUid owner, bool isCut)
		{
			int? cut;
			int? count;
			if (this.WiresSystem.TryGetData<int?>(owner, PowerWireActionKey.CutWires, out cut, null) && this.WiresSystem.TryGetData<int?>(owner, PowerWireActionKey.WireCount, out count, null))
			{
				int? num = cut;
				int? num2 = count;
				if (!(num.GetValueOrDefault() == num2.GetValueOrDefault() & num != null == (num2 != null)) || !isCut)
				{
					num2 = cut;
					int num3 = 0;
					if (!(num2.GetValueOrDefault() <= num3 & num2 != null) || isCut)
					{
						cut = (isCut ? (cut + 1) : (cut - 1));
						this.WiresSystem.SetData(owner, PowerWireActionKey.CutWires, cut, null);
						return;
					}
				}
				return;
			}
		}

		// Token: 0x06000C98 RID: 3224 RVA: 0x00041D4A File Offset: 0x0003FF4A
		[NullableContext(2)]
		private void SetElectrified(EntityUid used, bool setting, ElectrifiedComponent electrified = null)
		{
			if (electrified == null && !this.EntityManager.TryGetComponent<ElectrifiedComponent>(used, ref electrified))
			{
				return;
			}
			electrified.Enabled = setting;
		}

		// Token: 0x06000C99 RID: 3225 RVA: 0x00041D68 File Offset: 0x0003FF68
		private bool TrySetElectrocution(EntityUid user, Wire wire, bool timed = false)
		{
			ElectrifiedComponent electrified;
			if (!this.EntityManager.TryGetComponent<ElectrifiedComponent>(wire.Owner, ref electrified))
			{
				return true;
			}
			this.SetElectrified(wire.Owner, true, electrified);
			return !this._electrocutionSystem.TryDoElectrifiedAct(wire.Owner, user, 1f, null, null, null);
		}

		// Token: 0x06000C9A RID: 3226 RVA: 0x00041DB8 File Offset: 0x0003FFB8
		private void UpdateElectrocution(Wire wire)
		{
			bool allCut = this.AllWiresCut(wire.Owner);
			bool activePulse = false;
			bool pulsed;
			if (this.WiresSystem.TryGetData<bool>(wire.Owner, PowerWireActionKey.Pulsed, out pulsed, null))
			{
				activePulse = pulsed;
			}
			if (!this.WiresSystem.HasData(wire.Owner, PowerWireActionKey.ElectrifiedCancel, null) && activePulse && base.IsPowered(wire.Owner) && !allCut)
			{
				this.WiresSystem.StartWireAction(wire.Owner, (float)this._pulseTimeout, PowerWireActionKey.ElectrifiedCancel, new TimedWireEvent(new WireActionDelegate(this.AwaitElectrifiedCancel), wire));
				return;
			}
			if ((!activePulse && allCut) || this.AllWiresMended(wire.Owner))
			{
				this.SetElectrified(wire.Owner, false, null);
			}
		}

		// Token: 0x06000C9B RID: 3227 RVA: 0x00041E77 File Offset: 0x00040077
		public override void Initialize()
		{
			base.Initialize();
			this._electrocutionSystem = EntitySystem.Get<ElectrocutionSystem>();
		}

		// Token: 0x06000C9C RID: 3228 RVA: 0x00041E8C File Offset: 0x0004008C
		public override bool AddWire(Wire wire, int count)
		{
			if (!this.WiresSystem.HasData(wire.Owner, PowerWireActionKey.CutWires, null))
			{
				this.WiresSystem.SetData(wire.Owner, PowerWireActionKey.CutWires, 0, null);
			}
			if (count == 1)
			{
				this.WiresSystem.SetData(wire.Owner, PowerWireActionKey.MainWire, wire.Id, null);
			}
			this.WiresSystem.SetData(wire.Owner, PowerWireActionKey.WireCount, count, null);
			return true;
		}

		// Token: 0x06000C9D RID: 3229 RVA: 0x00041F17 File Offset: 0x00040117
		public override bool Cut(EntityUid user, Wire wire)
		{
			base.Cut(user, wire);
			if (!this.TrySetElectrocution(user, wire, false))
			{
				return false;
			}
			this.SetWireCuts(wire.Owner, true);
			this.SetPower(wire.Owner, false);
			return true;
		}

		// Token: 0x06000C9E RID: 3230 RVA: 0x00041F4C File Offset: 0x0004014C
		public override bool Mend(EntityUid user, Wire wire)
		{
			base.Mend(user, wire);
			if (!this.TrySetElectrocution(user, wire, false))
			{
				return false;
			}
			this.WiresSystem.TryCancelWireAction(wire.Owner, PowerWireActionKey.PulseCancel);
			this.WiresSystem.TryCancelWireAction(wire.Owner, PowerWireActionKey.ElectrifiedCancel);
			this.SetWireCuts(wire.Owner, false);
			this.SetPower(wire.Owner, false);
			return true;
		}

		// Token: 0x06000C9F RID: 3231 RVA: 0x00041FBC File Offset: 0x000401BC
		public override void Pulse(EntityUid user, Wire wire)
		{
			base.Pulse(user, wire);
			this.WiresSystem.TryCancelWireAction(wire.Owner, PowerWireActionKey.ElectrifiedCancel);
			bool electrocuted = !this.TrySetElectrocution(user, wire, true);
			bool pulsedKey;
			if (this.WiresSystem.TryGetData<bool>(wire.Owner, PowerWireActionKey.Pulsed, out pulsedKey, null) && pulsedKey)
			{
				return;
			}
			this.WiresSystem.SetData(wire.Owner, PowerWireActionKey.Pulsed, true, null);
			this.WiresSystem.StartWireAction(wire.Owner, (float)this._pulseTimeout, PowerWireActionKey.PulseCancel, new TimedWireEvent(new WireActionDelegate(this.AwaitPulseCancel), wire));
			if (electrocuted)
			{
				return;
			}
			this.SetPower(wire.Owner, true);
		}

		// Token: 0x06000CA0 RID: 3232 RVA: 0x00042074 File Offset: 0x00040274
		public override void Update(Wire wire)
		{
			this.UpdateElectrocution(wire);
			bool pulsed;
			if (!base.IsPowered(wire.Owner) && (!this.WiresSystem.TryGetData<bool>(wire.Owner, PowerWireActionKey.Pulsed, out pulsed, null) || !pulsed))
			{
				this.WiresSystem.TryCancelWireAction(wire.Owner, PowerWireActionKey.ElectrifiedCancel);
				this.WiresSystem.TryCancelWireAction(wire.Owner, PowerWireActionKey.PulseCancel);
			}
		}

		// Token: 0x06000CA1 RID: 3233 RVA: 0x000420E5 File Offset: 0x000402E5
		private void AwaitElectrifiedCancel(Wire wire)
		{
			if (this.AllWiresMended(wire.Owner))
			{
				this.SetElectrified(wire.Owner, false, null);
			}
		}

		// Token: 0x06000CA2 RID: 3234 RVA: 0x00042103 File Offset: 0x00040303
		private void AwaitPulseCancel(Wire wire)
		{
			this.WiresSystem.SetData(wire.Owner, PowerWireActionKey.Pulsed, false, null);
			this.SetPower(wire.Owner, false);
		}

		// Token: 0x040007BE RID: 1982
		[DataField("pulseTimeout", false, 1, false, false, null)]
		private int _pulseTimeout = 30;

		// Token: 0x040007BF RID: 1983
		private ElectrocutionSystem _electrocutionSystem;
	}
}
