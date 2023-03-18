using System;
using System.Runtime.CompilerServices;
using Content.Server.Doors.Systems;
using Content.Server.Wires;
using Content.Shared.Doors;
using Content.Shared.Doors.Components;
using Content.Shared.Wires;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Server.Doors
{
	// Token: 0x02000540 RID: 1344
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public sealed class DoorBoltLightWireAction : ComponentWireAction<AirlockComponent>
	{
		// Token: 0x17000431 RID: 1073
		// (get) Token: 0x06001C09 RID: 7177 RVA: 0x00095EA4 File Offset: 0x000940A4
		// (set) Token: 0x06001C0A RID: 7178 RVA: 0x00095EAC File Offset: 0x000940AC
		public override Color Color { get; set; } = Color.Lime;

		// Token: 0x17000432 RID: 1074
		// (get) Token: 0x06001C0B RID: 7179 RVA: 0x00095EB5 File Offset: 0x000940B5
		// (set) Token: 0x06001C0C RID: 7180 RVA: 0x00095EBD File Offset: 0x000940BD
		public override string Name { get; set; } = "wire-name-bolt-light";

		// Token: 0x06001C0D RID: 7181 RVA: 0x00095EC6 File Offset: 0x000940C6
		public override StatusLightState? GetLightState(Wire wire, AirlockComponent comp)
		{
			return new StatusLightState?(comp.BoltLightsEnabled ? StatusLightState.On : StatusLightState.Off);
		}

		// Token: 0x17000433 RID: 1075
		// (get) Token: 0x06001C0E RID: 7182 RVA: 0x00095ED9 File Offset: 0x000940D9
		public override object StatusKey { get; } = AirlockWireStatus.BoltLightIndicator;

		// Token: 0x06001C0F RID: 7183 RVA: 0x00095EE1 File Offset: 0x000940E1
		public override bool Cut(EntityUid user, Wire wire, AirlockComponent door)
		{
			this.EntityManager.System<AirlockSystem>().SetBoltLightsEnabled(wire.Owner, door, false);
			return true;
		}

		// Token: 0x06001C10 RID: 7184 RVA: 0x00095EFC File Offset: 0x000940FC
		public override bool Mend(EntityUid user, Wire wire, AirlockComponent door)
		{
			this.EntityManager.System<AirlockSystem>().SetBoltLightsEnabled(wire.Owner, door, true);
			return true;
		}

		// Token: 0x06001C11 RID: 7185 RVA: 0x00095F17 File Offset: 0x00094117
		public override void Pulse(EntityUid user, Wire wire, AirlockComponent door)
		{
			this.EntityManager.System<AirlockSystem>().SetBoltLightsEnabled(wire.Owner, door, !door.BoltLightsEnabled);
		}
	}
}
