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
	// Token: 0x02000543 RID: 1347
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class DoorTimingWireAction : ComponentWireAction<AirlockComponent>
	{
		// Token: 0x1700043A RID: 1082
		// (get) Token: 0x06001C29 RID: 7209 RVA: 0x000961F2 File Offset: 0x000943F2
		// (set) Token: 0x06001C2A RID: 7210 RVA: 0x000961FA File Offset: 0x000943FA
		public override Color Color { get; set; } = Color.Orange;

		// Token: 0x1700043B RID: 1083
		// (get) Token: 0x06001C2B RID: 7211 RVA: 0x00096203 File Offset: 0x00094403
		// (set) Token: 0x06001C2C RID: 7212 RVA: 0x0009620B File Offset: 0x0009440B
		public override string Name { get; set; } = "wire-name-door-timer";

		// Token: 0x06001C2D RID: 7213 RVA: 0x00096214 File Offset: 0x00094414
		public override StatusLightState? GetLightState(Wire wire, AirlockComponent comp)
		{
			float autoCloseDelayModifier = comp.AutoCloseDelayModifier;
			if (autoCloseDelayModifier == 0.01f)
			{
				return new StatusLightState?(StatusLightState.Off);
			}
			if (autoCloseDelayModifier > 0.5f)
			{
				return new StatusLightState?(StatusLightState.On);
			}
			return new StatusLightState?(StatusLightState.BlinkingSlow);
		}

		// Token: 0x1700043C RID: 1084
		// (get) Token: 0x06001C2E RID: 7214 RVA: 0x0009624E File Offset: 0x0009444E
		public override object StatusKey { get; } = AirlockWireStatus.TimingIndicator;

		// Token: 0x06001C2F RID: 7215 RVA: 0x00096256 File Offset: 0x00094456
		public override bool Cut(EntityUid user, Wire wire, AirlockComponent door)
		{
			this.WiresSystem.TryCancelWireAction(wire.Owner, DoorTimingWireAction.PulseTimeoutKey.Key);
			this.EntityManager.System<SharedAirlockSystem>().SetAutoCloseDelayModifier(door, 0.01f);
			return true;
		}

		// Token: 0x06001C30 RID: 7216 RVA: 0x00096287 File Offset: 0x00094487
		public override bool Mend(EntityUid user, Wire wire, AirlockComponent door)
		{
			this.EntityManager.System<SharedAirlockSystem>().SetAutoCloseDelayModifier(door, 1f);
			return true;
		}

		// Token: 0x06001C31 RID: 7217 RVA: 0x000962A0 File Offset: 0x000944A0
		public override void Pulse(EntityUid user, Wire wire, AirlockComponent door)
		{
			this.EntityManager.System<SharedAirlockSystem>().SetAutoCloseDelayModifier(door, 0.5f);
			this.WiresSystem.StartWireAction(wire.Owner, (float)this._timeout, DoorTimingWireAction.PulseTimeoutKey.Key, new TimedWireEvent(new WireActionDelegate(this.AwaitTimingTimerFinish), wire));
		}

		// Token: 0x06001C32 RID: 7218 RVA: 0x000962F3 File Offset: 0x000944F3
		public override void Update(Wire wire)
		{
			if (!base.IsPowered(wire.Owner))
			{
				this.WiresSystem.TryCancelWireAction(wire.Owner, DoorTimingWireAction.PulseTimeoutKey.Key);
			}
		}

		// Token: 0x06001C33 RID: 7219 RVA: 0x0009631C File Offset: 0x0009451C
		private void AwaitTimingTimerFinish(Wire wire)
		{
			AirlockComponent door;
			if (!wire.IsCut && this.EntityManager.TryGetComponent<AirlockComponent>(wire.Owner, ref door))
			{
				this.EntityManager.System<SharedAirlockSystem>().SetAutoCloseDelayModifier(door, 1f);
			}
		}

		// Token: 0x0400122F RID: 4655
		[DataField("timeout", false, 1, false, false, null)]
		private int _timeout = 30;

		// Token: 0x02000A17 RID: 2583
		[NullableContext(0)]
		private enum PulseTimeoutKey : byte
		{
			// Token: 0x0400234F RID: 9039
			Key
		}
	}
}
