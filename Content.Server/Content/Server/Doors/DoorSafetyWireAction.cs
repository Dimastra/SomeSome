using System;
using System.Runtime.CompilerServices;
using Content.Server.Wires;
using Content.Shared.Doors;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Wires;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Doors
{
	// Token: 0x02000542 RID: 1346
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class DoorSafetyWireAction : ComponentWireAction<AirlockComponent>
	{
		// Token: 0x17000437 RID: 1079
		// (get) Token: 0x06001C1D RID: 7197 RVA: 0x0009608A File Offset: 0x0009428A
		// (set) Token: 0x06001C1E RID: 7198 RVA: 0x00096092 File Offset: 0x00094292
		public override Color Color { get; set; } = Color.Red;

		// Token: 0x17000438 RID: 1080
		// (get) Token: 0x06001C1F RID: 7199 RVA: 0x0009609B File Offset: 0x0009429B
		// (set) Token: 0x06001C20 RID: 7200 RVA: 0x000960A3 File Offset: 0x000942A3
		public override string Name { get; set; } = "wire-name-door-safety";

		// Token: 0x06001C21 RID: 7201 RVA: 0x000960AC File Offset: 0x000942AC
		public override StatusLightState? GetLightState(Wire wire, AirlockComponent comp)
		{
			return new StatusLightState?(comp.Safety ? StatusLightState.On : StatusLightState.Off);
		}

		// Token: 0x17000439 RID: 1081
		// (get) Token: 0x06001C22 RID: 7202 RVA: 0x000960BF File Offset: 0x000942BF
		public override object StatusKey { get; } = AirlockWireStatus.SafetyIndicator;

		// Token: 0x06001C23 RID: 7203 RVA: 0x000960C7 File Offset: 0x000942C7
		public override bool Cut(EntityUid user, Wire wire, AirlockComponent door)
		{
			this.WiresSystem.TryCancelWireAction(wire.Owner, DoorSafetyWireAction.PulseTimeoutKey.Key);
			this.EntityManager.System<SharedAirlockSystem>().SetSafety(door, false);
			return true;
		}

		// Token: 0x06001C24 RID: 7204 RVA: 0x000960F4 File Offset: 0x000942F4
		public override bool Mend(EntityUid user, Wire wire, AirlockComponent door)
		{
			this.EntityManager.System<SharedAirlockSystem>().SetSafety(door, true);
			return true;
		}

		// Token: 0x06001C25 RID: 7205 RVA: 0x0009610C File Offset: 0x0009430C
		public override void Pulse(EntityUid user, Wire wire, AirlockComponent door)
		{
			this.EntityManager.System<SharedAirlockSystem>().SetSafety(door, false);
			this.WiresSystem.StartWireAction(wire.Owner, (float)this._timeout, DoorSafetyWireAction.PulseTimeoutKey.Key, new TimedWireEvent(new WireActionDelegate(this.AwaitSafetyTimerFinish), wire));
		}

		// Token: 0x06001C26 RID: 7206 RVA: 0x0009615B File Offset: 0x0009435B
		public override void Update(Wire wire)
		{
			if (!base.IsPowered(wire.Owner))
			{
				this.WiresSystem.TryCancelWireAction(wire.Owner, DoorSafetyWireAction.PulseTimeoutKey.Key);
			}
		}

		// Token: 0x06001C27 RID: 7207 RVA: 0x00096184 File Offset: 0x00094384
		private void AwaitSafetyTimerFinish(Wire wire)
		{
			AirlockComponent door;
			if (!wire.IsCut && this.EntityManager.TryGetComponent<AirlockComponent>(wire.Owner, ref door))
			{
				this.EntityManager.System<SharedAirlockSystem>().SetSafety(door, true);
			}
		}

		// Token: 0x0400122B RID: 4651
		[DataField("timeout", false, 1, false, false, null)]
		private int _timeout = 30;

		// Token: 0x02000A16 RID: 2582
		[NullableContext(0)]
		private enum PulseTimeoutKey : byte
		{
			// Token: 0x0400234D RID: 9037
			Key
		}
	}
}
