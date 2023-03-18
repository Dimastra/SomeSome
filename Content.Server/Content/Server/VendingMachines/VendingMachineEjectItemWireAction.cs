using System;
using System.Runtime.CompilerServices;
using Content.Server.Wires;
using Content.Shared.VendingMachines;
using Content.Shared.Wires;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Server.VendingMachines
{
	// Token: 0x020000D3 RID: 211
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class VendingMachineEjectItemWireAction : ComponentWireAction<VendingMachineComponent>
	{
		// Token: 0x1700008F RID: 143
		// (get) Token: 0x060003B2 RID: 946 RVA: 0x0001387A File Offset: 0x00011A7A
		// (set) Token: 0x060003B3 RID: 947 RVA: 0x00013882 File Offset: 0x00011A82
		public override Color Color { get; set; } = Color.Red;

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060003B4 RID: 948 RVA: 0x0001388B File Offset: 0x00011A8B
		// (set) Token: 0x060003B5 RID: 949 RVA: 0x00013893 File Offset: 0x00011A93
		public override string Name { get; set; } = "wire-name-vending-eject";

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060003B6 RID: 950 RVA: 0x0001389C File Offset: 0x00011A9C
		[Nullable(2)]
		public override object StatusKey { [NullableContext(2)] get; } = EjectWireKey.StatusKey;

		// Token: 0x060003B7 RID: 951 RVA: 0x000138A4 File Offset: 0x00011AA4
		public override StatusLightState? GetLightState(Wire wire, VendingMachineComponent comp)
		{
			return new StatusLightState?(comp.CanShoot ? StatusLightState.BlinkingFast : StatusLightState.On);
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x000138B7 File Offset: 0x00011AB7
		public override void Initialize()
		{
			base.Initialize();
			this._vendingMachineSystem = this.EntityManager.System<VendingMachineSystem>();
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x000138D0 File Offset: 0x00011AD0
		public override bool Cut(EntityUid user, Wire wire, VendingMachineComponent vending)
		{
			this._vendingMachineSystem.SetShooting(wire.Owner, true, vending);
			return true;
		}

		// Token: 0x060003BA RID: 954 RVA: 0x000138E6 File Offset: 0x00011AE6
		public override bool Mend(EntityUid user, Wire wire, VendingMachineComponent vending)
		{
			this._vendingMachineSystem.SetShooting(wire.Owner, false, vending);
			return true;
		}

		// Token: 0x060003BB RID: 955 RVA: 0x000138FC File Offset: 0x00011AFC
		public override void Pulse(EntityUid user, Wire wire, VendingMachineComponent vending)
		{
			this._vendingMachineSystem.EjectRandom(wire.Owner, true, false, vending);
		}

		// Token: 0x0400024F RID: 591
		private VendingMachineSystem _vendingMachineSystem;
	}
}
