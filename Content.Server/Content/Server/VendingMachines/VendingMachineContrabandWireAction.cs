using System;
using System.Runtime.CompilerServices;
using Content.Server.Wires;
using Content.Shared.VendingMachines;
using Content.Shared.Wires;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.VendingMachines
{
	// Token: 0x020000D2 RID: 210
	[NullableContext(2)]
	[Nullable(0)]
	[DataDefinition]
	public sealed class VendingMachineContrabandWireAction : BaseToggleWireAction
	{
		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060003A8 RID: 936 RVA: 0x00013784 File Offset: 0x00011984
		// (set) Token: 0x060003A9 RID: 937 RVA: 0x0001378C File Offset: 0x0001198C
		public override Color Color { get; set; } = Color.Green;

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060003AA RID: 938 RVA: 0x00013795 File Offset: 0x00011995
		// (set) Token: 0x060003AB RID: 939 RVA: 0x0001379D File Offset: 0x0001199D
		[Nullable(1)]
		public override string Name { [NullableContext(1)] get; [NullableContext(1)] set; } = "wire-name-vending-contraband";

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060003AC RID: 940 RVA: 0x000137A6 File Offset: 0x000119A6
		public override object StatusKey { get; } = ContrabandWireKey.StatusKey;

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x060003AD RID: 941 RVA: 0x000137AE File Offset: 0x000119AE
		public override object TimeoutKey { get; } = ContrabandWireKey.TimeoutKey;

		// Token: 0x060003AE RID: 942 RVA: 0x000137B8 File Offset: 0x000119B8
		[NullableContext(1)]
		public override StatusLightState? GetLightState(Wire wire)
		{
			VendingMachineComponent vending;
			if (this.EntityManager.TryGetComponent<VendingMachineComponent>(wire.Owner, ref vending))
			{
				return new StatusLightState?(vending.Contraband ? StatusLightState.BlinkingSlow : StatusLightState.On);
			}
			return new StatusLightState?(StatusLightState.Off);
		}

		// Token: 0x060003AF RID: 943 RVA: 0x000137F4 File Offset: 0x000119F4
		public override void ToggleValue(EntityUid owner, bool setting)
		{
			VendingMachineComponent vending;
			if (this.EntityManager.TryGetComponent<VendingMachineComponent>(owner, ref vending))
			{
				vending.Contraband = !setting;
			}
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x0001381C File Offset: 0x00011A1C
		public override bool GetValue(EntityUid owner)
		{
			VendingMachineComponent vending;
			return this.EntityManager.TryGetComponent<VendingMachineComponent>(owner, ref vending) && !vending.Contraband;
		}
	}
}
