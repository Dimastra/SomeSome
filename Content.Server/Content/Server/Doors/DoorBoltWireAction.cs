using System;
using System.Runtime.CompilerServices;
using Content.Server.Doors.Systems;
using Content.Server.Wires;
using Content.Shared.Doors;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Wires;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Server.Doors
{
	// Token: 0x02000541 RID: 1345
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class DoorBoltWireAction : ComponentWireAction<AirlockComponent>
	{
		// Token: 0x17000434 RID: 1076
		// (get) Token: 0x06001C13 RID: 7187 RVA: 0x00095F63 File Offset: 0x00094163
		// (set) Token: 0x06001C14 RID: 7188 RVA: 0x00095F6B File Offset: 0x0009416B
		public override Color Color { get; set; } = Color.Red;

		// Token: 0x17000435 RID: 1077
		// (get) Token: 0x06001C15 RID: 7189 RVA: 0x00095F74 File Offset: 0x00094174
		// (set) Token: 0x06001C16 RID: 7190 RVA: 0x00095F7C File Offset: 0x0009417C
		public override string Name { get; set; } = "wire-name-door-bolt";

		// Token: 0x06001C17 RID: 7191 RVA: 0x00095F85 File Offset: 0x00094185
		public override StatusLightState? GetLightState(Wire wire, AirlockComponent comp)
		{
			return new StatusLightState?(comp.BoltsDown ? StatusLightState.On : StatusLightState.Off);
		}

		// Token: 0x17000436 RID: 1078
		// (get) Token: 0x06001C18 RID: 7192 RVA: 0x00095F98 File Offset: 0x00094198
		public override object StatusKey { get; } = AirlockWireStatus.BoltIndicator;

		// Token: 0x06001C19 RID: 7193 RVA: 0x00095FA0 File Offset: 0x000941A0
		public override bool Cut(EntityUid user, Wire wire, AirlockComponent airlock)
		{
			this.EntityManager.System<SharedAirlockSystem>().SetBoltWireCut(airlock, true);
			if (!airlock.BoltsDown && base.IsPowered(wire.Owner))
			{
				this.EntityManager.System<AirlockSystem>().SetBoltsWithAudio(wire.Owner, airlock, true);
			}
			return true;
		}

		// Token: 0x06001C1A RID: 7194 RVA: 0x00095FEE File Offset: 0x000941EE
		public override bool Mend(EntityUid user, Wire wire, AirlockComponent door)
		{
			this.EntityManager.System<SharedAirlockSystem>().SetBoltWireCut(door, true);
			return true;
		}

		// Token: 0x06001C1B RID: 7195 RVA: 0x00096004 File Offset: 0x00094204
		public override void Pulse(EntityUid user, Wire wire, AirlockComponent door)
		{
			if (base.IsPowered(wire.Owner))
			{
				this.EntityManager.System<AirlockSystem>().SetBoltsWithAudio(wire.Owner, door, !door.BoltsDown);
				return;
			}
			if (!door.BoltsDown)
			{
				this.EntityManager.System<AirlockSystem>().SetBoltsWithAudio(wire.Owner, door, true);
			}
		}
	}
}
